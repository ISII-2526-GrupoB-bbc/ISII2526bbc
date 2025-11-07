using AppForSEII2526.Models;
using System;
using System.Drawing;
using AppForSEII2526.API.DTOs.ReviewDTO;

namespace AppForSEII2526.API.DTOs.ReseñarDTOs


{
    public class ReseñarDetailDTO : ReseñarForCreateDTO
    {
        public ReseñarDetailDTO(string username, string country, string drivertype, IList<ReseñarItemDTO> rentalitems, DateTime reviewed)
            : base(username, country, drivertype, rentalitems)
        {
            reviewed = reviewed;
        }
        public DateTime Reviewed { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ReseñarDetailDTO dto &&
                   base.Equals(obj) &&
                   Reviewed == dto.Reviewed;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Reviewed);
        }
    }
}

    

