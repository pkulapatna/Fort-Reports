using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FieldsColumnSelect.Model;
using FieldsColumnSelect.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using RTRep.Services;
using static RTRep.Services.ClsApplicationService;

namespace FieldsColumnSelect.ViewModels
{
    public class FieldSelectViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;
        private readonly FieldSelectModel fieldSelModel;

        private Window ModWindow;

        private string _outPutColString;
        public string OutPutColString
        {
            get { return _outPutColString; }
            set { SetProperty(ref _outPutColString, value); }
        }



        /// <summary>
        /// Sql Fields List
        /// </summary>
        private List<Tuple<string, string>> _sqlFieldList;
        public List<Tuple<string, string>> SqlFieldList
        {
            get { return _sqlFieldList; }
            set { SetProperty(ref _sqlFieldList, value); }
        }
        private ObservableCollection<string> _sqlColumnList;
        public ObservableCollection<string> SqlColumnList
        {
            get { return _sqlColumnList; }
            set { SetProperty(ref _sqlColumnList, value); }
        }
        private string _selectedColName;
        public string SelectedColName
        {
            get { return _selectedColName; }
            set { SetProperty(ref _selectedColName, value); }
        }
        private int _colSelectedIndex;
        public int ColSelectedIndex
        {
            get { return _colSelectedIndex; }
            set { SetProperty(ref _colSelectedIndex, value); }
        }

        /// <summary>
        /// Custom fields List
        /// </summary>
        /// 
        private List<Tuple<string, string>> _custFieldList;
        public List<Tuple<string, string>> CustFieldList
        {
            get { return _custFieldList; }
            set { SetProperty(ref _custFieldList, value); }
        }
        private ObservableCollection<string> _custColumnList;
        public ObservableCollection<string> CustColumnList
        {
            get { return _custColumnList; }
            set { SetProperty(ref _custColumnList, value); }
        }
        private string _selectedCusColName;
        public string SelectedCusColName
        {
            get { return _selectedCusColName; }
            set { SetProperty(ref _selectedCusColName, value); }
        }
        private int _CusColSelectedIndex;
        public int CusColSelectedIndex
        {
            get { return _CusColSelectedIndex; }
            set { SetProperty(ref _CusColSelectedIndex, value); }
        }


        //---------------------------------------------------------------------------
        /// <summary>
        /// Moisture type list
        /// </summary>
        private List<Tuple<string, string,string,string>> _moisturetFieldList;
        public List<Tuple<string, string,string,string>> MoistureFieldList
        {
            get { return _moisturetFieldList; }
            set { SetProperty(ref _moisturetFieldList, value); }
        }


        private ObservableCollection<string> _mtColumnList;
        public ObservableCollection<string> MtColumnList
        {
            get { return _mtColumnList; }
            set { SetProperty(ref _mtColumnList, value); }
        }
        private string _selectedMtColName;
        public string SelectedMtColName
        {
            get { return _selectedMtColName; }
            set { SetProperty(ref _selectedMtColName, value); }
        }
        private int mtColSelectedIndex;
        public int MtColSelectedIndex
        {
            get { return mtColSelectedIndex; }
            set { SetProperty(ref mtColSelectedIndex, value); }
        }
        //----------------------------------------------------------------------------


        private ObservableCollection<SqlReportField> _reportField;
        public ObservableCollection<SqlReportField> ReportField
        {
            get { return _reportField; }
            set { SetProperty(ref _reportField, value); }
        }

        private int _SelectedS1TabIndex;
        public int SelectedS1TabIndex
        {
            get { return _SelectedS1TabIndex; }
            set { SetProperty(ref _SelectedS1TabIndex, value); }
        }

        private bool _bModify =  false;
        public bool BModify
        {
            get { return _bModify; }
            set { SetProperty(ref _bModify, value); }
        }


        private int _selectHdrIndex = 0;
        public int SelectHdrIndex
        {
            get { return _selectHdrIndex; }
            set { SetProperty(ref _selectHdrIndex, value); }
        }


        private object _selectHdrItem;
        public object SelectHdrItem
        {
            get { return _selectHdrItem; }
            set { SetProperty(ref _selectHdrItem, value); }
        }

        private DelegateCommand _loadedPageICommand;
        public DelegateCommand LoadedPageICommand =>
        _loadedPageICommand ?? (_loadedPageICommand = new DelegateCommand(LoadPageExecute));
        private void LoadPageExecute()
        {
            int listidx = 0;
          
            if (SqlFieldList != null) SqlFieldList = null;
            SqlFieldList = new List<Tuple<string, string>>();
            SqlFieldList = fieldSelModel.GetColumnList();
            SqlFieldList = SqlFieldList.OrderBy(q => q).ToList();
            SqlColumnList = new ObservableCollection<string>();

            for (int i = 0; i < SqlFieldList.Count; i++)
            {
                if(SqlFieldList[i].Item1.ToString() != "Moisture")
                    SqlColumnList.Add(SqlFieldList[i].Item1.ToString());
                listidx = i;
            }

            ReportField = fieldSelModel.GetXMLReportList();
            OutPutColString = string.Empty;
            OutPutColString = ClsCommon.ReportFields;

            CustColumnList = new ObservableCollection<string>();
            CustFieldList = new List<Tuple<string, string>>() 
            { 
                new Tuple<string, string>("BlankField1", "nvarchar"),
                new Tuple<string, string>("BlankField2", "nvarchar"),
                new Tuple<string, string>("BlankField3", "nvarchar")
            };
            for (int i = 0; i < CustFieldList.Count; i++)
            {
                CustColumnList.Add(CustFieldList[i].Item1.ToString());
            }

            MtColumnList = new ObservableCollection<string>();
            MoistureFieldList = new List<Tuple<string, string, string, string>>()
            {
                new Tuple<string, string,string,string>("%MC", "moisture", "MC %", "real"),
                new Tuple<string, string,string,string>("%MR", "Moisture/(1-(Moisture/100))", "MR %", "real"),
                new Tuple<string, string,string,string>("%AD", "(100-Moisture)/.9", "AD %", "real"),
                new Tuple<string, string,string,string>("%BD", "100-Moisture", "BD %", "real")
            };
            for (int i = 0; i < MoistureFieldList.Count; i++)
            {
                MtColumnList.Add(MoistureFieldList[i].Item1.ToString());
            }
        }

        private DelegateCommand _settingsCommand;
        public DelegateCommand SettingsCommand =>
        _settingsCommand ?? (_settingsCommand = new DelegateCommand(SettingsCommandExecute));
        private void SettingsCommandExecute()
        {
            BModify = true;
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
        _cancelCommand ?? (_cancelCommand = 
            new DelegateCommand(CancelCommandExecute).ObservesCanExecute(() => BModify));
        private void CancelCommandExecute()
        {
            BModify = false;
            //Not Save Data and Close!
            CloseChildWindows();
        }
        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
        _saveCommand ?? (_saveCommand =
            new DelegateCommand(SaveCommandExecute).ObservesCanExecute(() => BModify));
        private void SaveCommandExecute()
        {
            BModify = false;

            //Save Data and Close!
            fieldSelModel.SaveFieldColumns(ReportField);
            CloseChildWindows();
        }
        private void CloseChildWindows()
        {
            _eventAggregator.GetEvent<CloseFieldSelWindow>().Publish(true);
        }

        /// <summary>
        /// Move selected column one position to left 
        /// </summary>
        private DelegateCommand _moveUpCommand;
        public DelegateCommand MoveUpCommand =>
        _moveUpCommand ?? (_moveUpCommand =
            new DelegateCommand(MoveUpCommandExecute).ObservesCanExecute(() => BModify));
        private void MoveUpCommandExecute()
        {
            if (SelectHdrIndex > 0)
            {
                ObservableCollection<SqlReportField> newlist = (ObservableCollection<SqlReportField>)ReportField;
                int NewIndex = SelectHdrIndex - 1;

                if ((NewIndex > -1) || (NewIndex >= ReportField.Count))
                {
                    object selected = SelectHdrItem;

                    // Removing removable element ItemsControl.ItemsSource
                    newlist.Remove((SqlReportField)selected);
                    // Insert it in new position
                    newlist.Insert(NewIndex, (SqlReportField)selected);
                    // Restore selection
                    ReportField = newlist;
                }
            }
        }

        /// <summary>
        ///  Move selected column one position to right 
        /// </summary>
        private DelegateCommand _moveDownCommand;
        public DelegateCommand MoveDownCommand =>
        _moveDownCommand ?? (_moveDownCommand =
            new DelegateCommand(MoveDownCommandExecute).ObservesCanExecute(() => BModify));
        private void MoveDownCommandExecute()
        {
            if ((SelectHdrIndex > -1) & (SelectHdrIndex + 1 < ReportField.Count))
            {
                ObservableCollection<SqlReportField> newlist = (ObservableCollection<SqlReportField>)ReportField;
                int NewIndex = SelectHdrIndex + 1;
                object selected = SelectHdrItem;

                //Remove selected item
                newlist.Remove((SqlReportField)selected);
                // Insert it in new position
                newlist.Insert(NewIndex, (SqlReportField)selected);
                // Restore selection
                ReportField = newlist;
            }
        }

        private DelegateCommand _insertItemCommand;
        public DelegateCommand InsertItemCommand =>
        _insertItemCommand ?? (_insertItemCommand = 
            new DelegateCommand(InsertItemCommandExecute).ObservesCanExecute(() => BModify));
        private void InsertItemCommandExecute()
        {
            switch (SelectedS1TabIndex)
            {
                case 0: //tab 1

                    for (int i = 0; i < SqlFieldList.Count; i++)
                    {
                        if (SqlFieldList[i].Item1 == SelectedColName)
                            AddToReportField(i, SqlFieldList);
                    }
                    break;

                case 1: //tab 2 CustomField Blank Fields
                    for (int i = 0; i < CustFieldList.Count; i++)
                    {
                        if (CustFieldList[i].Item1 == SelectedCusColName)
                            AddToReportField(i, CustFieldList);
                    }
                    break;

                case 2: //tab 3 Moisture fields
                    for (int i = 0; i < MoistureFieldList.Count; i++)
                    {
                        if (MoistureFieldList[i].Item1 == SelectedMtColName)
                            AddMtToReportField(i, MoistureFieldList);
                    }
                    break;

                default:
                    break;
            }
        }

        private void AddToReportField(int i, List<Tuple<string, string>> sqlFieldList)
        {  
            ReportField.Add(new SqlReportField(sqlFieldList[i].Item1, sqlFieldList[i].Item1, sqlFieldList[i].Item1, GetFieldFormat(sqlFieldList[i].Item2), sqlFieldList[i].Item2));
        }
        private void AddMtToReportField(int i, List<Tuple<string, string, string, string>> sqlFieldList)
        {
            ReportField.Add(new SqlReportField(sqlFieldList[i].Item1, sqlFieldList[i].Item2, sqlFieldList[i].Item3, GetFieldFormat(sqlFieldList[i].Item4), sqlFieldList[i].Item4));
        }

        private string GetFieldFormat(string item)
        {
            string strFormat = item;

            if (item == "int") strFormat = "0";
            if (item == "real") strFormat = "00.00";
            if (item == "nvarchar") strFormat = "@";
            if (item == "datetime") strFormat = "mm/dd/yyyy";

            if (item == "bit") strFormat = "0";
            if (item == "smallint") strFormat = "0";

            return strFormat;
        }


        /// <summary>
        /// Remove selected column from Header.
        /// </summary>
        private DelegateCommand _removeitemCommand;
        public DelegateCommand RemoveitemCommand =>
        _removeitemCommand ?? (_removeitemCommand =
            new DelegateCommand(RemoveitemCommandExecute).ObservesCanExecute(()=> BModify));
        private void RemoveitemCommandExecute()
        {
            ObservableCollection<SqlReportField> newlist = (ObservableCollection<SqlReportField>)ReportField;
            if (SelectHdrIndex > -1)
            {
                object selected = SelectHdrItem;
                newlist.Remove((SqlReportField)selected);
                ReportField = newlist;
            }
        }

        public FieldSelectViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;

            fieldSelModel = new FieldSelectModel(_eventAggregator);
            ReportField = new ObservableCollection<SqlReportField>();

            _eventAggregator.GetEvent<ListViewHdrClickEvent>().Subscribe(ListViewHdrClick);
            _eventAggregator.GetEvent<CloseFieldModWindow>().Subscribe(CloseFieldModDialog);
        }

        private void LoadListView(bool obj)
        {
            if(ReportField == null) ReportField = new ObservableCollection<SqlReportField>();    
        }

        private void ListViewHdrClick(string obj)
        {
            if ((ReportField.Count > 0) & (obj != "SQL Field"))
            {
                if (ModWindow != null) ModWindow = null;
                ModWindow = new Window
                {
                    Title = "Field Modification " + ReportField[SelectHdrIndex].FieldName.ToString() + " => Field Expression",
                    Width = 560,
                    Height = 360,
                    Content = new FieldModifyView(_eventAggregator, ReportField, SelectHdrIndex, obj)
                };

                ModWindow.ResizeMode = ResizeMode.NoResize;
                ModWindow.ShowDialog();
            }
        }
        private void CloseFieldModDialog(bool obj)
        {
            if (obj)
            {
                ReportField = ClsCommon.ReportGridView;

                if (ModWindow != null)
                {
                    ModWindow.Close();
                    ModWindow = null;
                }
            }
        }
    }
}
