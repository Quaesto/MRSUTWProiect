using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MRSTWEb.Domain.Repositories
{
    public class DeliveryRepository : IRepository<DeliveryCost>
    {
        private EF.AppContext db;
        public DeliveryRepository(EF.AppContext db)
        {
            this.db = db;
        }
        public void Create(DeliveryCost item)
        {
            db.DeliveryCost.Add(item);

        }

        public void Delete(int id)
        {
            var deliveryCost = db.DeliveryCost.Find(id);
            db.DeliveryCost.Remove(deliveryCost);

        }

        public DeliveryCost Get(int id)
        {
            return db.DeliveryCost.Find(id);
        }

        public IEnumerable<DeliveryCost> GetAll()
        {
            return db.DeliveryCost.ToList();
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
            }
        }
    }
}
