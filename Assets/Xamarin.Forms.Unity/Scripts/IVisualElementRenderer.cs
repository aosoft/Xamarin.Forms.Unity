using System;
using Xamarin.Forms.Internals;
using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	public interface IVisualElementRenderer : IRegisterable, IDisposable
	{
		GameObject ContainerElement { get; }

		VisualElement Element { get; }

		event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

		void SetElement(VisualElement element);

		GameObject GetNativeElement();
	}
}