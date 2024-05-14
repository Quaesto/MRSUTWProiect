using MRSTWEb.BusinessLogic.BusinessModels;
using MRSTWEb.BusinessLogic.DTO;
using System.Collections.Generic;

namespace MRSTWEb.BusinessLogic.Interfaces
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
