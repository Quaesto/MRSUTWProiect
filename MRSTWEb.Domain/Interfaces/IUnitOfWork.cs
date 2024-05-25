using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Identity;
using System;
using System.Threading.Tasks;

namespace MRSTWEb.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        IRepository<DeliveryCost> DeliveryCost { get; }
        IRepository<Discount> Discounts { get; }
        IRepository<Book> Books { get; }
        IRepository<Order> Orders { get; }
        IRepository<Review> Reviews { get; }

        ApplicationUserManager UserManager { get; }
        IClientManager ClientManager { get; }
        ApplicationRoleManager RoleManager { get; }

        Task SaveAsync();
        void Save();
    }
}
