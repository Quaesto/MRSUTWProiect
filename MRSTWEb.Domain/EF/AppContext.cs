﻿using Microsoft.AspNet.Identity.EntityFramework;
using MRSTWEb.Domain.Entities;
using System.Data.Entity;

namespace MRSTWEb.Domain.EF
{
    public class AppContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ClientProfile> ClientProfiles { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<DeliveryCost> DeliveryCost { get; set; }
        public DbSet<Discount> Discounts { get; set; }

        public AppContext() : base("DefaultConnection") { }
        public static AppContext Create()
        {
            return new AppContext();
        }
    }
}
