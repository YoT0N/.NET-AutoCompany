using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalService.Bll.DTOs.Bus
{
    public class UpdateBusDto
    {
        public string? BoardingNumber { get; set; }
        public string? Brand { get; set; }
        public int? PassengerCapacity { get; set; }
        public decimal? Mileage { get; set; }
        public int? CurrentStatusId { get; set; }
        public DateTime? WriteoffDate { get; set; }
    }
}
