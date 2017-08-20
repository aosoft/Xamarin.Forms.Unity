using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	public class LabelRenderer : VisualElementRenderer<Label, UnityEngine.UI.Text>
	{
		public LabelRenderer()
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var elem = new System.Windows.Forms.Label();
					elem.Anchor =
						System.Windows.Forms.AnchorStyles.Left |
						System.Windows.Forms.AnchorStyles.Top |
						System.Windows.Forms.AnchorStyles.Right |
						System.Windows.Forms.AnchorStyles.Bottom;
					SetNativeControl(elem);
				}

				//_isInitiallyDefault = Element.IsDefault();

				UpdateText(Control);
				/*UpdateColor(Control);
				UpdateAlign(Control);
				UpdateFont(Control);
				UpdateLineBreakMode(Control);*/
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Label.TextProperty.PropertyName || e.PropertyName == Label.FormattedTextProperty.PropertyName)
				UpdateText(Control);

			base.OnElementPropertyChanged(sender, e);
		}

		void UpdateText(System.Windows.Forms.Label nativeElement)
		{
			//_perfectSizeValid = false;

			if (nativeElement == null)
				return;

			Label label = Element;
			if (label != null)
			{
				nativeElement.Text = label.Text;
			}
		}
	}
}
