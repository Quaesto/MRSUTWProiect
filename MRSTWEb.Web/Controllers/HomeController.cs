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
    public class HomeController : Controller
    {
        private ICartService cartService;
        private IWishListService wishListService;

        public HomeController()
        {
            this.cartService = new CartService();
            this.wishListService = new WishListService();
        }

        public ActionResult Index()
        {

            var booksDTO = cartService.GetBooks();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<BookDTO, BookViewModel>()).CreateMapper();
            var books = mapper.Map<IEnumerable<BookDTO>, List<BookViewModel>>(booksDTO);

            return View(books);
        }



        //WishList

        [HttpPost]
        public ActionResult AddToWishList(int bookId)
        {
            var bookDto = cartService.GetPBook(bookId);
            wishListService.AddToWishList(bookDto);
            var books = getBooksFromWishList();

            return PartialView("_addToWishList", books);
        }
        [HttpPost]
        public ActionResult RemoveFromWishList(int bookId)
        {
            wishListService.RemoveFromTheList(bookId);
            var books = getBooksFromWishList();
            return PartialView("_addToWishList", books);
        }
        [HttpGet]
        public ActionResult WishList()
        {
            var books = getBooksFromWishList();
            return View(books);
        }
        [HttpGet]
        public JsonResult getWishList()
        {
            var books = getBooksFromWishList();
            return Json(new { books }, JsonRequestBehavior.AllowGet);
        }
        private List<BookViewModel> getBooksFromWishList()
        {
            var wishList = wishListService.GetWishList();
            var books = new List<BookViewModel>();
            foreach (var item in wishList)
            {
                var bookViewModel = new BookViewModel
                {
                    Id = item.BookDTO.Id,
                    Title = item.BookDTO.Title,
                    PathImage = item.BookDTO.PathImage,
                    Price = item.BookDTO.Price,
                    Language = item.BookDTO.Language,
                    Genre = item.BookDTO.Genre,
                    Author = item.BookDTO.Author,

                };
                books.Add(bookViewModel);
            }
            return books;
        }

        //
        public JsonResult GetBookDetails(int id)
        {
            var book = cartService.GetPBook(id);
            return Json(new { book }, JsonRequestBehavior.AllowGet);
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
            wishListService.Dispose();
            base.Dispose(disposing);
        }

    }
}