using MRSTWEb.BuisnessLogic.BuisnessModels;
using MRSTWEb.BuisnessLogic.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSTWEb.BuisnessLogic.Interfaces
{
    public interface ICartService
    {
        void AddToCart(int BookId);
        List<Item> GetCart();
        void ClearSession();
        void RemoveFromTheCart(int BookId);
        IEnumerable<BookDTO> GetBooks();
        BookDTO GetPBook(int? id);
        decimal CalculateTotalPrice();
        void Dispose();
    }
}
