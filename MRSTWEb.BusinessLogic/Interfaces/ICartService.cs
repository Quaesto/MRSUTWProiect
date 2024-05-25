using MRSTWEb.BusinessLogic.BusinessModels;
using MRSTWEb.BusinessLogic.DTO;
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
        void SetDiscount(BookDTO bookDto);
        decimal CalculateDiscountAmount(decimal bookPrice, decimal percentage);
        decimal GetBookPriceWithoutDiscount(decimal bookPrice, decimal percentage);
        bool RemoveDiscount(int bookId);
        void SetDelivery(DeliveryCostDTO deliveryDto);
        IEnumerable<DeliveryCostDTO> GetAllDeliveriesCost();
        bool RemoveDeliveryCost(int deliveryCostId);
        void Dispose();
    }
}
