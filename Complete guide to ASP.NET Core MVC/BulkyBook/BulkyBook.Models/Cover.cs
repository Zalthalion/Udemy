using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    public class Cover
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

    }
}
