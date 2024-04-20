using Microsoft.AspNet.Identity.EntityFramework;
using MRSTWEb.Domain.Entities;
using System.Data.Entity;
using MRSTWEb.Domain.Entities;

namespace MRSTWEb.Domain.EF
{
    public class AppContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ClientProfile> ClientProfiles { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public AppContext() : base("DefaultConnection") { }
        public static AppContext Create()
        {
            return new AppContext();
        }
    }
}
