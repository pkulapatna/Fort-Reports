using Prism.Commands;
using Prism.Events;
using RTRep.Services;
using System;
using System.Collections.Generic;
using System.Data;
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
using WLReports.Model;
using WLReports.ViewModels;

namespace WLReports.Views
{
    /// <summary>
    /// Interaction logic for WLReportView.xaml
    /// </summary>
    public partial class WLReportView : UserControl
    {
        protected readonly IEventAggregator _eventAggregator = new EventAggregator();

        public WLReportView()
        {
            InitializeComponent();
            this.DataContext = new WLReportViewModel(_eventAggregator);
        }


      
        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.StartsWith("ID"))
                e.Column.Header = "Number";

            if (e.PropertyName.StartsWith("BalerID"))
                e.Column.Header = "Baler";

            if (e.PropertyName.StartsWith("Param1"))
                e.Column.Header = "MAX";

            if (e.PropertyName.StartsWith("Param2"))
                e.Column.Header = "MIN";

            if (e.PropertyName.StartsWith("Deviation"))
                e.Column.Header = "CV";

            if (e.PropertyName.StartsWith("Time1"))
                e.Column.Header = "Inp";

            if (e.PropertyName.StartsWith("Time2"))
                e.Column.Header = "Mdl";

            if (e.PropertyName.StartsWith("Time3"))
                e.Column.Header = "Out";

            if (e.PropertyName.StartsWith("Sample"))
                e.Column.Header = "Total";

            if (e.PropertyName.StartsWith("Layers"))
                e.Column.Visibility = Visibility.Hidden;

            if (e.PropertyName.StartsWith("Title"))
                e.Column.Visibility = Visibility.Hidden;
        }
    }
}
