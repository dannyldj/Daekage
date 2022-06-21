using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using Daekage.Constants;
using Daekage.Contracts.Services;
using Daekage.Core.Contracts.Services;
using Daekage.Core.Models;
using Daekage.Models;
using Daekage.Properties;
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
        private bool _isTeacher;
        private ObservableCollection<NoticeModel> _notices;
        private ICommand _updateCommand;
        private ICommand _deleteCommand;
        private ICommand _navigatePostPageCommand;

        public bool IsTeacher
        {
            get => _isTeacher;
            set => SetProperty(ref _isTeacher, value);
        }

        public ObservableCollection<NoticeModel> Notices
        {
            get => _notices;
            set => SetProperty(ref _notices, value);
        }

        public List<DropDownItem> DropDownItems { get; set; }

        public ICommand UpdateCommand => _updateCommand ??= new DelegateCommand<NoticeModel>(OnUpdateCommand);

        public ICommand DeleteCommand => _deleteCommand ??= new DelegateCommand<NoticeModel>(OnDeleteCommand);

        public ICommand NavigatePostPageCommand =>
            _navigatePostPageCommand ??= new DelegateCommand(OnNavigatePostPageCommand);

        public NoticeViewModel(IRegionManager regionManager, IRestService restService)
        {
            _navigationService = regionManager.Regions[Regions.Main].NavigationService;
            _restService = restService;

            DropDownItems = new List<DropDownItem>
            {
                new DropDownItem { Title = "수정", Command = UpdateCommand },
                new DropDownItem { Title = "삭제", Command = DeleteCommand }
            };

        }

        private void OnUpdateCommand(NoticeModel selectedItem)
        {
            if (Application.Current.Properties["Userinfo"] is UserinfoModel userInfo)
                if (selectedItem.WriterEmail != userInfo.Email)
                {
                    _ = MessageBox.Show(Resources.PermissionMessage);
                    return;
                }
        }

        private async void OnDeleteCommand(NoticeModel selectedItem)
        {
            if (Application.Current.Properties["Userinfo"] is UserinfoModel userInfo)
                if (selectedItem.WriterEmail != userInfo.Email)
                {
                    _ = MessageBox.Show(Resources.PermissionMessage);
                    return;
                }

            bool isSuccess =
                await _restService.NotReturnRestRequest(Method.Delete, $"api/Notices/{selectedItem.Key}", null);
            if (!isSuccess) return;

            _ = Notices.Remove(selectedItem);
        }

        private void OnNavigatePostPageCommand()
        {
            _navigationService.RequestNavigate(PageKeys.PostNotice);
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (Application.Current.Properties["Userinfo"] is UserinfoModel userInfo)
                IsTeacher = await _restService.NotReturnRestRequest(Method.Get, $"api/Auth/{userInfo.Email}", null);
            else IsTeacher = false;

            var returnValue = await _restService.RestRequest<List<NoticeModel>>(Method.Get, "api/Notices", null);
            if (returnValue is null) return;

            Notices = new ObservableCollection<NoticeModel>(returnValue);
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;
    }
}
