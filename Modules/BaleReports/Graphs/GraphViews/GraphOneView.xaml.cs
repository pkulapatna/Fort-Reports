using BaleReports.Graphs.GraphViewModels;
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

namespace BaleReports.Graphs.GraphViews
{
    /// <summary>
    /// Interaction logic for GraphOne.xaml
    /// </summary>
    public partial class GraphOneView : UserControl
    {
        public GraphOneViewModel GraphViewModel;

        public GraphOneView(string reportTitle, IEventAggregator _eventAggregator)
        {
            InitializeComponent();

            GraphViewModel = new GraphOneViewModel(reportTitle, _eventAggregator);
            DataContext = GraphViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDlg = new PrintDialog();

            try
            {
                if(printDlg.ShowDialog() == true)
                {
                    printDlg.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;
                    Size pageSize = new Size(printDlg.PrintableAreaWidth - 30, printDlg.PrintableAreaHeight - 30);
                    _PrintGrid.Measure(pageSize);
                    _PrintGrid.Arrange(new Rect(0, 0, pageSize.Width, pageSize.Height));
                    _PrintGrid.UpdateLayout();
                    printDlg.PrintVisual(_PrintGrid, "My Canvas");
                }

            }
            finally
            {
                printDlg = null;
            }
        }
    }
}
