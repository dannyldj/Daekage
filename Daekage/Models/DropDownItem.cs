using System.Windows.Input;

namespace Daekage.Models
{
    public class DropDownItem
    {
        public string Title { get; set; }

        public ICommand Command { get; set; }
    }
}
