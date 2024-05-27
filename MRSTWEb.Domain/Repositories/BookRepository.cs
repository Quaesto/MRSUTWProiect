using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;

namespace MRSTWEb.Domain.Repositories
{
    public class BookRepository : IRepository<Book>
    {
        private EF.AppContext db;

        private MemoryCache _cache = MemoryCache.Default;
        public BookRepository()
        {
            this.db = new EF.AppContext();
        }
        public void Create(Book item)
        {
            db.Books.Add(item);
            db.SaveChanges();
            InvalidateCache();
        }

        public void Delete(int id)
        {
            var book = db.Books.FirstOrDefault(c => c.Id == id);
            if (book != null)
            {
                db.Books.Remove(book);
                db.SaveChanges();
                InvalidateCache();
            }
        }

        public void UpdateBookDiscount(decimal Price, int bookId)
        {
            var book = db.Books.Find(bookId);
            if (book != null)
            {
                book.Price = Price;
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                InvalidateCache();
            }
        }

        public Book Get(int id)
        {
            string cacheKey = $"Book_{id}";
            if (_cache.Contains(cacheKey))
            {
                return (Book)_cache.Get(cacheKey);
            }

            var book = db.Books.Include(x => x.Review).Include(x => x.Discount).FirstOrDefault(p => p.Id == id);
            if (book != null)
            {
                _cache.Set(cacheKey, book, DateTimeOffset.Now.AddMinutes(10));
            }

            return book;
        }

        public IEnumerable<Book> GetAll()
        {
            string cacheKey = "AllBooks";
            if (_cache.Contains(cacheKey))
            {
                return (IEnumerable<Book>)_cache.Get(cacheKey);
            }

            var books = db.Books.Include(x => x.Review).Include(x => x.Discount).ToList();
            if (books != null && books.Any())
            {
                _cache.Set(cacheKey, books, DateTimeOffset.Now.AddHours(10));
            }

            return books;
        }

        public IEnumerable<Order> GetAllOrdersWithUsers(string userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Book> GetByBook(int bookId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Order> GetOrdersByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public void Update(Book item)
        {
            var book = db.Books.Find(item.Id);
            if (book != null)
            {
                book.Author = item.Author;
                book.PathImage = item.PathImage;
                book.Title = item.Title;
                book.Price = item.Price;
                book.Genre = item.Genre;
                book.Language = item.Language;

                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                InvalidateCache();
            }
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
