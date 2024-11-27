using CSVReport.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using RTRep.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static RTRep.Services.ClsApplicationService;

namespace BaleReports.Graphs.GraphViewModels
{
    public class GraphOneViewModel : BindableBase
    {
        private Window CsvWindow;
        private Xmlhandler MyXml;

        protected readonly IEventAggregator _eventAggregator;

        public Action CloseAction { get; set; }

        private ObservableCollection<KeyValuePair<float, int>> _baleList;
        public ObservableCollection<KeyValuePair<float, int>> ItemsList
        {
            get { return _baleList; }
            set { SetProperty(ref _baleList, value); }
        }

        private ObservableCollection<KeyValuePair<float, int>> _averageList;
        public ObservableCollection<KeyValuePair<float, int>> ItemsAvg
        {
            get { return _averageList; }
            set { SetProperty(ref _averageList, value); }
        }

        private string CSVFileName;

        private DelegateCommand _loadedGraph1ICommand;
        public DelegateCommand LoadedGraph1ICommand =>
        _loadedGraph1ICommand ?? (_loadedGraph1ICommand = new DelegateCommand(LoadPageExecute));
        private void LoadPageExecute()
        {
            MenuOneChecked = true;
        }

        private DelegateCommand _closedGraph1ICommand;
        public DelegateCommand ClosedGraph1ICommand =>
        _closedGraph1ICommand ?? (_closedGraph1ICommand = new DelegateCommand(ClosePageExecute));
        private void ClosePageExecute()
        {
            
        }

        private DelegateCommand _writeCSVCommand;
        public DelegateCommand WriteCSVCommand =>
        _writeCSVCommand ?? (_writeCSVCommand = new DelegateCommand(WriteCSVCommandExecute));
        private void WriteCSVCommandExecute()
        {
            if (BaleDataTable.Rows.Count > 0)
            {
                try
                {
                    int iEnd = BaleDataTable.Rows.Count;
                    if (iEnd > 0)
                    {
                        using (CSVReportView CSVView = new CSVReportView(BaleDataTable, CSVFileName, _eventAggregator))
                        {
                            
                            CsvWindow = new Window()
                            {
                                Title = "CSV Window",
                                Width = 400,
                                Height = 300,
                                Topmost = true,
                                Content = CSVView
                            };
                            CsvWindow.WindowStyle = WindowStyle.None;
                            CsvWindow.ShowDialog();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR in WriteCSVAllExecute " + ex);
                }
            }
        }


        private DelegateCommand _appExitCommand;
        public DelegateCommand AppExitCommand =>
        _appExitCommand ?? (_appExitCommand =
          new DelegateCommand(AppExitExecute));
        private void AppExitExecute()
        {
            _eventAggregator.GetEvent<CloseGraphWindowEvent>().Publish(true);
        }

        private string _TxtStatus;
        public string TxtStatus
        {
            get { return _TxtStatus; }
            set { SetProperty(ref _TxtStatus, value); }
        }

        private long _baleInGraph;
        public long BaleInGraph
        {
            get { return _baleInGraph; }
            set { SetProperty(ref _baleInGraph, value); }
        }

        private DataTable _baleDataTable;
        public DataTable BaleDataTable
        {
            get { return _baleDataTable; }
            set { SetProperty(ref _baleDataTable, value); }
        }

        //Graph height lower
        private double _minimumheight;
        public double MinimumHeight
        {
            get { return _minimumheight; }
            set { SetProperty(ref _minimumheight, value); }
        }
        //Graph height Upper
        private double _maximumheight;
        public double MaximumHeight
        {
            get { return _maximumheight; }
            set { SetProperty(ref _maximumheight, value); }
        }

        private string _lowvalue;
        public string LowValue
        {
            get { return _lowvalue; }
            set { SetProperty(ref _lowvalue, value); }
        }

        private string _hiValue;
        public string HiValue
        {
            get { return _hiValue; }
            set { SetProperty(ref _hiValue, value); }
        }
        private string _AvgValue;
        public string AvgValue
        {
            get { return _AvgValue; }
            set { SetProperty(ref _AvgValue, value); }
        }
        private string _stdValue;
        public string STDValue
        {
            get { return _stdValue; }
            set { SetProperty(ref _stdValue, value); }
        }

        private string _charttitle;
        public string ChartTitle
        {
            get { return _charttitle; }
            set { SetProperty(ref _charttitle, value); }
        }

        private string _ItemUnit;
        public string ItemUnit
        {
            get { return _ItemUnit; }
            set { SetProperty(ref _ItemUnit, value); }
        }

        private string _ItemLegend;
        public string ItemLegend
        {
            get { return _ItemLegend; }
            set { SetProperty(ref _ItemLegend, value); }
        }

        private bool _menuOneChecked = true;
        public bool MenuOneChecked
        {
            get { return _menuOneChecked; }
            set
            {
                if (value)
                {
                    ChartTitle = "Graph of " +  CSVFileName; // + ClsCommon.MoistureUnit;  
                     ItemUnit =  ClsCommon.MoistureUnit;
                     ItemLegend =  ClsCommon.MoistureUnit; 
                     ProcessGraph("Moisture");
                }
                SetProperty(ref _menuOneChecked, value);
            }
        }

      
        private bool _menuTwoChecked;
        public bool MenuTwoChecked
        {
            get { return _menuTwoChecked; }
            set
            {
                if (value)
                {
                    ChartTitle = "Graph of " + CSVFileName; // Weight " + ClsCommon.WeightUnit; 

                    ItemUnit = ClsCommon.WeightUnit;
                    ItemLegend = ClsCommon.WeightUnit; // "Weight";
                    ProcessGraph("Weight");
                }
                SetProperty(ref _menuTwoChecked, value);
            }
        }

        private bool _menuThreeChecked;
        public bool MenuThreeChecked
        {
            get { return _menuThreeChecked; }
            set
            {
                if (value)
                {
                    ChartTitle = "Graph of " + CSVFileName; //;

                    ItemUnit = string.Empty;
                    ItemLegend = "CV%";
                    ProcessGraph("SpareSngFld3");
                }
                SetProperty(ref _menuThreeChecked, value);
            }
        }

        public GraphOneViewModel(string ReportTitle, IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            CSVFileName = ReportTitle;
            MyXml = Xmlhandler.Instance;
        }

        private void ProcessGraph(string ind)
        {
            DataTable MyGraphData = new DataTable();

            //Get and Set Colum Header Names
            ObservableCollection<SqlReportField> ReportXmlFile = MyXml.ReadXMlRepColumn(ClsCommon.ReportXmlFilePath);

            for (int i = 0; i < BaleDataTable.Columns.Count; i++)
            {
                BaleDataTable.Columns[i].ColumnName = ReportXmlFile[i].FieldName;
            }
            BaleDataTable.AcceptChanges();

            DataColumnCollection columns = BaleDataTable.Columns;

            try
            {
                if(BaleDataTable.Rows.Count > 0)
                {

                    if (ItemsList != null) ItemsList = null;
                    if (ItemsAvg != null) ItemsAvg = null;

                    ItemsList = new ObservableCollection<KeyValuePair<float, int>>();
                    ItemsAvg = new ObservableCollection<KeyValuePair<float, int>>();

                    float flvalue = 0;
                    double Coef = 1;
                    List<float> ItemList = new List<float>();
                    int i = 1;
                    double sumOfDerivation = 0;


                    switch(ind)
                    {
                        case "Moisture":
                            foreach (DataRow Item in BaleDataTable.Rows)
                            {
                                if (columns.Contains("Moisture"))
                                {
                                    if (ind == "Moisture")
                                        flvalue = Convert.ToSingle(ClsCommon.CalulateMoisture(Item.Field<float>(ind).ToString(), ClsCommon.MoistureType));
                                }
                                else if (columns.Contains("MR %"))
                                {
                                    flvalue = Item.Field<float>("MR %");
                                }

                                ItemsList.Add(new KeyValuePair<float, int>(flvalue, i));
                                i += 1;
                                ItemList.Add(flvalue);
                            }
                            break;

                        case "Weight":
                            foreach (DataRow Item in BaleDataTable.Rows)
                            {
                                if (columns.Contains("Weight"))
                                {
                                    if (ind == "Weight")
                                        flvalue = (float)(Item.Field<float>(ind) * Coef);
                                }
                                ItemsList.Add(new KeyValuePair<float, int>(flvalue, i));
                                i += 1;
                                ItemList.Add(flvalue);
                            }  
                            break;


                        case "SpareSngFld3":
                            foreach (DataRow Item in BaleDataTable.Rows)
                            {
                                if (columns.Contains("%CV"))
                                {
                                    if (ind == "SpareSngFld3") //CV%
                                        flvalue = Item.Field<float>("%CV");
                                }

                                ItemsList.Add(new KeyValuePair<float, int>(flvalue, i));
                                i += 1;
                                ItemList.Add(flvalue);
                            }
                            break;
                    }

                    HiValue = ItemList.Max().ToString("#0.00");
                    LowValue = ItemList.Min().ToString("#0.00");
                    AvgValue = ItemList.Average().ToString("#0.00");

                    //Calculate STD
                    foreach (var Value in ItemList)
                    {
                        sumOfDerivation += (Value - Convert.ToSingle(AvgValue)) * (Value - Convert.ToSingle(AvgValue));
                    }
                    float Variance = (float)(sumOfDerivation / (ItemList.Count - 1));

                    STDValue = Math.Sqrt(Variance).ToString("#0.00");

                    i = 0;
                    foreach (DataRow Item in BaleDataTable.Rows)
                    {
                        ItemsAvg.Add(new KeyValuePair<float, int>(Convert.ToSingle(AvgValue), i));
                        i += 1;
                    }
                    TxtStatus = "Bales in this Graph = " + BaleDataTable.Rows.Count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in ProcessGraph " + ex.Message);
            }
        }

        internal void SetupGraph(DataTable cryRepTable)
        {
            BaleDataTable = cryRepTable;
            BaleInGraph = BaleDataTable.Rows.Count;

            try
            {
                



            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in GraphOneModel SetupGraph " + ex.Message);
                
            }
        }
    }
}
