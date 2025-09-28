using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AspShop.Data.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        [Column(TypeName = "decimal(15, 2)")]
        public double Price { get; set; }

        public Guid? DiscountId { get; set; }
        [JsonIgnore]
        public User User { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; } = [];
    }
}
