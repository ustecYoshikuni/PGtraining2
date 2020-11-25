using Prism.Ioc;
using PGtraining.SimpleRis.Views;
using System.Windows;
using Prism.Modularity;
using PGtraining.SimpleRis.Modules.ModuleName;
using PGtraining.SimpleRis.Services.Interfaces;
using PGtraining.SimpleRis.Services;
using PGtraining.SimpleRis.Modules.WorkList;

namespace PGtraining.SimpleRis
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMessageService, MessageService>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleNameModule>();
            moduleCatalog.AddModule<WorkListModule>();
        }
    }
}
