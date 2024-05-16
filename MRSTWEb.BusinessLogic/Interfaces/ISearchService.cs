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
        IEnumerable<BookDTO> Search(string keyword);
        IEnumerable<BookDTO> FilterPrice(int minValue, int maxValue);
        void Dispose();
    }
}
