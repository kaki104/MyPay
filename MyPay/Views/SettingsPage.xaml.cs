using Windows.UI.Xaml.Controls;
using MyPay.ViewModels;

namespace MyPay.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ���
        /// </summary>
        public SettingsViewModel ViewModel => DataContext as SettingsViewModel;
    }
}
