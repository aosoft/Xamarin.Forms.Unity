using Xamarin.Forms;

namespace XamlPad
{
	public class App : Application
	{
		public App()
		{
			MainPage = new XamlPadPage();
			MainPage.BindingContext = new XamlPadBindingContext(Xamarin.Forms.Platform.Unity.Forms.Activity);
		}

		protected override void OnStart()
		{
			base.OnStart();
			(MainPage.BindingContext as XamlPadBindingContext)?.InitializePropertyValues();
		}

		protected override void OnSleep()
		{
			base.OnSleep();
		}

		protected override void OnResume()
		{
			base.OnResume();
		}
	}
}