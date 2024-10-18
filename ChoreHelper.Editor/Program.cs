using System.Reactive.Concurrency;
using System.Windows;
using ChoreHelper.Editor.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Serilog.Sinks.SystemConsole.Themes;
using Splat;
using Microsoft.Extensions.Hosting;
using Serilog;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace ChoreHelper.Editor;

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
                services.AddChoreoHelperEditor();
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

        if (Locator.Current.GetRequiredService<IViewFor<MainViewModel>>() is not Window mainView)
        {
            app.Shutdown();
            return;
        }

        app.MainWindow = mainView;
        mainView.Show();
        app.Run();
    }
}