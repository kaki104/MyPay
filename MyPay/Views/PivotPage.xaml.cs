using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MyPay.ViewModels;

namespace MyPay.Views
{
    public sealed partial class PivotPage : Page
    {
        /// <summary>
        /// »ý¼ºÀÚ
        /// </summary>
        public PivotPage()
        {
            // We use NavigationCacheMode.Required to keep track the selected item on navigation. For further information see the following links.
            // https://msdn.microsoft.com/en-us/library/windows/apps/xaml/windows.ui.xaml.controls.page.navigationcachemode.aspx
            // https://msdn.microsoft.com/en-us/library/windows/apps/xaml/Hh771188.aspx
            NavigationCacheMode = NavigationCacheMode.Required;
            InitializeComponent();
        }
        /// <summary>
        /// ºä¸ðµ¨
        /// </summary>
        public PivotViewModel ViewModel => DataContext as PivotViewModel;
    }
}
