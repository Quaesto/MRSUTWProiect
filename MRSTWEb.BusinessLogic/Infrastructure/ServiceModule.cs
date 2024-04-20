using MRSTWEb.Domain.Interfaces;
using MRSTWEb.Domain.Repositories;
using Ninject.Modules;

namespace MRSTWEb.BusinessLogic.Infrastructure
{
    public class ServiceModule : NinjectModule
    {
        private string connectionString;
        public ServiceModule(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public override void Load()
        {
            Bind<IUnitOfWork>().To<EFUnitOfWork>().WithConstructorArgument(connectionString);
        }
    }
}
