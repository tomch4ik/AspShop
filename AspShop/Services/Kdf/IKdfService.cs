namespace AspShop.Services.Kdf
{
    public interface IKdfService
    {
        String Dk(string password, string salt);
    }
}
