using MRSTWEb.BusinessLogic.Interfaces;
using MRSTWEb.Domain.Repositories;

namespace MRSTWEb.BusinessLogic.Services
{
    public class ServiceCreator : IServiceCreator
    {
        public IUserService CreateUserService()
        {
            return new UserService(new EFUnitOfWork());
        }
    }
}
