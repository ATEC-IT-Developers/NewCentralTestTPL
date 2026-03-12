using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace CentralTestTPL
{
    public partial class Main : Form
    {
        private static string baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\";
        private static string AutoFillDetails = Path.Combine(baseDirectory, "AutoFillDetails.txt");
        private static string CustomerCodeFilePath = Path.Combine(baseDirectory, "CustomerCode.txt");
        private static string LoadTestProg = Path.Combine(baseDirectory, "LaunchApp.exe");

        public Main()
        {
            InitializeComponent();
            if (!File.Exists(AutoFillDetails)) File.WriteAllText(AutoFillDetails, "");
            if (!File.Exists(CustomerCodeFilePath)) File.WriteAllText(CustomerCodeFilePath, "");
        }

        private Timer timer = new Timer();
        private bool timerStarted = false;
        private void StartTimer()
        {
            timer.Interval = 1000; // 1 second
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();                 // Stop immediately
            timer.Tick -= Timer_Tick;     // Remove event (important!)
            SendKeys.Send("{ENTER}");     // Simulate Enter
        }

        //private void ShowError(string message) =>
        //MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void ShowError(string message, TextBox txtbox)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtbox.Clear();
            txtbox.Enabled = true;
            txtbox.Focus();
            timerStarted = false;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            txtLotnumber.Focus();
        }

        static string GetLocalIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] add = Dns.GetHostAddresses(hostName);
            foreach (var ip in add)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No IPv4 address found!");
        }

        private void SetControlState(Control disableCtrl, Control enableCtrl, bool setFocus = true)
        {
            disableCtrl.Enabled = false;
            enableCtrl.Enabled = true;

            if (setFocus)
                enableCtrl.Focus();
        }

        bool FileExistsOnFtp(string ftpPath, string lotNumber, string keyword, string[] allowedExtensions)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            // if FTP requires login
            request.Credentials = new NetworkCredential(CentralTest.Username, CentralTest.Password);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(responseStream))
            {
                while (!reader.EndOfStream)
                {
                    string name = reader.ReadLine();
                    string ext = Path.GetExtension(name);

                    if (allowedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) &&
                        name.StartsWith(lotNumber + "-" + keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool ValidateProcess(
        DataAccess dataAccess,
        string processCode,
        string notExistMessage,
        string processName,
        string lot,
        string device,
        string customer,
        string path,
        string machine,
        string ip,
        TextBox txtbox)
        {
            var loadForm = new Loading();
            loadForm.Show();
            bool exists = dataAccess.checkCORBIN(processCode, ip);
            string Testdevice = "";
            string testmsg = "";
            if (processCode == "-CORR") {
                Testdevice = CORR.Device;
                testmsg = CORR.msg;
            } else {
                Testdevice = BINNING.Device;
                testmsg = BINNING.msg;
            }

            if (!exists)
            {
                dataAccess.insertMasterLogs(notExistMessage, lot, device, customer, path, machine, ip);
                ShowError(notExistMessage, txtbox);
                loadForm.Hide();
                return false;
            }

            if (device != Testdevice)
            {
                string msg = $"Change Device., Please Perform {processName}, Last Run Device: {Testdevice}, LTC Device: {device}";
                dataAccess.insertMasterLogs(msg, lot, device, customer, path, machine, ip);
                ShowError(msg.Replace(", ", "\n"), txtbox);
                loadForm.Hide();
                return false;
            }

            if (!string.IsNullOrEmpty(testmsg))
            {
                string msg = $"{processName} {testmsg}";
                dataAccess.insertMasterLogs(msg, lot, device, customer, path, machine, ip);
                ShowError(msg, txtbox);
                loadForm.Hide();
                return false;
            }

            loadForm.Hide();
            return true;
        }

        private void txtLotnumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!timerStarted)
            {
                timerStarted = true;
                StartTimer();
            }
            if (e.KeyChar != (char)Keys.Enter) return;
            timerStarted = false;
            if (string.IsNullOrWhiteSpace(txtLotnumber.Text))
            {
                new DataAccess().insertMasterLogs("Invalid Lot.", txtLotnumber.Text, "", "", "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError("Invalid Lot.\nPlease Scan again.", txtLotnumber);
                return;
            }
            else
            {
                var list = new DataAccess().SelectLotInfo(txtLotnumber.Text);
                if (list.Count > 0) {
                    var allowedCodes = User.CustomerCodes.Split(',');
                    if (allowedCodes.Contains(LotInfo.CustomerCode.ToString())) {

                        var dataAccess = new DataAccess();
                        string ip = GetLocalIPAddress();
                        string lot = txtLotnumber.Text;
                        string device = LotInfo.Device;
                        string customer = LotInfo.CustomerCode.ToString();
                        string path = CentralTest.EngDatalogPath;
                        string machine = CentralTest.MachineName;

                        // Validate Test Correlation
                        if (!ValidateProcess(dataAccess, "-CORR",
                            "Please Perform Test Correlation.",
                            "Test Correlation ",
                            lot, device, customer, path, machine, ip, txtLotnumber))
                            return;

                        // Validate Binning Consistency
                        if (!ValidateProcess(dataAccess, "-BINCON",
                            "Please Perform Binning Consistency Check.",
                            "Binning Consistency Check ",
                            lot, device, customer, path, machine, ip, txtLotnumber))
                            return;

                        txtDetailLotnumber.Text = LotInfo.LotNumber;
                        txtDetailQty.Text = LotInfo.SubLotQty.ToString();
                        txtDetailDevice.Text = LotInfo.Device;
                        txtDetailProduct.Text = LotInfo.ProductID;
                        txtDetailPkg.Text = LotInfo.PkgLD;
                        txtDetailLead.Text = LotInfo.LdType;
                        File.WriteAllText(CustomerCodeFilePath, LotInfo.CustomerCode.ToString());
                        txtCarrierID.Text = LotInfo.CarrierTape;
                        txtCoverID.Text = LotInfo.CoverTape;
                        txtReelID.Text = LotInfo.Reel;
                        Global.TPLStage = LotInfo.TPL_Stage;
                        txtLotnumber.Enabled = false;

                        string[] programs = LotInfo.TestProgram.Split(',');
                        cmbTestProg.Items.Clear(); // optional but recommended

                        foreach (string prog in programs)
                        {
                            cmbTestProg.Items.Add(prog.Trim());
                        }

                        txtLBoard.Enabled = true;
                        txtLBoard.Focus();
                    }
                    else {
                        new DataAccess().insertMasterLogs("Operator not qualified. " + User.Emp_No, txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                        ShowError("Operator not qualified.", txtLotnumber);
                        return;
                    }
                } else {
                    new DataAccess().insertMasterLogs("Lot not found.", txtLotnumber.Text, "", "", "", CentralTest.MachineName, GetLocalIPAddress());
                    ShowError("Lot not found.\nPlease Scan again.", txtLotnumber);
                    return;
                }
            }
        }

        private bool IsInRange(string scanned, string rangeText)
        {
            var parts = rangeText.Split(',');

            if (parts.Length != 2)
                return false;

            string start = parts[0].Trim();
            string end = parts[1].Trim();

            string prefixStart = new string(start.TakeWhile(c => !char.IsDigit(c)).ToArray());
            string prefixEnd = new string(end.TakeWhile(c => !char.IsDigit(c)).ToArray());
            string prefixScan = new string(scanned.TakeWhile(c => !char.IsDigit(c)).ToArray());

            if (prefixStart != prefixEnd || prefixScan != prefixStart)
                return false;

            int numStart = int.Parse(new string(start.Where(char.IsDigit).ToArray()));
            int numEnd = int.Parse(new string(end.Where(char.IsDigit).ToArray()));
            int numScan = int.Parse(new string(scanned.Where(char.IsDigit).ToArray()));

            return numScan >= numStart && numScan <= numEnd;
        }

        private bool MatchHardware(TextBox HW, string DatabaseHW)
        {
            var allowedHW = DatabaseHW.Split(',');

            return allowedHW
                    .Select(x => x.Trim())
                    .Any(x => x.Equals(HW.Text.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        private void HandleScan(KeyPressEventArgs e, TextBox currentTextBox, string expectedValue, TextBox nextTextBox, string itemName, bool matgroup)
        {
            if (string.IsNullOrWhiteSpace(currentTextBox.Text))
            {
                new DataAccess().insertMasterLogs($"Invalid {itemName}.", txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError($"Invalid {itemName}.\nPlease Scan again.", currentTextBox);
                return;
            }

            if (!IsInRange(currentTextBox.Text.Trim(), expectedValue))
            {
                new DataAccess().insertMasterLogs($"{itemName} not Match in Database.", txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError($"{itemName} not Match in Database.\nPlease Scan again.", currentTextBox);
                return;
            }

            currentTextBox.Enabled = false;
            groupBox3.Enabled = matgroup;
            txtCarrierLot.Enabled = matgroup;

            if (itemName == "Load Board") {
                Global.CheckLB = true;
            }

            if (nextTextBox != null)
            {
                nextTextBox.Enabled = true;
                nextTextBox.Focus();
            }
        }

        private void txtLBoard_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!timerStarted)
            {
                timerStarted = true;
                StartTimer();
            }
            if (e.KeyChar != (char)Keys.Enter) return;
            timerStarted = false;
            Global.CheckLB = false;
            HandleScan(e, txtLBoard, LotInfo.LBoard, txtHIBs, "Load Board", false);

            if (Global.CheckLB == true && CORR.LBoard != "" && txtLBoard.Text != CORR.LBoard)
            {
                new DataAccess().insertMasterLogs("Load Board Change. Please Perform Test Correlation and Please Perform Binning Consistency Check." + txtLBoard.Text + ", " + CORR.LBoard, txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), CentralTest.EngDatalogPath, CentralTest.MachineName, GetLocalIPAddress());
                ShowError("Load Board Change./nPlease Perform Test Correlation and Please Perform Binning Consistency Check.", txtLBoard);
                txtHIBs.Enabled = false;
                return;
            }
        }

        private HashSet<TextBox> _validatedScans = new HashSet<TextBox>();

        private readonly DataAccess _da = new DataAccess();

        private void ProcessHIBsCable(KeyPressEventArgs e, TextBox txtBox, string expectedValue, TextBox nxtTxtbox, string hardware, bool group, string AllHardware)
        {
            if (!timerStarted)
            {
                timerStarted = true;
                StartTimer();
            }
            if (e.KeyChar != (char)Keys.Enter) return;
            timerStarted = false;
            string message = "";
            string logMessage = "";

            // 🚫 Prevent rescanning ONLY if already validated
            if (_validatedScans.Contains(txtBox))
            {
                message = $"{hardware} already scanned.\nRescanning is not allowed.";
                ShowError(message, txtBox);
                DataAccess da = new DataAccess();
                da.insertMasterLogs(
                    message,
                    txtLotnumber.Text,
                    LotInfo.Device,
                    LotInfo.CustomerCode.ToString(),
                    CentralTest.EngDatalogPath,
                    CentralTest.MachineName,
                    GetLocalIPAddress()
                );
                return;
            }

            // ✅ SUCCESS
            _validatedScans.Add(txtBox);   // Mark as successfully scanned

            HandleScan(e, txtBox, expectedValue, nxtTxtbox, hardware, group);

            if (!string.IsNullOrEmpty(CORR.LBoard))
            {
                bool ok = MatchHardware(txtBox, AllHardware);
                if (!ok)
                {
                    message = $"{hardware} Change.\nPlease Perform Test Correlation and Please Perform Binning Consistency Check.";
                    logMessage = $"{hardware} Change.\n{message} {txtBox.Text}, {AllHardware}";
                    DataAccess da = new DataAccess();
                    da.insertMasterLogs(
                        logMessage,
                        txtLotnumber.Text,
                        LotInfo.Device,
                        LotInfo.CustomerCode.ToString(),
                        CentralTest.EngDatalogPath,
                        CentralTest.MachineName,
                        GetLocalIPAddress()
                    );
                    nxtTxtbox.Enabled = false;
                    ShowError(message, txtBox);
                    return;
                }
            }

        }

        private void txtHIBs_KeyPress(object sender, KeyPressEventArgs e)
        {
            ProcessHIBsCable(e, txtHIBs, LotInfo.Hibs, txtCable, "HIBs", false, CORR.AllHIBs);
        }

        private void txtHIBs2_KeyPress(object sender, KeyPressEventArgs e)
        {
            ProcessHIBsCable(e, txtHIBs2, LotInfo.Hibs, txtCable2, "HIBs", false, CORR.AllHIBs);
        }

        private void txtHIBs3_KeyPress(object sender, KeyPressEventArgs e)
        {
            ProcessHIBsCable(e, txtHIBs3, LotInfo.Hibs, txtCable3, "HIBs", false, CORR.AllHIBs);
        }

        private void txtHIBs4_KeyPress(object sender, KeyPressEventArgs e)
        {
            ProcessHIBsCable(e, txtHIBs4, LotInfo.Hibs, txtCable4, "HIBs", false, CORR.AllHIBs);
        }

        private void txtCable_KeyPress(object sender, KeyPressEventArgs e)
        {
            ProcessHIBsCable(e, txtCable, LotInfo.TPLCable, txtHIBs2, "Cable", true, CORR.AllCable);
        }

        private void txtCable2_KeyPress(object sender, KeyPressEventArgs e)
        {
            ProcessHIBsCable(e, txtCable2, LotInfo.TPLCable, txtHIBs3, "Cable", true, CORR.AllCable);
        }

        private void txtCable3_KeyPress(object sender, KeyPressEventArgs e)
        {
            ProcessHIBsCable(e, txtCable3, LotInfo.TPLCable, txtHIBs4, "Cable", true, CORR.AllCable);
        }

        private void txtCable4_KeyPress(object sender, KeyPressEventArgs e)
        {
            ProcessHIBsCable(e, txtCable4, LotInfo.TPLCable, txtCarrierLot, "Cable", true, CORR.AllCable);
        }

        private void txtCarrierLot_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!timerStarted)
            {
                timerStarted = true;
                StartTimer();
            }
            if (e.KeyChar != (char)Keys.Enter) return;
            timerStarted = false;
            if (string.IsNullOrWhiteSpace(txtCarrierLot.Text))
            {
                new DataAccess().insertMasterLogs($"Invalid Carrier Lot. " + txtCarrierLot.Text, txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError("Invalid Carrier Lot.\nPlease Scan again.", txtCarrierLot);
                return;
            }
            else {
                var mat = new DataAccess().AXCheckMaterial(txtCarrierLot.Text, txtCarrierID.Text);
                if (mat.Count <= 0) {
                    new DataAccess().insertMasterLogs($"Carrier Lot not found in AX. " + txtCarrierLot.Text, txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                    ShowError("Carrier Lot not found in AX.\nPlease Scan again.", txtCarrierLot);
                    return;
                }
                else {
                    if (AXMaterial.ErrorMsg != "")
                    {
                        new DataAccess().insertMasterLogs(AXMaterial.ErrorMsg + " " + txtCarrierLot.Text, txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                        ShowError(AXMaterial.ErrorMsg, txtCarrierLot);
                        return;
                    }
                    else
                    {
                        if(CORR.LBoard == "")
                        {
                            bool update = new DataAccess().UpdateHardWare(txtLBoard.Text,
                                                            txtHIBs.Text,
                                                            txtHIBs2.Text,
                                                            txtHIBs3.Text,
                                                            txtHIBs4.Text,
                                                            txtCable.Text,
                                                            txtCable2.Text,
                                                            txtCable3.Text,
                                                            txtCable4.Text);
                            if (update) {
                                txtCarrierLot.Enabled = false;
                                txtCoverLot.Enabled = true;
                                txtCoverLot.Focus();
                                groupBox2.Enabled = false;
                            }
                            else {
                                new DataAccess().insertMasterLogs($"Hardware Update Fail." + txtLBoard.Text + "," +
                                                                                             txtHIBs.Text + "," +
                                                                                             txtHIBs2.Text + "," +
                                                                                             txtHIBs3.Text + "," +
                                                                                             txtHIBs4.Text + "," +
                                                                                             txtCable.Text + "," +
                                                                                             txtCable2.Text + "," +
                                                                                             txtCable3.Text + "," +
                                                                                             txtCable4.Text, txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                                ShowError("Please Scan again.", txtCarrierLot);
                                return;
                            }
                        }
                        else
                        {
                            txtCarrierLot.Enabled = false;
                            txtCoverLot.Enabled = true;
                            txtCoverLot.Focus();
                            groupBox2.Enabled = false;
                        }
                    }
                }
            }
        }

        private void txtCoverLot_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!timerStarted)
            {
                timerStarted = true;
                StartTimer();
            }
            if (e.KeyChar != (char)Keys.Enter) return;
            timerStarted = false;
            if (string.IsNullOrWhiteSpace(txtCoverLot.Text))
            {
                new DataAccess().insertMasterLogs($"Invalid Cover Lot. " + txtCoverLot.Text, txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError("Invalid Cover Lot.\nPlease Scan again.", txtCoverLot);
                return;
            }
            else {
                var mat = new DataAccess().AXCheckMaterial(txtCoverLot.Text, txtCoverID.Text);
                if (mat.Count <= 0)
                {
                    new DataAccess().insertMasterLogs($"Cover Lot not found in AX. " + txtCoverLot.Text, txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                    ShowError("Cover Lot not found in AX.\nPlease Scan again.", txtCoverLot);
                    return;
                }
                else
                {
                    if (AXMaterial.ErrorMsg != "")
                    {
                        new DataAccess().insertMasterLogs(AXMaterial.ErrorMsg + " " + txtCoverLot.Text, txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                        ShowError(AXMaterial.ErrorMsg, txtCoverLot);
                        return;
                    }
                    else
                    {
                        txtCoverLot.Enabled = false;
                        txtReelLot.Enabled = true;
                        txtReelLot.Focus();
                    }
                }
            }
        }

        private void txtReelLot_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!timerStarted)
            {
                timerStarted = true;
                StartTimer();
            }
            if (e.KeyChar != (char)Keys.Enter) return;
            timerStarted = false;
            if (string.IsNullOrWhiteSpace(txtReelLot.Text))
            {
                new DataAccess().insertMasterLogs($"Invalid Reel Lot. " + txtReelLot.Text, txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError("Invalid Reel Lot.\nPlease Scan again.", txtReelLot);
                return;
            }
            else {
                var mat = new DataAccess().AXCheckMaterial(txtReelLot.Text, txtReelID.Text);
                if (mat.Count <= 0)
                {
                    new DataAccess().insertMasterLogs($"Reel Lot not found in AX. " + txtReelLot.Text, txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                    ShowError("Reel Lot not found in AX.\nPlease Scan again.", txtReelLot);
                    return;
                }else
                {
                    if (AXMaterial.ErrorMsg != "")
                    {
                        new DataAccess().insertMasterLogs(AXMaterial.ErrorMsg + " " + txtReelLot.Text, txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                        ShowError(AXMaterial.ErrorMsg, txtReelLot);
                        return;
                    }
                    else
                    {
                        txtReelLot.Enabled = false;
                        cmbTestProg.Enabled = true;
                        cmbTestProg.DroppedDown = true;
                    }
                }
            }
        }

        private void cmbTestProg_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtTestProgram.Enabled == false)
            {
                txtLotNaming.Clear();
                txtTestProgram.Enabled = true;
            }
            else
            {
                txtTestProgram.Clear();
                txtTestProgram.Enabled = true;
                txtLotNaming.Enabled = false;
                txtLotNaming.Clear();
                txtLotNamingSeq.Clear();
                btnLaunch.Enabled = false;
                btnLaunch.BackColor = Color.Gray;
            }

        }

        private void cmbTestProg_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtTestProgram.Enabled == false)
            {
                txtTestProgram.Enabled = true;
                txtTestProgram.Focus();
            }
            else
            {
                txtTestProgram.Clear();
                txtTestProgram.Enabled = true;
                txtTestProgram.Focus();
                txtLotNaming.Enabled = false;
                txtLotNaming.Clear();
                txtLotNamingSeq.Clear();
                btnLaunch.Enabled = false;
                btnLaunch.BackColor = Color.Gray;
            }
        }

        private void CleanLocalFolders(string basePath)
        {
            if (!Directory.Exists(basePath)) return;
            foreach (var folder in Directory.GetDirectories(basePath))
            {
                try
                {
                    Directory.Delete(folder, true);
                }
                catch (Exception ex)
                {
                    new DataAccess().insertMasterLogs($"Failed to delete folder: {folder} - {ex.Message}", txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                }
            }
        }

        static void DownloadAllFilesAndFolders(string ftpUrl, string localFolder, string username, string password)
        {
            try
            {
                // Step 1: List directory contents using ListDirectoryDetails (to check if entry is file/folder)
                FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(ftpUrl);
                listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                listRequest.Credentials = new NetworkCredential(username, password);
                listRequest.UsePassive = true;
                listRequest.UseBinary = true;
                listRequest.KeepAlive = false;

                List<string> files = new List<string>();
                List<string> folders = new List<string>();

                using (FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse())
                using (Stream listStream = listResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(listStream))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] parts = line.Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 9)
                            continue;

                        string name = parts[8];
                        string permissions = parts[0];

                        if (permissions.StartsWith("d")) // directory
                        {
                            if (name != "." && name != "..")
                                folders.Add(name);
                        }
                        else // file
                        {
                            files.Add(name);
                        }
                    }
                }
                // Step 2: Create local folder if needed
                if (!Directory.Exists(localFolder))
                    Directory.CreateDirectory(localFolder);
                // Step 3: Download files
                foreach (string file in files)
                {
                    string sourceUrl = ftpUrl + "/" + file;
                    string destinationPath = Path.Combine(localFolder, file);

                    Console.WriteLine("⬇ Downloading: " + file);

                    FtpWebRequest downloadRequest = (FtpWebRequest)WebRequest.Create(sourceUrl);
                    downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                    downloadRequest.Credentials = new NetworkCredential(username, password);
                    downloadRequest.UsePassive = true;
                    downloadRequest.UseBinary = true;
                    downloadRequest.KeepAlive = false;

                    using (FtpWebResponse response = (FtpWebResponse)downloadRequest.GetResponse())
                    using (Stream ftpStream = response.GetResponseStream())
                    using (FileStream fileStream = new FileStream(destinationPath, FileMode.Create))
                    {
                        byte[] buffer = new byte[10240];
                        int bytesRead;
                        while ((bytesRead = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fileStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
                // Step 4: Recursively download folders
                foreach (string folder in folders)
                {
                    string remoteSubfolder = ftpUrl.TrimEnd('/') + "/" + folder;
                    string localSubfolder = Path.Combine(localFolder, folder);
                    DownloadAllFilesAndFolders(remoteSubfolder, localSubfolder, username, password);
                }
            }
            catch (Exception ex)
            {
                Global.hasTestProg = false;
                new DataAccess().insertMasterLogs(ex.Message, "", "", "", "", CentralTest.MachineName, GetLocalIPAddress());
            }
        }

        private void txtTestProgram_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!timerStarted)
            {
                timerStarted = true;
                StartTimer();
            }
            if (e.KeyChar != (char)Keys.Enter) return;
            timerStarted = false;
            if (cmbTestProg.SelectedItem?.ToString() != txtTestProgram.Text) {
                new DataAccess().insertMasterLogs("Invalid Testprogram not match. " + txtTestProgram.Text, txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError("Invalid Testprogram not match.\nPlease Scan again.", txtTestProgram);
                return;
            } else {
                var loadForm = new Loading(); loadForm.Show();
                string FTPPath = LotInfo.FTPpath + CentralTest.Source2;
                CleanLocalFolders(CentralTest.Destination);
                if (!Directory.Exists(CentralTest.Destination)) Directory.CreateDirectory(CentralTest.Destination);

                Global.hasTestProg = true;
                DownloadAllFilesAndFolders(FTPPath + LotInfo.TestProgramFolder, CentralTest.Destination + "\\" + LotInfo.TestProgramFolder, CentralTest.Username, CentralTest.Password);

                if (!Global.hasTestProg) {
                    new DataAccess().insertMasterLogs("Test Program not available in the server. " + LotInfo.TestProgramFolder, txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), FTPPath, CentralTest.MachineName, GetLocalIPAddress());
                    ShowError("Test Program not available in the server.\nPlease contact engineer.", txtTestProgram);
                }
                else
                {
                    txtTestProgram.Enabled = false;
                    txtLotNaming.Enabled = true;
                }
                loadForm.Hide();
            }
        }

        private void txtLotNaming_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!timerStarted)
            {
                timerStarted = true;
                StartTimer();
            }
            if (e.KeyChar != (char)Keys.Enter) return;

            timerStarted = false;

            var list = LotInfo.LotNaming.Split(',');

            bool isMatch = list.Any(x => x.Equals(txtLotNaming.Text.Trim(), StringComparison.OrdinalIgnoreCase));

            if (isMatch)
            {
                string cmbPrg = cmbTestProg.SelectedItem?.ToString().Trim();
                int cmblastUnderscore = cmbPrg.LastIndexOf('_');
                string prgresult = "";
                if (cmblastUnderscore >= 0 && cmblastUnderscore < cmbPrg.Length - 1)
                {
                    prgresult = cmbPrg.Substring(cmblastUnderscore + 1);
                }

                string lot = txtLotNaming.Text.Trim();
                int lastUnderscore = lot.LastIndexOf('_');
                string result = "";
                if (lastUnderscore >= 0 && lastUnderscore < lot.Length - 1)
                {
                    result = lot.Substring(lastUnderscore + 1);
                }

                if (result == "Q")
                {
                    Global.CurrentTPLStage = "QA";
                }
                else if (result == "FR")
                {
                    Global.CurrentTPLStage = "Retest";
                }
                else if (result == "FL")
                {
                    Global.CurrentTPLStage = "Leads";
                }
                else
                {
                    Global.CurrentTPLStage = "Test";
                }

                if (LotInfo.FirstTest == true && result != "FT")
                {
                    new DataAccess().insertMasterLogs("Invalid lot naming. Please scan. " + LotInfo.TPL_Stage + " Lot naming format " + cmbTestProg.SelectedItem?.ToString() + " " + txtLotNaming.Text,
                                                       txtLotnumber.Text,
                                                       LotInfo.Device,
                                                       LotInfo.CustomerCode.ToString(),
                                                       "",
                                                       CentralTest.MachineName,
                                                       GetLocalIPAddress());
                    ShowError("Invalid lot naming.\nPlease scan. " + LotInfo.TPL_Stage + " Lot naming format", txtLotNaming);
                    return;
                }

                if (prgresult == "QC.prg" && result != "Q")
                {
                    new DataAccess().insertMasterLogs("Invalid lot naming. Please scan. QA Lot naming format " + cmbTestProg.SelectedItem?.ToString() + " " + txtLotNaming.Text,
                                                       txtLotnumber.Text,
                                                       LotInfo.Device,
                                                       LotInfo.CustomerCode.ToString(),
                                                       "",
                                                       CentralTest.MachineName,
                                                       GetLocalIPAddress());
                    ShowError("Invalid lot naming.\nPlease scan. QA Lot naming format", txtLotNaming);
                    return;
                }

                if (prgresult == "FT.prg" && result == "Q")
                {
                    new DataAccess().insertMasterLogs("Invalid lot naming. Please scan. " + LotInfo.TPL_Stage + " Lot naming format " + cmbTestProg.SelectedItem?.ToString() + " " + txtLotNaming.Text,
                                                       txtLotnumber.Text,
                                                       LotInfo.Device,
                                                       LotInfo.CustomerCode.ToString(),
                                                       "",
                                                       CentralTest.MachineName,
                                                       GetLocalIPAddress());
                    ShowError("Invalid lot naming.\nPlease scan. " + LotInfo.TPL_Stage + " Lot naming format", txtLotNaming);
                    return;
                }

                if (Global.TPLStage == "Retest" && result == "FT")
                {
                    new DataAccess().insertMasterLogs("Invalid lot naming. Please scan. " + LotInfo.TPL_Stage + " Lot naming format " + cmbTestProg.SelectedItem?.ToString() + " " + txtLotNaming.Text,
                                                       txtLotnumber.Text,
                                                       LotInfo.Device,
                                                       LotInfo.CustomerCode.ToString(),
                                                       "",
                                                       CentralTest.MachineName,
                                                       GetLocalIPAddress());
                    ShowError("Invalid lot naming.\nPlease scan. " + LotInfo.TPL_Stage + " Lot naming format", txtLotNaming);
                    txtLotNaming.Clear();
                    return;
                }

                var LotSeq = new DataAccess().GetLotNamingSeq(Global.CurrentTPLStage);

                if (LotSeq.Count > 0)
                {
                    txtLotNamingSeq.Text = NamingSeq.LotNamingSequence.ToString();
                }
                else
                {
                    txtLotNamingSeq.Text = "1";
                }

                File.WriteAllText(AutoFillDetails, string.Join(Environment.NewLine,
                Path.GetFileNameWithoutExtension(txtTestProgram.Text),
                LotInfo.TestProgramFolder,
                txtTestProgram.Text,
                txtLotNaming.Text,
                User.Emp_Name,
                User.Password));

                Global.LotNaming = txtLotNaming.Text;
                Global.LotNamingSequence = Convert.ToInt64(txtLotNamingSeq.Text);
                txtLotNaming.Enabled = false;
                btnLaunch.Enabled = true;
                btnLaunch.BackColor = Color.Green;

            }
            else
            {
                new DataAccess().insertMasterLogs("Invalid lot naming. " + txtLotNaming.Text, txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError("Invalid lot naming.\nPlease scan ." + LotInfo.TPL_Stage + " Lot naming format", txtLotNaming);
            }
        }

        private void btnLaunch_Click(object sender, EventArgs e)
        {
            bool success = new DataAccess().StartTLPLogs(
                txtLotnumber.Text,
                txtTestProgram.Text,
                txtLBoard.Text,
                txtHIBs.Text,
                txtHIBs2.Text,
                txtHIBs3.Text,
                txtHIBs4.Text,
                txtCable.Text,
                txtCable2.Text,
                txtCable3.Text,
                txtCable4.Text,
                txtCarrierLot.Text,
                txtCoverLot.Text,
                txtReelLot.Text,
                CentralTest.MachineName,
                GetLocalIPAddress());
            if (success)
            {
                // Launch external process
                Process.Start(Path.Combine(baseDirectory, LoadTestProg));
                
                // Exit application
                Application.Exit();
            }
            else
            {
                new  DataAccess().insertMasterLogs("Failed to insert logs. Application will not launch. " + LotInfo.TestProgramFolder, txtLotnumber.Text, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                MessageBox.Show("Failed to insert logs. Application will not launch.\nPlease Try Again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

    }
}
