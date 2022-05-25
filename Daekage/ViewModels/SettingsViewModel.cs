using System;
using System.Windows;
using System.Windows.Input;

using Daekage.Contracts.Services;
using Daekage.Core.Models;
using Daekage.Models;
using Daekage.Properties;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Daekage.ViewModels
{
    // TODO WTS: Change the URL for your privacy policy in the appsettings.json file, currently set to https://YourPrivacyUrlGoesHere
    public class SettingsViewModel : BindableBase, INavigationAware
    {
        private readonly AppConfig _appConfig;
        private readonly IThemeSelectorService _themeSelectorService;
        private readonly ISystemService _systemService;
        private readonly IApplicationInfoService _applicationInfoService;
        private readonly IOAuthService _oAuthService;
        private AppTheme _theme;
        private string _versionDescription;
        private UserinfoModel _userinfo;
        private ICommand _googleAuthCommand;
        private ICommand _logoutCommand;
        private ICommand _setThemeCommand;
        private ICommand _privacyStatementCommand;

        public AppTheme Theme
        {
            get => _theme;
            set => SetProperty(ref _theme, value);
        }

        public string VersionDescription
        {
            get => _versionDescription;
            set => SetProperty(ref _versionDescription, value);
        }

        public bool IsThereInfo => !(Userinfo is null);

        public UserinfoModel Userinfo
        {
            get => _userinfo;
            set
            {
                _ = SetProperty(ref _userinfo, value);
                RaisePropertyChanged(nameof(IsThereInfo));
            }
        }

        public ICommand GoogleAuthCommand => _googleAuthCommand ??= new DelegateCommand(OnAuth);

        public ICommand LogoutCommand => _logoutCommand ??= new DelegateCommand(OnLogout);

        public ICommand SetThemeCommand => _setThemeCommand ??= new DelegateCommand<string>(OnSetTheme);

        public ICommand PrivacyStatementCommand => _privacyStatementCommand ??= new DelegateCommand(OnPrivacyStatement);

        public SettingsViewModel(AppConfig appConfig, IThemeSelectorService themeSelectorService,
            ISystemService systemService, IApplicationInfoService applicationInfoService, IOAuthService oAuthService)
        {
            _appConfig = appConfig;
            _themeSelectorService = themeSelectorService;
            _systemService = systemService;
            _applicationInfoService = applicationInfoService;
            _oAuthService = oAuthService;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            VersionDescription = $"{Properties.Resources.AppDisplayName} - {_applicationInfoService.GetVersion()}";
            Theme = _themeSelectorService.GetCurrentTheme();

            object userinfo = App.Current.Properties["Userinfo"];
            if (userinfo is { })
            {
                Userinfo = userinfo as UserinfoModel;
            }
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private async void OnAuth()
        {
            await _oAuthService.GoogleAuth();
            var userinfo = App.Current.Properties["Userinfo"] as UserinfoModel;

            if (userinfo?.Domain != "dgsw.hs.kr")
            {
                _ = MessageBox.Show(string.Format(Resources.InvalidAccMessage, Environment.NewLine));
                OnLogout();
                return;
            }

            Userinfo = userinfo;
        }

        private void OnLogout()
        {
            App.Current.Properties.Remove("Userinfo");
            App.Current.Properties.Remove("AccessToken");
            App.Current.Properties.Remove("RefreshToken");

            Userinfo = null;
        }

        private void OnSetTheme(string themeName)
        {
            var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
            _themeSelectorService.SetTheme(theme);
        }

        private void OnPrivacyStatement()
            => _systemService.OpenInWebBrowser(_appConfig.PrivacyStatement);

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;
    }
}
