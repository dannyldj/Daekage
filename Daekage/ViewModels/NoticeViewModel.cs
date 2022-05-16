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

            //NoticeList = new ObservableCollection<NoticeModel>
            //{
            //    new NoticeModel
            //    {
            //        Writer = "아무개",
            //        Date = DateTime.Now.ToString("yyyy년 MM월 dd일"),
            //        Files = new List<string> { "취업 정보.png", "2.png", "3.png" },
            //        Text = "ㄴㅁ이;ㅏ럼ㄴ이;ㅏ럼;ㅣㄴ어ㅏㅣㄻ;니아ㅓㄻ;니아럼;니아럼;니아럼;ㅣㄴ아러미;ㄴㅇㄴㄹ\nㅁㄴ이라ㅓㅗㅁ니ㅏ어리ㅏㅁ넝리ㅏ먼ㅇ;ㅏㅣ러",
            //    },
            //    new NoticeModel
            //    {
            //        Writer = "아무개",
            //        Date = DateTime.Now.ToString("yyyy년 MM월 dd일"),
            //        Files = new List<string> { "1.png", "2.png", "3.png" },
            //        Text = "ㄴㅁ이;ㅏ럼ㄴ이;ㅏ럼;ㅣㄴ어ㅏㅣㄻ;니아ㅓㄻ;니아럼;니아럼;니아럼;ㅣㄴ아러미;ㄴㅇㄴㄹ\nㅁㄴ이라ㅓㅗㅁ니ㅏ어리ㅏㅁ넝리ㅏ먼ㅇ;ㅏㅣ러",
            //    },
            //    new NoticeModel
            //    {
            //        Writer = "아무개",
            //        Date = DateTime.Now.ToString("yyyy년 MM월 dd일"),
            //        Files = new List<string> { "1.png", "2.png", "3.png" },
            //        Text = "ㄴㅁ이;ㅏ럼ㄴ이;ㅏ럼;ㅣㄴ어ㅏㅣㄻ;니아ㅓㄻ;니아럼;니아럼;니아럼;ㅣㄴ아러미;ㄴㅇㄴㄹ\nㅁㄴ이라ㅓㅗㅁ니ㅏ어리ㅏㅁ넝리ㅏ먼ㅇ;ㅏㅣ러",
            //    },
            //    new NoticeModel
            //    {
            //        Writer = "아무개",
            //        Date = DateTime.Now.ToString("yyyy년 MM월 dd일"),
            //        Files = new List<string> { "1.png", "2.png", "3.png" },
            //        Text = "ㄴㅁ이;ㅏ럼ㄴ이;ㅏ럼;ㅣㄴ어ㅏㅣㄻ;니아ㅓㄻ;니아럼;니아럼;니아럼;ㅣㄴ아러미;ㄴㅇㄴㄹ\nㅁㄴ이라ㅓㅗㅁ니ㅏ어리ㅏㅁ넝리ㅏ먼ㅇ;ㅏㅣ러",
            //    },
            //    new NoticeModel
            //    {
            //        Writer = "아무개",
            //        Date = DateTime.Now.ToString("yyyy년 MM월 dd일"),
            //        Files = new List<string> { "1.png", "2.png", "3.png" },
            //        Text = "ㄴㅁ이;ㅏ럼ㄴ이;ㅏ럼;ㅣㄴ어ㅏㅣㄻ;니아ㅓㄻ;니아럼;니아럼;니아럼;ㅣㄴ아러미;ㄴㅇㄴㄹ\nㅁㄴ이라ㅓㅗㅁ니ㅏ어리ㅏㅁ넝리ㅏ먼ㅇ;ㅏㅣ러",
            //    },
            //    new NoticeModel
            //    {
            //        Writer = "아무개",
            //        Date = DateTime.Now.ToString("yyyy년 MM월 dd일"),
            //        Files = new List<string> { "1.png", "2.png", "3.png", "3.png", "3.png", "3.png", "3.png", "3.png",
            //            "3.png", "3.png", "3.png"
            //        },
            //        Text = "ㄴㅁ이;ㅏ럼ㄴ이;ㅏ럼;ㅣㄴ어ㅏㅣㄻ;니아ㅓㄻ;니아럼;니아럼;니아럼;ㅣㄴ아러미;ㄴㅇㄴㄹ\nㅁㄴ이라ㅓㅗㅁ니ㅏ어리ㅏㅁ넝리ㅏ먼ㅇ;ㅏㅣ러",
            //    },
            //    new NoticeModel
            //    {
            //        Writer = "아무개",
            //        Date = DateTime.Now.ToString("yyyy년 MM월 dd일"),
            //        Files = new List<string> { "1.png", "2.png", "3.png" },
            //        Text = "ㄴㅁ이;ㅏ럼ㄴ이;ㅏ럼;ㅣㄴ어ㅏㅣㄻ;니아ㅓㄻ;니아럼;니아럼;니아럼;ㅣㄴ아러미;ㄴㅇㄴㄹ\nㅁㄴ이라ㅓㅗㅁ니ㅏ어리ㅏㅁ넝리ㅏ먼ㅇ;ㅏㅣ러",
            //    },
            //};
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            var result = await _restService.RestRequest<List<NoticeModel>>(Method.Get, "api/Notices", null);
            NoticeList = new ObservableCollection<NoticeModel>(result);
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;
    }
}
