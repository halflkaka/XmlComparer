
using XmlCompare.Views;
using System.Windows;
using Prism.Mef;
using System.ComponentModel.Composition.Hosting;
using XmlCompare.Core;
using XmlCompare.DAL;
using XmlCompare.ToolSetting;
using XmlCompare.UseControl;

namespace XmlCompare
{
    class Bootstrapper : MefBootstrapper
    {
        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(CoreModule).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(DALModule).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(ToolSettingModule).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(UseControlModule).Assembly));
            //this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(CheckModule).Assembly));
            //this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(ScDbServiceModule).Assembly));
            //this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(EedDbServiceModule).Assembly));
            //this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(ExcelServiceModule).Assembly));
            //this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(EmDbServiceModule).Assembly));

        }

        protected override DependencyObject CreateShell()
        {
            return Container.GetExportedValue<MainWindow>();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            //Application.Current.MainWindow = (MainWindow)this.Shell;
            Application.Current.MainWindow.Show();
        }
    }
}
