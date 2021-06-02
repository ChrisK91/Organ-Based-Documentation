using DocuPOC.Models;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
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
