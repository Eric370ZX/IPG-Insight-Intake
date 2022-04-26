using System;

namespace Insight.Intake.Models
{
    public class PersonInfo: LocationInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MI { get; set; }
        public string FullName
        {
            get
            {
                var patientName = $"{LastName}, {FirstName}";

                if (!string.IsNullOrWhiteSpace(MI))
                {
                    patientName += $", {MI.Substring(0, 1)}";
                }

                return patientName;
            }
        }
        public string FullNameForUB04 
        { get 
            {
                return $"{FirstName}{(string.IsNullOrEmpty(MI) ? "" : $" { MI}")} {LastName}";
            } 
        }
        public DateTime? DOB { get; set; }
        public string DOBDay { get { return DOB?.Day.ToString(); } }
        public string DOBMonth { get { return DOB?.Month.ToString(); } }
        public string DOBYear { get { return DOB?.Year.ToString(); } }
        public ipg_Gender? Gender { get; set; }
        public string Phone { get; set; }
        public string PhoneCode { get { return Phone?.Length > 2 ? Phone.Substring(0, 3) : null; } }
        public string PhoneMainPart { get { return Phone?.Length > 3 ? Phone.Substring(3) : null; } }
        public string MemberIdNumber { get; set; }
        public string GroupNumber { get; set; }
    }
}
