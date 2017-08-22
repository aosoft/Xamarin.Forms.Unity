using System;
using Xamarin.Forms.Internals;
using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	public interface IVisualElementRenderer : IRegisterable
	{
		VisualElement Element { get; }

		Component Component { get; }

		event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

		void SetElement(VisualElement element);
	}
}