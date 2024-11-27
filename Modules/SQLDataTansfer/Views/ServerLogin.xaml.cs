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
    /// Interaction logic for ServerLogin.xaml
    /// </summary>
    public partial class ServerLogin : UserControl
    {
        private IEventAggregator _eventAggregator;
        public ServerLogin(IEventAggregator eventAggregator, string ServerName)
        {
            InitializeComponent();
            this._eventAggregator = eventAggregator;
            this.DataContext = new ServerLoginViewModel(_eventAggregator,ServerName);
        }
    }
}
