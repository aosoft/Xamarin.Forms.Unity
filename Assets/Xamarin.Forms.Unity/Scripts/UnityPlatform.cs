using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	public class UnityPlatform : IPlatform, IDisposable, INavigation
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

		//	TODO: Unity Canvas から確定する必要がある
		internal Rectangle ContainerBounds
		{
			get;
			set;
		}

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
				var previousRenderer = GetRenderer(previousPage);
				previousRenderer.ContainerElement.transform.parent = null;

				if (popping)
					previousPage.Cleanup();
			}

			newPage.Layout(ContainerBounds);

			var pageRenderer = newPage.GetOrCreateRenderer();
			pageRenderer.ContainerElement.transform.parent = _container.transform;

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

		/*-----------------------------------------------------------------*/
		#region INavigation

		public IReadOnlyList<Page> ModalStack
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IReadOnlyList<Page> NavigationStack
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void InsertPageBefore(Page page, Page before)
		{
			throw new NotImplementedException();
		}

		public Task<Page> PopAsync()
		{
			throw new NotImplementedException();
		}

		public Task<Page> PopAsync(bool animated)
		{
			throw new NotImplementedException();
		}

		public Task<Page> PopModalAsync()
		{
			throw new NotImplementedException();
		}

		public Task<Page> PopModalAsync(bool animated)
		{
			throw new NotImplementedException();
		}

		public Task PopToRootAsync()
		{
			throw new NotImplementedException();
		}

		public Task PopToRootAsync(bool animated)
		{
			throw new NotImplementedException();
		}

		public Task PushAsync(Page page)
		{
			throw new NotImplementedException();
		}

		public Task PushAsync(Page page, bool animated)
		{
			throw new NotImplementedException();
		}

		public Task PushModalAsync(Page page)
		{
			throw new NotImplementedException();
		}

		public Task PushModalAsync(Page page, bool animated)
		{
			throw new NotImplementedException();
		}

		public void RemovePage(Page page)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
