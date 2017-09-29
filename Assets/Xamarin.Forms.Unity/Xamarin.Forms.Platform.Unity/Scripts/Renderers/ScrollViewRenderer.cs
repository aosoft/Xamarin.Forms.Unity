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
	public class ScrollViewRenderer : ViewRenderer<ScrollView, UnityEngine.UI.ScrollRect>
	{
		/*-----------------------------------------------------------------*/
		#region Field

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		protected override void Awake()
		{
			base.Awake();
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region IVisualElementRenderer

		public override Transform UnityContainerTransform => UnityComponent?.content;

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<ScrollView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				UpdateOrientation();
				UpdateContentSize();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == ScrollView.OrientationProperty.PropertyName)
			{
				UpdateOrientation();
			}
			else if (e.PropertyName == ScrollView.ContentSizeProperty.PropertyName)
			{
				UpdateContentSize();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Internals

		void UpdateOrientation()
		{
			var element = Element;
			var scroll = UnityComponent;
			if (element != null)
			{
				switch (element.Orientation)
				{
					case ScrollOrientation.Vertical:
						{
							scroll.horizontal = false;
							scroll.vertical = true;
						}
						break;

					case ScrollOrientation.Horizontal:
						{
							scroll.horizontal = true;
							scroll.vertical = false;
						}
						break;

					case ScrollOrientation.Both:
						{
							scroll.horizontal = true;
							scroll.vertical = true;
						}
						break;
				}
			}
		}

		void UpdateContentSize()
		{
			var element = Element;
			var content = UnityComponent?.content;
			if (element != null && content != null)
			{
				var size = element.ContentSize;

				var pivot = content.pivot;
				content.anchorMin = new Vector2();
				content.anchorMax = new Vector2();
				content.anchoredPosition = new Vector2();
				content.pivot = new Vector2();
				content.sizeDelta = new Vector2((float)size.Width, (float)size.Height);
			}
		}

		#endregion
	}
}
