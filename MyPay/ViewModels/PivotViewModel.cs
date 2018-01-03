using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyPay.Helpers;
using MyPay.Interfaces;
using MyPay.Views;

namespace MyPay.ViewModels
{
    /// <summary>
    ///     ÇÇ¹þ ºä¸ðµ¨
    /// </summary>
    public class PivotViewModel : Observable
    {
        private PivotItem _selectedView;
        private bool _showAppBar;

        /// <summary>
        ///     ±âº» »ý¼ºÀÚ
        /// </summary>
        public PivotViewModel()
        {
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != nameof(SelectedView)
                    || SelectedView == null) return;
                ShowAppBar = SelectedView.Content is GridPage;
            };

            SaveExcelCommand = new RelayCommand(ExecuteSaveExcelCommand);
        }

        /// <summary>
        ///     ¾Û¹Ù º¸±â
        /// </summary>
        public bool ShowAppBar
        {
            get => _showAppBar;
            set => Set(ref _showAppBar, value);
        }

        /// <summary>
        ///     ÅÇÀÌ º¯°æµÉ ¶§¸¶´Ù Ä¿¸Çµå ½ÇÇà
        /// </summary>
        public PivotItem SelectedView
        {
            get => _selectedView;
            set
            {
                if (_selectedView == value) return;

                var view = _selectedView?.Content as FrameworkElement;
                var viewModel = view?.DataContext as ILoadedUnloaded;
                viewModel?.UnloadedCommand.Execute(null);
                Set(ref _selectedView, value);

                view = _selectedView?.Content as FrameworkElement;
                viewModel = view?.DataContext as ILoadedUnloaded;
                viewModel?.LoadedCommand.Execute(null);
            }
        }

        /// <summary>
        ///     ¿¢¼¿ ÀúÀå Ä¿¸Çµå
        /// </summary>
        public ICommand SaveExcelCommand { get; set; }

        private void ExecuteSaveExcelCommand()
        {
            var view = SelectedView?.Content as GridPage;
            view?.ViewModel.SaveExcelCommand.Execute(null);
        }
    }
}
