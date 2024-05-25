using Microsoft.AspNet.Identity.EntityFramework;
using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Identity;
using MRSTWEb.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace MRSTWEb.Domain.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private EF.AppContext db;
        private BookRepository bookRepository;
        private ReviewRepository reviewRepository;
        private OrderRepository orderRepository;
        private ApplicationUserManager userManager;
        private ApplicationRoleManager roleManager;

        private DiscountRepository discountRepository;

        private DeliveryRepository deliveryCostRepository;

        private IClientManager clientManager;

        public EFUnitOfWork()
        {
            this.db = new EF.AppContext();
            userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db));
            clientManager = new ClientManager(db);
        }
        public IRepository<Discount> Discounts
        {
            get
            {
                if (discountRepository == null) return new DiscountRepository(db);
                return discountRepository;
            }
        }

        public IRepository<DeliveryCost> DeliveryCost
        {
            get
            {
                if (deliveryCostRepository == null) return new DeliveryRepository(db);
                return deliveryCostRepository;
            }
        }

        public IRepository<Book> Books
        {
            get
            {
                if (bookRepository == null) return new BookRepository(db);
                return bookRepository;
            }
        }

        public IRepository<Order> Orders
        {
            get
            {
                if (orderRepository == null) return new OrderRepository(db);
                return orderRepository;
            }
        }

        public ApplicationUserManager UserManager => userManager;

        public IClientManager ClientManager => clientManager;

        public ApplicationRoleManager RoleManager => roleManager;

        public IRepository<Review> Reviews
        {
            get
            {
                if (reviewRepository == null) return new ReviewRepository(db);
                return reviewRepository;
            }
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool disposed = false;
        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {

                    userManager.Dispose();
                    roleManager.Dispose();
                    clientManager.Dispose();
                }
                this.disposed = true;
            }
        }
        public void Save()
        {
            db.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}
