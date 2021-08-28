using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Raytracer.Wpf.Utils
{
	public sealed class RelayCommand : ICommand
	{
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		private readonly Action<object> m_Execute;
		private readonly Predicate<object> m_CanExecute;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="execute"></param>
		/// <param name="canExecute"></param>
		public RelayCommand(Action execute, Predicate<object> canExecute = null)
			: this(o => execute(), canExecute)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="execute"></param>
		/// <param name="canExecute"></param>
		public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
		{
			m_Execute = execute ?? throw new ArgumentNullException("execute");
			m_CanExecute = canExecute;
		}

		#endregion

		#region ICommand Members

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return m_CanExecute == null || m_CanExecute(parameter);
		}

		public void Execute(object parameter)
		{
			m_Execute(parameter);
		}

		#endregion
	}
}
