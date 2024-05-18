using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MRSTWEb.Domain.Repositories
{
    public class ReviewRepository : IRepository<Review>
    {
        private EF.AppContext db;
        public ReviewRepository() { db = new EF.AppContext(); }

        public void Create(Review item)
        {
            db.Reviews.Add(item);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var review = db.Reviews.Find(id);
            if (review != null)
            {
                db.Reviews.Remove(review);
                db.SaveChanges();
            }
        }

        public Review Get(int id)
        {
            return db.Reviews.Find(id);
        }

        public IEnumerable<Review> GetAll()
        {
            return db.Reviews.ToList();
        }
        public IEnumerable<Review> GetByBook(int bookId)
        {
            return db.Reviews.Where(x => x.BookId == bookId).ToList();

        }
        public IEnumerable<Order> GetAllOrdersWithUsers(string userId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Order> GetOrdersByUserId(string userId)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Review item)
        {
            var review = db.Reviews.Find(item.Id);
            if (review != null)
            {
                review.Id = item.Id;
                review.Rating = item.Rating;
                review.Comment = item.Comment;
                review.ApplicationUserId = item.ApplicationUserId;
                review.BookId = item.BookId;
                review.IsFavourite = item.IsFavourite;
                db.SaveChanges();
            }
        }
    }
}
