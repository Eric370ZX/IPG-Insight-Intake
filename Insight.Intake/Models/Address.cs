namespace Insight.Intake.Models
{
    public class LocationInfo
    {
        public string LocationName { get; set; }
        public string ZipCode { get; set; }
        public string ShortZipCode { get { return ZipCode?.Length > 5 ? ZipCode.Substring(0, 5) : ZipCode; } }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string StateAbbr { get; set; }
    }
}
