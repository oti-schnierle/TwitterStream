using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TwitterStub.Interfaces.Data;

namespace TwitterStub.Repository
{
    public class BaseRepository : IRepository
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<BaseRepository> _logger;

        public BaseRepository(ILogger<BaseRepository> logger, IServiceProvider services)
        {
            _services = services;
            _logger = logger;
        }

        public async Task Add<T>(T item) where T : class, new()
        {
            try
            {
                using (var scope = _services.CreateScope())
                {
                    var _dbContext =
                        scope.ServiceProvider
                            .GetRequiredService<TwitterStubContext>();

                    await _dbContext.AddAsync(item);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            using (var scope = _services.CreateScope())
            {
                var _dbContext =
                    scope.ServiceProvider
                        .GetRequiredService<TwitterStubContext>();

                return await _dbContext.Set<T>().Where(expression).AsNoTracking().ToListAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAll<T>() where T : class, new()
        {
            using (var scope = _services.CreateScope())
            {
                var _dbContext =
                    scope.ServiceProvider
                        .GetRequiredService<TwitterStubContext>();

                return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
            }
        }
    }
}
