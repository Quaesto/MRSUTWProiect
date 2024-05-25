using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MRSTWEb.Domain.Repositories
{
    public class BookRepository : IRepository<Book>
    {
        private EF.AppContext db;
        public BookRepository(EF.AppContext db)
        {
            this.db = db;
        }
        public void Create(Book item)
        {
            db.Books.Add(item);
        }

        public void Delete(int id)
        {
            var book = db.Books.FirstOrDefault(c => c.Id == id);
            if (book != null)
            {
                db.Books.Remove(book);
            }
        }

        public Book Get(int id)
        {
            return db.Books.Include(x => x.Review).Include(x => x.Discount).FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Book> GetAll()
        {
            return db.Books.Include(x => x.Review).Include(x => x.Discount).ToList();
        }

        public IEnumerable<Order> GetAllOrdersWithUsers(string userId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Book> GetByBook(int bookId)
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
            if (book != null)
            {
                book.Author = item.Author;
                book.PathImage = item.PathImage;
                book.Title = item.Title;
                book.Price = item.Price;
                book.Genre = item.Genre;
                book.Language = item.Language;
            }
        }
    }
}
