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

        public AccountController(ICartService cartService, IOrderService orderService)
        {
            this.cartService = cartService;
            this.orderService = orderService;

        }
        [HttpPost]


        [Authorize(Roles = "admin")]

  

        [HttpGet]   

        [Authorize]
        public ActionResult ChangePassword()
        {

            return View();
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


                        return RedirectToAction("", "Account");
                    }
                    else if (userRole == "user")
                    {

                        authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, claim);


                        return RedirectToAction("", "Account");
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
                AddedTime = book.AddedTime,

            };
        }

        protected override void Dispose(bool disposing)
        {
            userService.Dispose();
            cartService.Dispose();

            base.Dispose(disposing);
        }


        #endregion
    }
}