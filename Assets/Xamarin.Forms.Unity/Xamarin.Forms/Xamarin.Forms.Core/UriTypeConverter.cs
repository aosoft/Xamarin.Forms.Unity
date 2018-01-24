﻿using System;

namespace Xamarin.Forms
{
	[Xaml.ProvideCompiled("Xamarin.Forms.Core.XamlC.UriTypeConverter")]
	public class UriTypeConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;
			return new Uri(value, UriKind.RelativeOrAbsolute);
		}

		private bool CanConvert(Type type)
		{
			if (type == typeof(string))
				return true;
			if (type == typeof(Uri))
				return true;

			return false;
		}
	}
}