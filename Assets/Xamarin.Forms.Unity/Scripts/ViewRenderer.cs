using UnityEngine;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
	public class ViewRenderer<TElement, TNativeElement> :
		VisualElementRenderer<TElement, TNativeElement>
		where TElement : View
		where TNativeElement : GameObject
	{
		protected override void OnElementChanged(ElementChangedEventArgs<TElement> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				UpdateBackgroundColor();
			}
		}

	}
}