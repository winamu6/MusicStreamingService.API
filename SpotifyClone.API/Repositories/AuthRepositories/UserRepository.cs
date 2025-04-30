using Microsoft.AspNetCore.Identity;
using SpotifyClone.API.Models.Entities;
using SpotifyClone.API.Repositories.AuthRepositories.AuthRepositoriesInterfaces;

namespace SpotifyClone.API.Repositories.AuthRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task AddToRoleAsync(ApplicationUser user, string role)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            return result.Succeeded;
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId) 
        { 
            return await _userManager.FindByIdAsync(userId); 
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user) 
        { 
            return await _userManager.UpdateAsync(user); 
        }

        public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword) 
        { 
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword); 
        }
    }

}
