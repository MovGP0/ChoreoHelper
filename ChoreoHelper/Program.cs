﻿using System.Windows;
using ChoreoHelper.MainWindow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using ReactiveUI.Extensions;
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
        var host = Host
            .CreateDefaultBuilder(args)
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