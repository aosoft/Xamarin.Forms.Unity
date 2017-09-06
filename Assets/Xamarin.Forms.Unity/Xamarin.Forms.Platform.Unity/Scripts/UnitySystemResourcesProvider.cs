using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
	internal class UnitySystemResourcesProvider : ISystemResourcesProvider
	{
		ResourceDictionary _dictionary;

		public IResourceDictionary GetSystemResources()
		{
			_dictionary = new ResourceDictionary();

			return _dictionary;
		}
	}
}
