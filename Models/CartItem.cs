using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHang.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }
        [ForeignKey("CartId")]
        public virtual Cart Cart { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public int Quantity { get; set; }
    }
}