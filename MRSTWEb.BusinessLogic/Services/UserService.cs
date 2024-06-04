using Microsoft.AspNet.Identity;
using MRSTWEb.BusinessLogic.DTO;
using MRSTWEb.BusinessLogic.Infrastructure;
using MRSTWEb.BusinessLogic.Interfaces;
using MRSTWEb.Domain.Entities;
using MRSTWEb.Domain.Interfaces;
using MRSTWEb.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MRSTWEb.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        IUnitOfWork Database { get; set; }
        public UserService(IUnitOfWork uow) { Database = uow; }

        public async Task<ClaimsIdentity> Authenticate(UserDTO userDTO)
        {
            ClaimsIdentity claim = null;
            ApplicationUser user = await Database.UserManager.FindAsync(userDTO.UserName, userDTO.Password);


            if (user != null)
            {
                claim = await Database.UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            }
            return claim;
        }

        public async Task<OperationDetails> Create(UserDTO userDTO)
        {
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(userDTO.Email);
            if (user == null)
            {
                user = new ApplicationUser { Email = userDTO.Email, UserName = userDTO.UserName };
                var result = await Database.UserManager.CreateAsync(user, userDTO.Password);

                if (result.Errors.Count() > 0) return new OperationDetails(false, result.Errors.FirstOrDefault(), "");
                //Adauga roluri
                await Database.UserManager.AddToRoleAsync(user.Id, userDTO.Role);
                //Creaza profilul utilizatorului
                ClientProfile clientProfile = new ClientProfile { Id = user.Id, Address = userDTO.Address, Name = userDTO.Name, ProfileImage = userDTO.ProfileImage };
                Database.ClientManager.Create(clientProfile);
                await Database.SaveAsync();
                return new OperationDetails(true, "Registration was successfull", "");
            }
            else
            {
                return new OperationDetails(false, "User with this login already exists", "Email");
            }
        }


        public async Task<OperationDetails> DeleteUserByUserId(string userId)
        {
            var user = await Database.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new OperationDetails(false, "User not found", "");
            }

            if (user.Orders != null)
            {
                var orderIds = user.Orders.Select(o => o.Id).ToList();
                foreach (var orderId in orderIds)
                {
                    Database.Orders.Delete(orderId);
                }
                Database.Save();
            }

            if (user.Reviews.Any())
            {
                var reviewIds = user.Reviews.Select(r => r.Id).ToList();
                foreach (var reviewId in reviewIds)
                {
                    Database.Reviews.Delete(reviewId);
                }
                Database.Save();
            }

            if (user.ClientProfile != null)
            {
                Database.ClientManager.Delete(user.ClientProfile);
                await Database.SaveAsync();
            }

            var result = await Database.UserManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                await Database.SaveAsync();
                return new OperationDetails(true, "User deleted successfully", "");
            }
            else
            {
                return new OperationDetails(false, "Failed to delete user", "");
            }
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            var users = await Database.UserManager.Users.ToListAsync();
            if (!users.Any()) return null;
            return users.Select(u => new UserDTO
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName,
                Address = u.ClientProfile.Address,
                Name = u.ClientProfile.Name,
                ProfileImage = u.ClientProfile.ProfileImage,

            }).ToList();
        }

        public async Task<UserDTO> GetUserById(string userId)
        {
            var user = await Database.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }


            return new UserDTO
            {
                Id = user.ClientProfile.Id,
                Email = user.Email,
                UserName = user.UserName,
                Address = user.ClientProfile.Address,
                Name = user.ClientProfile.Name,
                Password = user.PasswordHash,
                ProfileImage = user.ClientProfile?.ProfileImage,
            };
        }

        public async Task<UserDTO> GetUserByUsername(string username)
        {
            var user = await Database.UserManager.FindByNameAsync(username);
            if (user != null)
            {
                return new UserDTO
                {
                    Id = user.Id,
                    UserName = username,
                    Email = user.Email,
                    Address = user.ClientProfile.Address,
                    Name = user.ClientProfile.Name,
                    ProfileImage = user.ClientProfile?.ProfileImage,
                };
            }
            return null;
        }

        public async Task SetInitialData(UserDTO adminDto, List<string> roles)
        {
            foreach(string roleName in roles) {
                var role = await Database.RoleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    role = new ApplicationRole { Name = roleName };
                    await Database.RoleManager.CreateAsync(role);
                }
            }
            await Create(adminDto);
        }

        public async Task<OperationDetails> UpdateClient(UserDTO user)
        {
            ClientProfile client = Database.ClientManager.GetClientProfileById(user.Id);

            if (client == null)
            {
                return new OperationDetails(false, "Client profile not found", "");
            }

            if (!string.IsNullOrEmpty(user.Name))
            {
                client.Name = user.Name;
            }

            if (!string.IsNullOrEmpty(user.Address))
            {
                client.Address = user.Address;
            }

            if (!string.IsNullOrEmpty(user.ProfileImage))
            {
                client.ProfileImage = user.ProfileImage;
            }
            Database.ClientManager.UpdateClientProfile(client);


            ApplicationUser applicationUser = await Database.UserManager.FindByIdAsync(user.Id);

            if (applicationUser == null)
            {
                return new OperationDetails(false, "User not found", "");
            }


            if (!string.IsNullOrEmpty(user.UserName))
            {
                applicationUser.UserName = user.UserName;
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                applicationUser.Email = user.Email;
            }
            if (!string.IsNullOrEmpty(user.Password))
            {
                applicationUser.PasswordHash = user.Password;
            }


            IdentityResult result = await Database.UserManager.UpdateAsync(applicationUser);

            if (result.Succeeded)
            {
                await Database.SaveAsync();
                return new OperationDetails(true, "Update was successful", "");
            }
            else
            {
                return new OperationDetails(false, "Something went wrong with user update", "");
            }
        }

        public async Task<UserDTO> FindByEmail(string email)
        {
            var user = await Database.UserManager.FindByEmailAsync(email);
            var userDto = new UserDTO
            {
                Id = user.ClientProfile.Id,
                Email = user.Email,
                UserName = user.UserName,
                Address = user.ClientProfile.Address,
                Name = user.ClientProfile.Name,
                Password = user.PasswordHash,
                ProfileImage = user.ClientProfile?.ProfileImage,
            };
            return userDto;
        }
        public async Task<string> GenerateResetPasswordToken(string userId)
        {
            var code = await Database.UserManager.GeneratePasswordResetTokenAsync(userId);
            return code;
        }
        public async Task<OperationDetails> ResetPassword(string userId, string code, string password)
        {
            var result = await Database.UserManager.ResetPasswordAsync(userId, code, password);
            if (result.Succeeded)
            {
                return new OperationDetails(true, "", "");
            }
            else
            {
                return new OperationDetails(false, "", "");
            }
        }


        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
