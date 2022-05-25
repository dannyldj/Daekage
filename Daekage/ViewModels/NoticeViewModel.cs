using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Daekage.Core.Contracts.Services;
using Daekage.Core.Models;
using Prism.Mvvm;
using Prism.Regions;
using RestSharp;

namespace Daekage.ViewModels
{
    public class NoticeViewModel : BindableBase, INavigationAware
    {
        private readonly IRestService _restService;
        private ObservableCollection<NoticeModel> _noticeList = new ObservableCollection<NoticeModel>();

        public ObservableCollection<NoticeModel> NoticeList
        {
            get => _noticeList;
            set => SetProperty(ref _noticeList, value);
        }

        public NoticeViewModel(IRestService restService)
        {
            _restService = restService;
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            var returnValue = await _restService.RestRequest<List<NoticeModel>>(Method.Get, "api/Notices", null);
            NoticeList = new ObservableCollection<NoticeModel>(returnValue);
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;
    }
}
