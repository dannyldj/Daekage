using System;
using System.Windows;
using System.Windows.Input;
using Daekage.Constants;
using Daekage.Core.Contracts.Services;
using Daekage.Core.Models;
using Daekage.Properties;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestSharp;

namespace Daekage.ViewModels
{
    public class PostNoticeViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionNavigationService _navigationService;
        private readonly IRestService _restService;
        private string _inputMessage;
        private ICommand _backCommand;
        private DelegateCommand _postCommand;

        public string InputMessage
        {
            get => _inputMessage;
            set
            {
                _ = SetProperty(ref _inputMessage, value);
                PostCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand BackCommand => _backCommand ??= new DelegateCommand(OnBackCommand);
        public DelegateCommand PostCommand => _postCommand ??= new DelegateCommand(OnPostCommand, CanPostCommand);

        public PostNoticeViewModel(IRegionManager regionManager, IRestService restService)
        {
            _navigationService = regionManager.Regions[Regions.Main].NavigationService;
            _restService = restService;
        }

        private void OnBackCommand()
        {
            _navigationService.Journal.GoBack();
        }

        private async void OnPostCommand()
        {
            if (!(Application.Current.Properties["Userinfo"] is UserinfoModel userInfo)) return;

            var sendObj = new NoticeModel
            {
                Writer = userInfo.Name,
                WriterEmail = userInfo.Email,
                Date = DateTime.Now.ToString("yyyy년 MM월 dd일 hh:mm"),
                Text = InputMessage
            };
            bool isSuccess = await _restService.NotReturnRestRequest(Method.Post, "api/Notices", sendObj);

            if (!isSuccess)
                _ = MessageBox.Show(Resources.NetworkErrorMessage);
            
            OnBackCommand();
        }

        private bool CanPostCommand() => !string.IsNullOrEmpty(InputMessage);

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            InputMessage = string.Empty;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;
    }
}
