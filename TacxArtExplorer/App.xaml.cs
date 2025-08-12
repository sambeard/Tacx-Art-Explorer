using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.Windows;
using TacxArtExplorer.Services;
using Microsoft.Extensions.DependencyInjection;
using TacxArtExplorer.ViewModels;
using Microsoft.Extensions.Http; 
using System.Net.Http;
using Microsoft.Extensions.Logging;
using TacxArtExplorer.Services.HTTPClients;

namespace TacxArtExplorer;

  public partial class App : Application
{
    private IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddHttpClient<ArticArtworkClient>();
                services.AddHttpClient<ArticImageClient>();

                // domain/services
                services.AddSingleton<IArtCacheService, CacheService>();
                services.AddSingleton<IArtAPIService, ArtAPIService>();
                services.AddSingleton<ArtService>();
                services.AddSingleton<INavigationService, NavigationService>();

                // viewmodels
                services.AddTransient<ArtDetailViewModel>();
                services.AddTransient<ArtListViewModel>();
                services.AddSingleton<MainWindowViewModel>();

                // views
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        _host.Services.GetRequiredService<INavigationService>().NavigateToList();
        _host.Services.GetRequiredService<MainWindow>().Show();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        base.OnExit(e);
    }
}



