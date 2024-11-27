using Prism.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RTRep.Services
{
    public class CsvFileService
    {
        protected readonly Prism.Events.IEventAggregator _eventAggregator;

        private static readonly object padlock = new object();
        private static CsvFileService instance = null;
        public string ExCsvTextMsg = string.Empty;

        public static CsvFileService Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new CsvFileService(ClsApplicationService.Instance.EventAggregator);
                    }
                    return instance;
                }
            }
        }
        public CsvFileService(IEventAggregator EventAggregator)
        {
            _eventAggregator = EventAggregator;
        }

        public string WriteRtCSVFile(System.Data.DataTable Mydatatable, string FileName)
        {
            List<string> indwxlist = new List<string>();
            int iEnd = Mydatatable.Rows.Count;

            string strMessage = string.Empty;

            try
            {
                if (iEnd > 0)
                {
                    string StrFilePatch = ClsCommon.ExCsvFileLocation + "\\" + FileName + DateTime.Now.ToString("_HH_mm") + ".csv";
                    StreamWriter outFile = new StreamWriter(StrFilePatch);
                    List<string> headerValues = new List<string>();

                    foreach (DataColumn column in Mydatatable.Columns)
                    {
                        headerValues.Add(QuoteValue("'" + column.ColumnName));
                    }
                    //Header
                    outFile.WriteLine(string.Join(",", headerValues.ToArray()));
                    foreach (DataRow row in Mydatatable.Rows)
                    {
                        string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                        outFile.WriteLine(String.Join(",", fields));
                    }
                    outFile.Close();
                    strMessage = "Write RT CSV file Done " + DateTime.Now.Date.ToString("MM/dd/yyyy");
                    ClsSerilog.LogMessage(ClsSerilog.Info, $"Write RT CSV file Done  {DateTime.Now.Date.ToString("MM/dd/yyyy")}");
                }
            }
            catch (Exception ex)
            {
                ClsSerilog.LogMessage(ClsSerilog.Error, $"ERROR in CsvFileService WriteRtCSVFile  { ex.Message}");
            }
            return strMessage;
        }

        public async Task WriteRtCSVFileAsync(System.Data.DataTable Mydatatable, string FileName)
        {
            
            List<string> indwxlist = new List<string>();
            int iEnd = Mydatatable.Rows.Count;
            //FileName = FileName + DateTime.Now.ToString(" mm");

            try
            {
                if (iEnd > 0)
                {
                    if (!ClsCommon.UsedCusField)
                    {
                        foreach (DataColumn column in Mydatatable.Columns)
                        {
                            if (column.ColumnName == "SpareSngFld3")
                                column.ColumnName = "%CV";
                            else if (column.ColumnName == "Param1")
                                column.ColumnName = "MAX";
                            else if (column.ColumnName == "Param2")
                                column.ColumnName = "MIN";
                            else if (column.ColumnName == "Moisture")
                                column.ColumnName = ClsCommon.MoistureUnit;
                            else if (column.ColumnName == "Column1")
                                column.ColumnName = "IW @CCKg";
                            else if (column.ColumnName == "SourceName")
                                column.ColumnName = "Machine Number";
                            else if (column.ColumnName == "StockName")
                                column.ColumnName = "Quality";
                        }
                        Mydatatable.AcceptChanges();
                    }
                    else
                    {
                        foreach (DataColumn column in Mydatatable.Columns)
                        {
                            if (column.ColumnName == "SpareSngFld3")
                                column.ColumnName = "%CV";
                            else if (column.ColumnName == "Moisture")
                                column.ColumnName = ClsCommon.MoistureUnit;
                        }
                        Mydatatable.AcceptChanges();
                    }


                    await Task.Run(() =>
                    {
                        string StrFilePatch = ClsCommon.ExCsvFileLocation + "\\" + FileName + ".csv";
                        StreamWriter outFile = new StreamWriter(StrFilePatch);
                        List<string> headerValues = new List<string>();

                        foreach (DataColumn column in Mydatatable.Columns)
                        {
                            headerValues.Add(QuoteValue("'" + column.ColumnName));
                        }
                        //Header
                        outFile.WriteLine(string.Join(",", headerValues.ToArray()));
                        foreach (DataRow row in Mydatatable.Rows)
                        {
                            string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                            outFile.WriteLine(String.Join(",", fields));
                        }
                        outFile.Close();
                    });
                    ExCsvTextMsg = "Write RT CSV file Done " + DateTime.Now.Date.ToString("MM/dd/yyyy");
                    ClsSerilog.LogMessage(ClsSerilog.Info, $"ExCsvTextMsg { DateTime.Now.Date.ToString("MM/dd/yyyy")}");
                }
            }
            catch (Exception ex)
            {
                ClsSerilog.LogMessage(ClsSerilog.Error, $"ERROR in CsvFileService WriteRtCSVFileAsync {ex.Message}");
               
                // MessageBox.Show("ERROR in ReportModel WriteRtCSVFileAsync " + ex.Message);
            }
        }

        private string QuoteValue(string value)
        {
            return string.Concat("" + value + "");
        }

       
        public async void WriteWlCSVFile(System.Data.DataTable Mydatatable, string StrFileName)
        {
            List<string> indwxlist = new List<string>();
            int iEnd = Mydatatable.Rows.Count;

            StrFileName = StrFileName + DateTime.Now.ToString(" MM_dd_yyyy_hh_mm");

            try
            {
                await Task<string>.Run(() =>
                {
                    if (iEnd > 0)
                    {
                        string StrPathFile = ClsCommon.ExCsvFileLocation + "\\" + StrFileName + ".csv";
                        StreamWriter outFile = new StreamWriter(StrPathFile);
                        List<string> headerValues = new List<string>();

                        foreach (DataColumn column in Mydatatable.Columns)
                        {
                            if (column.ColumnName == "Deviation")
                                headerValues.Add(QuoteValue("'" + "%CV"));
                            else if (column.ColumnName == "Param1")
                                headerValues.Add(QuoteValue("'" + "MAX"));
                            else if (column.ColumnName == "Param2")
                                headerValues.Add(QuoteValue("'" + "MIN"));
                            else if (column.ColumnName == "Moisture")
                                headerValues.Add(QuoteValue("'" + "%MR"));
                            else
                                headerValues.Add(QuoteValue("'" + column.ColumnName));
                        }

                        //Header
                        outFile.WriteLine(string.Join(",", headerValues.ToArray()));
                        foreach (DataRow row in Mydatatable.Rows)
                        {
                            string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                            outFile.WriteLine(String.Join(",", fields));
                        }
                        outFile.Close();
                        ExCsvTextMsg = "Write WL CSV file Done " + DateTime.Now.Date.ToString("MM/dd/yyyy");
                    }
                    else
                        ExCsvTextMsg = "No Wet Layers Data!";
                    return ExCsvTextMsg;
                });
            }
            catch (Exception ex )
            {
                ClsSerilog.LogMessage(ClsSerilog.Error, $" ERROR in CsvFileService WriteWlCSVFile {ex.Message}");
            }
        }

    }
}
