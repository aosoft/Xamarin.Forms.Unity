using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamlPad
{
	public class XamlPadPage : ContentPage
	{
		public XamlPadPage()
		{
			var path = System.IO.Path.Combine(UnityEngine.Application.streamingAssetsPath, "XamlPadPage.xaml");
			var xaml = System.IO.File.ReadAllText(path);
			Xamarin.Forms.Platform.Unity.XamlLoader.LoadXaml(this, xaml);
		}
	}
}
