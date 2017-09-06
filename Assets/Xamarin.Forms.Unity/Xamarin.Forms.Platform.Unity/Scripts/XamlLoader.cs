using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Xamarin.Forms.Platform.Unity
{
	class XamlLoader
	{
		static public void LoadXaml(object view, string xaml)
		{
			var assem = Assembly.GetAssembly(typeof(Xamarin.Forms.Xaml.Extensions));
			var type = assem.GetType("Xamarin.Forms.Xaml.XamlLoader");
			var method = type.GetMethod("Load", new Type[] { typeof(object), typeof(string) });
			method.Invoke(null, new object[] { view, xaml });
		}
	}
}
