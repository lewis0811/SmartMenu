﻿using System.Linq.Expressions;

namespace SmartMenu.Domain.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        T GetByID(Guid id);

        IEnumerable<T> GetAll();

        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        Task<T> FindObjectAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindListAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        IQueryable<T> EnableQuery();

        void Add(T entity);

        void AddRange(IEnumerable<T> entities);

        void Update(T entity);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);
    }
}