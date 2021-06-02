using DocuPOC.Models;
using DotLiquid;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.Models
{

    [LiquidType("*")]
    public class Admission
    {
        public int AdmissionId { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public DateTime AdmissionDateTime { get; set; }
        public DateTime DischargeDateTime { get; set; }

        public string Diagnosis { get; set; }
        public string Neurologic { get; set; }
        public string Pulmonal { get; set; }
        public string Cardiology { get; set; }
        public string Renal { get; set; }
        public string Abdominal { get; set; }
        public string Infectiology { get; set; }
        public string ToDo { get; set; }
        public string Procedere { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }

        public int AdmissionTimeInDays
        {
            get
            {
                return AdmissionDateTime.DaysDifference();
            }
        }
    }
}
