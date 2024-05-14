using Microsoft.AspNet.Identity;
using MRSTWEb.Domain.Entities;

namespace MRSTWEb.Domain.Identity
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store) : base(store) { }

    }
}
