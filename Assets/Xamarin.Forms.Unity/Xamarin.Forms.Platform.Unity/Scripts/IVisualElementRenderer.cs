using System;
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
        /// 自身を保有する GameObject を破棄する
        /// </summary>
        void DestroyObject();
    }
}