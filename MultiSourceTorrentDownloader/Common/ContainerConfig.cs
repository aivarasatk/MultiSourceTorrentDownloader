using MultiSourceTorrentDownloader.Interfaces;
using MultiSourceTorrentDownloader.Services;
using MultiSourceTorrentDownloader.ViewModels;
using Ninject.Modules;

namespace MultiSourceTorrentDownloader.Common
{
    public class ContainerConfig : NinjectModule
    {
        public override void Load()
        {
            Bind<MainViewModel>().ToSelf();
            Bind<TorrentInfoDialogViewModel>().ToSelf();

            Bind<ILogService>().To<LogService>().InSingletonScope();

            Bind<ILeetxSource>().To<LeetxSource>().InTransientScope();
            Bind<ILeetxParser>().To<LeetxParser>().InTransientScope();

            Bind<IThePirateBaySource>().To<ThePirateBaySource>().InTransientScope();
            Bind<IThePirateBayParser>().To<ThePirateBayParser>().InTransientScope();

            Bind<IRargbSource>().To<RargbSource>().InTransientScope();
            Bind<IRargbParser>().To<RargbParser>().InTransientScope();

            Bind<IUserConfiguration>().To<UserConfiguration>().InSingletonScope();
        }
    }
}
