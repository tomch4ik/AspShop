using AspShop.Data;
using AspShop.Services.Kdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace AspShop.Controllers.Api
{
    [Route("api/user")]
    [ApiController]
    public class UserController(DataContext dataContext, IKdfService kdfService) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IKdfService _kdfService = kdfService;

        [HttpGet]
        public object Authenticate()
        {
            String? header = HttpContext.Request.Headers.Authorization;
            if (header == null)      // Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new { Status = "Authorization Header Required" };
            }

            /* Д.З. Реалізувати повний цикл перевірок даних, що передаються
             * для автентифікації
             * - заголовок починається з 'Basic '
             * - credentials успішно декодуються з Base64
             * - userPass ділиться на дві частини (може не містити ":")
             */
            const string prefix = "Basic ";
            if (!header.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new { Status = "Invalid Authorization Scheme" };
            }

            String credentials =    // 'Basic ' - length = 6
                header[prefix.Length..];        // QWxhZGRpbjpvcGVuIHNlc2FtZQ==
            if (string.IsNullOrWhiteSpace(credentials))
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new { Status = "Empty credentials" };
            }

            String userPass;        // Aladdin:open sesame
            try
            {
                userPass = System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(credentials));
            }
            catch
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new { Status = "Invalid Base64 credentials" };
            }

            String[] parts = userPass.Split(':', 2);
            if (parts.Length != 2)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new { Status = "Credentials must be in format login:password" };
            }

            String login = parts[0];
            String password = parts[1];

            var userAccess = _dataContext
                .UserAccesses
                .AsNoTracking()
                .Include(ua => ua.User)
                .Include(ua => ua.Role)
                .FirstOrDefault(ua => ua.Login == login);

            if (userAccess == null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new { Status = "Credentials rejected" };
            }

            String dk = _kdfService.Dk(password, userAccess.Salt);
            if (dk != userAccess.Dk)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new { Status = "Credentials rejected." };
            }
            // зберігаємо у сесії факт успішної автентифікації
            HttpContext.Session.SetString(
                "UserController::Authenticate",
                JsonSerializer.Serialize(userAccess)
            );
            return userAccess;
        }


        [HttpPost]
        public object SignUp()
        {
            return new { Status = "SignUp Works" };
        }

        [HttpPost("admin")]  // POST /api/user/admin
        public object SignUpAdmin()
        {
            return new { Status = "SignUpAdmin Works" };
        }
    }
}

/* Відмінності АРІ та MVC контролерів
 * MVC:
 *  адресація за назвою дії (Action) - різні дії -- різні адреси
 *  GET  /Home/Index     --> HomeController.Index()
 *  POST /Home/Index     --> HomeController.Index()
 *  GET  /Home/Privacy   --> HomeController.Privacy()
 *  повернення - IActionResult частіше за все View
 *  
 * API:
 *  адресація за анотацією [Route("api/user")], різниця
 *  у методах запиту
 *  GET  api/user  ---> [HttpGet] Authenticate()
 *  POST api/user  ---> [HttpPost] SignUp()
 *  PUT  api/user  ---> 
 *  
 *  C   POST
 *  R   GET
 *  U   PUT(replace) PATCH(partially update)
 *  D   DELETE
 */
/* Авторизація. Схеми.
 * 0) Кукі (Cookie) - заголовки НТТР-пакету, які зберігаються у клієнта
 *      та автоматично включаються ним до всіх наступних запитів до сервера
 *      "+" простота використання
 *      "-" автоматизовано тільки у браузерах, в інших програмах це справа
 *           програміста. 
 *      "-" відкритість, легкість перехоплення даних
 *      
 * 1) Сесії (серверні): базуються на Кукі, проте всі дані зберігаються
 *     на сервері, у куках передається тільки ідентифікатор сесії
 *     "+" покращена безпека
 *     "-" велике навантаження на сховище сервера
 *     
 * 2) Токени (клієнтські): клієнт зберігає токен, який лише перевіряється
 *     сервером.
 *     "+" відмова від кукі та сесій
 *     "-" більше навантаження на роботу сервера
 *  2а) Токени-ідентифікатори
 *  2б) Токени з даними (JWT)
 */