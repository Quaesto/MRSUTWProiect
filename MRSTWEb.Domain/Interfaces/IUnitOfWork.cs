using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Identity;
using System;
using System.Threading.Tasks;

namespace MRSTWEb.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable  
    {

        IRepository<Review> Reviews { get; }
        IRepository<Book> Books {  get; }   
        IRepository<Order> Orders { get; }
        ApplicationUserManager UserManager { get; }
        IClientManager ClientManager { get; }
        ApplicationRoleManager RoleManager { get; }
        Task SaveAsync();
        void Save();
    }
}
