using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MRSTWEb.BusinessLogic.DTO;
using MRSTWEb.BusinessLogic.Infrastructure;
using MRSTWEb.BuisnessLogic.Interfaces;
using MRSTWEb.BusinessLogic.Interfaces;
using MRSTWEb.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MRSTWEb.BuisnessLogic.Services;
using MRSTWEb.BusinessLogic.Services;

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
            this.manageBooksService = new ManageBooksService();
            this.reviewService = new ReviewService();
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

        [Authorize(Roles = "admin")]

        public async Task<ActionResult> OtherUsers()
        {
            var users = await GetAllUsers();
            return View(users);
        }



        [Authorize(Roles = "admin")]
        public async Task<ActionResult> AdminDashboard()
        {
            var userAdmin = await GetUserAdmin();
            var adminModel = MapToUserModel(userAdmin);
            adminModel.ProfileImage = userAdmin.ProfileImage;

            return View(adminModel);
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



        [Authorize]
        public async Task<ActionResult> ClientProfile()
        {
            var userDto = await userService.GetUserById(User.Identity.GetUserId());
            var user = new UserModel
            {
                Id = userDto.Id,
                Email = userDto.Email,
                Address = userDto.Address,
                Name = userDto.Name,
                UserName = userDto.UserName,
                ProfileImage = userDto.ProfileImage
            };

            if (user == null) return HttpNotFound();

            return View(user);
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
            /*  await SetInitialData();*/
            if (ModelState.IsValid)
            {
                UserDTO userDTO = new UserDTO
                {
                    Email = model.Email,
                    Name = model.Name,
                    Address = model.Address,
                    UserName = model.UserName,
                    Password = model.Password,
                    ProfileImage = "/Images/client.jpg",
                    Role = "user",

                };
                OperationDetails operationDetalis = await userService.Create(userDTO);
                if (operationDetalis.Succeeded) return RedirectToAction("Login");
                else ModelState.AddModelError(operationDetalis.Property, operationDetalis.Message);
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> OrderDetails(string userId)
        {
            var user = await GetUserWithOrders(userId);
            return View(user);
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult ItemsBought(int OrderId)
        {
            var orderDTO = orderService.GetOrder(OrderId);
            var books = new List<BookViewModel>();
            if (orderDTO != null)
            {
                var order = MapOrderToOrderViewModel(orderDTO);

                foreach (var item in order.Items)
                {
                    var bookDto = cartService.GetPBook(item.Book.Id);
                    var bookViewModel = new BookViewModel
                    {
                        Id = bookDto.Id,
                        Title = bookDto.Title,
                        Author = bookDto.Author,
                        Quantity = item.Quantity,
                        PathImage = bookDto.PathImage,
                        Price = bookDto.Price,
                        ExpirationTime = bookDto.ExpirationTime,
                        SetTime = bookDto.SetTime,
                        Percentage = bookDto.Percentage,

                    };

                    books.Add(bookViewModel);
                }
            }

            return View(books);
        }

        //Discount functions
        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult SetBookDiscount(int bookId)
        {

            var bookDto = cartService.GetPBook(bookId);
            var book = MapBookToBookModel(bookDto);
            if (bookDto.Percentage != 0)
            {
                book.Percentage = bookDto.Percentage;
                book.ExpirationTime = bookDto.ExpirationTime;

            }
            return View(book);
        }
        [HttpPost]
        public ActionResult SetBookDiscount(BookViewModel bookModel)
        {
            if (ModelState.IsValid)
            {
                var bookDto = new BookDTO
                {
                    Id = bookModel.Id,
                    ExpirationTime = bookModel.ExpirationTime,
                    SetTime = bookModel.SetTime,
                    Percentage = bookModel.Percentage,
                    Price = bookModel.Price,
                };

                cartService.SetDiscount(bookDto);
                return RedirectToAction("ViewAllProducts");
            }
            return View(bookModel);
        }
        [HttpPost]
        public ActionResult DeleteBookDiscount(int bookId)
        {
            bool isDeleted = cartService.RemoveDiscount(bookId);
            if (isDeleted)
            {
                return RedirectToAction("SetBookDiscount", new { bookId = bookId });
            }
            else
            {
                var bookDto = cartService.GetPBook(bookId);
                var book = MapBookToBookModel(bookDto);
                if (bookDto.Percentage != 0)
                {
                    book.Percentage = bookDto.Percentage;
                    book.ExpirationTime = bookDto.ExpirationTime;
                }

                ModelState.AddModelError("", "This discount was already removed.");

                return View("SetBookDiscount", book);
            }
        }


        //End


        //Delivery functions

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult RemoveDelivery(int deliveryCostId)
        {
            bool isDeleted = false;
            var deliveryDto = cartService.GetAllDeliveriesCost().LastOrDefault();
            var delivery = new DeliveryViewModel();
            if (deliveryDto != null)
            {
                delivery.Cost = deliveryDto.Cost;
                delivery.Id = deliveryDto.Id;
                isDeleted = cartService.RemoveDeliveryCost(deliveryDto.Id);
            }

            if (isDeleted)
            {
                TempData["Message"] = "Delivery removed successfully.";
                return RedirectToAction("DeliveryPage", new DeliveryViewModel());

            }
            else
            {

                ModelState.AddModelError("", "Delivery Cost was already removed.");

                return View("DeliveryPage", delivery);
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult DeliveryPage()
        {
            var deliveryDto = cartService.GetAllDeliveriesCost().LastOrDefault();
            var delivery = new DeliveryViewModel();
            if (deliveryDto != null)
            {
                delivery.Cost = deliveryDto.Cost;
                delivery.Id = deliveryDto.Id;
            }
            ViewBag.MessageRemoval = TempData["Message"];
            return View(delivery);
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult SetDelivery(DeliveryViewModel deliveryCostViewModel)
        {
            if (ModelState.IsValid)
            {
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<DeliveryViewModel, DeliveryCostDTO>()).CreateMapper();
                var delivery = mapper.Map<DeliveryViewModel, DeliveryCostDTO>(deliveryCostViewModel);
                cartService.SetDelivery(delivery);
                ViewBag.Message = "Delivery Cost Has Been Set Successfuly.";
                return View("DeliveryPage", deliveryCostViewModel);
            }
            return View("DeliveryPage", deliveryCostViewModel);
        }
 

        //END Delivery functions


        public async Task<string> GetProfileImage(string id)
        {
            var user = await userService.GetUserById(id);
            return user.ProfileImage;
        }


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

        public async Task<ActionResult> EditClientProfile()
        {
            var user = await userService.GetUserById(User.Identity.GetUserId());
            var editModel = new EditModel
            {
                UserName = user.UserName,
                Name = user.Name,
                Address = user.Address,
                Email = user.Email,
                ProfileImage = user.ProfileImage,
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
                string pathImage = SaveImage("ProfileImage");
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
                if (!string.IsNullOrEmpty(pathImage))
                {
                    model.ProfileImage = pathImage;
                    user.ProfileImage = model.ProfileImage;
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
                        ProfileImage = user.ProfileImage,
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
        private UserModel MapToUserModel(UserDTO user)
        {
            return new UserModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Address = user.Address,
                Id = user.Id,
                Name = user.Name,
                ProfileImage = user.ProfileImage,
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
                ExpirationTime = book.ExpirationTime,
                SetTime = book.SetTime,
                Percentage = book.Percentage,

            };
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
        private async Task<UserDTO> GetUserAdmin()
        {
            var userId = User.Identity.GetUserId();
            return await userService.GetUserById(userId);
        }

        private async Task<UserModel> GetUserWithOrders(string userId)
        {
            var userDto = await userService.GetUserById(userId);
            var ordersDto = orderService.GetOrdersByUserId(userId);
            var user = new UserModel();
            if (userDto != null)
            {
                user = MapToUserModel(userDto);
                AddOrdersToUser(user, ordersDto);

            }
            return user;
        }
        private OrderViewModel MapOrderToOrderViewModel(OrderDTO orderDTO)
        {
            return new OrderViewModel
            {
                Id = orderDTO.Id,
                FirstName = orderDTO.FirstName,
                LastName = orderDTO.LastName,
                Address = orderDTO.Address,
                Phone = orderDTO.Phone,
                PostCode = orderDTO.PostCode,
                BuyingTime = orderDTO.BuyingTime,
                Email = orderDTO.Email,
                City = orderDTO.City,
                ApplicationUserId = orderDTO.ApplicationUserId,
                TotalSumToPay = orderDTO.TotalSumToPay,
                Items = orderDTO.Items,
            };
        }
        private void AddOrdersToUser(UserModel user, IEnumerable<OrderDTO> ordersDto)
        {
            if (ordersDto != null)
            {
                user.Orders = ordersDto.Select(o => MapOrderToOrderViewModel(o)).ToList();
            }
        }
        protected override void Dispose(bool disposing)
        {
            userService.Dispose();
            cartService.Dispose();
            manageBooksService.Dispose();
            reviewService.Dispose();
            base.Dispose(disposing);
        }


        #endregion
    }
}