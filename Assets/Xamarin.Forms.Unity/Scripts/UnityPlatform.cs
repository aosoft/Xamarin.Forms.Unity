using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	public class UnityPlatform : IPlatform, IDisposable
	{
		/*-----------------------------------------------------------------*/
		#region Private Field

		Canvas _container;
		Page _currentPage;
		readonly NavigationModel _navModel = new NavigationModel();

		#endregion

		/*-----------------------------------------------------------------*/
		#region Constructor / Dispose

		internal UnityPlatform(Canvas container)
		{
			_container = container;
		}

		public void Dispose()
		{
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Property

		internal static readonly BindableProperty RendererProperty = BindableProperty.CreateAttached("Renderer",
			typeof(IVisualElementRenderer), typeof(UnityPlatform), default(IVisualElementRenderer));



		#endregion

		/*-----------------------------------------------------------------*/
		#region Public Method

		public static IVisualElementRenderer CreateRenderer(VisualElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			IVisualElementRenderer renderer = Registrar.Registered.GetHandler<IVisualElementRenderer>(element.GetType()) ?? new DefaultRenderer();
			renderer.SetElement(element);
			return renderer;
		}

		public static IVisualElementRenderer GetRenderer(VisualElement element)
		{
			return (IVisualElementRenderer)element.GetValue(RendererProperty);
		}

		public static void SetRenderer(VisualElement element, IVisualElementRenderer value)
		{
			element.SetValue(RendererProperty, value);
			element.IsPlatformEnabled = value != null;
		}

		internal void SetPage(Page newRoot)
		{
			if (newRoot == null)
				throw new ArgumentNullException(nameof(newRoot));

			_navModel.Clear();

			_navModel.Push(newRoot, null);
			SetCurrent(newRoot, true);
			Application.Current.NavigationProxy.Inner = this;
		}

		/*async*/
		void SetCurrent(Page newPage, bool popping = false, Action completedCallback = null)
		{
			if (newPage == _currentPage)
				return;

			newPage.Platform = this;

			if (_currentPage != null)
			{
				Page previousPage = _currentPage;
				IVisualElementRenderer previousRenderer = GetRenderer(previousPage);
				_container.Controls.Remove(previousRenderer.ContainerElement);

				if (popping)
					previousPage.Cleanup();
			}

			newPage.Layout(ContainerBounds);

			IVisualElementRenderer pageRenderer = newPage.GetOrCreateRenderer();
			_container.Controls.Add(pageRenderer.ContainerElement);

			pageRenderer.ContainerElement.Width = _container.Width;
			pageRenderer.ContainerElement.Height = _container.Height;

			completedCallback?.Invoke();

			_currentPage = newPage;

			//UpdateToolbarTracker();
			//await UpdateToolbarItems();
		}


		#endregion

		/*-----------------------------------------------------------------*/
		#region IPlatform

		public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			throw new NotImplementedException();
		}
		
		#endregion
	}
}
