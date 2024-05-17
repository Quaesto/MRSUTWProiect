using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MRSTWEb.Domain.Repositories
{
    public class BookRepository : IRepository<Book>
    {
        private EF.AppContext db;
        public BookRepository()
        {
            this.db = new EF.AppContext();
        }
        public void Create(Book item)
        {
            db.Books.Add(item);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var book = db.Books.FirstOrDefault( c => c.Id == id);
            if ( book != null)
            {
                db.Books.Remove(book);
                db.SaveChanges();
            }
        }

        public Book Get(int id)
        {
            return db.Books.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Book> GetAll()
        {
            return db.Books.ToList();
        }

        public IEnumerable<Order> GetAllOrdersWithUsers(string userId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Order> GetOrdersByUserId(string userId)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Book item)
        {
           var book = Get(item.Id);
            if(book != null )
            {
                book.Author = item.Author;
                book.PathImage = item.PathImage;
                book.Title = item.Title;
                book.Price = item.Price;  
                book.Language = item.Language;  
                book.Genre = item.Genre;
            }
            db.SaveChanges();
        }
    }
}
