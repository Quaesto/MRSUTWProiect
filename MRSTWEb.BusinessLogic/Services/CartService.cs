using AutoMapper;
using MRSTWEb.BusinessLogic.DTO;
using MRSTWEb.BusinessLogic.Infrastructure;
using MRSTWEb.BuisnessLogic.Interfaces;
using MRSTWEb.BusinessLogic.BusinessModels;
using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MRSTWEb.BuisnessLogic.Services
{
    public class CartService : ICartService
    {
        private IUnitOfWork DataBase { get; set; }
        public CartService(IUnitOfWork uow) { DataBase = uow; }

        public void AddToCart(int BookId)
        {
            var book = DataBase.Books.Get(BookId);
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Book, BookDTO>()).CreateMapper();
            var bookDto = mapper.Map<Book, BookDTO>(book);
            if (book.Discount != null && book.Discount.ExpirationTime > DateTime.Now)
            {
                bookDto.ExpirationTime = book.Discount.ExpirationTime;
                bookDto.SetTime = book.Discount.ExpirationTime;
                bookDto.Percentage = book.Discount.Percentage;
                bookDto.Price -= CalculateDiscountAmount(bookDto.Price, bookDto.Percentage);
            }
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
        //Discounts

        public bool RemoveDiscount(int bookId)
        {
            var discount = DataBase.Discounts.Get(bookId);
            if (discount != null)
            {
                DataBase.Discounts.Delete(bookId);
                DataBase.Save();
                return true;
            }
            return false;

        }
        public decimal CalculateTotalPrice()
        {
            var cartItems = GetCart();

            var deliveryDto = GetAllDeliveriesCost().LastOrDefault();
            var deliveryCost = deliveryDto?.Cost ?? 0;

            decimal totalPrice = 0;

            foreach (var item in cartItems)
            {
                var bookPrice = item.Book.Price;

                totalPrice += item.Quantity * bookPrice;

            }

            totalPrice += deliveryCost;

            return totalPrice;
        }



        public void SetDiscount(BookDTO bookDto)
        {


            var Discount = new Discount
            {
                Id = bookDto.Id,
                ExpirationTime = bookDto.ExpirationTime,
                SetTime = bookDto.SetTime,
                Percentage = bookDto.Percentage,
            };
            if (DataBase.Discounts.Get(bookDto.Id) == null)
            {
                DataBase.Discounts.Create(Discount);

                DataBase.Save();
            }
            else
            {
                DataBase.Discounts.Update(Discount);
                DataBase.Save();
            }

        }
        public decimal CalculateDiscountAmount(decimal bookPrice, decimal percentage)
        {
            return (bookPrice * percentage) / 100;
        }
        public decimal GetBookPriceWithoutDiscount(decimal bookPrice, decimal percentage)
        {
            decimal originalPrice = bookPrice / (1 - percentage / 100);
            return originalPrice;
        }
        //end discount


        //Delivery functions
        public IEnumerable<DeliveryCostDTO> GetAllDeliveriesCost()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<DeliveryCost, DeliveryCostDTO>()).CreateMapper();
            var deliveries = DataBase.DeliveryCost.GetAll();
            var deliveriesDTO = mapper.Map<IEnumerable<DeliveryCost>, List<DeliveryCostDTO>>(deliveries);
            return deliveriesDTO;
        }
        public void SetDelivery(DeliveryCostDTO deliveryDto)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<DeliveryCostDTO, DeliveryCost>()).CreateMapper();
            var delivery = mapper.Map<DeliveryCostDTO, DeliveryCost>(deliveryDto);
            var allDelivery = DataBase.DeliveryCost.GetAll().LastOrDefault();
            if (allDelivery == null)
            {
                DataBase.DeliveryCost.Create(delivery);
                DataBase.Save();
            }
            else
            {
                var allDel = new DeliveryCost
                {
                    Id = allDelivery.Id,
                    Cost = delivery.Cost,
                };
                DataBase.DeliveryCost.Update(allDel);
                DataBase.Save();
            }
        }
        public bool RemoveDeliveryCost(int deliveryCostId)
        {
            var delivery = DataBase.DeliveryCost.GetAll().LastOrDefault();
            if (delivery != null)
            {
                DataBase.DeliveryCost.Delete(deliveryCostId);
                DataBase.Save();
                return true;
            }
            return false;
        }
        //End Delivery functions


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

            var books = DataBase.Books.GetAll();
            List<BookDTO> booksDto = new List<BookDTO>();
            foreach (var book in books)
            {
                if (book.Discount != null && book.Discount.ExpirationTime >= DateTime.Now)
                {
                    book.Price -= CalculateDiscountAmount(book.Price, book.Discount.Percentage);

                }
                var bookDto = new BookDTO
                {
                    Id = book.Id,
                    Title = book.Title,
                    Price = book.Price,
                    PathImage = book.PathImage,
                    Author = book.Author,
                    Genre = book.Genre,
                    Language = book.Language,
                };
                if (book.Discount != null)
                {
                    bookDto.SetTime = book.Discount.SetTime;
                    bookDto.ExpirationTime = book.Discount.ExpirationTime;
                    bookDto.Percentage = book.Discount.Percentage;
                }
                booksDto.Add(bookDto);
            }


            return booksDto;
        }
        public BookDTO GetPBook(int? id)
        {
            if (id == null) throw new ValidationException("The ID was not found!", "");
            var book = DataBase.Books.Get(id.Value);

            if (book == null) throw new ValidationException("The Book was not found", "");
            var bookDto = new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Price = book.Price,
                PathImage = book.PathImage,
                Author = book.Author,
                Genre = book.Genre,
                Language = book.Language,


            };
            if (book.Discount != null)
            {
                bookDto.ExpirationTime = book.Discount.ExpirationTime;
                bookDto.SetTime = book.Discount.SetTime;
                bookDto.Percentage = book.Discount.Percentage;
                if (book.Discount.ExpirationTime >= DateTime.Now)
                {
                    bookDto.Price -= CalculateDiscountAmount(bookDto.Price, bookDto.Percentage);
                }
            }


            return bookDto;
        }
    }
}
