using System.ComponentModel.DataAnnotations;

namespace DataConcentrator.src
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name {  get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
