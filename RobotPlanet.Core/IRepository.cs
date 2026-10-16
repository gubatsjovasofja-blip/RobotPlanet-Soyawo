using System;
using System.Collections.Generic;

namespace RobotPlanet.Core
{
    public interface IRepository<T> where T : Robot
    {
        void Add(T item);
        bool Remove(T item);
        IEnumerable<T> GetAll();
        T? Find(Func<T, bool> predicate);
    }
}