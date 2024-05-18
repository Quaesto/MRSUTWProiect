using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MRSTWEb.Domain.Repositories
{
    public class OrderRepository : IRepository<Order>
    {
        private EF.AppContext db;
        public OrderRepository()
        {
            this.db = new EF.AppContext();
        }
        public void Create(Order item)
        {
            db.Orders.Add(item);
        }

        public void Delete(int id)
        {
            db.Orders.Remove(db.Orders.Find(id));
        }
        public IEnumerable<Order> GetAllOrdersWithUsers(string userId)
        {
            return db.Orders
                 .Where(o => o.ApplicationUserId == userId)
                 .Include(o => o.ApplicationUser).Include(o => o.Items)
                 .ToList();
        }
        public Order Get(int id)
        {
            var order = db.Orders.Find(id); 
            return order;   
        }
        public IEnumerable<Order> GetOrdersByUserId(string userId)
        {
            List<Order> orders = new List<Order>();
            foreach (var order in db.Orders.Include(p => p.Items.Select(i => i.Book)))
            {
                if (order.ApplicationUserId == userId)
                {
                    orders.Add(order);
                }
            }
            return orders;
        }
        public IEnumerable<Order> GetAll()
        {
            return db.Orders.Include(o => o.ApplicationUser).Include(o => o.Items.Select(i => i.Book)).ToList();
        }

        public void Update(Order item)
        {
            var order = db.Orders.Find(item.Id);
            if(order != null)
            {
                order.Address = item.Address;
                order.City = item.City;
                order.Email = item.Email;   
                order.Phone = item.Phone;
                order.FirstName = item.FirstName;
                order.LastName = item.LastName;
                order.ApplicationUser = item.ApplicationUser;
                order.ApplicationUserId = item.ApplicationUserId;
                order.TotalSumToPay = item.TotalSumToPay;   
                order.BuyingTime = item.BuyingTime;
                order.PostCode = item.PostCode; 
            }
        }

        public IEnumerable<Order> GetByBook(int bookId)
        {
            throw new System.NotImplementedException();
        }
    }
}
