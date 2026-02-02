using System.Collections.Generic;
using System.Threading.Tasks;
using Labb3.Models;
using MongoDB.Driver;

namespace Labb3.Services
{
    internal sealed class CategoryService
    {
        private readonly IMongoCollection<Category> _categories;

        public CategoryService(IMongoDatabase database)
        {
            _categories = database.GetCollection<Category>("categories");
        }

        public async Task<IList<Category>> LoadAllAsync()
        {
            return await _categories.Find(_ => true)
                                   .SortBy(c => c.Name)
                                   .ToListAsync()
                                   .ConfigureAwait(false);
        }

        public async Task<Category?> GetByIdAsync(string id)
        {
            var filter = Builders<Category>.Filter.Eq(c => c.Id, id);
            return await _categories.Find(filter)
                                   .FirstOrDefaultAsync()
                                   .ConfigureAwait(false);
        }

        public async Task CreateAsync(Category category)
        {
            await _categories.InsertOneAsync(category).ConfigureAwait(false);
        }

        public async Task UpdateAsync(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Id))
            {
                await CreateAsync(category);
                return;
            }

            var filter = Builders<Category>.Filter.Eq(c => c.Id, category.Id);
            await _categories.ReplaceOneAsync(
                filter,
                category,
                new ReplaceOptions { IsUpsert = true }
            ).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<Category>.Filter.Eq(c => c.Id, id);
            await _categories.DeleteOneAsync(filter).ConfigureAwait(false);
        }

        public async Task<bool> ExistsAsync(string name)
        {
            var filter = Builders<Category>.Filter.Eq(c => c.Name, name);
            return await _categories.Find(filter)
                                   .AnyAsync()
                                   .ConfigureAwait(false);
        }
    }
}
