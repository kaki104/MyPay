using System;
using MyPay.Helpers;

namespace MyPay.Models
{
    /// <summary>
    ///     급여 정보
    /// </summary>
    public class PayInformation : Observable
    {
        private DateTime _endTime;
        private DateTime _startTime;
        private int _timePay;


        /// <summary>
        ///     아이디
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     값
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        ///     휴일 유무
        /// </summary>
        public bool IsHoliday { get; set; }

        /// <summary>
        ///     시작시간
        /// </summary>
        public DateTime StartTime
        {
            get => _startTime;
            set => Set(ref _startTime, value);
        }

        /// <summary>
        ///     종료시간
        /// </summary>
        public DateTime EndTime
        {
            get => _endTime;
            set => Set(ref _endTime, value);
        }

        /// <summary>
        ///     시급
        /// </summary>
        public int TimePay
        {
            get => _timePay;
            set => Set(ref _timePay, value);
        }
    }
}
