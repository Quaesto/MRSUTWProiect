using AutoMapper;
using MRSTWEb.BuisnessLogic.Interfaces;
using MRSTWEb.BuisnessLogic.Services;
using MRSTWEb.BusinessLogic.DTO;
using MRSTWEb.BusinessLogic.Interfaces;
using MRSTWEb.BusinessLogic.Services;
using MRSTWEb.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MRSTWEb.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;
        private readonly ICartService _cartService;

        public SearchController()
        {
            _searchService = new SearchService();
            _cartService = new CartService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchV2()
        {
            var allBooks = _searchService.GetAllBooks();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<BookDTO, BookViewModel>()).CreateMapper();
            var books = mapper.Map<IEnumerable<BookDTO>, List<BookViewModel>>(allBooks);
            return View(books);
        }

        [HttpPost]
        public PartialViewResult AdvancedSearch(string title, string author, string genre, string language, int minPrice, int maxPrice)
        {
            var booksDTO = _searchService.AdvancedSearch(title, author, genre, language, minPrice, maxPrice);
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<BookDTO, BookViewModel>()).CreateMapper();
            var books = mapper.Map<IEnumerable<BookDTO>, List<BookViewModel>>(booksDTO);
            return PartialView("_SearchResults", books);
        }

        [HttpGet]
        public JsonResult GetAllBooks()
        {
            var booksDTO = _cartService.GetBooks();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<BookDTO, BookViewModel>()).CreateMapper();
            var books = mapper.Map<IEnumerable<BookDTO>, List<BookViewModel>>(booksDTO);
            return Json(new { books }, JsonRequestBehavior.AllowGet);
        }

      

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _searchService.Dispose();
                _cartService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
