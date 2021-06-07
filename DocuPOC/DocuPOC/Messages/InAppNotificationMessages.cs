using DocuPOC.Models;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.Messages
{
    public class ShowInfoMessage : ValueChangedMessage<Tuple<string, int>>
    {
        public ShowInfoMessage(Tuple<string,int> value) : base(value) { }
    }

    public class PrintOverview : ValueChangedMessage<List<Room>>
    {
        public PrintOverview(List<Room> value) : base(value) { }
    }

    public class DisplayLoadingIndicator : ValueChangedMessage<object>
    {
        public DisplayLoadingIndicator(object o) : base(o) { }
    }
}
