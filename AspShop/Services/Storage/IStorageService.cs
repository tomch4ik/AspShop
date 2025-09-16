namespace AspShop.Services.Storage
{
    public interface IStorageService
    {
        String Save(IFormFile file);
        byte[]? Load(String fileName);
    }
}
