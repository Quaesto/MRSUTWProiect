using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MRSTWEb.Domain.Entities;

namespace MRSTWEb.Domain.Identity
{
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(RoleStore<ApplicationRole> store) : base(store) { }
    }
}
