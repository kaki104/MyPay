using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using MyPay.ViewModels;

namespace MyPay.Views
{
    /// <summary>
    ///     �׸��� ������
    /// </summary>
    public sealed partial class GridPage : Page
    {
        public GridPage()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     ���
        /// </summary>
        public GridViewModel ViewModel => DataContext as GridViewModel;

    }
}
