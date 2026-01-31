using System.Threading.Tasks;
using Labb3.Models;
using MongoDB.Driver;

namespace Labb3.Data
{
    internal sealed class SeedService
    {
        private readonly IMongoCollection<QuestionPack> _packs;

        public SeedService(IMongoDatabase db)
        {
            _packs = db.GetCollection<QuestionPack>("questionPacks");
        }

        public async Task SeedAsync()
        {
            var any = await _packs.Find(_ => true).AnyAsync().ConfigureAwait(false);
            if (any) return;

            var demo = new QuestionPack("Demo Pack", Difficulty.Easy, 20);
            demo.Questions.Add(new Question("2 + 2 = ?", "4", "1", "2", "3"));
            demo.Questions.Add(new Question("Huvudstad i Sverige?", "Stockholm", "Göteborg", "Malmö", "Uppsala"));

            await _packs.InsertOneAsync(demo).ConfigureAwait(false);
        }
    }
}

