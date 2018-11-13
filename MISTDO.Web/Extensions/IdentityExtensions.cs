using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Extensions
{
    public static class IdentityExtensions
    {
        public static IdentityBuilder AddSecondIdentity<TUser, TRole>(
            this IServiceCollection services, Action<IdentityOptions> setupAction)
            where TUser : class
            where TRole : class
        {
            services.TryAddScoped<IUserValidator<TUser>, UserValidator<TUser>>();
            services.TryAddScoped<IPasswordValidator<TUser>, PasswordValidator<TUser>>();
            services.TryAddScoped<IPasswordHasher<TUser>, PasswordHasher<TUser>>();
            services.TryAddScoped<IRoleValidator<TRole>, RoleValidator<TRole>>();
            services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<TUser>>();
            services.TryAddScoped<IUserClaimsPrincipalFactory<TUser>, UserClaimsPrincipalFactory<TUser, TRole>>();
            services.TryAddScoped<UserManager<TUser>, AspNetUserManager<TUser>>();
            services.TryAddScoped<SignInManager<TUser>, SignInManager<TUser>>();
            services.TryAddScoped<RoleManager<TRole>, AspNetRoleManager<TRole>>();
            // services.TryAddScoped<TotpSecurityStampBasedTokenProvider<TUser>, EmailTokenProvider<TUser>>();
            services.TryAddScoped<IUserTwoFactorTokenProvider<TUser>, DataProtectorTokenProvider<TUser>>();


            if (setupAction != null)
                services.Configure(setupAction);

            return new IdentityBuilder(typeof(TUser), typeof(TRole), services);
        }
    }
}
