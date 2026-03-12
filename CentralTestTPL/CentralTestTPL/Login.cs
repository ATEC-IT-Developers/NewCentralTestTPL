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
    public partial class Login : Form
    {
        private Timer timer = new Timer();
        private bool timerStarted = false;
        public Login()
        {
            InitializeComponent();
        }

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

        private void Login_Load(object sender, EventArgs e)
        {
            var appName = "CentralTestTPL";
            int app1Count = Process.GetProcessesByName(appName).Length;
            if (app1Count > 1)
            {
                new DataAccess().insertMasterLogs("TPL is already running., Warning.", "", "", "", "", "", GetLocalIPAddress());
                MessageBox.Show("TPL is already running.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
                return;
            }else{
                txtMachine.Focus();
            }
        }

        private void txtMachine_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!timerStarted)
            {
                timerStarted = true;
                StartTimer();
            }
            if (e.KeyChar != (char)Keys.Enter) return;
            timerStarted = false;
            if (string.IsNullOrWhiteSpace(txtMachine.Text))
            {
                new DataAccess().insertMasterLogs("Invalid Tester.\nPlease Scan again.", "", "", "", "", "", GetLocalIPAddress());
                ShowError("Invalid Tester.\nPlease Scan again.", txtMachine);
                return;
            }
            else
            {
                var list = new DataAccess().selectMachine(txtMachine.Text);

                if (list.Count > 0) {
                    int app2Count = Process.GetProcessesByName(CentralTest.Application2).Length;
                    if (app2Count >= 1)
                    {
                        foreach (var proc in Process.GetProcessesByName(CentralTest.Application2))
                        {
                            try
                            {
                                proc.Kill();
                                proc.WaitForExit();
                            }
                            catch (Exception ex)
                            {
                                new DataAccess().insertMasterLogs($"Failed to close process: {ex.Message}", "", "", "", "", CentralTest.MachineName, GetLocalIPAddress());
                                MessageBox.Show($"Failed to close process: {ex.Message}");
                            }
                        }
                    }
                    txtMachine.Enabled = false;
                    txtHandler.Enabled = true;
                    txtHandler.Focus();
                }
                else
                {
                    new DataAccess().insertMasterLogs("Tester not found in Database. " + txtMachine.Text, "", "", "", "", "", GetLocalIPAddress());
                    ShowError("Tester not found in Database.\nPlease Scan again.", txtMachine);
                }
            }
        }

        private void txtHandler_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!timerStarted)
            {
                timerStarted = true;
                StartTimer();
            }
            if (e.KeyChar != (char)Keys.Enter) return;
            timerStarted = false;
            if (string.IsNullOrWhiteSpace(txtHandler.Text))
            {
                new DataAccess().insertMasterLogs("Invalid Handler.", "", "", "", "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError("Invalid Handler.\nPlease Scan again.", txtHandler);
                return;
            }
            else
            {
                if (txtHandler.Text != CentralTest.Handler)
                {
                    new DataAccess().insertMasterLogs("Invalid Handler. " + txtHandler.Text, "", "", "", "", CentralTest.MachineName, GetLocalIPAddress());
                    ShowError("Invalid Handler.\nPlease Scan again.", txtHandler);
                }
                else
                {
                    txtHandler.Enabled = false;
                    txtUsername.Enabled = true;
                    txtUsername.Focus();
                }
            }
        }

        private void txtUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!timerStarted)
            {
                timerStarted = true;
                StartTimer();
            }
            if (e.KeyChar != (char)Keys.Enter) return;
            timerStarted = false;
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                new DataAccess().insertMasterLogs("Invalid Employee Number.", "", "", "", "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError("Invalid Employee Number.\nPlease Scan again.", txtUsername);
                return;
            }
            else
            {
                var list = new DataAccess().selectUser(txtUsername.Text);
                if (list.Count > 0)
                {
                    this.Hide();
                    new Main().Show();
                }
                else
                {
                    new DataAccess().insertMasterLogs("Employee Number not found in Database. " + txtUsername.Text, "", "", "", "", CentralTest.MachineName, GetLocalIPAddress());
                    ShowError("Employee Number not found in Database.\nPlease Scan again.", txtUsername);
                }
            }

        }
    }
}
