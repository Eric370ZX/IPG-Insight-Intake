using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Models
{
    public class CaseActualPart
    {
        public int Multiplier { get; set; }
        public decimal CostPrice { get; set; }
        public Decimal Quantity { get; set; }
        public EntityReference ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal BilledChg { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public Guid Id { get; set; }
        public EntityReference PartId { get; set; }
        public string PartName { get; set; }
        public EntityReference HcpcsId { get; set; }
        public string HcpcsName { get; set; }
        public string HcpcsDescription { get; set; }
        public EntityReference Hcpcs2Id { get; set; }
        public string Hcpcs2Name { get; set; }
        public string Hcpcs2Description { get; set; }
        public decimal ExtPrice { get; set; }
        public EntityReference UoMId { get; set; }
    }
}
