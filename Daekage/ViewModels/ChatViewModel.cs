using System;

using Prism.Mvvm;
using Prism.Regions;

namespace Daekage.ViewModels
{
    public class ChatViewModel : BindableBase, INavigationAware
    {
        private string _inputMessage;

        public string InputMessage
        {
            get => _inputMessage;
            set => SetProperty(ref _inputMessage, value);
        }

        public ChatViewModel()
        {
        }

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
