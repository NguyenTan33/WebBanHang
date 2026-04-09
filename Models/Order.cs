namespace WebBanHang.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Ai mua?
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }

        // TRẠNG THÁI: 0: Chờ duyệt, 1: Đã duyệt, 2: Đã hủy
        public int Status { get; set; } = 0;

        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
    }
}
