using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientXO
{
    class DelegateCommand : ICommand
    {
        private Func<object, bool> canExecute;
        private Action execute;


        public DelegateCommand(Action execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        public DelegateCommand(Action execute)
        {
            this.execute = execute;
            canExecute = new Func<object, bool>((parameter) => { return true; });
        }


        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }
        public void Execute(object parameter) => execute();


        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
