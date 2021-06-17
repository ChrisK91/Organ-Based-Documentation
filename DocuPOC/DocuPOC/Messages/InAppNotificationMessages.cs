using CommunityToolkit.Mvvm.Messaging.Messages;
using DocuPOC.Models;
using DocuPOC.ViewModels;
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

    public class ShowHistory : ValueChangedMessage<AdmissionDetailViewModel>
    {
        public ShowHistory(AdmissionDetailViewModel value) : base(value) { }
    }

    public class StartLoading : ValueChangedMessage<object>
    {
        public StartLoading(object o) : base(o) { }
    }

    public class DoneLoading : ValueChangedMessage<object>
    {
        public DoneLoading(object o) : base(o) { }
    }

    public class UpdateRefreshIndicator : ValueChangedMessage<object>
    {
        public UpdateRefreshIndicator(object o) : base(o) { }
    }

    public class FormatBold : ValueChangedMessage<object>
    {
        public FormatBold(object o) : base(o) { }
    }

    public class FormatItalic : ValueChangedMessage<object>
    {
        public FormatItalic(object o) : base(o) { }
    }
}
