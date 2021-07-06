using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Raytracer.Wpf.ViewModel
{
	public abstract class AbstractViewModel : IViewModel
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
