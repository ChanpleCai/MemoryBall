using System;
using System.Threading;
using System.Windows;

namespace MemoryBall
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex _mutex;

        /// <summary>Raises the <see cref="E:System.Windows.Application.Startup" /> event.</summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            _mutex = new Mutex(true, "{MemoryBall-2021}", out var createdNew);

            GC.KeepAlive(_mutex);

            if (!createdNew)
                Current.Shutdown();

            base.OnStartup(e);
        }

        /// <summary>Raises the <see cref="E:System.Windows.Application.Exit" /> event.</summary>
        /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs" /> that contains the event data.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            _mutex.ReleaseMutex();

            base.OnExit(e);
        }
    }
}
