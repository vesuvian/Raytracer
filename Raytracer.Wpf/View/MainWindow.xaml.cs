using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;
using Raytracer.Wpf.ViewModel;
using Point = System.Windows.Point;

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

		private void Image_MouseMove(object sender, MouseEventArgs e)
		{
			if (!FloatingTip.IsOpen)
				FloatingTip.IsOpen = true;

			UpdateTooltip();
		}

		private void Image_MouseLeave(object sender, MouseEventArgs e)
		{
			FloatingTip.IsOpen = false;
		}

		private void UpdateTooltip()
		{
			Point currentPos = Mouse.GetPosition(Image);

			FloatingTip.HorizontalOffset = currentPos.X + 20;
			FloatingTip.VerticalOffset = currentPos.Y;

			double width = Image.ActualWidth;
			double height = Image.ActualHeight;

			int bitmapX = (int)(currentPos.X / width * ViewModel.Bitmap.Width);
			int bitmapY = (int)(currentPos.Y / height * ViewModel.Bitmap.Height);

			Color pixel = ViewModel.Buffer.GetPixel(bitmapX, bitmapY);

			FloatingTipLabel.Text =
				$"(x={bitmapX}, y={bitmapY})\n" +
				$"(r={pixel.R}, g={pixel.G}, b={pixel.B})";
		}
	}
}
