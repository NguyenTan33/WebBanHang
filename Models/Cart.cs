using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        // Liên kết với User của ASP.NET Identity
        [Required]
        public string UserId { get; set; }

        // Một giỏ hàng có nhiều món đồ
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}