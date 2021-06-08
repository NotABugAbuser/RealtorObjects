using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RealtorObjects.Model
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
