using Prism.Events;
using Prism.Mvvm;
using RTRep.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using static RTRep.Services.ClsApplicationService;

namespace FieldsColumnSelect.Model
{
    public class FieldSelectModel
    {
        protected readonly IEventAggregator _eventAggregator;
        private ClsSQLhandler SqlHandler;
        private Xmlhandler MyXml;
   //     public ObservableCollection<SqlReportField> RepField;

        public FieldSelectModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            SqlHandler = ClsSQLhandler.Instance;
            MyXml = Xmlhandler.Instance;

            MyXml.CheckandCreateXMLFiles(ClsCommon.ReportXmlFilePath, "PrintColumn");
        }

        /// <summary>
        /// Get List from SQL header.
        /// </summary>
        /// <returns></returns>
        internal List<Tuple<string, string>> GetColumnList()
        {
            DataTable HdrTable = new DataTable();
            List<Tuple<string, string>> SqlList = new List<Tuple<string, string>>();
            try
            {
                HdrTable = SqlHandler.GetSqlScema();
                foreach (DataRow item in HdrTable.Rows)
                {
                    SqlList.Add(new Tuple<string, string>(item[1].ToString(), item[2].ToString()));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in GetColumnList " + ex.Message);
            }
            return SqlList;
        }

        /// <summary>
        /// Save to XML file and show in ClsCommon.ReportFields
        /// </summary>
        /// <param name="reportField"></param>
        internal void SaveFieldColumns(ObservableCollection<SqlReportField> reportField)
        {
            char[] charsToTrim = { ',' };

           MyXml.UpdateXMlRepColumn(reportField);

            try
            {
               if(reportField.Count > 0)
               {
                    ClsCommon.ReportFields = string.Empty;

                    for (int i =0; i < reportField.Count; i++ )
                    {
                        ClsCommon.ReportFields += reportField[i].FieldExp + ",";
                    }
                    ClsCommon.ReportFields = ClsCommon.ReportFields.TrimEnd(charsToTrim);
                } 
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in  SaveFieldColumns " + ex.Message);
            }
        }

        internal ObservableCollection<SqlReportField> GetXMLReportList()
        {
            return MyXml.ReadXMlRepColumn(ClsCommon.ReportXmlFilePath);
        }
    }

    
}
