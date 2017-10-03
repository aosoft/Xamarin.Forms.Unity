using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	public class PageRenderer : VisualElementRenderer<Page, UnityEngine.Canvas>
	{
		public PageRenderer()
		{
		}

		protected override void UpdateNativeControl()
		{
			base.UpdateNativeControl();

			var renderTransform = GetComponent<RectTransform>();
			if (renderTransform != null)
			{
				renderTransform.anchorMin = new Vector2(0.0f, 0.0f);
				renderTransform.anchorMax = new Vector2(0.0f, 0.0f);
			}
		}
	}
}
