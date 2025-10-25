namespace AppForSEII2526.API.DTOs.ReseñarDTOs
{
    public class ReseñarForCreateDTO
    {
        public ReseñarForCreateDTO(string username, string country, string drivertype, IList<ReseñarItemDTO> rentalitems)
        {
            username = username ?? throw new ArgumentNullException(nameof(username));
            country = country ?? throw new ArgumentNullException(nameof(country));
            drivertype = drivertype ?? throw new ArgumentNullException(nameof(drivertype));
        }
        public ReseñarForCreateDTO()
        {
            Rentalitems = new List<ReseñarItemDTO>();
        }
        [Required(ErrorMessage = "El nombre del usuario es obligatorio.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "El país es obligatorio.")]
        public string Country { get; set; }
        [Required(ErrorMessage = "El tipo de conductor es obligatorio.")]
        [RegularExpression("^(?i)(novato|experto)$", ErrorMessage = "El tipo de conductor debe ser 'novato' o 'experto'.")]
        public string DriverType { get; set; }
        [Required(ErrorMessage = "Debe seleccionar al menos un coche para reseñar.")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos una reseña de coche.")]
        public IList<ReseñarItemDTO> Rentalitems { get; set; }

        public override bool Equals(object? obj)
        {
           return obj is ReseñarForCreateDTO dto &&
                  UserName == dto.UserName &&
                  Country == dto.Country &&
                  DriverType == dto.DriverType &&
                 Rentalitems.SequenceEqual(dto.Rentalitems);
        }

    }
}
