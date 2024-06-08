using MRSTWEb.BusinessLogic.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSTWEb.BusinessLogic.Interfaces
{
    public interface ISearchService
    {
        IEnumerable<BookDTO> GetAllBooks();
        IEnumerable<BookDTO> Search(string keyword);
      
        IEnumerable<BookDTO> AdvancedSearch(string title, string author, string genre, string language, int minPrice, int maxPrice);
        void Dispose();
    }
}
