using DocuPOC.ViewModels;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.Messages
{
    public class ShowPdfMessage : ValueChangedMessage<String>
    {
        public ShowPdfMessage(String value) : base(value) { }
    }

    public class OpenPatientArchiveMessage : ValueChangedMessage<object>
    {
        public OpenPatientArchiveMessage() : base(null) { }
    }

    public class CloseGenericTabMessage : ValueChangedMessage<ITabViewModel>
    {
        public CloseGenericTabMessage(ITabViewModel value) : base(value) { }
    }
}
