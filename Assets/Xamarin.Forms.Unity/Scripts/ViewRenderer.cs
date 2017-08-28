using System.ComponentModel;
using UnityEngine;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
	public class ViewRenderer<TElement, TNativeElement> :
		VisualElementRenderer<TElement, TNativeElement>
		where TElement : View
		where TNativeElement : MonoBehaviour
	{
		/*-----------------------------------------------------------------*/
		#region Private Field

		UnityEngine.UI.LayoutElement _layoutElement;		


		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		protected void Awake()
		{
			_layoutElement = GetComponent<UnityEngine.UI.LayoutElement>();
			if (_layoutElement == null)
			{
				_layoutElement = this.gameObject.AddComponent<UnityEngine.UI.LayoutElement>();
			}
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TElement> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				UpdateBackgroundColor();
			}
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Internals

		void UpdateLayout()
		{
			var nativeElement = Component;
			var view = Element;
			if (nativeElement == null || view == null)
			{
				return;
			}

		}

		#endregion

	}
}