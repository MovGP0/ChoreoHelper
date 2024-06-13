using System.Reactive.Concurrency;
using System.Windows;
using ChoreoHelper.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace ChoreoHelper;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostBuilderContext, services) =>
            {
                services.AddLogging();
                services.UseMicrosoftDependencyResolver();
                var resolver = Locator.CurrentMutable;
                resolver.InitializeSplat();
                resolver.InitializeReactiveUI();
                services.AddChoreoHelper();
            })
            .ConfigureLogging((hostBuilderContext, loggingBuilder) =>
            {
                loggingBuilder.AddSerilog(dispose: true);
            })
            .UseSerilog((hostBuilderContext, loggerConfiguration) =>
            {
                loggerConfiguration
                    .MinimumLevel.Information()
                    .WriteTo.Console(theme: AnsiConsoleTheme.Code);
            })
            .Build();

        host.Services.UseMicrosoftDependencyResolver();

        var app = new Application();

        RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
        RxApp.MainThreadScheduler = DispatcherScheduler.Current;

        if (Locator.Current.GetRequiredService<IViewFor<MainWindowViewModel>>() is not Window mainView)
        {
            app.Shutdown();
            return;
        }

        app.MainWindow = mainView;
        mainView.Show();
        app.Run();
    }
}