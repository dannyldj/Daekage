using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Daekage.Constants;
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
        private readonly IRegionNavigationService _navigationService;
        private readonly IRestService _restService;
        private ObservableCollection<NoticeModel> _noticeList;
        private ICommand _navigatePostPageCommand;

        public ObservableCollection<NoticeModel> NoticeList
        {
            get => _noticeList;
            set => SetProperty(ref _noticeList, value);
        }

        public ICommand NavigatePostPageCommand =>
            _navigatePostPageCommand ??= new DelegateCommand(OnNavigatePostPageCommand);

        public NoticeViewModel(IRegionManager regionManager, IRestService restService)
        {
            _navigationService = regionManager.Regions[Regions.Main].NavigationService;
            _restService = restService;
        }

        private void OnNavigatePostPageCommand()
        {
            _navigationService.RequestNavigate(PageKeys.PostNotice);
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
    }
}
