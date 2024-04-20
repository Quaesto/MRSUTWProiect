using AutoMapper;
using MRSTWEb.BusinessLogic.BusinessModels;
using MRSTWEb.BusinessLogic.DTO;
using MRSTWEb.BusinessLogic.Infrastructure;
using MRSTWEb.BusinessLogic.Interfaces;
using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using MRSTWEb.Domain.Repositories;
using System.Collections.Generic;
using System.Web;

namespace MRSTWEb.BusinessLogic.Services
{
    public class CartService : ICartService
    {
        private IUnitOfWork DataBase { get; set; }
        public CartService() { DataBase = new EFUnitOfWork(); }

        public void AddToCart(int BookId)
        {
            var book = DataBase.Books.Get(BookId);
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Book, BookDTO>()).CreateMapper();
            var bookDto = mapper.Map<Book, BookDTO>(book);

            List<Item> cart = GetCart();



            Item newItem = new Item { Book = bookDto, Quantity = 1 };

            int index = IsInCart(cart, BookId);
            if (index != -1)
            {
                cart[index].Quantity++;
            }
            else
            {

                cart.Add(newItem);
            }

            UpdateCart(cart);

        }
        public decimal CalculateTotalPrice()
        {
            decimal totalPrice = 0;
            var cart = GetCart();
            foreach(var item in cart)
            {
                var price = item.Book.Price;
                var quantity = item.Quantity;
                totalPrice += price * quantity; 
            }
            return totalPrice;
        }
        private void UpdateCart(List<Item> cart)
        {
            HttpContext.Current.Session["cart"] = cart;
        }
        public List<Item> GetCart()
        {
            if (HttpContext.Current.Session["cart"] == null)
            {
                HttpContext.Current.Session["cart"] = new List<Item>();
            }
            return (List<Item>)HttpContext.Current.Session["cart"];
        }

        public void Dispose()
        {
            DataBase.Dispose();
        }
        private int IsInCart(List<Item> cart, int BookId)
        {
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Book.Id == BookId)
                {
                    return i;
                }
            }
            return -1;
        }
        public void ClearSession()
        {

            HttpContext.Current.Session.Clear();
        }

        public void RemoveFromTheCart(int BookId)
        {
            List<Item> cart = GetCart();
            int index = IsInCart(cart, BookId);
            if (cart[index].Quantity > 1)
            {

                cart[index].Quantity--;
            }
            else
            {
                cart.RemoveAt(index);
            }
            UpdateCart(cart);
        }

        public IEnumerable<BookDTO> GetBooks()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Book, BookDTO>()).CreateMapper();
            var books = DataBase.Books.GetAll();
            return mapper.Map<IEnumerable<Book>, List<BookDTO>>(books);
        }
        public BookDTO GetPBook(int? id)
        {
            if (id == null) throw new ValidationException("The ID was not found!", "");
            var book = DataBase.Books.Get(id.Value);
            if (book == null) throw new ValidationException("The Book was not found", "");

            return new BookDTO { Id = book.Id, Title = book.Title, Price = book.Price, PathImage = book.PathImage,Author = book.Author };
        }
    }
}
