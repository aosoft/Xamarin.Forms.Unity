using System;
using Xamarin.Forms.Internals;
using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	public interface IVisualElementRenderer : IRegisterable
	{
		/// <summary>
		/// Xamarin.Forms の VisualElement
		/// </summary>
		VisualElement Element { get; }

		/// <summary>
		/// Unity 側で対応する Component (Native Element)
		/// </summary>
		Component UnityComponent { get; }

		/// <summary>
		/// Unity 側の RectTransform コンポーネント
		/// </summary>
		RectTransform UnityRectTransform { get; }

		/// <summary>
		/// Unity Component (GameObject) の親となるコンテナ。
		/// 子はこの Transform を SetParent する。
		/// 通常は UnityComponent.transform 。
		/// </summary>
		Transform UnityContainerTransform { get; }

		event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

		void SetElement(VisualElement element);

		/// <summary>
		/// 自身をベースとした指定の VisualElementRenderer の AnchorPoint を取得する。
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		Vector2 GetChildAnchorPoint(IVisualElementRenderer child);
	}
}