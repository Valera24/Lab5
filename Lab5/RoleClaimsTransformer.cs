using Lab4.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Lab5
{
    public class RoleClaimsTransformer : IClaimsTransformation
    {
        private readonly UserManager<Lab4User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleClaimsTransformer(
            UserManager<Lab4User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            // Клонируем principal для добавления новых claims
            var clone = principal.Clone();
            var newIdentity = (ClaimsIdentity)clone.Identity;

            // Проверяем, что пользователь аутентифицирован через Google
            if (principal.HasClaim(c => c.Issuer == "https://accounts.google.com"))
            {
                // Находим пользователя в базе
                var email = principal.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(email);
               
                if (user != null)
                {
                    // Автоматическое назначение ролей
                    await AssignRoles(user, principal);

               
                } 
            }
            return clone;
        }

        private async Task AssignRoles(Lab4User user, ClaimsPrincipal principal)
        {
            var email = user.Email?.ToLower();
            if (!string.IsNullOrEmpty(email))
            {
                if (email.EndsWith("2005@gmail.com"))
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }

        }
    }
}
