namespace AspShop.Services.Storage
{
    public class DiskStorageService : IStorageService
    {
        private const String path = "D:\\Storage\\Asp32\\";
        private static readonly String[] allowedExtensions = [".jpg", ".jpeg", ".png"];
        public byte[]? Load(string filename)
        {
            String fullName = Path.Combine(path, filename);
            if (File.Exists(fullName))
            {
                return File.ReadAllBytes(Path.Combine(path, filename));
            }
            else
            {
                return null;
            }
        }
        public String Save(IFormFile file)
        {
            int dotindex = file.FileName.LastIndexOf('.');
            if (dotindex == -1)
            {
                throw new Exception("File name must have an extension");
            }
            String ext = file.FileName[dotindex..].ToLower();
            if (!allowedExtensions.Contains(ext))
            {
                throw new Exception($"File extension '{ext}' no supported");
            }
            String filename = Guid.NewGuid().ToString() + ext;
            using FileStream fileStream = new(Path.Combine(path, filename), FileMode.Create);
            file.CopyTo(fileStream);
            return filename;
        }
    }
}
