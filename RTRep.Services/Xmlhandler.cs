
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace RTRep.Services
{
    public class Xmlhandler
    {
        public string GetBaseDirPath()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }

        public string SettingsGdvFile
        {
            get { return Path.Combine(GetBaseDirPath(), "GridviewItems.xml"); }
        }

        public string XMLOutSerialOne
        {
            get { return Path.Combine(GetBaseDirPath(), "OutSerialOne.xml"); }
        }

        public string XMLSharedListFile
        {
            get { return Path.Combine(GetBaseDirPath(), "OutSharedFile.xml"); }
        }

        public string XMLPrintBaleFile
        {
            get { return Path.Combine(GetBaseDirPath(), "PrintBaleFile.xml"); }
        }

        public string XMLScaleRequestString
        {
            get { return Path.Combine(GetBaseDirPath(), "ScaleRequestString.xml"); }
        }


        public string XMLoutputfile { get; set; }

        public string XMLGdvFilePath
        {
            get { return Path.Combine(GetBaseDirPath(), "GridviewItems.xml"); }
        }

        private static Xmlhandler instance = null;
        private static readonly object padlock = new object();
        public static Xmlhandler Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Xmlhandler();
                    }
                    return instance;
                }
            }
        }

        public Xmlhandler()
        {

        }

        

        public ObservableCollection<DataOutput> ReadXmlStringOut(int InstanceID)
        {
            ObservableCollection<DataOutput> SerialOneOutList = new ObservableCollection<DataOutput>();
            SerialOneOutList.Clear();
            XmlDocument xmldoc = new XmlDocument();
            int i = 0;

            XMLoutputfile = GetXmlFile(InstanceID);

            try
            {
                if (File.Exists(XMLoutputfile))
                {
                    xmldoc.Load(XMLoutputfile);
                    XmlNodeList xmlnode;

                    using (FileStream fsx = new FileStream(XMLoutputfile, FileMode.Open, FileAccess.Read))
                    {
                        xmldoc.Load(fsx);
                        xmlnode = xmldoc.SelectNodes("SerialOneOutGridView/Field");
                        for (i = 0; i <= xmlnode.Count - 1; i++)
                        {
                            SerialOneOutList.Add(new DataOutput(Convert.ToInt32(xmlnode[i].ChildNodes.Item(0).InnerText.Trim()),
                                xmlnode[i].ChildNodes.Item(1).InnerText.Trim(),
                                xmlnode[i].ChildNodes.Item(2).InnerText.Trim(),
                                xmlnode[i].ChildNodes.Item(3).InnerText.Trim()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in ReadSerilaOneList - " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.Error, $" ERROR in ReadSerilaOneList {ex.Message}");
                
            }
            return SerialOneOutList;
        }

        private string GetXmlFile(int instanceID)
        {
            string strXml = string.Empty;

            switch (instanceID)
            {
                case 0:
                    strXml = XMLOutSerialOne;
                    break;

                case 1:
                    strXml = XMLSharedListFile;
                    break;

                case 2:
                    strXml = XMLPrintBaleFile;
                    break;

                case 3:
                    strXml = XMLScaleRequestString;
                    break;
            }
            return strXml;
        }

        public List<string> ReadXmlGridView(string FileLocation)
        {
            List<string> XmlGridView = new List<string>();
            XmlGridView.Clear();
            XmlDocument doc = new XmlDocument();

            try
            {
                if (File.Exists(FileLocation))
                {
                    doc.Load(FileLocation);
                    XmlNodeList xnl = doc.SelectNodes("CustomGridView/Field/Name");

                    if ((xnl != null) && (xnl.Count > 0))
                    {
                        foreach (XmlNode xn in xnl)
                        {
                            if (File.Exists(FileLocation))
                                XmlGridView.Add(xn.InnerText);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in ReadXmlGridView - " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.Error, $" ERROR in ReadXmlGridView {ex.Message}");
            }
            return XmlGridView;
        }

        public void UpdateXMlcolumnList(ObservableCollection<string> selectedHdrList, string settingsGdvFile)
        {

            try
            {
                if (File.Exists(settingsGdvFile)) File.Delete(settingsGdvFile);

                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true
                };
                using (XmlWriter writer = XmlWriter.Create(settingsGdvFile, settings))
                {
                    //Begin write
                    writer.WriteStartDocument();
                    //Node
                    writer.WriteStartElement("CustomGridView");

                    foreach (var item in selectedHdrList)
                    {
                        writer.WriteStartElement("Field");
                        writer.WriteElementString("Name", item);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Close();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error in Update XMlcolumnList " + ex);
                ClsSerilog.LogMessage(ClsSerilog.Error, $" ERROR in XMlcolumnList {ex.Message}");
              
            }
        }

        public void UpdateSerialOutOneList(ObservableCollection<DataOutput> serialOutOne, int targetId)
        {
            XMLoutputfile = GetXmlFile(targetId);

            try
            {
                if ((serialOutOne.Count == 0) & (File.Exists(XMLoutputfile)))
                {
                    File.Delete(XMLoutputfile);
                    ClsSerilog.LogMessage(ClsSerilog.Info, $" Deleted XMLoutputfile @  { DateTime.Now}");
                }
                else if (serialOutOne.Count > 0)
                {
                    if (File.Exists(XMLoutputfile)) File.Delete(XMLoutputfile);

                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true
                    };
                    using (XmlWriter writer = XmlWriter.Create(XMLoutputfile, settings))
                    {
                        //Begin write
                        writer.WriteStartDocument();
                        //Node
                        writer.WriteStartElement("SerialOneOutGridView");

                        foreach (var item in serialOutOne)
                        {
                            writer.WriteStartElement("Field");
                            writer.WriteElementString("Id", item.Id.ToString());
                            writer.WriteElementString("Name", item.Name);
                            writer.WriteElementString("FieldType", item.FieldType);
                            writer.WriteElementString("FieldFormat", item.FieldFormat);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                        writer.Close();
                    }
                    ClsSerilog.LogMessage(ClsSerilog.Info, $"UpdateSerialOutOneList @ {DateTime.Now}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in UpdateSerialOutOneList " + ex);
                ClsSerilog.LogMessage(ClsSerilog.Error, $"Error in UpdateSerialOutOneList  {ex.Message}");
              
            }
        }

        public List<int> ReadXmlHdrList(string FileLocation)
        {
            List<int> ihdrlist = new List<int>();
            ihdrlist.Clear();
            XmlDocument doc = new XmlDocument();

            try
            {
                if (File.Exists(FileLocation))
                {
                    doc.Load(FileLocation);
                    XmlNodeList xnl = doc.SelectNodes("CustomHdr/Field/Value");
                    if ((xnl != null) && (xnl.Count > 0))
                    {
                        foreach (XmlNode xn in xnl)
                        {
                            ihdrlist.Add(Int32.Parse(xn.InnerText));
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error in ReadXmlHdrList - " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.Error, $"Error in ReadXmlHdrList  {ex.Message}");
            }
            return ihdrlist;
        }

        public void WriteXmlGridView(List<CheckedListItem> StringsListBox, string FileLocation)
        {
            try
            {
                if (System.IO.File.Exists(FileLocation))
                    System.IO.File.Delete(FileLocation);

                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true
                };

                using (XmlWriter writer = XmlWriter.Create(FileLocation, settings))
                {
                    //Begin write
                    writer.WriteStartDocument();
                    //Node
                    writer.WriteStartElement("CustomGridView");

                    foreach (var item in StringsListBox)
                    {
                        writer.WriteStartElement("Field");
                        writer.WriteElementString("Id", item.Id.ToString());
                        writer.WriteElementString("Name", item.Name);
                        writer.WriteElementString("FieldType", item.FieldType);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndDocument();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in WriteXmlGridView - " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.Error, $"Error in WriteXmlGridView  {ex.Message}");
                
            }
        }

        public void CheckandCreateXMLFiles(string FileLocationPath, string StartElement)
        {
           
            if (!System.IO.File.Exists(FileLocationPath))
            {
                
                ClsSerilog.LogMessage(ClsSerilog.Info, $"Create xml file  => { FileLocationPath}");
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true
                };

                using (XmlWriter writer = XmlWriter.Create(FileLocationPath, settings))
                {
                    //Begin write
                    writer.WriteStartDocument();
                    //Node
                    writer.WriteStartElement(StartElement);

                    writer.WriteEndDocument();
                    writer.Close();
                }
            }
            ClsSerilog.LogMessage(ClsSerilog.Info, $"Checked XML file  {FileLocationPath}");
        }

        /// <summary>
        /// ClsCommon.ReportFields come from FieldExp
        /// </summary>
        /// <param name="FileLocation"></param>
        /// <returns></returns>
        public ObservableCollection<SqlReportField> ReadXMlRepColumn(string FileLocation)
        {
            ObservableCollection<SqlReportField> RepColList = new ObservableCollection<SqlReportField>();
            RepColList.Clear();
            XmlDocument doc = new XmlDocument();
            char[] charsToTrim = { ',' };

            try
            {
                if (File.Exists(FileLocation))
                {
                    doc.Load(FileLocation);

                    XmlNodeList xmlnode = doc.GetElementsByTagName("PrintColumn");
                    XmlNodeList ANode = doc.SelectNodes("PrintColumn/Field");
                    ClsCommon.ReportFields = string.Empty;

                    if ((ANode != null) && (ANode.Count > 0))
                    {
                        for (int i = 0; i <= ANode.Count - 1; i++)
                        {
                            var DbField = xmlnode[0].ChildNodes.Item(i).ChildNodes.Item(0).InnerText.Trim();
                            var FieldExp = xmlnode[0].ChildNodes.Item(i).ChildNodes.Item(1).InnerText.Trim();
                            var Name = xmlnode[0].ChildNodes.Item(i).ChildNodes.Item(2).InnerText.Trim();
                            var Format = xmlnode[0].ChildNodes.Item(i).ChildNodes.Item(3).InnerText.Trim();
                            var FieldType = xmlnode[0].ChildNodes.Item(i).ChildNodes.Item(4).InnerText.Trim();

                            RepColList.Add(new SqlReportField(DbField, FieldExp, Name, Format, FieldType));
                            ClsCommon.ReportFields += FieldExp + ",";
                        }
                    }
                    ClsCommon.ReportFields = ClsCommon.ReportFields.TrimEnd(charsToTrim);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in  ReadXMlRepColumn " + ex.Message);
            }
            return RepColList;
        }

        public void UpdateXMlRepColumn(ObservableCollection<SqlReportField> HdrColumnList)
        {
            try
            {
                if (File.Exists(ClsCommon.ReportXmlFilePath))
                {
                    File.SetAttributes(ClsCommon.ReportXmlFilePath, FileAttributes.Normal);
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true
                    };

                    using (XmlWriter writer = XmlWriter.Create(ClsCommon.ReportXmlFilePath, settings))
                    {
                        //Begin write
                        writer.WriteStartDocument();
                        //Node
                        writer.WriteStartElement("PrintColumn");

                        foreach (var item in HdrColumnList)
                        {
                            writer.WriteStartElement("Field");
                            writer.WriteElementString("DbField", item.DbField);
                            writer.WriteElementString("FieldExp", item.FieldExp);
                            writer.WriteElementString("Name", item.FieldName);
                            writer.WriteElementString("Format", item.Format);
                            writer.WriteElementString("FieldType", item.FieldType);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                        writer.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in  UpdateXMlRepColumn {ex.Message}");
            }
        }

        public ObservableCollection<SqlOutFields> ReadWlXfertable()
        {
            ObservableCollection<SqlOutFields> WlXfertable = new ObservableCollection<SqlOutFields>();
            WlXfertable.Clear();
            XmlDocument doc = new XmlDocument();
            char[] charsToTrim = { ',' };
            return WlXfertable;
        }


        public ObservableCollection<SqlOutFields> ReadRtXfertable()
        {
            ObservableCollection<SqlOutFields> RtXfertable = new ObservableCollection<SqlOutFields>();
            RtXfertable.Clear();
            XmlDocument doc = new XmlDocument();
            char[] charsToTrim = { ',' };

            try
            {
                if (File.Exists(ClsCommon.XferRtItemsXmlFilePath))
                {
                    doc.Load(ClsCommon.XferRtItemsXmlFilePath);
                    XmlNodeList xmlnode = doc.GetElementsByTagName("XferRtItems");
                    XmlNodeList ANode = doc.SelectNodes("XferRtItems/Field");

                    if ((ANode != null) && (ANode.Count > 0))
                    {
                        for (int i = 0; i <= ANode.Count - 1; i++)
                        {
                            var ClientFieldName = xmlnode[0].ChildNodes.Item(i).ChildNodes.Item(0).InnerText.Trim();
                            var ClientFieldType = xmlnode[0].ChildNodes.Item(i).ChildNodes.Item(1).InnerText.Trim();
                            var ForteFieldName = xmlnode[0].ChildNodes.Item(i).ChildNodes.Item(2).InnerText.Trim();
                            var ForteFieldType = xmlnode[0].ChildNodes.Item(i).ChildNodes.Item(3).InnerText.Trim();
                            RtXfertable.Add(new SqlOutFields(ClientFieldName, ClientFieldType, ForteFieldName, ForteFieldType));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in  ReadXMlXferColumn {ex.Message}");
            }
            return RtXfertable;
        }


        public bool SaveRtXfertable(ObservableCollection<SqlOutFields> sqlOutputFields)
        {
            bool done = false;

            try
            {
                if (File.Exists(ClsCommon.XferRtItemsXmlFilePath))
                {
                    File.SetAttributes(ClsCommon.XferRtItemsXmlFilePath, FileAttributes.Normal);
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true
                    };
                    using (XmlWriter writer = XmlWriter.Create(ClsCommon.XferRtItemsXmlFilePath, settings))
                    {

                        //Begin write
                        writer.WriteStartDocument();
                        //Node
                        writer.WriteStartElement("XferRtItems");

                        foreach (var item in sqlOutputFields)
                        {
                            writer.WriteStartElement("Field");
                            writer.WriteElementString("ClientFieldName", item.ClientFieldName);
                            writer.WriteElementString("ClientFieldType", item.ClientFieldType);
                            writer.WriteElementString("ForteFieldName", item.ForteFieldName);
                            writer.WriteElementString("ForteFieldType", item.ForteFieldType);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                        writer.Close();
                        done = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in  SaveRtXfertable {ex.Message}");
            }
            return done;
        }

        public bool SaveWlXfertable(ObservableCollection<SqlOutFields> wlSqlOutputFields)
        {
            bool done = false;

            try
            {
                if (File.Exists(ClsCommon.XferWlItemsXmlFilePath))
                {



                }



            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in  SaveWlXfertable {ex.Message}");
            }
            return done;
        }
    }




    public class SqlReportField : BindableBase
    {

        private string _dbField;
        public string DbField
        {
            get { return _dbField; }
            set { SetProperty(ref _dbField, value); }
        }

        private string _fieldExp;
        public string FieldExp
        {
            get { return _fieldExp; }
            set { SetProperty(ref _fieldExp, value); }
        }


        private string _fieldName;
        public string FieldName
        {
            get { return _fieldName; }
            set { SetProperty(ref _fieldName, value); }
        }

        public string _format;
        public string Format
        {
            get { return _format; }
            set { SetProperty(ref _format, value); }
        }

        public string _fieldType;
        public string FieldType
        {
            get { return _fieldType; }
            set { SetProperty(ref _fieldType, value); }
        }

        public SqlReportField(string item1, string item2, string item3, string item4, string item5)
        {
            this.DbField = item1;
            this.FieldExp = item2;
            this.FieldName = item3;
            this.Format = item4;
            this.FieldType = item5;
        }


    }
}
