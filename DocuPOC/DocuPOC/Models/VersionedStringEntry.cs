using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.Models
{
    public enum EntryType
    {
        Diagnosis = 1,
        Neurologic = 2,
        Pulmonal = 3,
        Cardiology = 4,
        Renal = 5,
        Abdominal = 6,
        Infectiology = 7,
        ToDo = 8,
        Procedere = 9,
        Notes = 10
    }

    [Index(nameof(VersionedStringEntryId), nameof(Timestamp), nameof(EntryType))]
    public class VersionedStringEntry
    {
        [Key]
        [Required]
        public Guid VersionedStringEntryId { get; private set; }

        [Required]
        public string Value { get; private set; }

        public DateTime Timestamp { get; private set; }

        public EntryType EntryType { get; private set; }

        public VersionedStringEntry() { } // Required for EF

        public VersionedStringEntry(string value, EntryType type, Admission a = null, Patient p = null) : this(value, DateTime.Now, type, a, p)
        { }

        internal VersionedStringEntry(string value, DateTime timestamp, EntryType type, Admission a = null, Patient p = null)
        {
#if DEBUG
            Debug.Assert(value != null);
#endif
            Timestamp = timestamp;
            Value = value;
            VersionedStringEntryId = new Guid();
            EntryType = type;
            Admission = a;
            Patient = p;
        }

        public int? AdmissionId { get; private set; }
        public Admission Admission { get; private set; }

        public int? PatientId { get; private set; }
        public Patient Patient { get; private set; }
    }
}
