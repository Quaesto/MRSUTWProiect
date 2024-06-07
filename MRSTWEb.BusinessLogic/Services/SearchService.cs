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

        public IEnumerable<BookDTO> FilterPrice(int minValue, int maxValue)
        {
            decimal minPrice = (decimal)minValue;
            decimal maxPrice = (decimal)maxValue;

            var books = DataBase.Books.GetAll().Where(x => x.Price >= minPrice && x.Price <= maxPrice);

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Book, BookDTO>()).CreateMapper();
            var bookDTOs = mapper.Map<IEnumerable<Book>, IEnumerable<BookDTO>>(books);

            return bookDTOs;
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

            var advancedSearch = PerformAdvancedSearch(books, keyword, maxPrice);

            return MapToDTO(advancedSearch.Any() ? advancedSearch : books);
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

        private IEnumerable<Book> PerformAdvancedSearch(IQueryable<Book> books, string keyword, decimal? maxPrice)
        {
            var advancedSearch = books.Where(x => FuzzyMatch(x.Title.ToLower(), keyword) ||
                                                   FuzzyMatch(x.Author.ToLower(), keyword) ||
                                                   FuzzyMatch(x.Genre.ToLower(), keyword) ||
                                                   FuzzyMatch(x.Language.ToLower(), keyword) ||
                                                   (maxPrice.HasValue && (x.Price >= maxPrice - 2 && x.Price <= maxPrice + 2)));

            return advancedSearch;
        }
        private IEnumerable<BookDTO> MapToDTO(IEnumerable<Book> books)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Book, BookDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Book>, List<BookDTO>>(books);
        }
        private bool FuzzyMatch(string text, string keyword)
        {
            int maxErrors = 2;
            int errors = 0;
            int i = 0, j = 0;
            while (i < text.Length && j < keyword.Length)
            {
                if (text[i] != keyword[j])
                {
                    errors++;
                    if (errors > maxErrors)
                        return false;
                }
                else
                {
                    j++;
                }
                i++;
            }
            return true;
        }
        public void Dispose()
        {
            DataBase.Dispose();
        }

        public IEnumerable<BookDTO> AdvancedSearch(string title, string author, string genre, string language, int minPrice, int maxPrice)
        {
            var results = DataBase.Books.GetAll().Where(x => x.Price >= minPrice && x.Price <= maxPrice
            && FuzzyMatch(x.Title.ToLower(), title)
            && FuzzyMatch(x.Author.ToLower(), author)
            && FuzzyMatch(x.Genre.ToLower(), genre)
            && FuzzyMatch(x.Language.ToLower(), language));

            return MapToDTO(results);
        }
    }
}
