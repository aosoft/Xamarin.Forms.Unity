using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

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

			var scrollRect = Control;
			if (scrollRect != null)
			{
				var vbar = scrollRect.verticalScrollbar;
				var hbar = scrollRect.horizontalScrollbar;

				vbar?.OnValueChangedAsObservable()
					.BlockReenter()
					.Subscribe(value =>
					{
						var element = Element;
						if (element != null)
						{
							var y = (1.0 - value) * element.ContentSize.Height;
							element.SetScrolledPosition(element.ScrollX, y);

							//Debug.Log(string.Format("Unity: vbar={0} -> XF: ScollView.SetScrolledPosition({1}, {2})", value, element.ScrollX, y));
						}
					});

				hbar?.OnValueChangedAsObservable()
					.BlockReenter()
					.Subscribe(value =>
					{
						var element = Element;
						if (element != null)
						{
							var x = (1.0 - value) * element.ContentSize.Width;
							element.SetScrolledPosition(x, element.ScrollY);

							//Debug.Log(string.Format("Unity: hbar={0} -> XF: ScollView.SetScrolledPosition({1}, {2})", value, x, element.ScrollY));
						}
					});
			}
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region IVisualElementRenderer

		public override Transform UnityContainerTransform => Control?.content;

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
			else if (e.PropertyName == ScrollView.ScrollXProperty.PropertyName)
			{
				UpdateScrollXPosition();
			}
			else if (e.PropertyName == ScrollView.ScrollYProperty.PropertyName)
			{
				UpdateScrollYPosition();
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
			var scroll = Control;
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

		void UpdateScrollXPosition()
		{
			var element = Element;
			var hbar = Control?.horizontalScrollbar;

			if (element != null && hbar != null)
			{
				var size = element.ContentSize;
				if (size.Width > 0.0)
				{
					hbar.value = (float)(1.0f - element.ScrollX / size.Width);
					//Debug.Log(string.Format("XF: {0} -> Unity: hbar.value = {1}", element.ScrollX, hbar.value));
				}
			}
		}

		void UpdateScrollYPosition()
		{
			var element = Element;
			var vbar = Control?.verticalScrollbar;

			if (element != null && vbar != null)
			{
				var size = element.ContentSize;
				if (size.Height > 0.0)
				{
					vbar.value = (float)(1.0f - element.ScrollY / size.Height);
					//Debug.Log(string.Format("XF: {0} -> Unity: vbar.value = {1}", element.ScrollY, vbar.value));
				}
			}
		}

		void UpdateContentSize()
		{
			var element = Element;
			var scrollRect = Control;
			if (element == null || scrollRect == null)
			{
				return;
			}
			var content = scrollRect.content;
			var vbar = scrollRect.verticalScrollbar;
			var hbar = scrollRect.horizontalScrollbar;
			if (content != null)
			{
				var size = element.ContentSize;
				var x = element.ScrollX;
				var y = element.ScrollY;

				var pivot = content.pivot;
				content.anchorMin = new Vector2();
				content.anchorMax = new Vector2();
				content.anchoredPosition = new Vector2();
				content.pivot = new Vector2();

				content.sizeDelta = new Vector2((float)size.Width, (float)size.Height);
				float w = 0.0f;
				float h = 0.0f;
				if (size.Width > 0.0)
				{
					w = (float)(1.0f - x / size.Width);
				}
				else
				{
					hbar = null;
				}
				if (size.Height > 0.0 && vbar != null)
				{
					h = (float)(1.0f - y / size.Height);
				}
				else
				{
					vbar = null;
				}

				Action<Unit> f = _ =>
				{
					if (hbar != null)
					{
						hbar.value = w;
					}
					if (vbar != null)
					{
						vbar.value = h;
					}
				};

				//	ここで設定しても Unity 側で上書きされるので次 Update で設定するのが本命
				f(Unit.Default);
				this.UpdateAsObservable().Take(1).Subscribe(f);
			}
		}

		#endregion
	}
}
