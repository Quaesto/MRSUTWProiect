using MRSTWEb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSTWEb.Domain.Interfaces
{
    public interface IClientManager : IDisposable
    {
        void Create(ClientProfile item);
        void Delete(ClientProfile item);
        void UpdateClientProfile(ClientProfile item);
        ClientProfile GetClientProfileById(string Id);
    }
}
