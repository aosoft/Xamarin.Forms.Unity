using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
	internal struct TextTracker
	{
		UnityEngine.UI.Text _componentText;
		UnityEngine.Color _defaultTextColor;

		public TextTracker(UnityEngine.UI.Text componentText)
		{
			//	初期値がデフォルトカラー

			_componentText = componentText;
			_defaultTextColor = componentText != null ? componentText.color : new UnityEngine.Color();
		}

		public void UpdateText(string text)
		{
			if (_componentText != null)
			{
				_componentText.text = text;
			}
		}

		public void UpdateTextColor(Color color)
		{
			_componentText?.SetTextColor(color, _defaultTextColor);
		}

		public void UpdateFont(IFontElement element)
		{
			_componentText?.SetFont(element);
		}
	}
}
