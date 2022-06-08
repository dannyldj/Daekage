using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Daekage.Core.Contracts.Services;
using Daekage.Core.Models;
using Daekage.Properties;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestSharp;

namespace Daekage.ViewModels
{
    public class NoticeViewModel : BindableBase, INavigationAware
    {
        private readonly IRestService _restService;
        private ObservableCollection<NoticeModel> _noticeList;
        private string _inputMessage;
        private DelegateCommand _sendCommand;

        public ObservableCollection<NoticeModel> NoticeList
        {
            get => _noticeList;
            set => SetProperty(ref _noticeList, value);
        }

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

        public NoticeViewModel(IRestService restService)
        {
            _restService = restService;
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            var returnValue = await _restService.RestRequest<List<NoticeModel>>(Method.Get, "api/Notices", null);
            if (returnValue is null) return;

            NoticeList = new ObservableCollection<NoticeModel>(returnValue);
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;

        private async void OnSendCommand() {
            if (!(App.Current.Properties["Userinfo"] is UserinfoModel userInfo)) return;

            var sendObj = new NoticeModel
            {
                Writer = userInfo.Name,
                WriterEmail = userInfo.Email,
                Date = DateTime.Now.ToString("yyyy년 MM월 dd일 hh:mm"),
                Text = InputMessage
            };
            var result = await _restService.RestRequest<NoticeModel>(Method.Post, "api/Notices", sendObj);

            if (result is null)
            {
                _ = MessageBox.Show(Resources.NetworkErrorMessage);
                return;
            }

            NoticeList.Add(result);
            InputMessage = string.Empty;
        }

        private bool CanSendCommand() => !string.IsNullOrEmpty(InputMessage);
    }
}
