using Microsoft.Owin.Security.DataProtection;
using MRSTWEb.BusinessLogic.Interfaces;
using MRSTWEb.Domain.Repositories;

namespace MRSTWEb.BusinessLogic.Services
{
    public class ServiceCreator : IServiceCreator
    {
        private readonly IDataProtectionProvider dataProtectionProvider;
        public ServiceCreator(IDataProtectionProvider dataProtectionProvider)
        {
            this.dataProtectionProvider = dataProtectionProvider;
        }
        public IUserService CreateUserService()
        {
            return new UserService(new EFUnitOfWork(dataProtectionProvider));
        }
    }
}
