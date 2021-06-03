using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.ViewModels
{
    /// <summary>
    /// Using this interface ensures, that all necessary properties for the main tab view are present.
    /// </summary>
    public interface ITabViewModel
    {
        public string Header { get; }
        public Microsoft.UI.Xaml.Controls.Symbol Symbol { get; }

        public bool CanClose { get; }
        public bool CanDrag { get; }
    }
}
