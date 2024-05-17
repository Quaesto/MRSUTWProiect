using AutoMapper;
using MRSTWEb.BusinessLogic.DTO;
using MRSTWEb.BusinessLogic.Interfaces;
using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using MRSTWEb.Domain.Repositories;

namespace MRSTWEb.BusinessLogic.Services
{
    public class ManageBooksService : IManageBooksService
    {
        private IUnitOfWork DataBase { get; set; }
        public ManageBooksService() { DataBase = new EFUnitOfWork(); }
        public void AddBook(BookDTO bookDTO)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<BookDTO, Book>()).CreateMapper();
            var book = mapper.Map<BookDTO, Book>(bookDTO);
            DataBase.Books.Create(book);
    
        }

        public void DeleteBook(int BookId)
        {
            DataBase.Books.Delete(BookId);
 
        }

       

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

            DataBase.Books.Update(book);

        }
        public void Dispose()
        {
            DataBase.Dispose();
        }
    }
}
