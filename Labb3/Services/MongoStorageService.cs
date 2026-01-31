using System.Collections.Generic;
using System.Threading.Tasks;
using Labb3.Models;
using MongoDB.Driver;

namespace Labb3.Services
{
    internal sealed class MongoStorageService
    {
        private readonly IMongoCollection<QuestionPack> _packs;

        public MongoStorageService(IMongoDatabase database)
        {
            _packs = database.GetCollection<QuestionPack>("questionPacks");
        }

        public async Task<IList<QuestionPack>> LoadAsync()
        {
            return await _packs.Find(_ => true)
                              .ToListAsync()
                              .ConfigureAwait(false);
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
