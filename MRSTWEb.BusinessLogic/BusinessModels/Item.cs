using MRSTWEb.BusinessLogic.DTO;

namespace MRSTWEb.BusinessLogic.BusinessModels
{
    public class Item
    {
        public BookDTO Book { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Book.Price * Quantity;
    }
}
