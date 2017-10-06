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

			_root = this.FindByName<Grid>("_root");
		}

		Grid _root;

		public static readonly BindableProperty InnerContentProperty = BindableProperty.Create(
			nameof(InnerContent),
			typeof(View),
			typeof(XamlPadPage),
			null,
			propertyChanged: (b, o, n) =>
			{
				var t = b as XamlPadPage;
				if (t != null && t._root != null)
				{
					t._root.Children.Add(n as View, 0, 2);
				}
			});

		public View InnerContent
		{
			get { return (View)GetValue(InnerContentProperty); }
			set { SetValue(InnerContentProperty, value); }
		}
	}
}
