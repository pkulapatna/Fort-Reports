using FortéRTREP.ViewModels;
using Prism.Events;
using RTRep.Services;
using System;
using System.Threading;
using System.Windows;
using Form = System.Windows.Forms;

namespace FortéRTREP.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow AppWindows;
        protected readonly IEventAggregator _eventAggregator = new EventAggregator();
        private System.Threading.Mutex objMutex = null;

        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItem1;

        public bool bWindowHideState = false;

        public MainWindow()
        {
            InitializeComponent();
            AppWindows = this;
           
            objMutex = new System.Threading.Mutex(false, "FortéReports.exe");
            if (!objMutex.WaitOne(0, false))
            {
                objMutex.Close();
                objMutex = null;
                MessageBox.Show("Another instance is already running!");
                AppWindows.Close();
                Thread.CurrentThread.Abort();
            }
       
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            menuItem1 = new System.Windows.Forms.MenuItem();

            // Initialize contextMenu1
            this.contextMenu1.MenuItems.AddRange(
                        new System.Windows.Forms.MenuItem[] { this.menuItem1 });

            // Initialize menuItem1
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "S&how";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);

            System.Windows.Forms.NotifyIcon m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.Icon = new System.Drawing.Icon("CLOCK02.ICO");
            m_notifyIcon.Visible = true;

            m_notifyIcon.BalloonTipText = "FortéRTReports is in Apps's tray.";
            m_notifyIcon.BalloonTipTitle = "Right Click on Icon to Restore";
            m_notifyIcon.Text = "FortéRT Reports";
            m_notifyIcon.ContextMenu = this.contextMenu1;   
        }
        private void menuItem1_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
            bWindowHideState = false;
        }

        private void MainWindow_Load(object sender, RoutedEventArgs e)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public void SetTitle(string strTitle)
        {
            this.Title = strTitle.ToString();
            MyTitle.Content = strTitle.ToString();
        }

        public void SetupAppTitle(string strTitle)
        {
            SetTitle(strTitle);
        }

        private void Win_LeftBtnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in MainWindow Win_LeftBtnMouseDown " + ex.Message);
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ClsSerilog.CloseLogger();
        }
    }
}
