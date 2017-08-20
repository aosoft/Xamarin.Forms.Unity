using System;
using Xamarin.Forms.Internals;
using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	public interface IVisualElementRenderer : IRegisterable, IDisposable
	{
		VisualElement Element { get; }

		MonoBehaviour Behaviour { get; }

		event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

		void SetElement(VisualElement element);
	}
}