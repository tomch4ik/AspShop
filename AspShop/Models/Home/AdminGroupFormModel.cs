using Microsoft.AspNetCore.Mvc;

namespace AspShop.Models.Home
{
    public class AdminGroupFormModel
    {
        [FromForm(Name = "group-name")]
        public String Name { get; set; } = null!;

        [FromForm(Name = "group-description")]
        public String Description { get; set; } = null!;

        [FromForm(Name = "group-slug")]
        public String Slug { get; set; } = null!;

        [FromForm(Name = "group-img")]
        public IFormFile Image { get; set; } = null!;
    }
}
