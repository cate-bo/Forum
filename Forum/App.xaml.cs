using Forum.Models;
using Forum.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Forum
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            ServiceCollection builder = new ServiceCollection();

            builder.AddDbContext<ForumContext>(options =>
            options.UseSqlServer("Data Source=CATE_;Initial Catalog=Forum;Integrated Security=True;TrustServerCertificate=True"));

            builder.AddTransient<MainWindow>();
            builder.AddTransient<MainViewModel>();

            ServiceProvider = builder.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow window = ServiceProvider.GetRequiredService<MainWindow>();
            window.Show();
        }
    }

}
