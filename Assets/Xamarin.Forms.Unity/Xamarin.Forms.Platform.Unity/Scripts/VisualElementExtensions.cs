using System;
using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	public static class VisualElementExtensions
	{
		public static IVisualElementRenderer GetOrCreateRenderer(this VisualElement self)
		{
			if (self == null)
				throw new ArgumentNullException("self");

			IVisualElementRenderer renderer = Platform.GetRenderer(self);
			if (renderer == null)
			{
				renderer = Platform.CreateRenderer(self);
				Platform.SetRenderer(self, renderer);
			}

			return renderer;
		}

		internal static void Cleanup(this VisualElement self)
		{
			if (self == null)
				throw new ArgumentNullException("self");

			IVisualElementRenderer renderer = Platform.GetRenderer(self);

			foreach (Element element in self.Descendants())
			{
				var visual = element as VisualElement;
				if (visual == null)
					continue;

				IVisualElementRenderer childRenderer = Platform.GetRenderer(visual);
				if (childRenderer != null)
				{
					var go = childRenderer.UnityComponent?.gameObject;
					if (go != null)
					{
						UnityEngine.Object.Destroy(go);
					}
					Platform.SetRenderer(visual, null);
				}
			}

			if (renderer != null)
			{
				var go = renderer.UnityComponent?.gameObject;
				if (go != null)
				{
					UnityEngine.Object.Destroy(go);
				}
				Platform.SetRenderer(self, null);
			}
		}
	}
}