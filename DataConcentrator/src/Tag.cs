using System.ComponentModel.DataAnnotations;

namespace DataConcentrator.src
{
    public class Tag
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public string Name {  get; set; }

        public string Description { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
