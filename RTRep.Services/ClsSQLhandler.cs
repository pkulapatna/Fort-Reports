
using RTRep.Services.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RTRep.Services.ClsApplicationService;

namespace RTRep.Services
{
    /// <summary>
    /// All Forte's  Sql tables 00.00 values are in 32 bit float or Single
    /// </summary>
    public class ClsSQLhandler : IDisposable
    {
        public readonly Prism.Events.IEventAggregator _eventAggregator;

        private static readonly object padlock = new object();
        private static ClsSQLhandler instance = null;
        public List<string> RemoveFieldsList = null;

        private Xmlhandler MyXml;
        string DateFormat = string.Empty;
        string TimeFormat = string.Empty;

        public string CurrentBaleTable { get; set; }
        public string PreviousBaleTable { get; set; }
        public string CurrentWLTable { get; set; }
        public string PreviousWLTable { get; set; }
        public string ClientConStr { get; set; }
        public string userid { get; set; }
        public string password { get; set; }

       
        public List<Tuple<string, string>> GetTableSchema()
        {
            DataTable dx = new DataTable();
            List<Tuple<string, string>> mylist = new List<Tuple<string, string>>();
            string strQuery = "SELECT ORDINAL_POSITION,COLUMN_NAME,DATA_TYPE FROM ForteData.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" +
                GetCurrentBaleTable() + "'";

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(WinAuConString))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(strQuery, sqlConnection))
                    {
                        adapter.Fill(dx);
                    }
                }
                foreach (var item in this.RemoveFieldsList)
                {
                    RemoveHrdItem(dx, item);
                }

                foreach (DataRow item in dx.Rows)
                {
                    mylist.Add(new Tuple<string, string>(item[1].ToString(), item[2].ToString()));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in ClsSQLhandler GetTableSchema -> {ex.Message}");   
            }
            return mylist;
        }

        /// <summary>
        /// using ClientConStr, which should already be set
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable GetClientTableHdr(string dbName, string tableName)
        {
            DataTable dx = new DataTable();
            List<Tuple<string, string>> myColumnlst = new List<Tuple<string, string>>();
            
            string strQuery = $"SELECT ORDINAL_POSITION,COLUMN_NAME,DATA_TYPE FROM " +
                $"{dbName}.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{tableName}'";

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ClientConStr))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(strQuery, sqlConnection))
                    {
                        adapter.Fill(dx);
                    }
                    foreach (DataRow item in dx.Rows)
                    {
                        myColumnlst.Add(new Tuple<string, string>(item[1].ToString(), item[2].ToString()));
                    }
                }
            }
            catch (Exception ex )
            {
                MessageBox.Show($"Error in ClsSQLhandler GetClientTableHdr -> {ex.Message}");
            }
            return dx;
        }

        public bool UpdateRemoteRtSqlTable(string strInsertfile)
        {
            bool bXfer = false;
            try
            {
                bXfer = UpdateBaleArchiveTableAsy(true, strInsertfile);
            }
            catch (Exception ex )
            {
                MessageBox.Show($"Error in ClsSQLhandler UpdateRemoteRtSqlTable -> {ex.Message}");
            }
            return bXfer;
        }


        public bool SendRtRemote(ObservableCollection<SqlOutFields> sqlOutputFields, string targettable, string xferRtDbName)
        {
            bool send = false;
            
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
                newForteDatastr = GetLatestForteDataLst(fortehdrlist);
           
                StrInsert = BuildInsertQueryString(fortehdrlist,targettable,newForteDatastr, xferRtDbName);
              //  ExecuteClientRtCommand(StrInsert);

                bool done = UpdateBaleArchiveTableAsy(true, StrInsert);
                if(done) UpdateXferStatus("Update Remote Client SQL Server");

                send = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in ClsSQLhandler GetClientTableHdr -> {ex.Message}");
            }
            return send;
        }

        private string BuildInsertQueryString(string fortehdrlist, string targettable, List<Tuple<string, string>> newForteDatastr, string xferRtDbName)
        {
            string strQuery = string.Empty;
            char[] charsToTrim = { ',' };
            List<string> OutputList = new List<string>();
            string Outputstring = string.Empty;

            try
            {
                for (int i = 0; i < newForteDatastr.Count;i++ )
                {
                    switch(newForteDatastr[i].Item2)
                    {
                        case "System.Int32":
                            OutputList.Add(newForteDatastr[i].Item1 + ',');
                        break;

                        case "bit":
                            OutputList.Add( $"'{newForteDatastr[i].Item1}',");
                            break;

                        case "System.String":
                            OutputList.Add($"'{newForteDatastr[i].Item1}',");
                            break;

                        case "System.Single":
                        case "System.Double":
                            OutputList.Add(newForteDatastr[i].Item1+ ',');
                            break;

                        case "smallint":
                            OutputList.Add(newForteDatastr[i].Item1+ ',');
                            break;

                        case "System.DateTime":
                            OutputList.Add($"'{newForteDatastr[i].Item1}',");
                            break;
                        default:
                            OutputList.Add(newForteDatastr[i].Item1+ ',');
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
                MessageBox.Show($"Error in ClsSQLhandler BuildInsertQueryString -> {ex.Message}");
            }
            return strQuery;
        }

        

        public List<Tuple<string, string>> GetLatestForteDataLst(string fortehdrlist)
        {
            string strQuery = $"SELECT TOP 1 {fortehdrlist} FROM { GetCurrentBaleTable()} with (NOLOCK) ORDER by UID DESC";
            DataTable mytable = new DataTable();
            char[] charsToTrim = { ',' };

            List<Tuple<string, string>> ForteNewDataList = new List<Tuple<string, string>>();

            try
            {             
                using (var sqlConnection = new SqlConnection(ConString))
                {
                    using (var adapter = new SqlDataAdapter(strQuery, sqlConnection))
                    {
                        adapter.Fill(mytable);
                    }
                }
                for (int i = 0; i < mytable.Columns.Count; i++)
                {   
                    ForteNewDataList.Add(new Tuple<string, string>(mytable.Rows[0][i].ToString(), mytable.Rows[0][i].GetType().ToString()));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in ClsSQLhandler GetLatestForteDataLst -> {ex.Message}");
            }
            return ForteNewDataList;
        }

        public List<string> Getsourcelist()
        {
            List<string> sList = new List<string>();
            try
            {
                sList = GetUniquIntitemlist("SourceID");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in ClsSQLhandler Getsourcelist -> {ex.Message}");
            }
            return sList;
        }

        private void RemoveHrdItem(DataTable Ttable, string strItem)
        {
            foreach (DataRow item in Ttable.Rows)
            {
                if (item[1].ToString() == strItem)
                {
                    item.Delete();
                }
            }
            Ttable.AcceptChanges();
        }

        //Work Stations
        public string LocalWorkStationID { get; set; }
        public string TargetWorkStationID { get; set; }

        public bool ExecuteClientRtCommand(string strquery)
        {
            bool bGood = false;
            try
            {
                using (var sqlConnection = new SqlConnection(ClientConStr))
                {
                    sqlConnection?.Open();
                    using (var command = new SqlCommand(strquery, sqlConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                    sqlConnection?.Close();
                    bGood = true;
                    UpdateXferStatus("Update Remote Client SQL Server");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in ExecuteClientRtCommand {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.Error, $"ERROR in ExecuteClientRtCommand {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.Error, $"String Query =  {strquery}");
            }
            return bGood;
        }


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

        public bool UpdateRemoteSQLTableAsy(ObservableCollection<SqlOutFields> sqlOutputFields, string xferRtDbName, string targettable, bool _condition)
        {
            bool bdone = false;

            string fortehdrlist = string.Empty;
            string clienthdrlist = string.Empty;
            char[] charsToTrim = { ',' };
            List<Tuple<string, string>> newForteDatastr = new List<Tuple<string, string>>();
            string StrInsert = string.Empty;

            try
            {
                bdone = ThreadPool.QueueUserWorkItem(
                  o =>
                  {
                      if (_condition)
                      {
                          try
                          {
                              foreach (var item in sqlOutputFields)
                              {
                                  fortehdrlist += item.ForteFieldName + ",";
                                  clienthdrlist += item.ClientFieldName + ",";
                              }
                              fortehdrlist = fortehdrlist.Trim(charsToTrim);
                              clienthdrlist = clienthdrlist.Trim(charsToTrim);
                              newForteDatastr = GetLatestForteDataLst(fortehdrlist + ",UID");
                              NewUID = Convert.ToInt32(newForteDatastr[newForteDatastr.Count - 1].Item1);
                              newForteDatastr.RemoveAt(newForteDatastr.Count - 1);

                              if (NewUID != CurUID)
                              {
                                  //Console.WriteLine($"CurUID= {CurUID} NewUID= {NewUID}");
                                  StrInsert = BuildInsertRtQueryString(fortehdrlist, targettable, newForteDatastr, xferRtDbName);
                                  CurUID = NewUID;
                                  Console.WriteLine($"StrInsert=  {StrInsert}");
                                  Thread.Sleep(200);

                                  using (var sqlConnection = new SqlConnection(ClientConStr))
                                  {  
                                      if (sqlConnection != null) sqlConnection.Open();
                                      using (var command = new SqlCommand(StrInsert, sqlConnection))
                                      {
                                          command.ExecuteNonQuery();
                                      }
                                      sqlConnection.Close();
                                      bdone = true;
                                  }
                              }
                          }
                          catch (Exception ex)
                          {
                              MessageBox.Show($"Error in UpdateBaleArchiveTableAsy -> {ex.Message}");
                              ClsSerilog.LogMessage(ClsSerilog.Error, $"ERROR in UpdateRemoteSQLTableAsy {ex.Message}");
                              ClsSerilog.LogMessage(ClsSerilog.Error, $"string Query =  {StrInsert}");
                          }
                      }
                  });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in UpdateRemoteSQLTableAsy {ex.Message}");
            }
            return bdone;
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

        public bool UpdateBaleArchiveTableAsy(bool _condition, string strquery)
        {
            bool bdone = false;
            bdone = ThreadPool.QueueUserWorkItem(
              o =>
              {
                  if (_condition)
                  {
                      try
                      {
                          using (var sqlConnection = new SqlConnection(ClientConStr))
                          {
                              sqlConnection?.Open();
                              using (var command = new SqlCommand(strquery, sqlConnection))
                              {
                                  command.ExecuteNonQuery();
                              }
                              sqlConnection?.Close();
                              bdone = true;
                          }
                      }
                      catch (Exception ex)
                      {
                          MessageBox.Show($"Error in UpdateBaleArchiveTableAsy -> {ex.Message}" );
                          ClsSerilog.LogMessage(ClsSerilog.Error, $"ERROR in UpdateBaleArchiveTableAsy {ex.Message}");
                          ClsSerilog.LogMessage(ClsSerilog.Error, $"String Query =  {strquery}");
                      }
                  }
              });
            return bdone;
        }



        private void UpdateXferStatus(string status)
        {
            _eventAggregator.GetEvent<UpdateXferStatus>().Publish(status);
        }

        //Connection strings
        public const string MASTER_DB = "Master";
        public string ConString { get; set; }
        public string WLConString { get; set; }
        private string WinAuConString { get; set; }

        public string WLConStr { get; set; }
        public string MasterConStr { get; set; }
        public string StrUserName { get; set; }
        public string StrPassWrd { get; set; }
        public string StrDatabase { get; set; }

        public string StrDataSource { get; set; }
        public string StrWLDatabase { get; set; }
        public string StrHostID { get; set; }
        public string StrInstance { get; set; }
        public string DataSource { get; set; }
        public int MoistureType { get; set; }
        public int WeightUnit { get; set; }

        private int iNewIndexNumber = 0;


        public static ClsSQLhandler Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ClsSQLhandler(ClsApplicationService.Instance.EventAggregator);
                    }
                    return instance;
                }
            }
        }

        public ClsSQLhandler(Prism.Events.IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;

            TargetWorkStationID = Settings.Default.Host;
            LocalWorkStationID = Environment.MachineName;
            StrUserName = Settings.Default.UserName;
            StrPassWrd = Settings.Default.PassWord;
            StrInstance = Settings.Default.Instance;

            MyXml = Xmlhandler.Instance;


            //Connections String
            ConString = SetSqlAuConnString(Settings.Default.RTDatabase);
            WLConString = SetSqlAuConnString(Settings.Default.WLDatabase);
            WinAuConString = SetWinAuConnString(Settings.Default.RTDatabase);

            ClientConStr = Settings.Default.ClientConStr; // WinAuConString;
        }

        public bool TestSqlConnections()
        {   
            return TestSqlConnection(Settings.Default.Host, Settings.Default.Instance, Settings.Default.UserName, Settings.Default.PassWord);
        }

        public bool CheckWorkStationTarget()
        {
            if (TargetWorkStationID == string.Empty) return false;
            else return true;
        }

      
        public bool TestSqlConnection(string host, string instant, string userid, string password)
        {
            bool bConnected = false;

            TargetWorkStationID = host;
            LocalWorkStationID = Environment.MachineName;
            StrUserName = userid;
            StrPassWrd = password;
            StrInstance = instant;
            StrDatabase = Settings.Default.RTDatabase;
            DataSource = host + @"\" + instant;

            string testconstring = "Data Source = '" + DataSource + "'; Database = " + StrDatabase + "; user id = '" + userid +
                                "'; Password = '" + password + "'; connection timeout=30;Persist Security Info=True;";

            try
            {
                using (var sqlConnection = new SqlConnection(testconstring))
                {
                    sqlConnection.Open();
                    bConnected = true;
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in TestSqlConnection " + ex.Message);
            }
            return bConnected;
        }

        public List<string> GetServerList()
        {
            List<string> ServerList = new List<string>();
            ServerList.Clear();

            try
            {
                System.Data.Sql.SqlDataSourceEnumerator instance = System.Data.Sql.SqlDataSourceEnumerator.Instance;
                System.Data.DataTable table = instance.GetDataSources();

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (row[1].ToString() == "SQLEXPRESS")
                            ServerList.Add(row[0].ToString() + @"\" + row[1].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in ClsSQLhandler GetServerList = {ex.Message}");
               // ClassCommon.LogObject.LogMessage(MsgTypes.WARNING, MsgSources.DBSQL, "GetServerList " + ex.Message);
            }
            return ServerList;
        }

        public bool TestSqlConnection(string host, string instant, string database, string userid, string password)
        {
            bool bConnected = false;

            string Source = host + @"\" + instant;
            string testconstring = "Data Source = '" + Source + "'; Database = " + database + "; user id = '" + userid +
                                "'; Password = '" + password + "'; connection timeout=30;Persist Security Info=True;";

            try
            {
                using (var sqlConnection = new SqlConnection(testconstring))
                {
                    sqlConnection.Open();
                    bConnected = true;
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in ClsSQLhandler TestSqlConnection  {ex.Message}");
             //   ClassCommon.LogObject.LogMessage(MsgTypes.WARNING, MsgSources.DBSQL, "TestSqlConnection " + ex.Message);
            }
            return bConnected;
        }

        public bool TestXferSqlConnection(string host, string instant, string database, string userid, string password, bool sQLAuChecked)
        {
            bool bConnected = false;
            string ConString = string.Empty;
            string Source = host + @"\" + instant;

            if(sQLAuChecked)
                ConString = $"Data Source = '{Source}'; Database ={database}; user id= '{userid}'; Password= '{password}';" +
                $" connection timeout=30;Persist Security Info=True;";
            else
                ConString = $"data source ='{Source}';integrated security=SSPI;persist security info=False;Trusted_Connection=Yes;";

            try
            {
                using (var sqlConnection = new SqlConnection(ConString))
                {
                    sqlConnection.Open();
                    bConnected = true;
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in ClsSQLhandler TestXferSqlConnection {ex.Message}");
                //   ClassCommon.LogObject.LogMessage(MsgTypes.WARNING, MsgSources.DBSQL, "TestSqlConnection " + ex.Message);
            }
            return bConnected;
        }

        private string SetSqlAuConnString(string strDatabase)
        {
            string StrDataSource = Settings.Default.Host + @"\" + Settings.Default.Instance;
            return "Data Source ='" + StrDataSource + "'; Database = "
               + strDatabase + "; User id= '" + Settings.Default.UserName + "'; Password = '"
               + Settings.Default.PassWord + "'; connection timeout=30;Persist Security Info=True;";
        }

        private string SetWinAuConnString(string SqlDatabase)
        {
            string strDataSource = Settings.Default.Host + @"\" + StrInstance;
            return "workstation id=" + Environment.MachineName +
                    ";packet size=4096;integrated security=SSPI;data source='" + strDataSource +
                    "';persist security info=False;initial catalog= " + SqlDatabase;
        }

        public bool SaveNewSqlCon()
        {
            Settings.Default.Host = TargetWorkStationID;
            Settings.Default.UserName = StrUserName;
            Settings.Default.PassWord = StrPassWrd;
            Settings.Default.Instance = StrInstance;
            Settings.Default.Save();
            return true;
        }

        public string GetCurrentBaleTable()
        {
            List<string> tablelist = new List<string>();
            CurrentBaleTable = string.Empty;
            string strquery = "USE ForteData SELECT top 2 [Name],create_date FROM sys.Tables with(NOLOCK) " +
                "WHERE [name] LIKE'%BaleArchive%'ORDER BY create_date DESC";

            try
            {
                using (var sqlConnection = new SqlConnection(ConString))
                {
                    sqlConnection?.Open();
                    using (var command = new SqlCommand(strquery, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    if (reader.GetString(0) != null)
                                        tablelist.Add(reader.GetString(0));
                                }
                        }
                    }
                    sqlConnection?.Close();
                }
                CurrentBaleTable = tablelist[0].ToString();
                PreviousBaleTable = (tablelist.Count > 1) ? tablelist[1].ToString() : tablelist[0].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in ClsSQLhandler GetCurrentBaleTable {ex.Message}" );
            }
            return CurrentBaleTable;
        }

        public string GetPreviousBaleTable()
        {
            GetCurrentBaleTable();
            return PreviousBaleTable;
        }


        /// <summary>
        /// Set Column and format here!
        /// Selected type of moisture to display
        /// (int)ClsCommon.MoistureType
        /// </summary>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        public DataTable GetForteDataTable(string strQuery, ObservableCollection<SqlReportField> xmlfield, int repMode, DateTime dayStart, DateTime dayEnd)
        {
            DataTable mytable = new DataTable();
            mytable.Clear();
            DataColumnCollection columns = mytable.Columns;
           // char charsToTrim = ',';

            List<int> blanklist = new List<int>();
            ClsCommon.BaleQueryItemList = new List<Tuple<string, string, string>>();

            int BFOne =0;
            int BFTwo = 0;
            int BRThree= 0;
            string strItems = string.Empty;

            if (xmlfield.Count > 0)
            {
                for (int i = 0; i < xmlfield.Count; i++)
                {
                    ClsCommon.BaleQueryItemList.Add(new Tuple<string, string, string>(xmlfield[i].FieldExp, xmlfield[i].FieldName, xmlfield[i].Format));

                    if ((xmlfield[i].FieldExp.ToString() == "BlankField1") | (xmlfield[i].FieldExp.ToString() == "BlankField2") | (xmlfield[i].FieldExp.ToString() == "BlankField3"))
                    {
                        if (xmlfield[i].FieldExp.ToString() == "BlankField1") BFOne = i;
                        if (xmlfield[i].FieldExp.ToString() == "BlankField2") BFTwo = i;
                        if (xmlfield[i].FieldExp.ToString() == "BlankField3") BRThree = i;
                        blanklist.Add(i);
                    }
                    else
                        strItems += xmlfield[i].FieldExp + ',';
                }
            }
           // string strQ = $"SELECT {strItems.TrimEnd(charsToTrim)} From {currBaleTable} with (NOLOCK) {timeFrame} ORDER by UID ASC ";

            try
            {
                using (var sqlConnection = new SqlConnection(ConString))
                {
                    sqlConnection?.Open();
                    using (SqlCommand comm = new SqlCommand(strQuery, sqlConnection))
                    {
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            if (reader.HasRows)
                                mytable.Load(reader);
                        }
                    }
                    sqlConnection?.Close();
                }

                if (mytable.Rows.Count > 0)
                {
                    DataRow[] rows = mytable.Select();
                    for (int i = 0; i < rows.Length; i++)
                    {
                        if (columns.Contains("Moisture"))
                        {
                            if((rows[i]["Moisture"] != null))
                            {
                                ClsCommon.CalulateMoisture(rows[i].Field<float>("Moisture").ToString(), ClsCommon.MoistureType);
                                mytable.Rows[i]["Moisture"] = String.Format("{0:0.00}", mytable.Rows[i]["Moisture"]);
                            }
                        }
                        if (columns.Contains("Weight"))
                        {
                            if (ClsCommon.WeightType == ClsCommon.WtEnglish) //English Unit lb
                            {
                                if ((columns.Contains("Weight")) & (rows[i]["Weight"] != null))
                                    rows[i]["Weight"] = rows[i].Field<float>("Weight") * 2.20462; //Lb
                                if (columns.Contains("BDWeight"))
                                {
                                    if ((rows[i]["BDWeight"] != null))
                                        rows[i]["BDWeight"] = (rows[i].Field<float>("BDWeight") * 2.20462); //Lb.
                                }
                                if (columns.Contains("NetWeight"))
                                {
                                    if ((rows[i]["NetWeight"] != null))
                                        rows[i]["NetWeight"] = (rows[i].Field<float>("NetWeight") * 2.20462); //Lb.
                                }
                            }
                            mytable.Rows[i]["Weight"] = String.Format("{0:0.00}", mytable.Rows[i]["Weight"]);
                        }
                    }

                    for (int i = 0; i < xmlfield.Count; i++)
                    {
                        if (xmlfield[i].DbField.ToString().Equals("BlankField1"))
                        {
                            mytable.Columns.Add("BlankField1", typeof(string)).SetOrdinal(BFOne);
                        }
                        if (xmlfield[i].DbField.ToString().Equals("BlankField2"))
                        {
                            mytable.Columns.Add("BlankField2", typeof(string)).SetOrdinal(BFTwo);
                        }
                        if (xmlfield[i].DbField.ToString().Equals("BlankField3"))
                        {
                            mytable.Columns.Add("BlankField3", typeof(string)).SetOrdinal(BRThree);
                        }
                    }
                    for (int i = 0; i < mytable.Rows.Count; i++)
                    {
                        if (mytable.Columns.Contains("BlankField1"))
                            mytable.Rows[i]["BlankField1"] = "";
                        if (mytable.Columns.Contains("BlankField2"))
                            mytable.Rows[i]["BlankField2"] = "";
                    }

                    if (ClsCommon.UsedProdTime == true)
                    {  
                        SetDateDayReport(mytable, dayStart, dayEnd);
                    }
                    mytable.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in ClsSQLhandler  GetForteDataTable -> {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.Error, $"ERROR in ClsSQLhandler GetForteDataTable! {ex.Message}");
            }
            return mytable;
        }

     
        private static void SetDateDayReport(DataTable mytable, DateTime dayStart, DateTime dayEnd)
        {
            DataColumnCollection columns = mytable.Columns;
            int DayOnReport = Convert.ToInt32(dayStart.ToString("dd"));
            try
            {

                for (int i = 0; i < mytable.Columns.Count; i++)
                {
                    if (columns[i].DataType == typeof(DateTime))
                    {
                        DateTime Datesel = (DateTime)DateTime.Now;

                        for (int y = 0; y < mytable.Rows.Count; y++)
                        {
                            var d3 = new DateTime(Datesel.Year, Datesel.Month, DayOnReport,
                                Convert.ToDateTime(mytable.Rows[y][$"{columns[i].ColumnName}"]).Hour,
                                Convert.ToDateTime(mytable.Rows[y][$"{columns[i].ColumnName}"]).Minute,
                                Convert.ToDateTime(mytable.Rows[y][$"{columns[i].ColumnName}"]).Second);
                            mytable.Rows[y].SetField(columns[i].ColumnName, d3);
                        }
                    }
                }
                mytable.AcceptChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in ClsSQLhandler SetDateDayReport {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.Error, $"ERROR in ClsSQLhandler SetDateDayReport! {ex.Message}");
            }
        }

        public object GetRtProdDate(DateTime dateTime)
        {
            string RetDate =String.Empty;
            var MidNight =  new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 00, 00, 00);
            var DatEnd = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, ClsCommon.ProdDayEnd.Hour, ClsCommon.ProdDayEnd.Minute, 00);

            if (ClsCommon.UsedProdTime)
            {
                //If time is after Midnight to production day End -> use previous date
                RetDate = ((dateTime >= MidNight) & (dateTime <= DatEnd)) ? 
                    dateTime.AddDays(-1).ToString(DateFormat) : dateTime.ToString(DateFormat);
            }
            else
            {
                RetDate = dateTime.ToString(DateFormat);
            }
            return RetDate;
        }

        public object GetWLProdDate(DateTime dateTime)
        {
            string RetDate = String.Empty;
            var MidNight = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 00, 00, 00);
            var DatEnd = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, ClsCommon.ProdDayEnd.Hour, ClsCommon.ProdDayEnd.Minute, 00);

            if (ClsCommon.UsedProdTime)
            {
                //If time is after Midnight to production day End -> use previous date
                RetDate = ((dateTime >= MidNight) & (dateTime <= DatEnd)) ?
                    dateTime.AddDays(-1).ToString() : dateTime.ToString();
            }
            else
            {
                RetDate = dateTime.ToString();
            }
            return RetDate;
        }

        public DataSet GetReportDataset(string strquery, string Targettable)
        {
            DataSet Mydataset;

            using (SqlConnection conn = new SqlConnection(ConString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                Mydataset = new DataSet();

                adapter.SelectCommand = new SqlCommand(strquery, conn);

                adapter.Fill(Mydataset);
                return Mydataset;
            }
        }

  
        public List<string> GetUniqueStrItemlist(string strItem)
        {
            List<string> itemList = new List<string>();
            string strQuery = "SELECT DISTINCT " + strItem + " From " + GetCurrentBaleTable() + " with (NOLOCK) ORDER BY " + strItem + ";";

            // if (strItem == "BalerID") constr = WLConStr;

            try
            {
                using (var sqlConnection = new SqlConnection(ConString))
                {
                    sqlConnection?.Open();
                    using (var command = new SqlCommand(strQuery, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    if (reader != null)
                                        itemList.Add(reader[0].ToString());
                                }
                        }
                    }
                    sqlConnection?.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in ClsSQLhandler GetUniqueStrItemlist {ex.Message}");
            }
            return itemList;
        }

        public List<string> GetUniquIntitemlist(string strItem)
        {
            List<string> itemList = new List<string>();
            string strQuery = "SELECT DISTINCT " + strItem + " From " + GetCurrentBaleTable() + "  with (NOLOCK) WHERE " + strItem + " > 0 ORDER BY " + strItem + ";";

            try
            {
                using (var sqlConnection = new SqlConnection(ConString))
                {
                    sqlConnection?.Open();
                    using (var command = new SqlCommand(strQuery, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    if (reader != null)
                                        itemList.Add(reader.GetInt32(0).ToString());
                                }
                        }
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in ClsSQLhandler GetUniquIntitemlist {ex.Message}");
            }
            return itemList;
        }

        public List<string> GetSqlDatalist()
        {
            List<string> ServerList = new List<string>();
            ServerList.Clear();

            try
            {
                System.Data.Sql.SqlDataSourceEnumerator instance = System.Data.Sql.SqlDataSourceEnumerator.Instance;
                System.Data.DataTable table = instance.GetDataSources();

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (row[1].ToString() == "SQLEXPRESS")
                            ServerList.Add(row[0].ToString() + @"\" + row[1].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in ClsSQLhandler ServerList " + ex.Message);
            }
            return ServerList;
        }

        public bool FindSqlDatabase(string StrTable)
        {
            bool bFoundTable = false;
            string strQuery = "SELECT * FROM sys.databases d WHERE d.database_id>4";

        //    ConString = SetSqlAuConnString(StrTable);

            try
            {
                using (var sqlConnection = new SqlConnection(ConString))
                {
                    sqlConnection?.Open();
                    using (var command = new SqlCommand(strQuery, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    if (reader != null)
                                    {
                                        if (reader[0].ToString() == StrTable)
                                            bFoundTable = true;
                                    }
                                }
                            }
                        }
                    }
                    sqlConnection?.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in SQL ClsSQLhandler FindSqlDatabase {ex.Message}");
              //  ClassCommon.LogObject.LogMessage(MsgTypes.WARNING, MsgSources.DBSQL, "FindSqlTable " + ex.Message);
            }
            return bFoundTable;
        }

        public List<string> getDbList(string strSqlServer, bool SqlAuChecked, string Userid, string Password)
        {
            List<string> SerList = new List<string>();
            
            //Windows Authentication connection.
            string WinAuConStr = $"data source ='{strSqlServer}';integrated security=SSPI;persist security info=False;Trusted_Connection=Yes;";

            //SQL Authentication connection.
            string SqlAuConStr = "Data Source = '" + strSqlServer + "'; user id = '" + Userid +
                                "'; Password = '" + Password + "'; connection timeout=30;Persist Security Info=True;";

            ClientConStr = (SqlAuChecked) ? SqlAuConStr : WinAuConStr;

            Settings.Default.ClientConStr = ClientConStr;
            Settings.Default.Save();

           string strQuery = "SELECT * FROM sys.databases d WHERE d.database_id>4";

            try
            {
                using (var sqlConnection = new SqlConnection(ClientConStr))
                {
                    sqlConnection?.Open();
                    using (var command = new SqlCommand(strQuery, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    if (reader != null)
                                    {
                                        SerList.Add(reader[0].ToString());
                                    }
                                }
                            }
                        }
                    }
                    sqlConnection?.Close();
                }
            }
            catch (SqlException)
            {
               //System.Windows.MessageBox.Show("ERROR in getDbList " + ex.Message);
            }
            return SerList;
        }

        public List<string> GetTableList(string serverName, string dbName)
        {
            List<string> tableLst = new List<string>();
            string connectionString = $"data source ='{serverName}';integrated security=SSPI;persist security info=False;Trusted_Connection=Yes;";
            string strquery = "USE " + dbName + " SELECT [Name] FROM sys.Tables with (NOLOCK) ORDER BY create_date DESC";

            try
            {
                using (var sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection?.Open();
                    using (var command = new SqlCommand(strquery, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    if (reader != null)
                                    {
                                        tableLst.Add(reader[0].ToString());
                                    }
                                }
                            }
                        }
                    }
                    sqlConnection?.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in GetTableList {ex.Message}");
            }
            return tableLst;
        }


        public string GetWLCurrMonth()
        {
            List<string> tablelist = new List<string>();
            string strquery = "SELECT TOP 2 [name],create_date FROM sys.tables  with (NOLOCK) WHERE [name] LIKE '%FValueReadings%' ORDER BY create_date DESC";

            try
            {
                
                using (var sqlConnection = new SqlConnection(WLConString))
                {
                    sqlConnection?.Open();
                    using (var command = new SqlCommand(strquery, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    if (reader.GetString(0) != null)
                                        tablelist.Add(reader.GetString(0));
                                }
                        }
                    }
                    sqlConnection?.Close();
                }
                CurrentWLTable = tablelist[0].ToString();
                PreviousWLTable = (tablelist.Count > 1) ? tablelist[1].ToString() : tablelist[0].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in SQL ClsSQLhandler GetWLCurrMonth {ex.Message}");
            }
            return CurrentWLTable;
        }

        public string GetPreMonth()
        {
            GetWLCurrMonth();
            return PreviousWLTable;
        }


        public DataTable GetNewWLDataTable(string strWLTable, string strQuery)
        {
            DataTable mytable = new DataTable();
            try
            {
                using (var sqlConnection = new SqlConnection(WLConString))
                {
                    sqlConnection?.Open();
                    using (SqlCommand comm = new SqlCommand(strQuery, sqlConnection))
                    {
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            if (reader.HasRows)
                                mytable.Load(reader);
                        }
                    }
                    sqlConnection?.Close();
                 }

                for (int i = 0; i < mytable.Rows.Count; i++)
                {
                    mytable.Rows[i]["ReadTime"] = GetWLProdDate(mytable.Rows[i].Field<System.DateTime>("ReadTime"));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in ClsSQLhandler GetWetLayerDataTable" + ex);
                ClsSerilog.LogMessage(ClsSerilog.Error, $"ERROR in ClsSQLhandler GetWetLayerDataTable {ex.Message}");
            }
            return mytable;
        }


        public DataTable GetSqlScema()
        {
            DataTable dx = new DataTable();
            string strQuery = "SELECT ORDINAL_POSITION,COLUMN_NAME,DATA_TYPE " +
                $" FROM ForteData.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{GetCurrentBaleTable()}'";

            try
            {
                using (var sqlConnection = new SqlConnection(ConString))
                {
                    using (var adapter = new SqlDataAdapter(strQuery, sqlConnection))
                    {
                        adapter.Fill(dx);
                    }
                }
                SetRemoveFields();

                foreach (var item in this.RemoveFieldsList)
                {
                    RemoveHrdItem(dx, item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in GetSqlScema -> " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.Error, $"ERROR in ClsSQLhandler GetSqlScema {ex.Message}");
            }
            return dx;
        }

        private void SetRemoveFields()
        {
            if (RemoveFieldsList == null)
            {
                RemoveFieldsList = new List<string>
                {
                    "Index",
                    "Empty",
                    "QualityUID",
                    "AsciiFld1",
                    "AsciiFld2",
                    "OrderStr",
                    "QualityName",
                    "GradeLabel1",
                   // "StockLabel1",
                    //"StockLabel2",
                    //"StockLabel3",
                    //"StockLabel4",
                    "JobNum",
                    "Forte1",
                    "Forte2",
                    "ForteAveraging",
                    //"UpCount",
                    //"DownCount",
                    "DownCount2",
                    //"Brightness",
                    "BaleHeight",
                    "SourceId",
                    //"Finish",
                    "SheetArea",
                    "SheetCount",
                    "CalibrationID",
                    "PkgMoistMethod",
                    //"SpareSngFld1",
                    //"SpareSngFld2",
                    //"SpareSngFld3",
                    "LastInGroup",
                    "MoistMes",
                    //"ProdDayStart",
                    //"ProdDayEnd",
                    "SourceID",
                    //"LineID,
                    "StockID",
                    "GradeID",
                    "WtMes",
                    "AsciiFld3",
                    "AsciiFld4",
                    "SR",
                    "UID",
                    //"Package",
                    "ResultDesc",
                    "GradeLabel2",
                    "WLAlarm",
                    "WLAStatus",
                    //"Dirt",
                    "Status",
                    "WeightStatus",
                    "TemperatureStatus",
                    "OrigWeightStatus",
                    "ForteStatus",
                    "Forte1Status",
                    "Forte2Status",
                    "UpCountStatus",
                    "DownCountStatus",
                    "DownCount2Status",
                    "BrightnessStatus",
                    "TimeStartStatus",
                    "BaleHeightStatus",
                    "TimeStartStatus",
                    "TimeCompleteStatus",
                    "SourceIDStatus",
                    "StockIDStatus",
                    "GradeIDStatus",
                    "TareWeightStatus",
                    "AllowanceStatus",
                    "SheetCountStatus",
                    "MoistureStatus",
                    "NetWeightStatus",
                    "CalibrationIDStatus",
                    "SeriAlNumberStatus",
                    "LotNumberStatus",
                    "TemperatureStatus",
                    "UnitNumberStatus",
                    "UnitIdent",
                    //"FC_IdentString",
                    //"BasisWeight",
                    "Temperature"
                };
            }
        }

        public void Dispose()
        {
           
        }
    }
}
