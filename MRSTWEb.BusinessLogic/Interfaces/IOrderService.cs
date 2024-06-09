using MRSTWEb.BusinessLogic.DTO;
using System.Collections.Generic;

namespace MRSTWEb.BusinessLogic.Interfaces
{
    public interface IOrderService
    {
        void MakeOrder(OrderDTO orderDTO);
        /* IEnumerable<OrderDTO> GetOrders(string category);*/
        OrderDTO GetOrder(int id);
        IEnumerable<OrderDTO> GetOrdersByUserId(string id);
        bool DeleteOrdersByUserId(string userId);
        void Dispose();
    }
}
