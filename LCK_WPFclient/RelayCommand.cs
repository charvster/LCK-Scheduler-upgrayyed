using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Input;

namespace LCK_WPFclient
{
    [Serializable]
    public class RelayCommand : ICommand
    {

        #region Declarations

        readonly Func<Boolean> _canExecute;
        readonly Action _execute;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class and the command can always be executed.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Func<Boolean> canExecute)
        {

            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        [DebuggerStepThrough]
        public Boolean CanExecute(Object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public void Execute(Object parameter)
        {
            _execute();
        }

        #endregion
    }

    [Serializable]
    public class RelayCommand<T> : ICommand
    {

        #region Fields

        readonly Action<T> _execute = null;
        readonly Predicate<T> _canExecute = null;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {

        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)

                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute
         = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            { CommandManager.RequerySuggested += value; }

            remove { CommandManager.RequerySuggested -= value; }

        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        #endregion // ICommand Members

    }

}
