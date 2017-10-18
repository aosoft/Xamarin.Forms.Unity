using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	public class VisualElementPackager : IDisposable
	{
		readonly int _column;
		readonly int _columnSpan;

		readonly IVisualElementRenderer _renderer;
		readonly int _row;
		readonly int _rowSpan;
		bool _disposed;
		bool _isLoaded;

		public VisualElementPackager(IVisualElementRenderer renderer)
		{
			if (renderer == null)
				throw new ArgumentNullException("renderer");

			_renderer = renderer;
		}

		public VisualElementPackager(IVisualElementRenderer renderer, int row = 0, int rowSpan = 0, int column = 0, int columnSpan = 0) : this(renderer)
		{
			_row = row;
			_rowSpan = rowSpan;
			_column = column;
			_columnSpan = columnSpan;
		}

		IElementController ElementController => _renderer.Element as IElementController;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			VisualElement element = _renderer.Element;
			if (element != null)
			{
				element.ChildAdded -= OnChildAdded;
				element.ChildRemoved -= OnChildRemoved;
				element.ChildrenReordered -= OnChildrenReordered;
			}
		}

		public void Load()
		{
			if (_isLoaded)
				return;

			_isLoaded = true;
			_renderer.Element.ChildAdded += OnChildAdded;
			_renderer.Element.ChildRemoved += OnChildRemoved;
			_renderer.Element.ChildrenReordered += OnChildrenReordered;

			ReadOnlyCollection<Element> children = ElementController.LogicalChildren;
			for (var i = 0; i < children.Count; i++)
			{
				OnChildAdded(_renderer.Element, new ElementEventArgs(children[i]));
			}
		}

		void EnsureZIndex()
		{
			if (ElementController.LogicalChildren.Count == 0)
				return;

			for (var z = 0; z < ElementController.LogicalChildren.Count; z++)
			{
				var child = ElementController.LogicalChildren[z] as VisualElement;
				if (child == null)
					continue;

				IVisualElementRenderer childRenderer = Platform.GetRenderer(child);

				if (childRenderer == null)
				{
					continue;
				}
				//Debug.Log(string.Format("EnsureZIndex: {0}[{1}] = {2}", this._renderer.UnityRectTransform.GetHashCode(), z, childRenderer.UnityRectTransform.GetHashCode()));
				childRenderer.UnityRectTransform.SetSiblingIndex(z);
			}
		}

		void OnChildAdded(object sender, ElementEventArgs e)
		{
			var view = e.Element as VisualElement;

			if (view == null)
				return;

			IVisualElementRenderer childRenderer = Platform.CreateRenderer(view);
			Platform.SetRenderer(view, childRenderer);
			childRenderer.UnityRectTransform.SetParent(_renderer.UnityContainerTransform);

			EnsureZIndex();
		}

		void OnChildRemoved(object sender, ElementEventArgs e)
		{
			var view = e.Element as VisualElement;

			if (view == null)
				return;

			IVisualElementRenderer childRenderer = Platform.GetRenderer(view);
			if (childRenderer != null)
			{
				childRenderer.UnityRectTransform.SetParent(null);

				view.Cleanup();
			}
		}

		void OnChildrenReordered(object sender, EventArgs e)
		{
			EnsureZIndex();
		}
	}
}