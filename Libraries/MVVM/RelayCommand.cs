using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace APIUser.Libraries.MVVM
{
    public class RelayCommand : ICommand
    {

        private readonly Func<Object, bool> CanExcute;
        private readonly Action<Object> Excute;
        public RelayCommand(Func<Object, bool> CanExcute, Action<Object> Excute)
        {
            this.CanExcute = CanExcute;
            this.Excute = Excute;
        }
        public bool CanExecute(object parameter) => CanExcute?.Invoke(parameter) ?? true;
        public void Execute(object parameter) => Excute?.Invoke(parameter);
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
