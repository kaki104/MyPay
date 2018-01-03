using System;
using System.Collections.Generic;
using MyPay.Helpers;

namespace MyPay.Models
{
    /// <summary>
    ///     근무 아이템, Observable은 프로퍼티 체인지 이벤트를 발생 시키기 위해서 상속받음
    /// </summary>
    public class WorkItem : Observable
    {
        private double _basicWorkTime;
        private string _description;
        private double _overTime15;
        private double _overTime20;
        private double _overTime25;
        private double _todayWorkTime;

        /// <summary>
        ///     아이디
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     근무일
        /// </summary>
        public DateTime WorkDay { get; set; }

        /// <summary>
        ///     휴일여부
        /// </summary>
        public bool IsHoliday { get; set; }

        /// <summary>
        ///     시작시간
        /// </summary>
        public DateTime StartWork { get; set; }

        /// <summary>
        ///     종료시간
        /// </summary>
        public DateTime EndWork { get; set; }

        /// <summary>
        ///     기본 근무 시간 1.0
        /// </summary>
        public double BasicWorkTime
        {
            get => _basicWorkTime;
            set => Set(ref _basicWorkTime, value);
        }

        /// <summary>
        ///     1.5 근무 시간
        /// </summary>
        public double OverTime15
        {
            get => _overTime15;
            set => Set(ref _overTime15, value);
        }

        /// <summary>
        ///     2.0 근무 시간
        /// </summary>
        public double OverTime20
        {
            get => _overTime20;
            set => Set(ref _overTime20, value);
        }

        /// <summary>
        ///     2.5 근무 시간
        /// </summary>
        public double OverTime25
        {
            get => _overTime25;
            set => Set(ref _overTime25, value);
        }

        /// <summary>
        ///     총 근무 시간
        /// </summary>
        public double TodayWorkTime
        {
            get => _todayWorkTime;
            set => Set(ref _todayWorkTime, value);
        }
        /// <summary>
        /// 설명
        /// </summary>
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        public IList<object> ToList()
        {
            return new List<object>
            {
                Id,
                WorkDay.ToString("yyyy-MM-dd"),
                IsHoliday,
                StartWork.ToString("yyyy-MM-dd HH:mm"),
                EndWork.ToString("yyyy-MM-dd HH:mm"),
                BasicWorkTime,
                OverTime15,
                OverTime20,
                OverTime25,
                TodayWorkTime,
                Description
            };  
        }
    }
}
