using AutoMapper;
using MRSTWEb.BusinessLogic.DTO;
using MRSTWEb.BuisnessLogic.Interfaces;
using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;

namespace MRSTWEb.BuisnessLogic.Services
{
    public class ManageBooksService : IManageBooksService
    {
        private IUnitOfWork DataBase { get; set; }
        public ManageBooksService(IUnitOfWork DataBase) { this.DataBase = DataBase; }

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
            DataBase.Save();
        }
        public void AddBook(BookDTO bookDTO)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<BookDTO, Book>()).CreateMapper();
            var book = mapper.Map<BookDTO, Book>(bookDTO);
            DataBase.Books.Create(book);
            DataBase.Save();
        }
        public void DeleteBook(int BookId)
        {
            DataBase.Books.Delete(BookId);
            DataBase.Save();
        }

        public void Dispose()
        {
            DataBase.Dispose();
        }
    }
}
