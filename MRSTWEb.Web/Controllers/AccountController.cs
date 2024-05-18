using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MRSTWEb.BusinessLogic.DTO;
using MRSTWEb.BusinessLogic.Infrastructure;
using MRSTWEb.BusinessLogic.Interfaces;
using MRSTWEb.Models;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MRSTWEb.BusinessLogic.Services;
using System.Net;
using System.Linq;
using MRSTWEb.BuisnessLogic.Services;

namespace MRSTWEb.Controllers
{
    public class AccountController : Controller
    {
        private ICartService cartService;
        private IOrderService orderService;
        private IManageBooksService manageBooksService;
        private IReviewService reviewService;
        private IUserService userService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>();
            }
        }
        private IAuthenticationManager authenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public AccountController()
        {
            this.cartService = new CartService();
            this.orderService = new OrderService();
            manageBooksService  = new ManageBooksService(); 
            reviewService = new ReviewService();
        }


        [Authorize(Roles = "admin")]

        public async Task<ActionResult> OtherUsers()
        {
            var users = await GetAllUsers();
            return View(users);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var result = await userService.DeleteUserByUserId(userId);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
            }
            return RedirectToAction("OtherUsers");
        }

       


        [HttpGet]   

        [Authorize]
        public ActionResult ChangePassword()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> ChangePassword(PasswordModel model)
        {
            var user = await userService.GetUserById(User.Identity.GetUserId());
            if (user == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var passwordHasher = new PasswordHasher();
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user.Password, model.CurrentPassword);

            if (passwordVerificationResult == PasswordVerificationResult.Success)
            {
                var newPasswordHash = passwordHasher.HashPassword(model.Password);
                user.Password = newPasswordHash;
                OperationDetails operationDetails = await userService.UpdateClient(user);

                if (operationDetails.Succeeded)
                {
                    ViewBag.PasswordChanged = true;
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Error changing password. Please try again.");
                    return View(model);
                }
            }
            else
            {

                ModelState.AddModelError("", "The current password is incorrect.");
                return View(model);
            }


        }


        [Authorize(Roles = "admin")]
        public async Task<ActionResult> AdminDashboard()
        {
            var userAdmin = await GetUserAdmin();
            var adminModel = MapToUserModel(userAdmin);

            return View(adminModel);
        }

        [Authorize]
        public async Task<ActionResult> ClientProfile()
        {
            var userDto = await userService.GetUserById(User.Identity.GetUserId());
            var user = new UserModel { Id = userDto.Id, Email = userDto.Email, Address = userDto.Address, Name = userDto.Name, UserName = userDto.UserName };

            if (user == null) return HttpNotFound();

            return View(user);
        }

        [HttpGet]

        public async Task<ActionResult> EditClientProfile()
        {
            var user = await userService.GetUserById(User.Identity.GetUserId());
            var editModel = new EditModel
            {   
                UserName = user.UserName,
                Name = user.Name,
                Address = user.Address,
                Email = user.Email,
            };
            return View(editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "user, admin")]

        public async Task<ActionResult> EditClientProfile(EditModel model)
        {
            var user = await userService.GetUserById(User.Identity.GetUserId());
            if (user == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.NotFound);

            if (ModelState.IsValid)
            {
                // Update the properties only if the model properties are not empty
                if (!string.IsNullOrEmpty(model.Email))
                {
                    user.Email = model.Email;
                }
                if (!string.IsNullOrEmpty(model.Name))
                {
                    user.Name = model.Name;
                }

                if (!string.IsNullOrEmpty(model.Address))
                {
                    user.Address = model.Address;
                }


                if (!string.IsNullOrEmpty(model.UserName))
                {
                    user.UserName = model.UserName;
                }
                if (!string.IsNullOrEmpty(model.Email))
                {

                }
                OperationDetails operationDetails = await userService.UpdateClient(user);

                if (operationDetails.Succeeded)
                {
                    if (User.IsInRole("admin"))
                    {
                        return RedirectToAction("AdminDashboard");
                    }
                    else
                    {
                        return RedirectToAction("ClientProfile");
                    }
                }
            }
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                UserDTO userDTO = new UserDTO { UserName = model.UserName, Password = model.Password };
                ClaimsIdentity claim = await userService.Authenticate(userDTO);

                if (claim == null)
                {
                    ModelState.AddModelError("", "Incorrect login or password.");
                }
                else
                {
                    var userRole = claim.FindFirst(ClaimTypes.Role)?.Value;
                    if (userRole == "admin")    
                    {
                        authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, claim);


                        return RedirectToAction("AdminDashboard", "Account");
                    }
                    else if (userRole == "user")
                    {

                        authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, claim);


                        return RedirectToAction("ClientProfile", "Account");
                    }
                }
            }
            return View(model);
        }

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login");
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
             await SetInitialData();
            if (ModelState.IsValid)
            {
                UserDTO userDTO = new UserDTO
                {
                    Email = model.Email,
                    Name = model.Name,
                    Address = model.Address,
                    UserName = model.UserName,
                    Password = model.Password,
                    Role = "user",


                };
                OperationDetails operationDetalis = await userService.Create(userDTO);
                if (operationDetalis.Succeeded) return RedirectToAction("Login");
                else ModelState.AddModelError(operationDetalis.Property, operationDetalis.Message);
            }
            return View(model);
        }



        //Manage Books functionalities added here
        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult ViewAllProducts()
        {
            var booksDto = cartService.GetBooks();
            var bookViewModel = new List<BookViewModel>();
            foreach (var book in booksDto)
            {
                bookViewModel.Add(MapBookToBookModel(book));
            }
            return View(bookViewModel);
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult EditProduct(int ProductId)
        {
            var bookDto = cartService.GetPBook(ProductId);
            var book = MapBookToBookModel(bookDto);

            return View(book);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = SaveImage("ImageFile");
                if (!string.IsNullOrEmpty(path))
                {
                    model.PathImage = path;
                }
                var bookDTO = new BookDTO
                {
                    Id = model.Id,
                    Author = model.Author,
                    Title = model.Title,
                    Genre = model.Genre,
                    Language = model.Language,
                    Price = model.Price,
                    PathImage = model.PathImage,
                };

                manageBooksService.UpdateProduct(bookDTO);
                return RedirectToAction("ViewAllProducts");
            }
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct(BookViewModel model)
        {

            if (ModelState.IsValid)
            {
                model.PathImage = SaveImage("PathImage");

                var bookDTO = new BookDTO
                {
                    Id = model.Id,
                    Author = model.Author,
                    Title = model.Title,
                    Genre = model.Genre,
                    Language = model.Language,
                    Price = model.Price,
                    PathImage = model.PathImage,

                };
                manageBooksService.AddBook(bookDTO);
                return RedirectToAction("ViewAllProducts");
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteProduct(int BookId)
        {
            manageBooksService.DeleteBook(BookId);

            return RedirectToAction("ViewAllProducts");
        }
        //END of manage books functionalities

        //Admin Manage Review
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> ViewClientReviews()
        {
            var usersDTO = await userService.GetAllUsers();
            var usersModel = new List<UserModel>();
            foreach (var user in usersDTO)
            {
                if (reviewService.GetUserReview(user.Id).Any())
                {

                    var userModel = new UserModel
                    {
                        Id = user.Id,
                        Name = user.Name,
                        UserName = user.UserName,
                        Address = user.Address,
                        Email = user.Email,
                    };
                    foreach (var reviewDto in reviewService.GetUserReview(user.Id))
                    {
                        var review = new ReviewViewModel
                        {
                            Id = reviewDto.Id,
                            ApplicationUserId = user.Id,
                            BookId = reviewDto.BookId,
                            Comment = reviewDto.Comment,
                            Rating = reviewDto.Rating,
                        };
                        userModel.Reviews.Add(review);
                    }
                    usersModel.Add(userModel);
                }
            }


            return View(usersModel);
        }


        #region Helpers
        private async Task SetInitialData()
        {
            UserDTO adminUser = GetAdminInfo();
            await userService.SetInitialData(adminUser, new List<string> { "user", "admin" });

        }

        protected override void Dispose(bool disposing)
        {
            userService.Dispose();
            manageBooksService.Dispose();
            reviewService.Dispose();
            cartService.Dispose();

            base.Dispose(disposing);
        }
        private UserDTO GetAdminInfo()
        {
            return new UserDTO
            {
                Email = "MRSUTWEB@mail.com",
                UserName = "Admin",
                Password = "Admin123",
                Name = "Application Admin",
                Address = "Chisinau,str.Studentilor",
                Role = "admin",
            };
        }

        private BookViewModel MapBookToBookModel(BookDTO book)
        {
            return new BookViewModel
            {

                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PathImage = book.PathImage,
                Language = book.Language,
                Genre = book.Genre,
                Price = book.Price,


            };
        }

        private UserModel MapToUserModel(UserDTO user)
        {
            return new UserModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Address = user.Address,
                Id = user.Id,
                Name = user.Name,
            };
        }

        private async Task<UserDTO> GetUserAdmin()
        {
            var userId = User.Identity.GetUserId();
            return await userService.GetUserById(userId);
        }

        private async Task<IEnumerable<UserModel>> GetAllUsers()
        {
            var usersDto = await userService.GetAllUsers();
            if (usersDto == null) return Enumerable.Empty<UserModel>();
            var users = new List<UserModel>();
            foreach (var userDto in usersDto)
            {
                var userModel = MapToUserModel(userDto);
                users.Add(userModel);
            }
            return users;
        }

        private string SaveImage(string name)
        {
            var file = Request.Files[name];
            if (file != null && file.ContentLength > 0)
            {
                string fileName = Path.GetFileName(file.FileName);
                string path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                file.SaveAs(path);

                return "/Images/" + fileName;
            }
            return string.Empty;
        }
        #endregion
    }
}