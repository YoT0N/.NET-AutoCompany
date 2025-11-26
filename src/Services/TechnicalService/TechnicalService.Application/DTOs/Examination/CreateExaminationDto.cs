using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalService.Bll.DTOs.Examination
{
    public class CreateExaminationDto
    {
        public string BusCountryNumber { get; set; } = string.Empty;
        public DateTime ExaminationDate { get; set; }
        public string ExaminationResult { get; set; } = string.Empty;
        public bool SentForRepair { get; set; }
        public decimal RepairPrice { get; set; }
        public string? MechanicName { get; set; }
        public string? Notes { get; set; }
        public List<ExaminationRepairPartDto>? RepairParts { get; set; }
    }
}
