using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	public class VisualElementPackager : IDisposable
	{
		readonly int _column;
		readonly int _columnSpan;

		readonly MonoBehaviour _monoBehaviour;
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

			_monoBehaviour = renderer.ContainerElement as MonoBehaviour;
			if (_monoBehaviour == null)
				throw new ArgumentException("Renderer's container element must be a Panel");
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
			/*if (ElementController.LogicalChildren.Count == 0)
				return;

			for (var z = 0; z < ElementController.LogicalChildren.Count; z++)
			{
				var child = ElementController.LogicalChildren[z] as VisualElement;
				if (child == null)
					continue;

				IVisualElementRenderer childRenderer = WinFormsPlatform.GetRenderer(child);

				if (childRenderer == null)
				{
					continue;
				}

				Canvas.SetZIndex(childRenderer.ContainerElement, z + 1);
			}*/
		}

		void OnChildAdded(object sender, ElementEventArgs e)
		{
			var view = e.Element as VisualElement;

			if (view == null)
				return;

			IVisualElementRenderer childRenderer = WinFormsPlatform.CreateRenderer(view);
			WinFormsPlatform.SetRenderer(view, childRenderer);

			/*
			if (_row > 0)
				Windows.UI.Xaml.Controls.Grid.SetRow(childRenderer.ContainerElement, _row);
			if (_rowSpan > 0)
				Windows.UI.Xaml.Controls.Grid.SetRowSpan(childRenderer.ContainerElement, _rowSpan);
			if (_column > 0)
				Windows.UI.Xaml.Controls.Grid.SetColumn(childRenderer.ContainerElement, _column);
			if (_columnSpan > 0)
				Windows.UI.Xaml.Controls.Grid.SetColumnSpan(childRenderer.ContainerElement, _columnSpan);
			*/

			_monoBehaviour.Controls.Add(childRenderer.ContainerElement);

			EnsureZIndex();
		}

		void OnChildRemoved(object sender, ElementEventArgs e)
		{
			var view = e.Element as VisualElement;

			if (view == null)
				return;

			IVisualElementRenderer childRenderer = WinFormsPlatform.GetRenderer(view);
			if (childRenderer != null)
			{
				/*
				if (_row > 0)
					childRenderer.ContainerElement.ClearValue(Windows.UI.Xaml.Controls.Grid.RowProperty);
				if (_rowSpan > 0)
					childRenderer.ContainerElement.ClearValue(Windows.UI.Xaml.Controls.Grid.RowSpanProperty);
				if (_column > 0)
					childRenderer.ContainerElement.ClearValue(Windows.UI.Xaml.Controls.Grid.ColumnProperty);
				if (_columnSpan > 0)
					childRenderer.ContainerElement.ClearValue(Windows.UI.Xaml.Controls.Grid.ColumnSpanProperty);
				*/
				_monoBehaviour.Controls.Remove(childRenderer.ContainerElement);

				view.Cleanup();
			}
		}

		void OnChildrenReordered(object sender, EventArgs e)
		{
			EnsureZIndex();
		}
	}
}