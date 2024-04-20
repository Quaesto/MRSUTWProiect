using MRSTWEb.BuisnessLogic.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSTWEb.BuisnessLogic.Interfaces
{
    public interface IOrderService
    {
        void MakeOrder(OrderDTO orderDTO);
        IEnumerable<OrderDTO> GetOrders(string category);
        OrderDTO GetOrder(int id);
        IEnumerable<OrderDTO> GetOrdersByUserId(string id);
        bool DeleteOrdersByUserId(string userId);
        void Dispose();
    }
}
