using BaleReports.Model;
using BaleReports.ViewModels;
using Prism.Events;
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

namespace BaleReports.Views
{
    /// <summary>
    /// Interaction logic for BaleSetupView.xaml
    /// </summary>
    public partial class BaleSetupView : UserControl
    {
        protected readonly IEventAggregator _eventAggregator;
        public BaleSetupView(IEventAggregator EventAggregator)
        {
            InitializeComponent();
            this._eventAggregator = EventAggregator;
            this.DataContext = new BaleSetupViewModel(_eventAggregator);
        }
    }
}
