using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MRSTWEb.Domain.Repositories
{
    public class DiscountRepository : IRepository<Discount>
    {
        private EF.AppContext db;
        public DiscountRepository(EF.AppContext db)
        {
            this.db = db;
        }
        public void Create(Discount item)
        {
            db.Discounts.Add(item);

        }

        public void Delete(int id)
        {
            var discount = db.Discounts.Find(id);
            db.Discounts.Remove(discount);
        }

        public Discount Get(int id)
        {
            return db.Discounts.Find(id);
        }

        public IEnumerable<Discount> GetAll()
        {
            return db.Discounts.ToList();
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
            }
        }
    }
}
