using MultiSourceTorrentDownloader.Interfaces;
using MultiSourceTorrentDownloader.Services;
using MultiSourceTorrentDownloader.ViewModels;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiSourceTorrentDownloader.Common
{
    public class ContainerConfig : NinjectModule
    {
        public override void Load()
        {
            Bind<MainViewModel>().ToSelf();
            Bind<ILogService>().To<LogService>().InSingletonScope();
            Bind<ITorrentSource>().To<ThePirateBaySource>();
        }
    }
}
