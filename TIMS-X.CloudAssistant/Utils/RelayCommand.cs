using System;
using System.Windows.Input;

namespace TIMS_X.CloudAssistant.Utils
{
	public class RelayCommand : ICommand
	{
		private readonly Func<bool> canExecuteEvaluator;
		private readonly Action methodToExecute;

		public RelayCommand(Action methodToExecute, Func<bool> canExecuteEvaluator)
		{
			this.methodToExecute = methodToExecute;
			this.canExecuteEvaluator = canExecuteEvaluator;
		}

		public RelayCommand(Action methodToExecute)
			: this(methodToExecute, null)
		{
		}

		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}


		public bool CanExecute(object parameter)
		{
			if (canExecuteEvaluator == null)
			{
				return true;
			}

			var result = canExecuteEvaluator.Invoke();
			return result;
		}

		public void Execute(object parameter)
		{
			methodToExecute.Invoke();
		}

		public void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}
	}

	public class RelayCommand<T> : ICommand
	{
		#region Fields

		private readonly Action<T> _execute;
		private readonly Predicate<T> _canExecute;

		#endregion

		#region Constructors

		/// <summary>
		///     Creates a new command that can always execute.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		public RelayCommand(Action<T> execute)
			: this(execute, null)
		{
		}

		/// <summary>
		///     Creates a new command with conditional execution.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public RelayCommand(Action<T> execute, Predicate<T> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}

		#endregion

		#region ICommand Members

		public bool CanExecute(object parameter)
		{
			if (parameter == null)
				return false;
			return _canExecute?.Invoke((T)parameter) ?? true;
		}

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

		public void Execute(object parameter)
		{
			_execute((T)parameter);
		}

		#endregion
	}
}