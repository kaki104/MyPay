using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Storage;
using MyPay.Helpers;
using MyPay.Interfaces;
using MyPay.Models;
using MyPay.Services;

namespace MyPay.ViewModels
{
    /// <summary>
    ///     설정 뷰모델
    /// </summary>
    public class SettingsViewModel : Observable, ILoadedUnloaded
    {
        private string _appDescription;

        // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings.md
        private bool _isLightThemeEnabled;

        /// <summary>
        ///     기본 생성자
        /// </summary>
        public SettingsViewModel()
        {
            SwitchThemeCommand = new RelayCommand(async () => { await ThemeSelectorService.SwitchThemeAsync(); });

            LoadedCommand = new RelayCommand(OnLoaded);
            UnloadedCommand = new RelayCommand(OnUnloaded);

            PayInformations = new List<PayInformation>
            {
                new PayInformation
                {
                    Id = "1.0",
                    Value = 1.0,
                    StartTime = DateTime.Parse("08:00"),
                    EndTime = DateTime.Parse("18:00"),
                    TimePay = 10000
                },
                new PayInformation
                {
                    Id = "1.5 Part1",
                    Value = 1.5,
                    StartTime = DateTime.Parse("18:00"),
                    EndTime = DateTime.Parse("22:00"),
                    TimePay = 15000
                },
                new PayInformation
                {
                    Id = "2.0",
                    Value = 2.0,
                    StartTime = DateTime.Parse("22:00"),
                    EndTime = DateTime.Parse("06:00"),
                    TimePay = 20000
                },
                new PayInformation
                {
                    Id = "1.5 Part2",
                    Value = 1.5,
                    StartTime = DateTime.Parse("06:00"),
                    EndTime = DateTime.Parse("08:00"),
                    TimePay = 15000
                },
                new PayInformation
                {
                    Id = "1.5 Holiday",
                    Value = 1.5,
                    IsHoliday = true,
                    StartTime = DateTime.Parse("08:00"),
                    EndTime = DateTime.Parse("18:00"),
                    TimePay = 15000
                },
                new PayInformation
                {
                    Id = "2.0 Holiday1",
                    Value = 2.0,
                    IsHoliday = true,
                    StartTime = DateTime.Parse("18:00"),
                    EndTime = DateTime.Parse("22:00"),
                    TimePay = 20000
                },
                new PayInformation
                {
                    Id = "2.5 Holiday",
                    Value = 2.5,
                    IsHoliday = true,
                    StartTime = DateTime.Parse("22:00"),
                    EndTime = DateTime.Parse("06:00"),
                    TimePay = 25000
                },
                new PayInformation
                {
                    Id = "2.0 Holiday2",
                    Value = 2.0,
                    IsHoliday = true,
                    StartTime = DateTime.Parse("06:00"),
                    EndTime = DateTime.Parse("08:00"),
                    TimePay = 20000
                }
            };
        }

        /// <summary>
        ///     급여 정보 목록
        /// </summary>
        public IList<PayInformation> PayInformations { get; set; }

        /// <summary>
        ///     밝은 테마 가능
        /// </summary>
        public bool IsLightThemeEnabled
        {
            get => _isLightThemeEnabled;
            set => Set(ref _isLightThemeEnabled, value);
        }

        /// <summary>
        ///     앱 설명
        /// </summary>
        public string AppDescription
        {
            get => _appDescription;
            set => Set(ref _appDescription, value);
        }

        /// <summary>
        ///     테마 스위치 커맨드
        /// </summary>
        public ICommand SwitchThemeCommand { get; }

        public ICommand LoadedCommand { get; }

        public ICommand UnloadedCommand { get; }

        private async void OnUnloaded()
        {
            //종료되기 전에 저장
            await ApplicationData.Current.LocalSettings.SaveAsync("PayInformations", PayInformations);
        }

        private async void OnLoaded()
        {
            //저장된 데이터 불러오기
            var pays = await ApplicationData.Current.LocalSettings.ReadAsync<IList<PayInformation>>("PayInformations");
            if (pays == null) return;
            PayInformations = pays;
        }

        /// <summary>
        ///     초기화
        /// </summary>
        public void Initialize()
        {
            IsLightThemeEnabled = ThemeSelectorService.IsLightThemeEnabled;
            AppDescription = GetAppDescription();
        }

        /// <summary>
        ///     어플리케이션 설명 가지고 오기
        /// </summary>
        /// <returns></returns>
        private string GetAppDescription()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{package.DisplayName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
