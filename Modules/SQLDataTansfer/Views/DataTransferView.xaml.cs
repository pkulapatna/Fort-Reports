using Prism.Events;
using RTRep.Services;
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
    /// Interaction logic for DataTransferView.xaml
    /// </summary>
    public partial class DataTransferView : UserControl
    {
        protected readonly IEventAggregator _eventAggregator = new EventAggregator();

        private DataTransferViewModel XferViewModel;

        public DataTransferView()
        {
            InitializeComponent();

            XferViewModel = new DataTransferViewModel(_eventAggregator);
            this.DataContext = XferViewModel;
        }

        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            XferViewModel.ListViewClickHandler(((GridViewColumnHeader)e.OriginalSource).Column.Header.ToString());
        }

        private void GridViewWlColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {

        }
    }
}
