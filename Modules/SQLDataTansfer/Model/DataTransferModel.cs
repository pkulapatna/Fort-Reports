using Prism.Events;
using Prism.Mvvm;
using RTRep.Services;
using SQLDataTansfer.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static RTRep.Services.ClsApplicationService;

namespace SQLDataTansfer.Model
{
    public class DataTransferModel
    {
        protected readonly IEventAggregator _eventAggregator;
        private ClsSQLhandler SqlHandler;
        private Xmlhandler MyXml = Xmlhandler.Instance;

        private string path = Directory.GetCurrentDirectory();
        private string TargetRTDir = Directory.GetCurrentDirectory() + @"\RemoteSpoolRT";

        
        public static string strRtTransferFile;

        private string TargetWLDir;

        #region RealTime Transfer

        private ServiceEventsTimer RtXferTimer;
        private ObservableCollection<SqlOutFields> SqlOutputFields { get; set; }

        public int NewUID = 0;
        public int CurUID
        {
            get => Settings.Default.CurrUID;
            set
            {
                Settings.Default.CurrUID = value;
                Settings.Default.Save();
            }
        }

        public string XferRtServerName
        {
            get => Settings.Default.XferRtServerName;
            set
            {
                Settings.Default.XferRtServerName = value;
                Settings.Default.Save();
            }
        }
        public string XferRtHost
        {
            get => Settings.Default.xferRtHost;
            set
            {
                Settings.Default.xferRtHost = value;
                Settings.Default.Save();
            }
        }
        public string XferRtInstant
        {
            get => Settings.Default.xferRtInstant;
            set
            {
                Settings.Default.xferRtInstant = value;
                Settings.Default.Save();
            }
        }
        public string XferRtDbName
        {
            get => Settings.Default.XferRtDbName;
            set
            {
                Settings.Default.XferRtDbName = value;
                Settings.Default.Save();
            }
        }

        public string XferRtTableName
        {
            get => Settings.Default.XferRtTableName;
            set
            {
                Settings.Default.XferRtTableName = value;
                Settings.Default.Save();
            }
        }

        public string XferRtUserId
        {
            get => Settings.Default.xferRtUserId;
            set
            {
                Settings.Default.xferRtUserId = value;
                Settings.Default.Save();
            }
        }

        public string XferRtUserPwd
        {
            get => Settings.Default.xferRtUserPwd;
            set
            {
                Settings.Default.xferRtUserPwd = value;
                Settings.Default.Save();
            }
        }

        public bool RtSQLAuthenticate
        {
            get => Settings.Default.RtSQLAuthenticate;
            set
            {
                Settings.Default.RtSQLAuthenticate = value;
                Settings.Default.Save();
            }
        }

        public bool RtXferScannerOn 
        { 
            get => Settings.Default.RtXferScannerOn; 
            set
            {
                Settings.Default.RtXferScannerOn = value;
                Settings.Default.Save();

                if(value)
                {
                    bool BTesting = TestSqlConnection(XferRtHost, XferRtInstant, XferRtDbName, XferRtUserId, XferRtUserPwd, RtSQLAuthenticate);
                    UpdateXferStatus((BTesting) ? "SQL connections Good" : "SQL connections Fail");
                }
                StartRtDataXfer(value);
            }
        }



        private void StartRtDataXfer(bool value)
        {
            try
            {
                if (value)
                {
                    bool ConStatus = SqlHandler.TestXferSqlConnection(XferRtHost, XferRtInstant, XferRtDbName, XferRtUserId, XferRtUserPwd, RtSQLAuthenticate);

                    if (ConStatus)
                    {
                        SqlOutputFields = MyXml.ReadRtXfertable();
                        if (SqlOutputFields.Count > 0)
                            RtXferTimer.StartXferRtTimer("XferRtTimer");
                        UpdateXferStatus("Transfer Timer Start!");
                    }
                    else
                    {
                        MessageBox.Show($"No Connections to the target SQL Server! {XferRtHost}");
                        UpdateXferStatus("Timer Not Start No connection to Remote SQL!");
                    }
                }
                else
                {
                    RtXferTimer.StopXferRtTimer();
                    UpdateXferStatus("Transfer Timer Stop!");
                }      
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in StartRtDataXfer {ex.Message}");
            }
        }

        /// <summary>
        /// Update New Data
        /// </summary>
        /// <param name="obj"></param>
        private void UpdateXferRTTimerEvent(DateTime obj)
        {
           bool bDone = WriteRTXferStrtofile(SqlOutputFields, XferRtDbName, XferRtTableName);
           if (bDone) UpdateXferStatus($"Update Rt Remote SQL Server{DateTime.Now.ToString("HH:mm")} ");
           ProcessFilesData();
           
            /*
            bool XferGood = SqlHandler.UpdateRemoteSQLTableAsy(SqlOutputFields, XferRtDbName, XferRtTableName, true);
            UpdateXferStatus((XferGood) ? $"SQL Data Transfer to {XferRtTableName} {DateTime.Now.ToString("HH:mm")}" :
                                             "Data Not Transfer to Client SQL Server");
            */


        }

        private void ProcessFilesData()
        {
            string[] files = System.IO.Directory.GetFiles(TargetRTDir);
            if(files.Length > 0)
            {
                for (var i = 0; i < files.Length; i++)
                {
                    bool BGood =  SqlHandler.UpdateRemoteRtSqlTable(File.ReadAllText(files[i]));
                    if(BGood)File.Delete(Path.Combine(TargetRTDir, files[i]));
                }
            }
        }


        public bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }



        private bool WriteRTXferStrtofile(ObservableCollection<SqlOutFields> sqlOutputFields, string xferRtDbName, string targettable)
        {
            bool WriteDone = false;
            string fortehdrlist = string.Empty;
            string clienthdrlist = string.Empty;
            char[] charsToTrim = { ',' };
            List<Tuple<string, string>> newForteDatastr = new List<Tuple<string, string>>();
            string StrInsert = string.Empty;

            try
            {
                foreach (var item in sqlOutputFields)
                {
                    fortehdrlist += item.ForteFieldName + ",";
                    clienthdrlist += item.ClientFieldName + ",";
                }
                fortehdrlist = fortehdrlist.Trim(charsToTrim);
                clienthdrlist = clienthdrlist.Trim(charsToTrim);
                newForteDatastr = SqlHandler.GetLatestForteDataLst($"{fortehdrlist},UID");
                NewUID = Convert.ToInt32(newForteDatastr[newForteDatastr.Count - 1].Item1);
                newForteDatastr.RemoveAt(newForteDatastr.Count - 1);

                if (NewUID != CurUID)
                {
                    StrInsert = BuildInsertRtQueryString(fortehdrlist, targettable, newForteDatastr, xferRtDbName);
                    CurUID = NewUID;

                    strRtTransferFile = TargetRTDir + @"\" + NewUID + ".txt";
                   
                    if (!Directory.Exists(TargetRTDir))
                        Directory.CreateDirectory(TargetRTDir);

                    if (!File.Exists(strRtTransferFile))
                    {
                        using (StreamWriter writer = new StreamWriter(strRtTransferFile, false)) //// true to append data to the file
                        {
                            writer.WriteLine(StrInsert);
                        }
                    }
                    WriteDone = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in WriteRTXferStrtofile {ex.Message}");
            }
            return WriteDone;
        }

        #endregion RealTime Transfer

        public bool WlXferScannerOn 
        { 
            get => Settings.Default.WlXferScannerOn;
            set
            {
                Settings.Default.WlXferScannerOn = value;
                Settings.Default.Save();
            }
        }



        public DataTransferModel(IEventAggregator eventAggregator)
        {
           this._eventAggregator = eventAggregator;
            SqlHandler = ClsSQLhandler.Instance;
            RtXferTimer = new ServiceEventsTimer(_eventAggregator);

           
            TargetWLDir = path + @"\RemoteSpoolWL";
            if (!Directory.Exists(TargetWLDir))
                Directory.CreateDirectory(TargetWLDir);

            _eventAggregator.GetEvent<UpdateRtXferTimerEvents>().Subscribe(UpdateXferRTTimerEvent);
        }

        internal List<Tuple<string, string>> GetColumnList(string DbName, string TableName)
        {
            DataTable HdrTable = new DataTable();
            List<Tuple<string, string>> SqlColHdrList = new List<Tuple<string, string>>();

            try
            {
                HdrTable = SqlHandler.GetClientTableHdr(DbName, TableName);
                foreach (DataRow item in HdrTable.Rows)
                {
                    SqlColHdrList.Add(new Tuple<string, string>(item[1].ToString(), item[2].ToString()));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in GetColumnList " + ex.Message);
            }
            return SqlColHdrList;
        }

        internal ObservableCollection<string> GetArchTableHdr()
        {
            ObservableCollection<string> ItemHdr = new ObservableCollection<string>();
            DataTable HdrTable = new DataTable();
            List<Tuple<string, string>> SqlList = new List<Tuple<string, string>>();

            try
            {
                HdrTable = SqlHandler.GetSqlScema();

                foreach (DataRow item in HdrTable.Rows)
                {
                    SqlList.Add(new Tuple<string, string>(item[1].ToString(), item[2].ToString()));
                    ItemHdr.Add(item[1].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in GetArchTableHdr " + ex.Message);
            }
            return ItemHdr;
        }

        internal bool TestSqlConnection(string xferHost, string xferInstant, string dbTableName, string xferUserId, string xferUserPwd, bool sQLAuChecked)
        {
            return SqlHandler.TestXferSqlConnection(xferHost, xferInstant,dbTableName,xferUserId,xferUserPwd,sQLAuChecked);
        }

        internal bool TestSendRtRemote(ObservableCollection<SqlOutFields> sqlOutputFields, string xferRtDbName, string targettable, bool TestMode)
        {
            string fortehdrlist = string.Empty;
            string clienthdrlist = string.Empty;
            char[] charsToTrim = { ',' };
            List<Tuple<string, string>> newForteDatastr = new List<Tuple<string, string>>();
            string StrInsert = string.Empty;

            try
            {
                foreach (var item in sqlOutputFields)
                {
                    fortehdrlist += item.ForteFieldName + ",";
                    clienthdrlist += item.ClientFieldName + ",";
                }
                fortehdrlist = fortehdrlist.Trim(charsToTrim);
                clienthdrlist = clienthdrlist.Trim(charsToTrim);

                if (TestMode)
                {
                    newForteDatastr = SqlHandler.GetLatestForteDataLst(fortehdrlist);
                    StrInsert = BuildInsertRtQueryString(fortehdrlist, targettable, newForteDatastr, xferRtDbName);
                }   
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in TestSendRtRemote {ex.Message}");
            }
            return  SqlHandler.UpdateBaleArchiveTableAsy(true,StrInsert);
        }

        private string BuildInsertRtQueryString(string fortehdrlist, string targettable, List<Tuple<string, string>> newForteDatastr, string xferRtDbName)
        {
            string strQuery = string.Empty;
            char[] charsToTrim = { ',' };
            List<string> OutputList = new List<string>();
            string Outputstring = string.Empty;

            try
            {
                for (int i = 0; i < newForteDatastr.Count; i++)
                {
                    switch (newForteDatastr[i].Item2)
                    {
                        case "System.Int32":
                            OutputList.Add(newForteDatastr[i].Item1 + ',');
                            break;

                        case "bit":
                            OutputList.Add($"'{newForteDatastr[i].Item1}',");
                            break;

                        case "System.String":
                            OutputList.Add($"'{newForteDatastr[i].Item1}',");
                            break;

                        case "System.Single":
                        case "System.Double":
                            OutputList.Add(newForteDatastr[i].Item1 + ',');
                            break;

                        case "smallint":
                            OutputList.Add(newForteDatastr[i].Item1 + ',');
                            break;

                        case "System.DateTime":
                            OutputList.Add($"'{newForteDatastr[i].Item1}',");
                            break;
                        default:
                            OutputList.Add(newForteDatastr[i].Item1 + ',');
                            break;
                    }
                }
                for (int x = 0; x < OutputList.Count; x++)
                {
                    Outputstring += OutputList[x];
                }
                Outputstring = Outputstring.TrimEnd(',');
                strQuery = $"USE[{xferRtDbName}] INSERT INTO {targettable}({fortehdrlist}) VALUES ({Outputstring});";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in ClsSQLhandler BuildInsertRtQueryString -> {ex.Message}");
            }
            return strQuery;
        }

        internal bool TestSendRtRemote2(ObservableCollection<SqlOutFields> sqlOutputFields, string XferRtDbName, string targettable, bool TestMode)
        {
            return SqlHandler.SendRtRemote(sqlOutputFields, targettable, XferRtDbName);
        }



        private void UpdateXferStatus(string status)
        {
            _eventAggregator.GetEvent<UpdateXferStatus>().Publish(status);
        }
    }
    
}
