using System;
using System.Collections.ObjectModel;
using Daekage.Core.Models;
using Prism.Mvvm;

namespace Daekage.ViewModels
{
    public class BambooViewModel : BindableBase
    {
        private ObservableCollection<BambooItemModel> _bambooItems;
        public ObservableCollection<BambooItemModel> BambooItems
        {
            get => _bambooItems;
            set => SetProperty(ref _bambooItems, value);
        }

        private string _inputText;
        public string InputText
        {
            get => _inputText;
            set => SetProperty(ref _inputText, value);
        }

        public BambooViewModel()
        {
            BambooItems = new ObservableCollection<BambooItemModel>();
        }
    }
}
