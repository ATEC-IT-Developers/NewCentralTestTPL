using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CentralTestTPL
{
    public class Code
    {
        public static Int64 CustomerCode { get; set; }
    }

    public class User
    {
        public static string Emp_No { get; set; }
        public static string CustomerCodes { get; set; }
        public static string Emp_Name { get; set; }
        public static string Password { get; set; }        
    }

    public class FTPInfo
    {
        public static String FtpProtocol { get; set; }
        public static string FtpServerName { get; set; }
        public static string FtpUserName { get; set; }
        public static string FtpPassword { get; set; }
        public static string FtpDestinationFolder { get; set; }
        public static string FtpSourceFolder { get; set; }
        public static string FtpBackupFolder { get; set; }
        public static string SSHKeyFingerprint { get; set; }
        public static string WaferFileDestination { get; set; }
        public static string WaferFileDestination2 { get; set; }
    }

    public class Paths
    {
        public static string testprogramPath { get; set; }
        public static string sourcePath1 { get; set; }
        public static string sourcePath2 { get; set; }
        public static string sourcePath3 { get; set; }
        public static string destinationPath { get; set; }
        public static string destinationPath2 { get; set; }
        public static string exePath { get; set; }
        public static string exePath2 { get; set; }
        public static string workStationPath { get; set; }
        public static string workStationPath2 { get; set; }
        public static string destDir1 { get; set; }
        public static string destDir2 { get; set; }
        public static string datalogsSourcePath { get; set; }
        public static string datalogsPath { get; set; }
        public static string appName { get; set; }
        public static string datalog { get; set; }
        public static string testprog { get; set; }
        public static string datalogfile { get; set; }
        public static string sFiles { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
    }

    public class LotInfo
    {
        public static string LotNumber { get; set; }
        public static Int64 LotCode { get; set; }
        public static Int64 CustomerCode { get; set; }
        public static string PkgLD { get; set; }
        public static string LdType { get; set; }
        public static string ProductID { get; set; }
        public static string Device { get; set; }
        public static Int64 ProductCode { get; set; }
        public static Int64 SubLotQty { get; set; }
        public static Int64 RecipeCode { get; set; }
        public static string StageID { get; set; }
        public static string TestProgramFolder { get; set; }
        public static string TestProgram { get; set; }
        public static string LBoard { get; set; }
        public static string Hibs { get; set; }
        public static string TPLCable { get; set; }
        public static string CoverTape { get; set; }
        public static string CarrierTape { get; set; }
        public static string Reel { get; set; }
        public static string FTPpath { get; set; }
        public static string LotNaming { get; set; }
        public static string TPL_Stage { get; set; }
        public static string msg { get; set; }
    }

    public class NamingSeq
    {
        public static Int64 LotNamingSequence { get; set; }
    }

    public class Global
    {
        public static string FileName { get; set; }
        public static string MachineName { get; set; }
        public static string SetPosition { get; set; }
        public static string Handler { get; set; }
        public static string windowsOS { get; set; }
        public static bool hasTestProg { get; set; }
        public static string LotNaming { get; set; }
        public static Int64 LotNamingSequence { get; set; }
        public static string TPLStage { get; set; }
        public static bool CheckLB { get; set; }
    }

    public static class SystemUtils
    {
        public static bool isWindowsXP
        {
            get
            {
                var os = Environment.OSVersion;
                return os.Platform == PlatformID.Win32NT &&
                      ((os.Version.Major < 6) ||
                       (os.Version.Major == 6 && os.Version.Minor <= 1));
            }
        }
    }

    public class Machine
    {
        public static string MachineModel { get; set; }
    }

    public class Logs
    {
        public static string Description { get; set; }
        public static string FolderName { get; set; }

    }

    public class CentralTest
    {
        public static string MachineName { get; set; }
        public static string Destination { get; set; }
        public static string Destination2 { get; set; }
        public static string Destination3 { get; set; }
        public static string Destination4 { get; set; }
        public static string Source { get; set; }
        public static string Source2 { get; set; }
        public static string Source3 { get; set; }
        public static string Source4 { get; set; }
        public static string Source5 { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
        public static string Handler { get; set; }
        public static string Application2 { get; set; }
        public static string EngDatalogPath { get; set; }
    }

    public class AXMaterial
    {
        public static string SIDNo { get; set; }
        public static string ErrorMsg { get; set; }
    }

    public class CORR
    {
        public static Int64 ID { get; set; }
        public static string Device { get; set; }
        public static string LBoard { get; set; }
        public static string HIBs1 { get; set; }
        public static string HIBs2 { get; set; }
        public static string HIBs3 { get; set; }
        public static string HIBs4 { get; set; }
        public static string AllHIBs { get; set; }
        public static string Cable1 { get; set; }
        public static string Cable2 { get; set; }
        public static string Cable3 { get; set; }
        public static string Cable4 { get; set; }
        public static string AllCable { get; set; }
        public static string msg { get; set; }
    }

    public class BINNING
    {
        public static Int64 ID { get; set; }
        public static string Device { get; set; }
        public static string LBoard { get; set; }
        public static string HIBs1 { get; set; }
        public static string HIBs2 { get; set; }
        public static string HIBs3 { get; set; }
        public static string HIBs4 { get; set; }
        public static string AllHIBs { get; set; }
        public static string Cable1 { get; set; }
        public static string Cable2 { get; set; }
        public static string Cable3 { get; set; }
        public static string Cable4 { get; set; }
        public static string AllCable { get; set; }
        public static string msg { get; set; }
    }


}
