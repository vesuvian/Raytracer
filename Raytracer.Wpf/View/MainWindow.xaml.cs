using System.ComponentModel;
using Raytracer.Wpf.ViewModel;

namespace Raytracer.Wpf.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindowViewModel ViewModel { get { return (MainWindowViewModel)DataContext; } }

		public MainWindow()
		{
			InitializeComponent();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			ViewModel.Closing();
		}
	}
}
