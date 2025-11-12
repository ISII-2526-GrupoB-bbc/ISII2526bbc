using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.Models
{
    public class Model
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }
        public IList<Car> Cars { get; set; } = new List<Car>();

    }
}