using AutoMapper;
using Microsoft.AspNet.Identity.Owin;
using MRSTWEb.BuisnessLogic.Interfaces;
using MRSTWEb.BuisnessLogic.Services;
using MRSTWEb.BusinessLogic.DTO;
using MRSTWEb.BusinessLogic.Interfaces;
using MRSTWEb.BusinessLogic.Services;
using MRSTWEb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MRSTWEb.Controllers
{
    public class HomeController : Controller
    {
        private ICartService cartService;
        private IWishListService wishListService;
        private IReviewService reviewService;
        private IManageBooksService manageBooksService;
        private IUserService userService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>();
            }
        }

        public HomeController()
        {
            this.cartService = new CartService();
            this.wishListService = new WishListService();
            this.reviewService = new ReviewService();
            this.manageBooksService = new ManageBooksService();
        }

        public async  Task<ActionResult> Index()
        {

            var booksDTO = cartService.GetBooks();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<BookDTO, BookViewModel>()).CreateMapper();
            var books = mapper.Map<IEnumerable<BookDTO>, List<BookViewModel>>(booksDTO);

            //Review Added to main Page
            var favouriteReviewsDto = reviewService.GetAllReviews().Where(x => x.IsFavourite == true).ToList();
            var reviewsMapper = new MapperConfiguration(cfg => cfg.CreateMap<ReviewDTO, ReviewViewModel>()).CreateMapper();
            var favouriteReviews = reviewsMapper.Map<IEnumerable<ReviewDTO>, List<ReviewViewModel>>(favouriteReviewsDto);
            var users = new List<UserModel>();
            foreach (var review in favouriteReviews)
            {
                var user = await getUserFromReview(review);
                user.Reviews.Add(review);
                users.Add(user);
            }
            ViewBag.UsersReview = users;

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

        //Reviews Functionalities
        [HttpPost]
        [Authorize(Roles ="admin")]
        
        public ActionResult RemoveReview(int reviewId)
        {
            reviewService.RemoveReview(reviewId);
            return RedirectToAction("ViewClientReviews", "Account");
        }

        [HttpGet]
        public JsonResult GetReviews(int bookId)
        {
            var reviews = reviewService.GetReviewByBookId(bookId);
            return Json(new { reviews }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetUserReviews(string userId, int bookId)
        {
            var review = reviewService.GetAllReviews()
                             .FirstOrDefault(x => x.ApplicationUserId == userId && x.BookId == bookId);
            var book = cartService.GetPBook(bookId);

            return Json(new { review, book }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
  
        public ActionResult PostReview(ReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var reviewDto = new ReviewDTO
                {
                    Id = model.Id,
                    Rating = model.Rating,
                    Comment = model.Comment,
                    ApplicationUserId = model.ApplicationUserId,
                    BookId = model.BookId,
                };

                reviewService.AddReview(reviewDto);
                return Json(new { success = true });

            }

            return Json(new { success = false });
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult RemovePostedReviewFromMainPage(int reviewId)
        {
            var reviewDto = reviewService.GetReview(reviewId);
            reviewDto.IsFavourite = false;
            reviewService.UpdateReview(reviewDto);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult AddReviewToMainPage(int reviewId)
        {
            var reviewDto = reviewService.GetReview(reviewId);
            if (reviewDto.IsFavourite == true)
            {
                return Json(new { success = false });
            }
            else
            {
                reviewDto.IsFavourite = true;

                reviewService.UpdateReview(reviewDto);

                return Json(new { success = true });
            }
        }
        //END of REVIEWS functionalities


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
        public ActionResult IsAuthenticated()
        {
            bool isAuthenticated = User.Identity.IsAuthenticated;
            return Json(new { isAuthenticated }, JsonRequestBehavior.AllowGet);
        }
        //Review HELPERS
        private async Task<UserModel> getUserFromReview(ReviewViewModel review)
        {
            var userDto = await userService.GetUserById(review.ApplicationUserId);
            var userModel = new UserModel
            {
                Id = userDto.Id,
                UserName = userDto.UserName,
                ProfileImage = userDto.ProfileImage,
            };
            return userModel;
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
            reviewService.Dispose();
            manageBooksService.Dispose();
            base.Dispose(disposing);
        }

    }
}