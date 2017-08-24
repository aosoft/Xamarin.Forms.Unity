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

			IVisualElementRenderer renderer = UnityPlatform.GetRenderer(self);
			if (renderer == null)
			{
				renderer = UnityPlatform.CreateRenderer(self);
				UnityPlatform.SetRenderer(self, renderer);
			}

			return renderer;
		}

		internal static void Cleanup(this VisualElement self)
		{
			if (self == null)
				throw new ArgumentNullException("self");

			IVisualElementRenderer renderer = UnityPlatform.GetRenderer(self);

			foreach (Element element in self.Descendants())
			{
				var visual = element as VisualElement;
				if (visual == null)
					continue;

				IVisualElementRenderer childRenderer = UnityPlatform.GetRenderer(visual);
				if (childRenderer != null)
				{
					var go = childRenderer.Component?.gameObject;
					if (go != null)
					{
						UnityEngine.Object.Destroy(go);
					}
					UnityPlatform.SetRenderer(visual, null);
				}
			}

			if (renderer != null)
			{
				var go = renderer.Component?.gameObject;
				if (go != null)
				{
					UnityEngine.Object.Destroy(go);
				}
				UnityPlatform.SetRenderer(self, null);
			}
		}
	}
}