using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using MyPay.ViewModels;

namespace MyPay.Views
{
    /// <summary>
    ///     그리드 페이지
    /// </summary>
    public sealed partial class GridPage : Page
    {
        public GridPage()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     뷰모델
        /// </summary>
        public GridViewModel ViewModel => DataContext as GridViewModel;

    }
}
