namespace GameStore.Models
{
    public class UserLibrary
    {
        public int Id { get; set; }
        public string AppUserId { get; set; } = string.Empty;
        public AppUser? AppUser { get; set; }

        public int GameId { get; set; }
        public Game? Game { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        
        public int PlayTimeInHours { get; set; } = 0;
        public bool IsRefunded { get; set; } = false;
    }
}
