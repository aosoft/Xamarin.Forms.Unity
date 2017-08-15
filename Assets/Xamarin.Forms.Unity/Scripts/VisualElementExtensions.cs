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
					childRenderer.Dispose();
					UnityPlatform.SetRenderer(visual, null);
				}
			}

			if (renderer != null)
			{
				renderer.Dispose();
				UnityPlatform.SetRenderer(self, null);
			}
		}
	}
}