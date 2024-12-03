using System.Windows;
using ChoreoHelper.Shell;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using ReactiveUI.Extensions;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

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

        var viewModel = Locator.Current.GetRequiredService<ShellViewModel>();

        if (Locator.Current.GetRequiredService<IViewFor<ShellViewModel>>() is not Window mainView)
        {
            app.Shutdown();
            return;
        }

        app.MainWindow = mainView;
        mainView.DataContext = viewModel;
        viewModel.Activator.Activate();
        mainView.Show();
        app.Run();
    }
}