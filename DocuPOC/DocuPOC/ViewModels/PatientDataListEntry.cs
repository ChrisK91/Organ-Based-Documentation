using DocuPOC.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace DocuPOC.ViewModels
{
    public class PatientDataListEntry : ObservableObject
    {
        private int id;

        public int Id { get => id; set => SetProperty(ref id, value); }

        private string name;
        public string Name { get => name; set => SetProperty(ref name, value); }


        private DateTimeOffset? dateOfBirth;
        public DateTimeOffset? DateOfBirth { get => dateOfBirth; set => SetProperty(ref dateOfBirth, value); }

        public Patient Patient { get; private set; }

        public PatientDataListEntry(Patient p)
        {
            Patient = p;
            Id = p.PatientId;
            Name = p.Name;
            DateOfBirth = p.Birthday;
        }

        public PatientDataListEntry() { }
    }
}
