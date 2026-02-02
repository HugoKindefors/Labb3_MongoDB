using System.Threading.Tasks;
using Labb3.Models;
using MongoDB.Driver;

namespace Labb3.Data
{
    internal sealed class SeedService
    {
        private readonly IMongoCollection<QuestionPack> _packs;
        private readonly IMongoCollection<Category> _categories;

        public SeedService(IMongoDatabase db)
        {
            _packs = db.GetCollection<QuestionPack>("questionPacks");
            _categories = db.GetCollection<Category>("categories");
        }

        public async Task SeedAsync()
        {
            await SeedCategoriesAsync().ConfigureAwait(false);
            await SeedQuestionPacksAsync().ConfigureAwait(false);
        }

        private async Task SeedCategoriesAsync()
        {
            var any = await _categories.Find(_ => true).AnyAsync().ConfigureAwait(false);
            if (any) return;

            var defaultCategories = new[]
            {
                new Category("Allmänkunskap"),
                new Category("Historia"),
                new Category("Geografi"),
                new Category("Vetenskap"),
                new Category("Sport"),
                new Category("Underhållning"),
                new Category("Musik"),
                new Category("Film"),
                new Category("Litteratur"),
                new Category("Teknologi")
            };

            await _categories.InsertManyAsync(defaultCategories).ConfigureAwait(false);
        }

        private async Task SeedQuestionPacksAsync()
        {
            var any = await _packs.Find(_ => true).AnyAsync().ConfigureAwait(false);
            if (any) return;

            var category = await _categories.Find(_ => true).FirstOrDefaultAsync().ConfigureAwait(false);

            var demo = new QuestionPack("Demo Pack", Difficulty.Easy, 20)
            {
                CategoryId = category?.Id
            };
            demo.Questions.Add(new Question("2 + 2 = ?", "4", "1", "2", "3"));
            demo.Questions.Add(new Question("Huvudstad i Sverige?", "Stockholm", "Göteborg", "Malmö", "Uppsala"));

            await _packs.InsertOneAsync(demo).ConfigureAwait(false);
        }
    }
}

