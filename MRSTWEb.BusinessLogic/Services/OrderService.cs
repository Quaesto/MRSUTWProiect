using AutoMapper;
using MRSTWEb.BuisnessLogic.BuisnessModels;
using MRSTWEb.BuisnessLogic.DTO;
using MRSTWEb.BuisnessLogic.Interfaces;
using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Web;

namespace MRSTWEb.BuisnessLogic.Services
{
    public class OrderService : IOrderService
    {
        IUnitOfWork DataBase { get; set; }
        public OrderService(IUnitOfWork uow) { DataBase = uow; }
        public bool DeleteOrdersByUserId(string userId)
        {
            try
            {
                var orders = DataBase.Orders.GetOrdersByUserId(userId);
                if (orders != null)
                {
                    foreach (var order in orders)
                    {
                        DataBase.Orders.Delete(order.Id);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        public void Dispose()
        {
            DataBase.Dispose();
        }

        public OrderDTO GetOrder(int id)
        {
            var order = DataBase.Orders.Get(id);
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderDTO>()).CreateMapper();
            return mapper.Map<Order, OrderDTO>(order);
        }

        public IEnumerable<OrderDTO> GetOrders(string category)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Order>, List<OrderDTO>>(DataBase.Orders.GetAll());
        }

        public IEnumerable<OrderDTO> GetOrdersByUserId(string id)
        {
            var orders = DataBase.Orders.GetOrdersByUserId(id);
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Order>, List<OrderDTO>>(orders);
        }

        public void MakeOrder(OrderDTO orderDTO)
        {
            var cartItems = (List<Item>)HttpContext.Current.Session["cart"];



            Order order = new Order
            {
                FirstName = orderDTO.FirstName,
                LastName = orderDTO.LastName,
                Phone = orderDTO.Phone,
                City = orderDTO.City,
                Address = orderDTO.Address,
                PostCode = orderDTO.PostCode,
                Email = orderDTO.Email,
                TotalSumToPay = orderDTO.TotalSumToPay,
                BuyingTime = DateTime.Now,
                ApplicationUserId = orderDTO.ApplicationUserId
            };

            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    BookId = cartItem.Book.Id, // Associate existing Product with OrderItem
                    Order = order,
                    Quantity = cartItem.Quantity,

                };

                order.Items.Add(orderItem);
            }
            DataBase.Orders.Create(order);
            DataBase.Save();
        }
    }
}
