using DotLiquid;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.Models
{
    [LiquidType("*")]
    public class Patient
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Notes { get; set; }
        public List<Admission> Admissions { get; set; }

        public string FormattedBirthday
        {
            get
            {
                return Birthday.ToShortDateString();
            }
        }


        public int AgeInYears
        {
            get
            {
                return Birthday.YearsDifference();
            }
        }
    }
}
