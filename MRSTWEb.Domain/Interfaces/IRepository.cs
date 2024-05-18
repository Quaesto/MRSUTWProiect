using MRSTWEb.Domain.Entities;
using System.Collections.Generic;

namespace MRSTWEb.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T Get(int id);
        void Create(T item);
        void Delete(int id);
        void Update(T item);
        IEnumerable<Order> GetAllOrdersWithUsers(string userId);
        IEnumerable<Order> GetOrdersByUserId(string userId);
        IEnumerable<T> GetByBook(int bookId);
    }
}
