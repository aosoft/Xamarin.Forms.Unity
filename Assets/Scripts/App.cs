using Xamarin.Forms;

public class App : Application
{
	public App()
	{
		/*var page = new ContentPage();
		var label = new Label();
		label.Text = "Hello, Xaramin.Forms!!";

		page.Content = label;

		MainPage = page;*/
		MainPage = new Page1();
	}

	protected override void OnStart()
	{
		base.OnStart();
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
