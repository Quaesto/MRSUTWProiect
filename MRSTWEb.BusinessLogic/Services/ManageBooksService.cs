using AutoMapper;
using MRSTWEb.BusinessLogic.DTO;
using MRSTWEb.BuisnessLogic.Interfaces;
using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using MRSTWEb.Domain.Repositories;
using System.Linq;
using System;

namespace MRSTWEb.BuisnessLogic.Services
{
    public class ManageBooksService : IManageBooksService
    {
        private IUnitOfWork DataBase { get; set; }
        public ManageBooksService() { this.DataBase = new EFUnitOfWork(null); }

        public void UpdateProduct(BookDTO bookDTO)
        {
            var book = new Book
            {
                Id = bookDTO.Id,
                Title = bookDTO.Title,
                Author = bookDTO.Author,
                Genre = bookDTO.Genre,
                Language = bookDTO.Language,
                PathImage = bookDTO.PathImage,
                Price = bookDTO.Price,
            };

            var bookDiscount = DataBase.Books.Get(book.Id);

            decimal previousPrice = bookDiscount.Price;
            if (previousPrice != book.Price && bookDiscount.Discount != null && bookDiscount.Discount.ExpirationTime >= DateTime.Now)
            {
                var discount = (book.Price * bookDiscount.Discount.Percentage) / 100;
                book.Price -= discount;


            }


            DataBase.Books.Update(book);
   
        }

        public void AddBook(BookDTO bookDTO)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<BookDTO, Book>()).CreateMapper();
            var book = mapper.Map<BookDTO, Book>(bookDTO);
            DataBase.Books.Create(book);
       
        }
        public void DeleteBook(int BookId)
        {
            var book = DataBase.Books.Get(BookId);
            if (book.Review.Any())
            {
                var reviewsId = book.Review.Select(r => r.Id).ToList();
                foreach (var reviewId in reviewsId)
                {
                    DataBase.Reviews.Delete(reviewId);
                }
            }
            if (book.Discount != null)
            {
                DataBase.Discounts.Delete(BookId);
            }
            DataBase.Books.Delete(BookId);
        
        }

        public void Dispose()
        {
            DataBase.Dispose();
        }
    }
}
