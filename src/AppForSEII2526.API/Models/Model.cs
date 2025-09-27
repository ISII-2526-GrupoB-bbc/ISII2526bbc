using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.Models
{
    public class Model
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        // Relación 1:N → un modelo puede tener muchos coches
        public List<Car> Cars { get; set; }
    }
}
