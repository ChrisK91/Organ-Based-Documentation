using CommunityToolkit.Mvvm.Messaging.Messages;
using DocuPOC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.Messages
{
    public class PatientUpdatedMessage : ValueChangedMessage<Patient>
    {
        public PatientUpdatedMessage(Patient value) : base(value) { }
    }
}
