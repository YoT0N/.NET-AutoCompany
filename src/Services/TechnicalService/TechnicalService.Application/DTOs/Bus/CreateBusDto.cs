using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalService.Bll.DTOs.Bus
{
    public class CreateBusDto
    {
        public string CountryNumber { get; set; } = string.Empty;
        public string BoardingNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int PassengerCapacity { get; set; }
        public int YearOfManufacture { get; set; }
        public decimal Mileage { get; set; }
        public DateTime DateOfReceipt { get; set; }
        public int CurrentStatusId { get; set; }
    }
}
