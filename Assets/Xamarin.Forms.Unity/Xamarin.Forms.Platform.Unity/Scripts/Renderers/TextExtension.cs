using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
	internal static class TextExtension
	{
		static public void SetTextColor(
			this UnityEngine.UI.Text self,
			Color color,
			UnityEngine.Color defaultColor)
		{
			if (color != Color.Default)
			{
				self.color = color.ToUnityColor();
			}
			else
			{
				self.color = defaultColor;
			}
		}

		static public void SetFont(
			this UnityEngine.UI.Text self,
			IFontElement element)
		{
			self.fontSize = (int)element.FontSize;
			self.fontStyle = element.FontAttributes.ToUnityFontStyle();
		}

		static public void SetTextAlign(
			this UnityEngine.UI.Text self,
			TextAlignment horizonatal, TextAlignment vertical)
		{
			self.alignment = Platform.ToUnityTextAnchor(horizonatal, vertical);
		}
	}
}
