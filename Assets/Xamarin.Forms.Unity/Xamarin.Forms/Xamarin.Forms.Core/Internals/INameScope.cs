using System;
using System.ComponentModel;
using System.Xml;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface INameScope
	{
		object FindByName(string name);

		void RegisterName(string name, object scopedElement);

		void UnregisterName(string name);

		[Obsolete] void RegisterName(string name, object scopedElement, IXmlLineInfo xmlLineInfo);
	}
}