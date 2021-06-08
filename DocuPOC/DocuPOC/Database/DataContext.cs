using DocuPOC.Models;
using DocuPOC.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Automation.Peers;
using System;
using System.Diagnostics;

namespace DocuPOC.Database
{
    public class DataContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Admission> Admissions { get; set; }
        public DbSet<VersionedStringEntry> VersionedStringEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var builder = new SqliteConnectionStringBuilder();
            builder.DataSource = Ioc.Default.GetService<ISettingsService>().GetSettingWithDefault("DatabaseLocation", SettingsService.DefaultDatabaseLocation);


            options.UseSqlite(builder.ToString(), o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));

#if false
            options.LogTo(m => Debugger.Log(0, null, m + "\r\n"), LogLevel.Information);
#endif
        }

        public void UpdateDiagnosis(Admission admission, string newDiagnosis, DateTime? timestamp = null)
        {
            updateGenericProperty(newDiagnosis, EntryType.Diagnosis, admission, null, timestamp);
        }

        public void UpdateNeurologic(Admission admission, string newDiagnosis, DateTime? timestamp = null)
        {
            updateGenericProperty(newDiagnosis, EntryType.Neurologic, admission, null, timestamp);
        }

        public void UpdatePulmonal(Admission admission, string newDiagnosis, DateTime? timestamp = null)
        {
            updateGenericProperty(newDiagnosis, EntryType.Pulmonal, admission, null, timestamp);
        }

        public void UpdateCardiology(Admission admission, string newDiagnosis, DateTime? timestamp = null)
        {
            updateGenericProperty(newDiagnosis, EntryType.Cardiology, admission, null, timestamp);
        }

        public void UpdateRenal(Admission admission, string newDiagnosis, DateTime? timestamp = null)
        {
            updateGenericProperty(newDiagnosis, EntryType.Renal, admission, null, timestamp);
        }

        public void UpdateAbdominal(Admission admission, string newDiagnosis, DateTime? timestamp = null)
        {
            updateGenericProperty(newDiagnosis, EntryType.Abdominal, admission, null, timestamp);
        }

        public void UpdateInfectiology(Admission admission, string newDiagnosis, DateTime? timestamp = null)
        {
            updateGenericProperty(newDiagnosis, EntryType.Infectiology, admission, null, timestamp);
        }

        public void UpdateTodo(Admission admission, string newDiagnosis, DateTime? timestamp = null)
        {
            updateGenericProperty(newDiagnosis, EntryType.ToDo, admission, null, timestamp);
        }

        public void UpdateProcedere(Admission admission, string newDiagnosis, DateTime? timestamp = null)
        {
            updateGenericProperty(newDiagnosis, EntryType.Procedere, admission, null, timestamp);
        }

        public void UpdateNotes(Patient patient, string newDiagnosis, DateTime? timestamp = null)
        {
            updateGenericProperty(newDiagnosis, EntryType.Notes, null, patient, timestamp);
        }

        private void updateGenericProperty(string newValue, EntryType target, Admission admission = null, Patient patient = null, DateTime? timestamp = null)
        {
            string oldValue = null;

            switch (target)
            {
                case EntryType.Diagnosis:
                    oldValue = admission.Diagnosis;
                    admission.Diagnosis = newValue;
                    break;
                case EntryType.Abdominal:
                    oldValue = admission.Abdominal;
                    admission.Abdominal = newValue;
                    break;
                case EntryType.Cardiology:
                    oldValue = admission.Cardiology;
                    admission.Cardiology = newValue;
                    break;
                case EntryType.Infectiology:
                    oldValue = admission.Infectiology;
                    admission.Infectiology = newValue;
                    break;
                case EntryType.Neurologic:
                    oldValue = admission.Neurologic;
                    admission.Neurologic = newValue;
                    break;
                case EntryType.Procedere:
                    oldValue = admission.Procedere;
                    admission.Procedere = newValue;
                    break;
                case EntryType.Pulmonal:
                    oldValue = admission.Pulmonal;
                    admission.Pulmonal = newValue;
                    break;
                case EntryType.Renal:
                    oldValue = admission.Renal;
                    admission.Renal = newValue;
                    break;
                case EntryType.ToDo:
                    oldValue = admission.ToDo;
                    admission.ToDo = newValue;
                    break;
                case EntryType.Notes:
                    oldValue = patient.Notes;
                    patient.Notes = newValue;
                    break;

                default:
                    throw new NotImplementedException("Enumeration value not supported");
            }

            if (oldValue != null && !String.Equals(oldValue, newValue))
            {
                var historyEntry = timestamp == null ?
                    new VersionedStringEntry(oldValue, target, admission, patient) :
                    new VersionedStringEntry(oldValue, timestamp.Value, target, admission, patient);

                VersionedStringEntries.Add(historyEntry);
            }
        }
    }
}
