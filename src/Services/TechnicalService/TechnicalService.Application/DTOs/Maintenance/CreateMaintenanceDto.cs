using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalService.Bll.DTOs.Maintenance
{
    public class CreateMaintenanceDto
    {
        public string BusCountryNumber { get; set; } = string.Empty;
        public DateTime MaintenanceDate { get; set; }
        public string MaintenanceType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Cost { get; set; }
        public string? MechanicName { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
    }
}
