using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;

namespace CentralTestTPL
{
    public class DataAccess
    {
        AppSQLConfig Sqlhandler = new AppSQLConfig();

        public List<User> selectUser(string empNo)
        {
            Sqlhandler.SetToEMMS1();
            List<User> userDetails = new List<User>();
            String sqltext = "[usp_CentralTest_TPL_Select_Emp]";//sp name
            Sqlhandler.CreateParameter(1);
            Sqlhandler.SetParameterValues(0, "@Emp", SqlDbType.NVarChar, empNo);
            DataTable dt = new DataTable();

            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.FillDataTable(sqltext, ref dt, CommandType.StoredProcedure))
                {

                }
                userDetails = ConvertDataTable<User>(dt);
            }
            Sqlhandler.CloseConnection();
            return userDetails;
        }

        public List<CentralTest> selectMachine(string machine)
        {
            Sqlhandler.SetToEMMS1();
            List<CentralTest> machDetails = new List<CentralTest>();
            String sqltext = "[usp_CentralTest_TPL_Select_Machine]";//sp name
            Sqlhandler.CreateParameter(1);
            Sqlhandler.SetParameterValues(0, "@Machine", SqlDbType.NVarChar, machine);

            DataTable dt = new DataTable();

            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.FillDataTable(sqltext, ref dt, CommandType.StoredProcedure))
                {

                }
                machDetails = ConvertDataTable<CentralTest>(dt);
            }
            Sqlhandler.CloseConnection();
            return machDetails;
        }

        public List<LotInfo> SelectLotInfo(string LotNo)
        {
            Sqlhandler.SetToEMMS1();
            List<LotInfo> lotInfo = new List<LotInfo>();
            try
            {
                var suffix = LotNo.Substring(LotNo.LastIndexOf('-'));

                String sqltext = "[usp_CentralTest_TPL_Get_Lot_Details]";//sp name
                Sqlhandler.CreateParameter(2);
                Sqlhandler.SetParameterValues(0, "@Lot", SqlDbType.NVarChar, LotNo);
                Sqlhandler.SetParameterValues(1, "@Machine", SqlDbType.NVarChar, CentralTest.MachineName);
                DataTable dt = new DataTable();
                if (Sqlhandler.OpenConnection())
                {
                    if (Sqlhandler.FillDataTable(sqltext, ref dt, CommandType.StoredProcedure))
                    {
                        lotInfo = ConvertDataTable<LotInfo>(dt);
                    }
                }
                Sqlhandler.CloseConnection();
            }
            catch (Exception ex)
            {

            }
            return lotInfo;
        }

        public List<Paths> selectPaths(long CustomerCode)
        {
            Sqlhandler.SetToEMMS1();
            List<Paths> pathInfo = new List<Paths>();
            String sqltext = "[TPL_SELECTPATH]";//sp name
            Sqlhandler.CreateParameter(1);
            Sqlhandler.SetParameterValues(0, "@CustomerCode", SqlDbType.BigInt, CustomerCode);

            DataTable dt = new DataTable();

            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.FillDataTable(sqltext, ref dt, CommandType.StoredProcedure))
                {
     
                }
                pathInfo = ConvertDataTable<Paths>(dt);
            }
            Sqlhandler.CloseConnection();
            return pathInfo;
        }

        public List<AXMaterial> AXCheckMaterial (string matLot, string matSID)
        {
            Sqlhandler.SetToEMMS1();
            List<AXMaterial> AXInfo = new List<AXMaterial>();

            Sqlhandler.CreateParameter(2);
            Sqlhandler.SetParameterValues(0, "@MatLot", SqlDbType.NVarChar, matLot);
            Sqlhandler.SetParameterValues(1, "@SID", SqlDbType.NVarChar, matSID);

            DataTable dt = new DataTable();

            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.FillDataTable("usp_CentralTest_TPL_Check_Material_Lot", ref dt, CommandType.StoredProcedure))
                {

                }
                AXInfo = ConvertDataTable<AXMaterial>(dt);
            }
            Sqlhandler.CloseConnection();
            return AXInfo;
        }

        public bool selectToLogs(string fname)
        {
            Sqlhandler.SetToEMMS1();
            bool result = false;
            SqlDataReader dr = null;
            String sqltext = "[usp_TPL_IXYS_Select_Logs]";
            Sqlhandler.CreateParameter(1);
            Sqlhandler.SetParameterValues(0, "@FName", SqlDbType.NVarChar, fname);
            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.FillDataReader(sqltext, ref dr, CommandType.StoredProcedure))
                {
                    if (dr.HasRows)
                    {
                        result = true;
                    }
                }
            }
            Sqlhandler.CloseConnection();
            return result;
        }

        public bool checkCORBIN(string test, string ip)
        {
            Sqlhandler.SetToEMMS1();
            bool result = false;
            SqlDataReader dr = null;
            DataTable dt = new DataTable();
            new List<CORR>();
            Sqlhandler.CreateParameter(2);
            Sqlhandler.SetParameterValues(0, "@test", SqlDbType.NVarChar, test);
            Sqlhandler.SetParameterValues(1, "@ipAdd", SqlDbType.NVarChar, ip);
            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.FillDataReader("usp_CentralTest_TPL_Check_CORRBIN", ref dr, CommandType.StoredProcedure))
                {
                    if (dr.HasRows)
                    {
                        result = true;
                        if (test == "-CORR") {
                            AutoCORR(test, ip);
                        }
                        else {
                            AutoBINNING(test, ip);
                        }
                    }
                }
            }
            Sqlhandler.CloseConnection();
            return result;
        }

        public List<CORR> AutoCORR(string test, string ip)
        {
            Sqlhandler.SetToEMMS1();
            List<CORR> TEST = new List<CORR>();
            Sqlhandler.CreateParameter(2);
            Sqlhandler.SetParameterValues(0, "@test", SqlDbType.NVarChar, test);
            Sqlhandler.SetParameterValues(1, "@ipAdd", SqlDbType.NVarChar, ip);
            DataTable dt = new DataTable();
            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.FillDataTable("usp_CentralTest_TPL_Check_CORRBIN", ref dt, CommandType.StoredProcedure))
                {

                }
                TEST = ConvertDataTable<CORR>(dt);
            }
            Sqlhandler.CloseConnection();
            return TEST;
        }

        public List<BINNING> AutoBINNING(string test, string ip)
        {
            Sqlhandler.SetToEMMS1();
            List<BINNING> TEST = new List<BINNING>();
            Sqlhandler.CreateParameter(2);
            Sqlhandler.SetParameterValues(0, "@test", SqlDbType.NVarChar, test);
            Sqlhandler.SetParameterValues(1, "@ipAdd", SqlDbType.NVarChar, ip);
            DataTable dt = new DataTable();
            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.FillDataTable("usp_CentralTest_TPL_Check_CORRBIN", ref dt, CommandType.StoredProcedure))
                {

                }
                TEST = ConvertDataTable<BINNING>(dt);
            }
            Sqlhandler.CloseConnection();
            return TEST;
        }

        public List<NamingSeq> GetLotNamingSeq(string TPL_Stage)
        {
            Sqlhandler.SetToEMMS1();
            List<NamingSeq> LotSeq = new List<NamingSeq>();
            Sqlhandler.CreateParameter(2);
            Sqlhandler.SetParameterValues(0, "@lotNumber", SqlDbType.NVarChar, LotInfo.LotNumber);
            Sqlhandler.SetParameterValues(1, "@TPLstage", SqlDbType.NVarChar, TPL_Stage);
            DataTable dt = new DataTable();
            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.FillDataTable("usp_CentralTest_TPL_Get_LotNaming_Sequence", ref dt, CommandType.StoredProcedure))
                {

                }
                LotSeq = ConvertDataTable<NamingSeq>(dt);
            }
            Sqlhandler.CloseConnection();
            return LotSeq;
        }

        public bool insertMasterLogs(string Logs, string lot, string device, string custcode, string destination, string machine, string IP)
        {
            Sqlhandler.SetToEMMS1();
            bool result = false;
            String sqltext = "[usp_CentralTest_TPL_Insert_Logs]";//sp name
            Sqlhandler.CreateParameter(9);
            Sqlhandler.SetParameterValues(0, "@Logs", SqlDbType.NVarChar, Logs);
            Sqlhandler.SetParameterValues(1, "@Lot", SqlDbType.NVarChar, lot);
            Sqlhandler.SetParameterValues(2, "@Device", SqlDbType.NVarChar, device);
            Sqlhandler.SetParameterValues(3, "@CustCode", SqlDbType.NVarChar, custcode);
            Sqlhandler.SetParameterValues(4, "@dest", SqlDbType.NVarChar, destination);
            Sqlhandler.SetParameterValues(5, "@Machine", SqlDbType.NVarChar, machine);
            Sqlhandler.SetParameterValues(6, "@IpAdd", SqlDbType.NVarChar, IP);
            Sqlhandler.SetParameterValues(7, "@Ucode", SqlDbType.NVarChar, "admin");
            Sqlhandler.SetParameterValues(8, "@App", SqlDbType.BigInt, 2);
            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.ExecuteNonQuery(sqltext, CommandType.StoredProcedure))
                {
                    result = true;
                }
            }
            return result;
        }

        public bool UpdateHardWare(string LB, string HB1, string HB2, string HB3, string HB4, string CB1, string CB2, string CB3, string CB4)
        {
            Sqlhandler.SetToEMMS1();
            bool result = false;
            Sqlhandler.CreateParameter(11);
            Sqlhandler.SetParameterValues(0, "@LB", SqlDbType.NVarChar, LB);
            Sqlhandler.SetParameterValues(1, "@HB1", SqlDbType.NVarChar, HB1);
            Sqlhandler.SetParameterValues(2, "@HB2", SqlDbType.NVarChar, HB2);
            Sqlhandler.SetParameterValues(3, "@HB3", SqlDbType.NVarChar, HB3);
            Sqlhandler.SetParameterValues(4, "@HB4", SqlDbType.NVarChar, HB4);
            Sqlhandler.SetParameterValues(5, "@CB1", SqlDbType.NVarChar, CB1);
            Sqlhandler.SetParameterValues(6, "@CB2", SqlDbType.NVarChar, CB2);
            Sqlhandler.SetParameterValues(7, "@CB3", SqlDbType.NVarChar, CB3);
            Sqlhandler.SetParameterValues(8, "@CB4", SqlDbType.NVarChar, CB4);
            Sqlhandler.SetParameterValues(9, "@CORRtestID", SqlDbType.BigInt, CORR.ID);
            Sqlhandler.SetParameterValues(10, "@BINtestID", SqlDbType.NVarChar, BINNING.ID);
            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.ExecuteNonQuery("usp_CentralTest_TPL_Update_Hardware", CommandType.StoredProcedure))
                {
                    result = true;
                }
            }
            return result;
        }

        public bool StartTLPLogs(string LotAlias,
                                 string TestProgram,
                                 string LBoard,
                                 string HB1,
                                 string HB2,
                                 string HB3,
                                 string HB4,
                                 string Cable1,
                                 string Cable2,
                                 string Cable3,
                                 string Cable4,
                                 string Carrier,
                                 string Cover,
                                 string Reel,
                                 string Machine,
                                 string IpAdd)
        {
            Sqlhandler.SetToEMMS1();
            bool result = false;
            String sqltext = "[usp_CentralTest_TPL_LaunchApp_Insert_Logs]";//sp name
            Sqlhandler.CreateParameter(31);
            Sqlhandler.SetParameterValues(0, "@LotCode", SqlDbType.BigInt, LotInfo.LotCode);
            Sqlhandler.SetParameterValues(1, "@LotAlias", SqlDbType.NVarChar, LotAlias);
            Sqlhandler.SetParameterValues(2, "@LotNumber", SqlDbType.NVarChar, LotInfo.LotNumber);
            Sqlhandler.SetParameterValues(3, "@CustomerCode", SqlDbType.BigInt, LotInfo.CustomerCode);
            Sqlhandler.SetParameterValues(4, "@PkgLD", SqlDbType.NVarChar, LotInfo.PkgLD);
            Sqlhandler.SetParameterValues(5, "@LdType", SqlDbType.NVarChar, LotInfo.LdType);
            Sqlhandler.SetParameterValues(6, "@ProductID", SqlDbType.NVarChar, LotInfo.ProductID);
            Sqlhandler.SetParameterValues(7, "@Device", SqlDbType.NVarChar, LotInfo.Device);
            Sqlhandler.SetParameterValues(8, "@ProductCode", SqlDbType.BigInt, LotInfo.ProductCode);
            Sqlhandler.SetParameterValues(9, "@SubLotQty", SqlDbType.BigInt, LotInfo.SubLotQty);
            Sqlhandler.SetParameterValues(10, "@RecipeCode", SqlDbType.BigInt, LotInfo.RecipeCode);
            Sqlhandler.SetParameterValues(11, "@StageID", SqlDbType.NVarChar, LotInfo.StageID);
            Sqlhandler.SetParameterValues(12, "@TestProgram", SqlDbType.NVarChar, TestProgram);
            Sqlhandler.SetParameterValues(13, "@LotNaming", SqlDbType.NVarChar, Global.LotNaming);
            Sqlhandler.SetParameterValues(14, "@LotNamingSequence", SqlDbType.BigInt, Global.LotNamingSequence);
            Sqlhandler.SetParameterValues(15, "@TPL_Stage", SqlDbType.NVarChar, Global.CurrentTPLStage);
            Sqlhandler.SetParameterValues(16, "@LBoard", SqlDbType.NVarChar, LBoard);
            Sqlhandler.SetParameterValues(17, "@Hibs", SqlDbType.NVarChar, HB1);
            Sqlhandler.SetParameterValues(18, "@Hibs2", SqlDbType.NVarChar, HB2);
            Sqlhandler.SetParameterValues(19, "@Hibs3", SqlDbType.NVarChar, HB3);
            Sqlhandler.SetParameterValues(20, "@Hibs4", SqlDbType.NVarChar, HB4);
            Sqlhandler.SetParameterValues(21, "@Cable", SqlDbType.NVarChar, Cable1);
            Sqlhandler.SetParameterValues(22, "@Cable2", SqlDbType.NVarChar, Cable2);
            Sqlhandler.SetParameterValues(23, "@Cable3", SqlDbType.NVarChar, Cable3);
            Sqlhandler.SetParameterValues(24, "@Cable4", SqlDbType.NVarChar, Cable4);
            Sqlhandler.SetParameterValues(25, "@CarrierTape", SqlDbType.NVarChar, Carrier);
            Sqlhandler.SetParameterValues(26, "@CoverTape", SqlDbType.NVarChar, Cover);
            Sqlhandler.SetParameterValues(27, "@Reel", SqlDbType.NVarChar, Reel);
            Sqlhandler.SetParameterValues(28, "@UserCode", SqlDbType.NVarChar, User.Emp_No);
            Sqlhandler.SetParameterValues(29, "@Machine", SqlDbType.NVarChar, Machine);
            Sqlhandler.SetParameterValues(30, "@Ipaddress", SqlDbType.NVarChar, IpAdd);

            if (Sqlhandler.OpenConnection())
            {
                if (Sqlhandler.ExecuteNonQuery(sqltext, CommandType.StoredProcedure))
                {
                    result = true;
                }
            }
            return result;
        }


        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

    }
}
