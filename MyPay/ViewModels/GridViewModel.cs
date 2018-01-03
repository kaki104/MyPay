using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using MyPay.Helpers;
using MyPay.Interfaces;
using MyPay.Models;

namespace MyPay.ViewModels
{
    /// <summary>
    ///     �׸��� ���
    /// </summary>
    public class GridViewModel : Observable, ILoadedUnloaded
    {
        private readonly int[] _monthData =
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
            30, 31
        };

        private string _baseMonth;
        private ICommand _saveExcelCommand;
        private IList<WorkItem> _totalWorks;

        private IList<WorkItem> _works;

        /// <summary>
        ///     �⺻ ������
        /// </summary>
        public GridViewModel()
        {
            //������ Ÿ�� ������ ����

            if (DesignMode.DesignModeEnabled)
            {
                #region DesignData

                Works = new List<WorkItem>
                {
                    new WorkItem
                    {
                        Id = 1,
                        WorkDay = DateTime.Parse("2017-08-23"),
                        StartWork = DateTime.Parse("08:00"),
                        EndWork = DateTime.Parse("19:00"),
                        BasicWorkTime = 450,
                        OverTime15 = 120,
                        OverTime20 = 240,
                        OverTime25 = 120,
                        TodayWorkTime = 800,
                        IsHoliday = true
                    },
                    new WorkItem
                    {
                        Id = 2,
                        WorkDay = DateTime.Parse("2017-08-24"),
                        StartWork = DateTime.Parse("08:00"),
                        EndWork = DateTime.Parse("19:00")
                    }
                };

                #endregion
            }
            else
            {
                Init();
            }
        }

        /// <summary>
        ///     �ٹ� ���
        /// </summary>
        public IList<WorkItem> Works
        {
            get => _works;
            set => Set(ref _works, value);
        }

        /// <summary>
        ///     �հ�
        /// </summary>
        public IList<WorkItem> TotalWorks
        {
            get => _totalWorks;
            set => Set(ref _totalWorks, value);
        }

        /// <summary>
        ///     �հ� 2�ٸ�..
        /// </summary>
        public IList<WorkItem> TotalWorks2Row => TotalWorks?.Take(2).ToList();

        /// <summary>
        ///     �ݾ���..
        /// </summary>
        public IList<WorkItem> TotalWorksPay => TotalWorks?.Skip(2).Take(1).ToList();

        /// <summary>
        ///     ���� ��
        /// </summary>
        public string BaseMonth
        {
            get => _baseMonth;
            set => Set(ref _baseMonth, value);
        }

        /// <summary>
        ///     SelectionChangedCommand
        /// </summary>
        public ICommand SelectionChangedCommand { get; set; }

        /// <summary>
        ///     �޿� ���� ���
        /// </summary>
        public IList<PayInformation> PayInformations { get; set; }

        public ICommand SaveExcelCommand
        {
            get => _saveExcelCommand;
            set => Set(ref _saveExcelCommand, value);
        }

        public ICommand LoadedCommand { get; private set; }
        public ICommand UnloadedCommand { get; private set; }

        private void Init()
        {
            LoadedCommand = new RelayCommand(OnLoaded);
            UnloadedCommand = new RelayCommand(OnUnloaded);
            SelectionChangedCommand = new RelayCommand(ExecuteSelectionChangedCommand);
            SaveExcelCommand = new RelayCommand(ExecuteSaveExcelCommand);
            BaseMonth = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");

            PropertyChanged += GridViewModel_PropertyChanged;

            CreateMonthData();

            TotalWorks = new List<WorkItem>
            {
                //���� ��
                new WorkItem {Id = 100, Description = "Month Total"},
                //���� �� - ����
                new WorkItem {Id = 101, Description = "Truncation"},
                //���� �ݾ�
                new WorkItem {Id = 102, Description = "Month Pay"}
            };
        }

        private async void ExecuteSaveExcelCommand()
        {
            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("CSV", new List<string> {".csv"});
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "MyPay" + BaseMonth;
            var result = await savePicker.PickSaveFileAsync();
            if (string.IsNullOrEmpty(result?.Name)) return;

            var saveDatas = from item in Works
                let itemText = string.Join(",", item.ToList())
                select itemText;

            var totalDatas = from item in TotalWorks
                let itemText = string.Join(",", item.ToList())
                select itemText;

            try
            {
                await FileIO.WriteTextAsync(result, string.Join("\n", saveDatas.Union(totalDatas)));
                await StaticCommonHelper.ShowMessageBoxAsync("�۾��� �Ϸ� �߽��ϴ�.");
            }
            catch (Exception e)
            {
                await StaticCommonHelper.ShowMessageBoxAsync(e.Message);
            }
        }

        private void ExecuteSelectionChangedCommand()
        {
            var set = (from item in Works
                //���۽ð��� ����ð��� ���� �ٸ� ��쿡��
                where item.StartWork != item.EndWork
                let min10 = item.BasicWorkTime = GetWorkMinute(item.StartWork, item.EndWork, item.IsHoliday, 1.0)
                let min15 = item.OverTime15 = GetWorkMinute(item.StartWork, item.EndWork, item.IsHoliday, 1.5)
                let min20 = item.OverTime20 = GetWorkMinute(item.StartWork, item.EndWork, item.IsHoliday, 2.0)
                let min25 = item.OverTime25 = GetWorkMinute(item.StartWork, item.EndWork, item.IsHoliday, 2.5)
                let total = item.TodayWorkTime = min10 + min15 + min20 + min25
                select item).Count();

            //�� �հ�
            var monthTotal = TotalWorks.First(p => p.Id == 100);
            monthTotal.BasicWorkTime = GetHourMin(Works.Sum(p => p.BasicWorkTime));
            monthTotal.OverTime15 = GetHourMin(Works.Sum(p => p.OverTime15));
            monthTotal.OverTime20 = GetHourMin(Works.Sum(p => p.OverTime20));
            monthTotal.OverTime25 = GetHourMin(Works.Sum(p => p.OverTime25));
            monthTotal.TodayWorkTime = GetHourMin(Works.Sum(p => p.TodayWorkTime));

            //���� ������
            var truncationTotal = TotalWorks.First(p => p.Id == 101);
            truncationTotal.BasicWorkTime = GetHourMin(Works.Sum(p => p.BasicWorkTime), true);
            truncationTotal.OverTime15 = GetHourMin(Works.Sum(p => p.OverTime15), true);
            truncationTotal.OverTime20 = GetHourMin(Works.Sum(p => p.OverTime20), true);
            truncationTotal.OverTime25 = GetHourMin(Works.Sum(p => p.OverTime25), true);
            truncationTotal.TodayWorkTime = GetHourMin(Works.Sum(p => p.TodayWorkTime), true);

            //����
            var pay = (from payTotal in TotalWorks
                where payTotal.Id == 102
                let subSet = (from item in PayInformations
                    let pay15 = Math.Abs(item.Value - 1.5) < 0.1 ? item.TimePay : 0
                    let pay20 = Math.Abs(item.Value - 2.0) < 0.1 ? item.TimePay : 0
                    let pay25 = Math.Abs(item.Value - 2.5) < 0.1 ? item.TimePay : 0
                    select new {Pay15 = pay15, Pay20 = pay20, Pay25 = pay25})
                let maxSubSet = new
                {
                    MaxPay15 = subSet.Max(p => p.Pay15),
                    MaxPay20 = subSet.Max(p => p.Pay20),
                    MaxPay25 = subSet.Max(p => p.Pay25)
                }
                let setPay15 = payTotal.OverTime15 = truncationTotal.OverTime15 * maxSubSet.MaxPay15
                let setPay20 = payTotal.OverTime20 = truncationTotal.OverTime20 * maxSubSet.MaxPay20
                let setPay25 = payTotal.OverTime25 = truncationTotal.OverTime25 * maxSubSet.MaxPay25
                let setTotal = payTotal.TodayWorkTime = setPay15 + setPay20 + setPay25
                select payTotal).Count();
        }

        /// <summary>
        ///     �ð�.�� ���·� ���� ��ȯ
        /// </summary>
        /// <returns></returns>
        private double GetHourMin(double source, bool truncation = false)
        {
            var hour = Convert.ToInt32(source) / 60;
            var min = truncation == false ? source % 60 : 0;
            return double.Parse($"{hour}.{min}");
        }

        /// <summary>
        ///     �� ��ȯ
        /// </summary>
        /// <param name="workStart"></param>
        /// <param name="workEnd"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private double GetWorkMinute(DateTime workStart, DateTime workEnd, bool isHoliday, double value)
        {
            var mins = from item in PayInformations
                //���̰� 0.1������ �༮�鸸
                where Math.Abs(item.Value - value) < 0.1
                      //���� ������ ������ �༮��
                      && item.IsHoliday == isHoliday
                //���۽ð��� ����ð����� ũ�� ����ð��� ���� ���ڷ�
                let workEnd2 = workStart > workEnd ? workEnd.AddDays(1) : workEnd
                //�� ���� �ð��� �ٹ��� ���� �ð����� ����
                let itemStart = DateTime.Parse($"{workStart:yyyy-MM-dd} {item.StartTime:HH:mm}")
                //�� ���� �ð��� �ٹ��� ���� ����ð����� �ӽ� ����
                let itemEndTemp = DateTime.Parse($"{workStart:yyyy-MM-dd} {item.EndTime:HH:mm}")
                //�� ���� �ð��� �� ���� �ð����� ũ�� �� ���� �ð��� �������� ����
                let itemEnd = itemStart > itemEndTemp ? itemEndTemp.AddDays(1) : itemEndTemp
                //��� ���� ���� �����ʹ� ���� �Ÿ�
                let ts = workEnd2 < itemStart || workStart > itemEnd
                    ? TimeSpan.Zero
                    //�ٹ� ���� �ð���, �� ���� �ð����� �۰ų� ����, �ٹ� ����ð���, �� ���� �ð����� ũ�ų� ������
                    : workStart <= itemStart && workEnd2 >= itemEnd
                        //�� ���� �ð����� �� ���� �ð��� ����
                        ? itemEnd.Subtract(itemStart)
                        //�ٹ� ���� �ð���, �� ���� �ð����� ũ��, �ٹ� ���� �ð���, �� ���� �ð����� ũ�ų� ������
                        : workStart > itemStart && workEnd2 >= itemEnd
                            //�� ���� �ð����� �ٹ� ���� �ð��� ����
                            ? itemEnd.Subtract(workStart)
                            //�ٹ� ���� �ð���, �� ���� �ð����� �۰ų� ����, �ٹ� ����ð���, �� ���� �ð����� ������
                            : workStart <= itemStart && workEnd2 < itemEnd
                                //�ٹ� ����ð����� �� ���� �ð��� ����
                                ? workEnd2.Subtract(itemStart)
                                //�ƹ��͵� �ƴϸ� 0
                                : TimeSpan.Zero
                select ts.TotalMinutes;
            return mins.Sum(p => p);
        }

        private void GridViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BaseMonth):
                    CreateMonthData();
                    break;
            }
        }

        /// <summary>
        ///     ������ �⺻ ������ ����
        /// </summary>
        private void CreateMonthData()
        {
            if (string.IsNullOrEmpty(BaseMonth)) return;
            //31���� �����͸� �Է�
            Works = (from num in _monthData
                let dayNullable = GetDate($"{BaseMonth}-{num}")
                where dayNullable != null
                let day = (DateTime) dayNullable
                let week = day.DayOfWeek
                let holiday = week == DayOfWeek.Saturday || week == DayOfWeek.Sunday
                select new WorkItem
                {
                    Id = num,
                    WorkDay = day,
                    IsHoliday = holiday,
                    StartWork = holiday
                        ? DateTime.Parse($"{day:yyyy-MM-dd} 00:00")
                        : DateTime.Parse($"{day:yyyy-MM-dd} 08:00"),
                    EndWork = holiday
                        ? DateTime.Parse($"{day:yyyy-MM-dd} 00:00")
                        : DateTime.Parse($"{day:yyyy-MM-dd} 18:00")
                }).ToList();
        }

        /// <summary>
        ///     ���ڿ� ��¥�� ����ƮŸ�� ������ ��ȯ
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private DateTime? GetDate(string date)
        {
            DateTime returnDate;
            if (DateTime.TryParse(date, out returnDate))
                return returnDate;
            return null;
        }

        private void OnUnloaded()
        {
        }

        private async void OnLoaded()
        {
            //Setting���� ����Ǵ� �ð��� �ʿ��ؼ� .5�� �Ŀ� �۾� ����
            await Task.Delay(500);

            //����� ������ �ҷ�����
            var pays = await ApplicationData.Current.LocalSettings.ReadAsync<IList<PayInformation>>("PayInformations");
            if (pays == null)
            {
                await StaticCommonHelper.ShowMessageBoxAsync("Please create basic data in setting screen");
                return;
            }
            PayInformations = pays;

            ////GetWorkMinute �׽�Ʈ
            //Debug.WriteLine($"1.0 ������: {GetWorkMinute(DateTime.Parse("2017-08-01 08:00"), DateTime.Parse("2017-08-01 02:30"), false, 1.0)}");
            //Debug.WriteLine($"1.5 ������: {GetWorkMinute(DateTime.Parse("2017-08-01 08:00"), DateTime.Parse("2017-08-01 02:30"), false, 1.5)}");
            //Debug.WriteLine($"2.0 ������: {GetWorkMinute(DateTime.Parse("2017-08-01 08:00"), DateTime.Parse("2017-08-01 02:30"), false, 2.0)}");

            //Debug.WriteLine($"1.0 ����: {GetWorkMinute(DateTime.Parse("2017-08-01 08:00"), DateTime.Parse("2017-08-01 18:30"), false, 1.0)}");
            //Debug.WriteLine($"1.5 ����: {GetWorkMinute(DateTime.Parse("2017-08-01 08:00"), DateTime.Parse("2017-08-01 18:30"), false, 1.5)}");
            //Debug.WriteLine($"2.0 ����: {GetWorkMinute(DateTime.Parse("2017-08-01 08:00"), DateTime.Parse("2017-08-01 18:30"), false, 2.0)}");

            //Debug.WriteLine($"1.0 ������: {GetWorkMinute(DateTime.Parse("2017-08-05 08:00"), DateTime.Parse("2017-08-05 02:30"), true, 1.0)}");
            //Debug.WriteLine($"1.5 ������: {GetWorkMinute(DateTime.Parse("2017-08-05 08:00"), DateTime.Parse("2017-08-05 02:30"), true, 1.5)}");
            //Debug.WriteLine($"2.0 ������: {GetWorkMinute(DateTime.Parse("2017-08-05 08:00"), DateTime.Parse("2017-08-05 02:30"), true, 2.0)}");
            //Debug.WriteLine($"2.5 ������: {GetWorkMinute(DateTime.Parse("2017-08-05 08:00"), DateTime.Parse("2017-08-05 02:30"), true, 2.5)}");
        }
    }
}
