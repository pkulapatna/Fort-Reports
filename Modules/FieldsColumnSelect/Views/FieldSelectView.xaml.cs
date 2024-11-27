using FieldsColumnSelect.ViewModels;
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
using static RTRep.Services.ClsApplicationService;

namespace FieldsColumnSelect.Views
{
    /// <summary>
    /// Interaction logic for FieldSelectView.xaml
    /// </summary>
    public partial class FieldSelectView : UserControl
    {
        protected readonly IEventAggregator _eventAggregator;
        public FieldSelectView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            this._eventAggregator = eventAggregator;
            this.DataContext = new FieldSelectViewModel(eventAggregator);
        }

        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ListViewHdrClickEvent>().Publish(((GridViewColumnHeader)e.OriginalSource).Column.Header.ToString());
        }
    }
}
