using System.ComponentModel.DataAnnotations;

namespace GameStore.Models
{
    public class Game
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime ReleaseDate { get; set; } = DateTime.Now;
        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
