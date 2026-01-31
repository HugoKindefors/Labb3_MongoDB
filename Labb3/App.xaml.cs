using System.Windows;
using Labb3.Data;

namespace Labb3
{
    public partial class App : Application
    {
        public static MongoContext Mongo { get; private set; } = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Mongo = new MongoContext();

            var seeder = new SeedService(Mongo.Database);
            await seeder.SeedAsync();
        }
    }
}
