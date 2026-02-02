using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Labb3.Models;
using MongoDB.Driver;

namespace Labb3.Services
{
    internal sealed class MongoStorageService
    {
        private readonly IMongoCollection<QuestionPack> _packs;
        private readonly IMongoCollection<Category> _categories;

        public MongoStorageService(IMongoDatabase database)
        {
            _packs = database.GetCollection<QuestionPack>("questionPacks");
            _categories = database.GetCollection<Category>("categories");
        }

        public async Task<IList<QuestionPack>> LoadAsync()
        {
            var packs = await _packs.Find(_ => true)
                              .ToListAsync()
                              .ConfigureAwait(false);

            var categories = await _categories.Find(_ => true)
                                              .ToListAsync()
                                              .ConfigureAwait(false);

            foreach (var pack in packs)
            {
                if (!string.IsNullOrWhiteSpace(pack.CategoryId))
                {
                    var category = categories.FirstOrDefault(c => c.Id == pack.CategoryId);
                    pack.CategoryName = category?.Name;
                }
            }

            return packs;
        }

        public async Task SaveAsync(IEnumerable<QuestionPack> packs)
        {
            foreach (var pack in packs)
            {
                if (string.IsNullOrWhiteSpace(pack.Id))
                {
                    await _packs.InsertOneAsync(pack).ConfigureAwait(false);
                }
                else
                {
                    var filter = Builders<QuestionPack>.Filter.Eq(x => x.Id, pack.Id);

                    await _packs.ReplaceOneAsync(
                        filter,
                        pack,
                        new ReplaceOptions { IsUpsert = true }
                    ).ConfigureAwait(false);
                }
            }
        }
    }
}
