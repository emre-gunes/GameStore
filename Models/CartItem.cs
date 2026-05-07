namespace GameStore.Models
{
    public class CartItem
    {
        public int GameId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}
