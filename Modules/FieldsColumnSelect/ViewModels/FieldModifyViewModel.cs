using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using RTRep.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static RTRep.Services.ClsApplicationService;

namespace FieldsColumnSelect.ViewModels
{
    public class FieldModifyViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;

        private bool _bModify = false;
        public bool BModify
        {
            get { return _bModify; }
            set
            {
                SetProperty(ref _bModify, value);
                BReadonly = !value;
            }
        }

        private bool _bReadonly = true;
        public bool BReadonly
        {
            get { return _bReadonly; }
            set { SetProperty(ref _bReadonly, value); }
        }

        private string _fdlValue;
        public string FdlValue
        {
            get { return _fdlValue; }
            set { SetProperty(ref _fdlValue, value); }
        }

        private string _dBField;
        public string DBField
        {
            get { return _dBField; }
            set { SetProperty(ref _dBField, value); }
        }

        private string _columnField;
        public string ColumnField
        {
            get { return _columnField; }
            set { SetProperty(ref _columnField, value); }
        }

        private int _headerIndex;
        public int HeaderIndex
        {
            get => _headerIndex; 
            set => SetProperty(ref _headerIndex, value); 
        }

       

        private ObservableCollection<SqlReportField> _reportField;
        public ObservableCollection<SqlReportField> ReportField
        {
            get => _reportField;
            set => SetProperty(ref _reportField, value); 
        }


        private DelegateCommand _loadedPageICommand;
        public DelegateCommand LoadedPageICommand =>
        _loadedPageICommand ?? (_loadedPageICommand = new DelegateCommand(LoadedPageICommandExecute));
        private void LoadedPageICommandExecute()
        {
         
        }

        private DelegateCommand _closedPageICommand;
        public DelegateCommand ClosedPageICommand =>
        _closedPageICommand ?? (_closedPageICommand = new DelegateCommand(ClosedPageICommandExecute));
        private void ClosedPageICommandExecute()
        {
           
        }

        private DelegateCommand _settingsCommand;
        public DelegateCommand SettingsCommand =>
        _settingsCommand ?? (_settingsCommand = new DelegateCommand(SettingsCommandExecute));
        private void SettingsCommandExecute()
        {
            BModify = true;
        }

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
        _saveCommand ?? (_saveCommand =
            new DelegateCommand(SaveCommandExecute).ObservesCanExecute(() => BModify));
        private void SaveCommandExecute()
        {
            BModify = false;
            SaveModColums(ReportField);
        }

        /// <summary>
        /// Update field that changes => save reportField
        /// </summary>
        /// <param name="reportField"></param>
        private void SaveModColums(ObservableCollection<SqlReportField> reportField)
        {
            try
            {
                switch (ColumnField)
                {
                    case "Field Expression":

                        reportField[HeaderIndex].FieldExp = FdlValue;
           
                        break;

                    case "FieldName":

                        reportField[HeaderIndex].FieldName = FdlValue;

                        break;

                    case "Format":

                        reportField[HeaderIndex].Format = FdlValue;

                        break;
                }
                ClsCommon.ReportGridView = reportField;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in SaveModColums" + ex.Message);
            }
            CloseChildWindows();
        }

        private void CloseChildWindows()
        {
            _eventAggregator.GetEvent<CloseFieldModWindow>().Publish(true);
        }

        public FieldModifyViewModel(IEventAggregator eventAggregator, ObservableCollection<SqlReportField> reportField, int hdrIndex, string column)
        {
            this._eventAggregator = eventAggregator;

       
             ReportField = reportField;
            HeaderIndex = hdrIndex;

            DBField = reportField[hdrIndex].DbField.ToString();
            ColumnField = column;

            switch (column)
            {
                case "Field Expression":

                    FdlValue = reportField[hdrIndex].FieldExp.ToString();
                    break;

                case "FieldName":

                    FdlValue = reportField[hdrIndex].FieldName.ToString();
                    break;

                case "Format":

                    FdlValue = reportField[hdrIndex].Format.ToString();
                    break;
            }
        }
    }
}
