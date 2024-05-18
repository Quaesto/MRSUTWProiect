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
        private OrderRepository orderRepository;
        private ApplicationUserManager userManager;
        private ApplicationRoleManager roleManager;

        private ReviewRepository reviewRepository;

        private IClientManager clientManager;

        public EFUnitOfWork()
        {
            this.db = new EF.AppContext();
            userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db));
            clientManager = new ClientManager(db);
        }

        public IRepository<Book> Books
        {
            get
            {
                if (bookRepository == null) return new BookRepository();
                return bookRepository;
            }
        }

        public IRepository<Order> Orders
        {
            get
            {
                if(orderRepository == null) return new OrderRepository(); 
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
                if (reviewRepository == null) return new ReviewRepository();
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
