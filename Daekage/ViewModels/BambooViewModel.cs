using System;
using System.Collections.ObjectModel;
using Daekage.Core.Models;
using Prism.Mvvm;

namespace Daekage.ViewModels
{
    public class BambooViewModel : BindableBase
    {
        private ObservableCollection<BambooModel> _bambooList;
        public ObservableCollection<BambooModel> BambooList
        {
            get => _bambooList;
            set => SetProperty(ref _bambooList, value);
        }

        private string _inputText;
        public string InputText
        {
            get => _inputText;
            set => SetProperty(ref _inputText, value);
        }

        public BambooViewModel()
        {
            BambooList = new ObservableCollection<BambooModel>();
        }
    }
}
