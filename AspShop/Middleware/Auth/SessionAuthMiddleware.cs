﻿using AspShop.Data.Entities;
using AspShop.Services.Auth;
using System.Security.Claims;
using System.Text.Json;

namespace AspShop.Middleware.Auth
{
    public class SessionAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService)
        {
            if (context.Request.Query.ContainsKey("logout"))
            {
                //context.Session.Remove("UserController::Authenticate");
                authService.RemoveAuth();
                context.Response.Redirect(context.Request.Path);
                return;
            }
            
            {
                if (authService.GetAuth<UserAccess>()
                    is UserAccess userAccess)
                {
                    context.User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            [
                                new Claim(ClaimTypes.Sid, userAccess.UserId.ToString()),
                                new Claim(ClaimTypes.Name, userAccess.User.Name),
                                new Claim(ClaimTypes.Role, userAccess.RoleId),
                                new Claim(ClaimTypes.Email, userAccess.User.Email)
                            ],
                            nameof(SessionAuthMiddleware)
                        )
                    );
                }
            }
            await _next(context);
        }

    }

    public static class SessionAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionAuth(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionAuthMiddleware>();
        }
    }
}
/* Д.З. Включити у склад авторизаційних даних відомості про E-mail
 * користувача. Додати до навігаційної панелі Layout кнопку-посилання
 * "надіслати лист" з переходом на "mailto:..."
 */
