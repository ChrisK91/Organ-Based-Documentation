using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.Models
{
    [Index(nameof(VersionedStringEntryId), nameof(Timestamp))]
    public class VersionedStringEntry
    {
        [Key]
        [Required]
        public Guid VersionedStringEntryId { get; private set; }

        [Required]
        public string Value { get; set; }

        public DateTime Timestamp { get; private set; }

        public VersionedStringEntry(string value)
        {
            Timestamp = DateTime.Now;
            Value = value;
            VersionedStringEntryId = new Guid();
        }

        internal VersionedStringEntry(string value, DateTime timestamp)
        {
            Timestamp = timestamp;
            Value = value;
            VersionedStringEntryId = new Guid();
        }

        public int? AdmissionId { get; set; }
        public Admission Admission { get; set; }

        public int? PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}
