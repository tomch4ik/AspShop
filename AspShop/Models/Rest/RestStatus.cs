namespace AspShop.Models.Rest
{
    public class RestStatus
    {
        public String Phrase { get; set; } = "Ok";
        public int Code { get; set; } = 200;
        public bool IsOK { get; set; } = true;

        public static readonly RestStatus Status400 = new() { IsOK = false, Code = 400, Phrase = "Invalid" };
        public static readonly RestStatus Status401 = new() { IsOK = false, Code = 401, Phrase = "Unauthorized" };

    }
}
