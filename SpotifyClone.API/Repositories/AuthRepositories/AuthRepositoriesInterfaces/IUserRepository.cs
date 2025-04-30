using Microsoft.AspNetCore.Identity;
using SpotifyClone.API.Models.Entities;

namespace SpotifyClone.API.Repositories.AuthRepositories.AuthRepositoriesInterfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task AddToRoleAsync(ApplicationUser user, string role);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task<ApplicationUser> FindByIdAsync(string userId); 
        Task<IdentityResult> UpdateAsync(ApplicationUser user); 
        Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
    }

}
