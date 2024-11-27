using BaleReports;
using FortéRTREP.Views;
using Prism.Ioc;
using Prism.Modularity;
using RepSetup;
using RTRep.Services;
using SQLDataTansfer;
using System;
using System.Windows;
using WLReports;

namespace FortéRTREP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private ClsSerilog LogMessage;
        private string swVersion = string.Empty;

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            swVersion = GetLastModTime();

            LogMessage = new ClsSerilog();
            ClsSerilog.LogMessage(ClsSerilog.Info, $"-------------------------------------------------");
            ClsSerilog.LogMessage(ClsSerilog.Info, $"Start Application < {swVersion} >  at {DateTime.Now}");
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            try
            {
                var SqlModuleType = typeof(RepSetupModule);
                moduleCatalog.AddModule(new ModuleInfo()
                {
                    ModuleName = SqlModuleType.Name,
                    ModuleType = SqlModuleType.AssemblyQualifiedName,
                    InitializationMode = InitializationMode.OnDemand
                });

                var BaleModuleType = typeof(BaleReportsModule);
                moduleCatalog.AddModule(new ModuleInfo()
                {
                    ModuleName = BaleModuleType.Name,
                    ModuleType = BaleModuleType.AssemblyQualifiedName,
                    InitializationMode = InitializationMode.OnDemand
                });

                var WLModuleType = typeof(WLReportsModule);
                moduleCatalog.AddModule(new ModuleInfo()
                {
                    ModuleName = WLModuleType.Name,
                    ModuleType = WLModuleType.AssemblyQualifiedName,
                    InitializationMode = InitializationMode.OnDemand
                });

                var XferModuleType = typeof(SQLDataTansferModule);
                moduleCatalog.AddModule(new ModuleInfo()
                {
                    ModuleName = XferModuleType.Name,
                    ModuleType = XferModuleType.AssemblyQualifiedName,
                    InitializationMode = InitializationMode.OnDemand
                });
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("ERROR in ConfigureModuleCatalog " + ex.Message);
            }

        }

        private string GetLastModTime()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(assembly.Location);
            DateTime lastModified = fileInfo.LastWriteTime;
            return $"SW.Ver: {lastModified.ToString()}";
        }

    }
}
