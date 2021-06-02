using DocuPOC.ViewModels;
using DotLiquid;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.Models
{
    [LiquidType("*")]
    public class Room
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public List<Admission> Admissions { get; set; }
    }
}
