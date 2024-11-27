using Prism.Events;
using SQLDataTansfer.ViewModels;
using System;
using System.Collections.Generic;
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

namespace SQLDataTansfer.Views
{
    /// <summary>
    /// Interaction logic for SqlFieldsMap.xaml
    /// </summary>
    public partial class SqlFieldsMap : UserControl
    {
        private IEventAggregator _eventAggregator;


        public SqlFieldsMap(IEventAggregator eventAggregator, string FieldName)
        {
            InitializeComponent();
            this._eventAggregator = eventAggregator;
            this.DataContext = new SqlFieldsMapViewModel(_eventAggregator, FieldName);
        }
    }
}
