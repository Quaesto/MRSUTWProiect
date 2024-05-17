using MRSTWEb.BusinessLogic.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSTWEb.BusinessLogic.Interfaces
{
    public interface IManageBooksService
    {
        void UpdateProduct(BookDTO bookDTO);
        void AddBook(BookDTO bookDTO);
        void DeleteBook(int BookId);
        void Dispose();
    }
}
