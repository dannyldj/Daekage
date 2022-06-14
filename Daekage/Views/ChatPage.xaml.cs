using System.Windows.Controls;
using System.Windows.Input;

namespace Daekage.Views
{
    public partial class ChatPage : UserControl
    {
        public ChatPage()
        {
            InitializeComponent();
        }

        private void MessageTb_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
                MessageTb.AcceptsReturn = true;
        }

        private void MessageTb_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyUp(Key.LeftShift))
                MessageTb.AcceptsReturn = false;
        }
    }
}
