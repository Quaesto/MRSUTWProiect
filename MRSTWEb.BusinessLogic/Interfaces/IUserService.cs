using MRSTWEb.BusinessLogic.DTO;
using MRSTWEb.BusinessLogic.Infrastructure;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MRSTWEb.BusinessLogic.Interfaces
{
    public interface IUserService : IDisposable
    {
        Task<OperationDetails> Create(UserDTO userDTO);
        Task<ClaimsIdentity> Authenticate(UserDTO userDTO);
        Task SetInitialData(UserDTO adminDto, List<string> roles);
        Task<IEnumerable<UserDTO>> GetAllUsers();
        Task<OperationDetails> DeleteUserByUserId(string userId);
        Task<UserDTO> GetUserById(string userId);
        Task<OperationDetails> UpdateClient(UserDTO user);
        Task<UserDTO> GetUserByUsername(string username);
        Task<UserDTO> FindByEmail(string email);
        Task<string> GenerateResetPasswordToken(string userId);
        Task<OperationDetails> ResetPassword(string userId, string code, string password);
    }
}
