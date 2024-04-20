using MRSTWEb.BuisnessLogic.Interfaces;
using MRSTWEb.Domain.Repositories;

namespace MRSTWEb.BuisnessLogic.Services
{
    public class ServiceCreator : IServiceCreator
    {
        public IUserService CreateUserService()
        {
            return new UserService(new EFUnitOfWork());
        }
    }
}
