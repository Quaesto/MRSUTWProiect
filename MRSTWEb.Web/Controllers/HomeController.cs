using AutoMapper;
using MRSTWEb.BusinessLogic.DTO;
using MRSTWEb.BusinessLogic.Interfaces;
using MRSTWEb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MRSTWEb.BusinessLogic.Services;

namespace MRSTWEb.Controllers
{
    public class HomeController : Controller
    {
        private ICartService cartService;
        public HomeController()
        {
            cartService = new CartService();
        }
        public ActionResult Index()
        {

            var booksDTO = cartService.GetBooks();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<BookDTO, BookViewModel>()).CreateMapper();
            var books = mapper.Map<IEnumerable<BookDTO>, List<BookViewModel>>(booksDTO);

            return View(books);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        protected override void Dispose(bool disposing)
        {
            cartService.Dispose();
            base.Dispose(disposing);
        }

    }
}