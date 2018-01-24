using System;

namespace Xamarin.Forms
{
	public interface ICellController
	{
		event EventHandler ForceUpdateSizeRequested;

		void SendAppearing();

		void SendDisappearing();
	}
}