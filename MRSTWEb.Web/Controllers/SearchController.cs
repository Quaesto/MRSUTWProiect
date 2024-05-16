using AutoMapper;
using MRSTWEb.BusinessLogic.DTO;
using MRSTWEb.BusinessLogic.Interfaces;
using MRSTWEb.BusinessLogic.Services;
using MRSTWEb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MRSTWEb.Controllers
{
    public class SearchController : Controller
    {
        private ISearchService searchService;
        private ICartService cartService;

        public SearchController()
        {
            searchService = new SearchService();
            cartService = new CartService();
        }
        public ActionResult Search(string keyword)
        {
            var booksDTO = searchService.Search(keyword);
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<BookDTO, BookViewModel>()).CreateMapper();
            var books = mapper.Map<IEnumerable<BookDTO>, List<BookViewModel>>(booksDTO);

            return View(books);
        }
        [HttpGet]
        public JsonResult GetAllBooks()
        {
            var booksDTO = cartService.GetBooks();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<BookDTO, BookViewModel>()).CreateMapper();
            var books = mapper.Map<IEnumerable<BookDTO>, List<BookViewModel>>(booksDTO);
            return Json(new { books }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult FilterBooksByPrice(int minValue, int maxValue)
        {
            var booksDTO = searchService.FilterPrice(minValue, maxValue);
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<BookDTO, BookViewModel>()).CreateMapper();
            var books = mapper.Map<IEnumerable<BookDTO>, List<BookViewModel>>(booksDTO);
            return View("Search", books);
        }
        protected override void Dispose(bool disposing)
        {
            searchService.Dispose();
            cartService.Dispose();
            base.Dispose(disposing);
        }
    }
}