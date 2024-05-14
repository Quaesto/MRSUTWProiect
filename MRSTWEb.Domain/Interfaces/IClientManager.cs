using MRSTWEb.Domain.Entities;
using System;

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
