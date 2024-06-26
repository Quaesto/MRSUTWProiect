﻿using Microsoft.AspNet.Identity;
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
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MRSTWEb.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private Domain.EF.AppContext db;
        IUnitOfWork Database { get; set; }
        public UserService(IUnitOfWork uow) { Database = uow; db = new Domain.EF.AppContext(); }

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
                user = new ApplicationUser { Email = userDTO.Email, UserName = userDTO.UserName, LockoutEnabled = true };
                var result = await Database.UserManager.CreateAsync(user, userDTO.Password);

                if (result.Errors.Count() > 0) return new OperationDetails(false, result.Errors.FirstOrDefault(), "");
                await Database.UserManager.AddToRoleAsync(user.Id, userDTO.Role);
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
        public async Task<bool> UserConfirmedEmail(string userId)
        {
            return await Database.UserManager.IsEmailConfirmedAsync(userId);    
        }
        public async Task SetLockoundEndDate(string userId, DateTimeOffset time)
        {
            await Database.UserManager.SetLockoutEndDateAsync(userId, time);
        }
        public async Task<string> GenereateEmailConfirmationToken(string userId)
        {
            var code = await Database.UserManager.GenerateEmailConfirmationTokenAsync(userId);
            return code;
        }
        public async Task<OperationDetails> ConfirmEmail(string userId,string code)
        {
            var result = await Database.UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                return new OperationDetails(true,"Confirmation Send","");
            }
            else
            {
                return new OperationDetails(false, "Confirmation Could Not Be send", "");

            }
        }
       
        public async Task<bool> IsUserLockedOut(string userId)
        {
            var user = await Database.UserManager.FindByIdAsync(userId);
            if (user.LockoutEndDateUtc.HasValue && user.LockoutEndDateUtc > DateTime.UtcNow) return true;
            return false;
        }
        public async Task<bool> UserHasLockedOutValue(string userId)
        {
            
            var user = await Database.UserManager.FindByIdAsync(userId);
            if (user.LockoutEnabled)
            {
                return true;
            }
            return false;

        }
        public async Task AccessFailed(string userId)
        {
            await Database.UserManager.AccessFailedAsync(userId);
        }
        public async Task ResetFailedCount(string userId)
        {
            await Database.UserManager.ResetAccessFailedCountAsync(userId);
        }


        public async Task<bool> CheckcUserPassword(UserDTO user, string password)
        {
            var ApplicationUser = await Database.UserManager.FindByEmailAsync(user.Email);
            return await Database.UserManager.CheckPasswordAsync(ApplicationUser, password);
        }

        public async Task<OperationDetails> DeleteUserByUserId(string userId)
        {
            using (var context = new Domain.EF.AppContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var user = await context.Users.Include(u => u.Orders)
                                                      .Include(u => u.Reviews)
                                                      .Include(u => u.ClientProfile)
                                                      .FirstOrDefaultAsync(u => u.Id == userId);
                        if (user == null)
                        {
                            return new OperationDetails(false, "User not found", "");
                        }

                        if (user.Orders.Any())
                        {
                            context.Orders.RemoveRange(user.Orders);
                        }

                        if (user.Reviews.Any())
                        {
                            context.Reviews.RemoveRange(user.Reviews);
                        }

                        if (user.ClientProfile != null)
                        {
                            context.ClientProfiles.Remove(user.ClientProfile);
                        }

                        context.Users.Remove(user);

                        await context.SaveChangesAsync();
                        transaction.Commit();
                        return new OperationDetails(true, "User deleted successfully", "");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        return new OperationDetails(false, $"Failed to delete user: {ex.Message}", "");
                    }
                }
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
                Address = u.ClientProfile != null ? u.ClientProfile.Address ?? string.Empty : string.Empty,
                Name = u.ClientProfile != null ? u.ClientProfile.Name ?? string.Empty : string.Empty,
                ProfileImage = u.ClientProfile != null ? u.ClientProfile.ProfileImage : null,
                IsLockedOut = (u.LockoutEndDateUtc.HasValue && u.LockoutEndDateUtc > DateTime.UtcNow) ? true : false,

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
                Address = user.ClientProfile != null ? user.ClientProfile.Address ?? string.Empty : string.Empty,
                Name = user.ClientProfile != null ? user.ClientProfile.Name ?? string.Empty : string.Empty,
                Password = user.PasswordHash,
                ProfileImage = user.ClientProfile != null ? user.ClientProfile.ProfileImage : null,
                IsLockedOut = user.LockoutEndDateUtc.HasValue && user.LockoutEndDateUtc > DateTime.UtcNow
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
                    Address = user.ClientProfile != null ? user.ClientProfile.Address ?? string.Empty : string.Empty,
                    Name = user.ClientProfile != null ? user.ClientProfile.Name ?? string.Empty : string.Empty,
                    ProfileImage = user.ClientProfile != null ? user.ClientProfile.ProfileImage : null,
                };
            }
            return null;
        }

        public async Task SetInitialData(UserDTO adminDto, List<string> roles)
        {
            foreach (string roleName in roles)
            {
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

                client.Name = user.Name;

                client.Address = user.Address;
            
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
            if (user == null)
            {
                return null;
            }
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
