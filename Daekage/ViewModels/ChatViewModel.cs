using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Daekage.ViewModels
{
    public class ChatViewModel : BindableBase, INavigationAware
    {
        private string _inputMessage;
        private DelegateCommand _sendCommand;

        public string InputMessage
        {
            get => _inputMessage;
            set
            {
                _ = SetProperty(ref _inputMessage, value);
                SendCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand SendCommand => _sendCommand ??= new DelegateCommand(OnSendCommand, CanSendCommand);

        public ChatViewModel()
        {
        }

        private void OnSendCommand()
        {
            throw new NotImplementedException();
        }

        private bool CanSendCommand() => string.IsNullOrWhiteSpace(InputMessage);

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;
    }
}
