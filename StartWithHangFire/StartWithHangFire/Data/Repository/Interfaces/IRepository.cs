using System;
using System.Collections.Generic;

namespace StartWithHangFire.Data.Repository.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class
    {
        List<T> GetAll();

        T GetById(string id);

        void Add(T obj);

        void Update(string id, T obj);

        void Remove(string id);

        int SaveChanges();
    }
}
