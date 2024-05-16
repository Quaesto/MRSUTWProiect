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





        #region Helpers
        private async Task SetInitialData()
        {
            UserDTO adminUser = GetAdminInfo();
            await userService.SetInitialData(adminUser, new List<string> { "user", "admin" });

        }

        protected override void Dispose(bool disposing)
        {
            userService.Dispose();
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