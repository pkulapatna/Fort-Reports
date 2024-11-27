using FieldsColumnSelect.ViewModels;
using Prism.Events;
using RTRep.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FieldsColumnSelect.Views
{
    /// <summary>
    /// Interaction logic for FieldModifyView.xaml
    /// </summary>
    public partial class FieldModifyView : UserControl
    {
        private IEventAggregator _eventAggregator;
        public FieldModifyView(IEventAggregator eventAggregator, ObservableCollection<SqlReportField> ReportField, int HdrIndex, string obj)
        {
            InitializeComponent();
            this._eventAggregator = eventAggregator;
            this.DataContext =  new FieldModifyViewModel(eventAggregator, ReportField, HdrIndex, obj);
        }

       
    }
}
