using DocuPOC.Models;
using DotLiquid;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.Models
{

    [LiquidType("*")]
    [Index(nameof(AdmissionDateTime))]
    [Index(nameof(DischargeDateTime))]
    public class Admission
    {
        public int AdmissionId { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public DateTime AdmissionDateTime { get; set; }
        public DateTime? DischargeDateTime { get; set; }

        public string Diagnosis { get; internal set; }

        public string Neurologic { get; internal set; }
        public string Pulmonal { get; internal set; }
        public string Cardiology { get; internal set; }
        public string Renal { get; internal set; }
        public string Abdominal { get; internal set; }
        public string Infectiology { get; internal set; }
        public string ToDo { get; internal set; }
        public string Procedere { get; internal set; }

        public int? RoomId { get; set; }
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
