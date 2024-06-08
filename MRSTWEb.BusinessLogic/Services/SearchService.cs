using AutoMapper;
using MRSTWEb.BusinessLogic.DTO;
using MRSTWEb.BusinessLogic.Interfaces;
using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using MRSTWEb.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MRSTWEb.BusinessLogic.Services
{
    public class SearchService : ISearchService
    {
        private IUnitOfWork DataBase;
        public SearchService() { DataBase = new EFUnitOfWork(null); }

        public IEnumerable<BookDTO> GetAllBooks()
        {
            var books = DataBase.Books.GetAll();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Book, BookDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Book>, IEnumerable<BookDTO>>(books);
        }

      

        public IEnumerable<BookDTO> Search(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return null;

            keyword = keyword.Trim();

            string searchItem;
            decimal? maxPrice;
            ProcessKeyword(keyword, out searchItem, out maxPrice);

            var books = SearchBooks(searchItem, maxPrice);

          

            return MapToDTO(books);
        }

        private void ProcessKeyword(string keyword, out string searchItem, out decimal? maxPrice)
        {
            searchItem = keyword;
            maxPrice = null;

            var priceMatch = Regex.Match(keyword, @"(\$|£|€)?(\d+(\.\d+)?)");
            if (priceMatch.Success)
            {
                searchItem = keyword.Substring(0, priceMatch.Index).Trim();
                string priceString = priceMatch.Value;
                decimal price;
                if (decimal.TryParse(priceString, out price))
                {
                    maxPrice = price;
                }
            }

            searchItem = searchItem.ToLower();
        }
        private IQueryable<Book> SearchBooks(string searchItem, decimal? maxPrice)
        {
            var books = DataBase.Books.GetAll()
                .Where(x => x.Title.ToLower().Contains(searchItem) ||
                            x.Author.ToLower().Contains(searchItem) ||
                            x.Genre.ToLower().Contains(searchItem) ||
                            x.Language.ToLower().Contains(searchItem) ||
                            (maxPrice.HasValue && (x.Price >= maxPrice - 2 && x.Price <= maxPrice + 2)));

            return books.AsQueryable();
        }

      
        private IEnumerable<BookDTO> MapToDTO(IEnumerable<Book> books)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Book, BookDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Book>, List<BookDTO>>(books);
        }
     
        public void Dispose()
        {
            DataBase.Dispose();
        }

        public IEnumerable<BookDTO> AdvancedSearch(string title, string author, string genre, string language, int minPrice, int maxPrice)
        {

            var results = DataBase.Books.GetAll().Where(x => x.Price != 0 && x.Price >= minPrice && x.Price <= maxPrice
            && x.Title != null &&  x.Title.ToLower().Contains(title.ToLower())
            && x.Author != null && x.Author.ToLower().Contains(author.ToLower())
            && x.Genre != null && x.Genre.ToLower().Contains(genre.ToLower())
            && x.Language != null &&  x.Language.ToLower().Contains(language.ToLower()));

            return MapToDTO(results);
        }
    }
}
