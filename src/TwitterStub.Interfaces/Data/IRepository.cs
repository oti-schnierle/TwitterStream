using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TwitterStub.Interfaces.Data
{
    public interface IRepository
    {
        Task Add<T>(T item) where T : class, new();

        Task<IEnumerable<T>> GetAll<T>() where T : class, new();

        Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> expression) where T : class, new();
    }
}