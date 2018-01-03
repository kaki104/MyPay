using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyPay.Interfaces
{
    /// <summary>
    /// ILoadedUnloaded
    /// </summary>
    public interface ILoadedUnloaded
    {
        ICommand LoadedCommand { get; }
        ICommand UnloadedCommand { get; }
    }
}
