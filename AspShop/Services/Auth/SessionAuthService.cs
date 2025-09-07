namespace AspShop.Services.Auth
{
    public class SessionAuthService(
        IHttpContextAccessor httpContextAccessor) : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        const String sessionKey = "ISessionAuthService";

        public object? GetAuth()
        {
            throw new NotImplementedException();
        }

        public void RemoveAuth()
        {
            _httpContextAccessor.HttpContext?.Session.Remove(sessionKey);
        }

        public void SetAuth(object payload)
        {
            throw new NotImplementedException();
        }
    }
}
