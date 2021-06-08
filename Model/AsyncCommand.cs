using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RealtorObjects.Model
{
    public class AsyncCommand : IAsyncCommand
    {
        private readonly Func<Task> command;
        public object Parameter { get; set; }
        public AsyncCommand(Func<Task> command)
        {
            this.command = command;
        }

        public Boolean CanExecute(object parameter) 
        {
            return true;
        }
        public Task ExecuteAsync(object parameter) 
        {
            return command();
        }
        public async void Execute(object parameter)
        {
            Parameter = parameter;
            await ExecuteAsync(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
