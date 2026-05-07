using System.ComponentModel.DataAnnotations;

namespace GameStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string AppUserId { get; set; } = string.Empty;
        public AppUser? AppUser { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Completed";

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
