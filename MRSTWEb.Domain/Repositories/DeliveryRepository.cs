using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace MRSTWEb.Domain.Repositories
{
    public class DeliveryRepository : IRepository<DeliveryCost>
    {
        private EF.AppContext db;
        private MemoryCache _cache = MemoryCache.Default;
        public DeliveryRepository()
        {
            this.db = new EF.AppContext();
        }
        public void Create(DeliveryCost item)
        {
            db.DeliveryCost.Add(item);
            db.SaveChanges();
            InvalidateCache();
        }

        public void Delete(int id)
        {
            var deliveryCost = db.DeliveryCost.Find(id);
            if (deliveryCost != null)
            {
                db.DeliveryCost.Remove(deliveryCost);
                db.SaveChanges();
                InvalidateCache();
            }
        }

        public DeliveryCost Get(int id)
        {
            string cacheKey = $"DeliveryCost_{id}";
            if (_cache.Contains(cacheKey))
            {
                return (DeliveryCost)_cache.Get(cacheKey);
            }

            var deliveryCost = db.DeliveryCost.Find(id);
            if (deliveryCost != null)
            {
                _cache.Set(cacheKey, deliveryCost, DateTimeOffset.Now.AddMinutes(10));
            }

            return deliveryCost;
        }

        public IEnumerable<DeliveryCost> GetAll()
        {
            string cacheKey = "AllDeliveryCosts";
            if (_cache.Contains(cacheKey))
            {
                return (IEnumerable<DeliveryCost>)_cache.Get(cacheKey);
            }

            var deliveryCosts = db.DeliveryCost.ToList();
            if (deliveryCosts != null && deliveryCosts.Any())
            {
                _cache.Set(cacheKey, deliveryCosts, DateTimeOffset.Now.AddMinutes(10));
            }

            return deliveryCosts;
        }

        public IEnumerable<Order> GetAllOrdersWithUsers(string userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DeliveryCost> GetByBook(int bookId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Order> GetOrdersByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public void Update(DeliveryCost item)
        {
            var delivery = db.DeliveryCost.Find(item.Id);
            if (delivery != null)
            {
                delivery.Cost = item.Cost;
                db.Entry(delivery).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                InvalidateCache();
            }
        }

        public void UpdateBookDiscount(decimal Price, int bookId)
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
    }
}
