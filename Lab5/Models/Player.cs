using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab3.Models
{
    [Table("roster")]
    public class Player
    {
        public string PlayerId { get; set; }
        public int Jersey { get; set; }

        [Display(Name = "First Name")]
        public string FName { get; set; }

        [Display(Name = "Last Name")]
        public string SName { get; set; }

        public string Position { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        public int Weight { get; set; }
        public int Height { get; set; }

        public string BirthCity { get; set; }
        public string BirthState { get; set; }
    }
}
