using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace MRSTWEb.Domain.Repositories
{
    public class DiscountRepository : IRepository<Discount>
    {
        private EF.AppContext db;
        private MemoryCache _cache = MemoryCache.Default;
        public DiscountRepository()
        {
            this.db = new EF.AppContext();
        }
        public void Create(Discount item)
        {
            db.Discounts.Add(item);
            db.SaveChanges();
            InvalidateCache();
        }

        public void Delete(int id)
        {
            var discount = db.Discounts.Find(id);
            if (discount != null)
            {
                db.Discounts.Remove(discount);
                db.SaveChanges();
                InvalidateCache();
            }
        }

        public Discount Get(int id)
        {
            string cacheKey = $"Discount_{id}";
            if (_cache.Contains(cacheKey))
            {
                return (Discount)_cache.Get(cacheKey);
            }

            var discount = db.Discounts.Find(id);
            if (discount != null)
            {
                _cache.Set(cacheKey, discount, DateTimeOffset.Now.AddMinutes(10));
            }

            return discount;
        }

        public IEnumerable<Discount> GetAll()
        {
            string cacheKey = "AllDiscounts";
            if (_cache.Contains(cacheKey))
            {
                return (IEnumerable<Discount>)_cache.Get(cacheKey);
            }

            var discounts = db.Discounts.ToList();
            if (discounts != null && discounts.Any())
            {
                _cache.Set(cacheKey, discounts, DateTimeOffset.Now.AddMinutes(10));
            }

            return discounts;
        }

        public IEnumerable<Order> GetAllOrdersWithUsers(string userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Discount> GetByBook(int bookId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Order> GetOrdersByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public void Update(Discount item)
        {
            Discount discount = db.Discounts.Find(item.Id);
            if (discount != null)
            {
                discount.ExpirationTime = item.ExpirationTime;
                discount.SetTime = item.SetTime;
                discount.Percentage = item.Percentage;
                db.Entry(discount).State = System.Data.Entity.EntityState.Modified;
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
