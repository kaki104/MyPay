using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace MyPay.Helpers
{
    public static class StaticCommonHelper
    {
        public static async Task ShowMessageBoxAsync(string message)
        {
            var messageBox = new MessageDialog(message);
            await messageBox.ShowAsync();
        }
    }
}
