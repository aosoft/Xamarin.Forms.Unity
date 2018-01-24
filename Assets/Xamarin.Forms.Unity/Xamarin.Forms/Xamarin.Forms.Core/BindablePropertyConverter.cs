using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms
{
	[Xaml.ProvideCompiled("Xamarin.Forms.Core.XamlC.BindablePropertyConverter")]
	public sealed class BindablePropertyConverter : TypeConverter, IExtendedTypeConverter
	{
		object IExtendedTypeConverter.ConvertFrom(CultureInfo culture, object value, IServiceProvider serviceProvider)
		{
			return ((IExtendedTypeConverter)this).ConvertFromInvariantString(value as string, serviceProvider);
		}

		object IExtendedTypeConverter.ConvertFromInvariantString(string value, IServiceProvider serviceProvider)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;
			if (serviceProvider == null)
				return null;
			var parentValuesProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideParentValues;
			var typeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
			if (typeResolver == null)
				return null;
			IXmlLineInfo lineinfo = null;
			var xmlLineInfoProvider = serviceProvider.GetService(typeof(IXmlLineInfoProvider)) as IXmlLineInfoProvider;
			if (xmlLineInfoProvider != null)
				lineinfo = xmlLineInfoProvider.XmlLineInfo;
			string[] parts = value.Split('.');
			Type type = null;
			if (parts.Length == 1)
			{
				if (parentValuesProvider == null)
				{
					string msg = string.Format("Can't resolve {0}", parts[0]);
					throw new XamlParseException(msg, lineinfo);
				}
				object parent = parentValuesProvider.ParentObjects.Skip(1).FirstOrDefault();
				if (parentValuesProvider.TargetObject is Setter)
				{
					var style = parent as Style;
					var triggerBase = parent as TriggerBase;
					if (style != null)
						type = style.TargetType;
					else if (triggerBase != null)
						type = triggerBase.TargetType;
				}
				else if (parentValuesProvider.TargetObject is Trigger)
					type = (parentValuesProvider.TargetObject as Trigger).TargetType;
				else if (parentValuesProvider.TargetObject is PropertyCondition && (parent as TriggerBase) != null)
					type = (parent as TriggerBase).TargetType;

				if (type == null)
					throw new XamlParseException($"Can't resolve {parts[0]}", lineinfo);

				return ConvertFrom(type, parts[0], lineinfo);
			}
			if (parts.Length == 2)
			{
				if (!typeResolver.TryResolve(parts[0], out type))
				{
					string msg = string.Format("Can't resolve {0}", parts[0]);
					throw new XamlParseException(msg, lineinfo);
				}
				return ConvertFrom(type, parts[1], lineinfo);
			}
			throw new XamlParseException($"Can't resolve {value}. Syntax is [[prefix:]Type.]PropertyName.", lineinfo);
		}

		public override object ConvertFromInvariantString(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;
			if (value.Contains(":"))
			{
				Log.Warning(null, "Can't resolve properties with xml namespace prefix.");
				return null;
			}
			string[] parts = value.Split('.');
			if (parts.Length != 2)
			{
				Log.Warning(null, $"Can't resolve {value}. Accepted syntax is Type.PropertyName.");
				return null;
			}
			Type type = Type.GetType("Xamarin.Forms." + parts[0]);
			return ConvertFrom(type, parts[1], null);
		}

		private BindableProperty ConvertFrom(Type type, string propertyName, IXmlLineInfo lineinfo)
		{
			string name = propertyName + "Property";
			FieldInfo bpinfo = type.GetField(fi => fi.Name == name && fi.IsStatic && fi.IsPublic && fi.FieldType == typeof(BindableProperty));
			if (bpinfo == null)
				throw new XamlParseException($"Can't resolve {name} on {type.Name}", lineinfo);
			var bp = bpinfo.GetValue(null) as BindableProperty;
			if (bp.PropertyName != propertyName)
				throw new XamlParseException($"The PropertyName of {type.Name}.{name} is not {propertyName}", lineinfo);
			return bp;
		}
	}
}