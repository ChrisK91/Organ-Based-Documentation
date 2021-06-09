using CommunityToolkit.Mvvm.Messaging.Messages;
using DocuPOC.Models;
using DocuPOC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.Messages
{
    public class OpenAdmissionDetailsMessage : ValueChangedMessage<Admission>
    {
        public OpenAdmissionDetailsMessage(Admission value) : base(value) { }
    }

    public class CloseAdmissionDetailsMessage : ValueChangedMessage<AdmissionDetailViewModel>
    {
        public CloseAdmissionDetailsMessage(AdmissionDetailViewModel value) : base(value) { }
    }

    public class AdmissionMovedMessage : ValueChangedMessage<Admission>
    {
        public AdmissionMovedMessage(Admission value) : base(value) { }
    }

    public class AdmissionDischargedMessage : ValueChangedMessage<Admission>
    {
        public AdmissionDischargedMessage(Admission value) : base(value) { }
    }

    public class NewAdmissionMessage : ValueChangedMessage<Admission>
    {
        public NewAdmissionMessage(Admission value) : base(value) { }
    }

    public class PrintAdmissionMessage : ValueChangedMessage<Admission>
    {
        public PrintAdmissionMessage(Admission value) : base(value) { }
    }

    public class ShowNewPatientDialog : ValueChangedMessage<Room>
    {
        public ShowNewPatientDialog(Room value) : base(value) { }
    }
}
