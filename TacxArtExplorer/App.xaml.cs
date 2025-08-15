using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http; 
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;
using TacxArtExplorer.Services;
using TacxArtExplorer.Services.HTTPClients;
using TacxArtExplorer.Services.SQL;
using TacxArtExplorer.Services.SQLClients;
using TacxArtExplorer.ViewModels;

namespace TacxArtExplorer;

  public partial class App : Application
{
    private IHost _host;
    private const string _connectionStringKey = "Data/artcache.sqlite";
    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // DB connection and initialization
                services.AddSingleton<ISqliteConnectionFactory>(_ =>
                    new SqliteConnectionFactory(_connectionStringKey));
                services.AddSingleton<IDbInitializer, SqliteDbInitializer>();

                services.AddScoped<SqliteConnection>(sp => sp.GetRequiredService<ISqliteConnectionFactory>().Create());

                // HTTP clients
                services.AddKeyedTransient<IArticApiClient, ArticApiClient>("ArtworkClient", (s, _) => new ArticApiClient("https://api.artic.edu/api/v1/"));
                services.AddKeyedTransient<IArticApiClient, ArticApiClient>("ImageClient", (s, _) => new ArticApiClient("https://www.artic.edu/iiif/2/"));

                // API clients
                services.AddTransient<IArticImageClient, ArticImageClient>();
                services.AddTransient<IArticArtworkClient, ArticArtworkClient>();

                // Domain services
                services.AddSingleton<IArtCacheService, CacheService>();
                services.AddSingleton<IArtAPIService, ArtAPIService>();

                services.AddHostedService<BackgroundSyncer>();

                // add keyed to allow mocking in tests whilst keeping general interface
                // and allowing specific instance to be required
                services.AddKeyedSingleton<IArtService, ArtService>("ArtStore");
                services.AddSingleton<IImageRepsitory, ImageRepository>();

                services.AddSingleton<INavigationService, NavigationService>();

                // Viewmodels
                services.AddTransient<ArtDetailViewModel>();
                services.AddTransient<ArtListViewModel>();
                services.AddSingleton<MainWindowViewModel>();

                // Views
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        // One-time DB creation/seed
        await _host.Services.GetRequiredService<IDbInitializer>().InitializeAsync();

        _host.Services.GetRequiredService<INavigationService>().NavigateToList();
        _host.Services.GetRequiredService<MainWindow>().Show();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
    }
}



