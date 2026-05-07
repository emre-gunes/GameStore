using System.ComponentModel.DataAnnotations;

namespace GameStore.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Game> Games { get; set; } = new List<Game>();
    }
}
