using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	public class LayoutRenderer : ViewRenderer<Layout, UnityEngine.Canvas>
	{
		protected override void UpdateLayout()
		{
			base.UpdateLayout();
			/*if (_rectTransform != null)
			{
				_rectTransform.sizeDelta = new Vector2();
				_rectTransform.anchoredPosition = new Vector2();
			}*/
		}
	}
}
