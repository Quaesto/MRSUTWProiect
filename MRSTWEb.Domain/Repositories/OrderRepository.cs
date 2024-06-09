using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;
using System.Transactions;

namespace MRSTWEb.Domain.Repositories
{
    public class OrderRepository : IRepository<Order>
    {
        private EF.AppContext db;
        private MemoryCache _cache = MemoryCache.Default;

        public OrderRepository()
        {
            this.db = new EF.AppContext();
        }

        public void Create(Order item)
        {

            db.Orders.Add(item);
            db.SaveChanges();
            InvalidateCache();
        }

        public void Delete(int id)
        {

            var order = db.Orders.Find(id);
            if (order != null)
            {
                db.Orders.Remove(order);
                db.SaveChanges();
                InvalidateCache();
            }
        }

        public IEnumerable<Order> GetAllOrdersWithUsers(string userId)
        {
            string cacheKey = $"OrdersWithUsers_{userId}";
            if (_cache.Contains(cacheKey))
            {
                return (IEnumerable<Order>)_cache.Get(cacheKey);
            }

            var orders = db.Orders
                .Where(o => o.ApplicationUserId == userId)
                .Include(o => o.ApplicationUser).Include(o => o.Items)
                .ToList();

            if (orders != null && orders.Any())
            {
                _cache.Set(cacheKey, orders, DateTimeOffset.Now.AddMinutes(10));
            }

            return orders;
        }

        public Order Get(int id)
        {
            string cacheKey = $"Order_{id}";
            if (_cache.Contains(cacheKey))
            {
                return (Order)_cache.Get(cacheKey);
            }

            var order = db.Orders
                .Include(o => o.Items.Select(i => i.Book))
                .SingleOrDefault(o => o.Id == id);

            if (order != null)
            {
                _cache.Set(cacheKey, order, DateTimeOffset.Now.AddMinutes(10));
            }

            return order;
        }

        public IEnumerable<Order> GetOrdersByUserId(string userId)
        {
            string cacheKey = $"OrdersByUser_{userId}";
            if (_cache.Contains(cacheKey))
            {
                return (IEnumerable<Order>)_cache.Get(cacheKey);
            }

            var orders = db.Orders
                .Include(o => o.Items.Select(i => i.Book))
                .Where(o => o.ApplicationUserId == userId)
                .ToList();

            if (orders != null && orders.Any())
            {
                _cache.Set(cacheKey, orders, DateTimeOffset.Now.AddMinutes(10));
            }

            return orders;
        }

        public IEnumerable<Order> GetAll()
        {
            string cacheKey = "AllOrders";
            if (_cache.Contains(cacheKey))
            {
                return (IEnumerable<Order>)_cache.Get(cacheKey);
            }

            var orders = db.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.Items.Select(i => i.Book))
                .ToList();

            if (orders != null && orders.Any())
            {
                _cache.Set(cacheKey, orders, DateTimeOffset.Now.AddMinutes(10));
            }

            return orders;
        }

        public void Update(Order item)
        {
            var order = db.Orders.Find(item.Id);
            if (order != null)
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

                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                InvalidateCache();
            }
        }

        public IEnumerable<Order> GetByBook(int bookId)
        {
            throw new NotImplementedException();
        }

        private void InvalidateCache()
        {
            foreach (var item in _cache)
            {
                _cache.Remove(item.Key);
            }
        }

        public void UpdateBookDiscount(decimal Price, int bookId)
        {
            throw new NotImplementedException();
        }
    }
}
