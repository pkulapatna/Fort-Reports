using BaleReports.ViewModels;
using Prism.Events;
using RTRep.Services;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace BaleReports.Views
{
    /// <summary>
    /// Interaction logic for BaleReportView.xaml
    /// </summary>
    public partial class BaleReportView : UserControl
    {
        protected readonly IEventAggregator _eventAggregator = new EventAggregator();
        private Xmlhandler MyXml;
        private ObservableCollection<SqlReportField> ReportXmlFile;

        public BaleReportView()
        {
            InitializeComponent();
            this.DataContext = new BaleReportViewModel(_eventAggregator);
            MyXml = Xmlhandler.Instance;

            ReportXmlFile = MyXml.ReadXMlRepColumn(ClsCommon.ReportXmlFilePath);

        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if(ReportXmlFile.Count > 0)
            {

                for (int i = 0; i < ReportXmlFile.Count; i++)
                {
                    if (e.PropertyName.Contains(ReportXmlFile[i].FieldName))
                    {
                        e.Column.ClipboardContentBinding.StringFormat = ReportXmlFile[i].Format;
                    }      
                }
            }
           

        }
    }
}
