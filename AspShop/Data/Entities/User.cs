﻿using System.Text.Json.Serialization;
namespace AspShop.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public String Name { get; set; } = null!;
        public String Email { get; set; } = null!;
        public DateTime? Birthdate { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime? DeleteAt { get; set; }

        [JsonIgnore]
        public ICollection<UserAccess> Accesses { get; set; } = [];
        public ICollection<CartItem> Carts { get; set; } = [];
    }
}
