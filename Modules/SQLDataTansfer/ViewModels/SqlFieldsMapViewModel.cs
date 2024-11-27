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

namespace SQLDataTansfer.ViewModels
{
    public class SqlFieldsMapViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;
        private ClsSQLhandler SqlHandler;

        private ObservableCollection<string> _comboList;
        public ObservableCollection<string> ComboList
        {
            get => _comboList;
            set => SetProperty(ref _comboList, value);
        }

        private int _selectComboIndex;
        public int SelectComboIndex
        {
            get => _selectComboIndex;
            set
            {
                SetProperty(ref _selectComboIndex, value);
            }
        }
        private string _selectComboVal;
        public string SelectComboVal
        {
            get => _selectComboVal;
            set => SetProperty(ref _selectComboVal, value);
        }

        private string _clientDBField;
        public string ClientDBField
        {
            get => _clientDBField;
            set => SetProperty(ref _clientDBField, value);
        }

       
        public SqlFieldsMapViewModel(IEventAggregator eventAggregator, string ClientField)
        {
            this._eventAggregator = eventAggregator;
            SqlHandler = ClsSQLhandler.Instance;

            ComboList = GetArchTableHdr();
            ClientDBField = ClientField;
        }

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
        _saveCommand ?? (_saveCommand =
            new DelegateCommand(SaveCommandExecute));
        private void SaveCommandExecute()
        {
            //save
            //SelectComboIndex
            //SelectComboVal
            CloseChildWindows(SelectComboVal);
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
        _cancelCommand ?? (_cancelCommand =
            new DelegateCommand(CancelCommandExecute));
        private void CancelCommandExecute()
        {
            CloseChildWindows("cancel");
        }

        
        private void CloseChildWindows(string fieldname)
        {
            _eventAggregator.GetEvent<CloseFieldMapWindow>().Publish(fieldname);
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
    }
}
