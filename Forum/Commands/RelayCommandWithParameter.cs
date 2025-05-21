using Forum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Forum.Commands
{
    public class RelayCommandWithParameter : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// activated when RaisCanExecuteChanged gets called
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// creates new command
        /// </summary>
        /// <param name="execute">action that gets executed</param>
        /// <param name="canExecute">delegate logic that checks if executed</param>
        /// <exception cref="ArgumentNullException"></exception>
        public RelayCommandWithParameter(Action<object> execute, Func<bool> canExecute)
        {
            if (execute == null) throw new ArgumentNullException("execute null");
            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommandWithParameter(Action<object> execute) : this(execute, null)
        {
            _execute = execute;
        }

        

        /// <summary>
        /// chacks if RelayCommand is executeable
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>true can be executed otherwise false</returns>
        public bool CanExecute(object? parameter)
        {
            if (_canExecute == null) return true;
            return _canExecute();
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }


        /// <summary>
        /// for calling the event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                {
                    CanExecuteChanged(this, EventArgs.Empty);
                }
            }
        }
    }
}
