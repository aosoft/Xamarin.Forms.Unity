using System;
using System.Collections.ObjectModel;

namespace Xamarin.Forms.Platform.Unity
{
    public class VisualElementPackager : IDisposable
    {
        private readonly int _column;
        private readonly int _columnSpan;

        private readonly IVisualElementRenderer _renderer;
        private readonly int _row;
        private readonly int _rowSpan;
        private bool _disposed;
        private bool _isLoaded;

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

        private IElementController ElementController => _renderer.Element as IElementController;

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

        private void EnsureZIndex()
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

        private void OnChildAdded(object sender, ElementEventArgs e)
        {
            var view = e.Element as VisualElement;

            if (view == null)
                return;

            IVisualElementRenderer childRenderer = Platform.CreateRenderer(view);
            Platform.SetRenderer(view, childRenderer);
            childRenderer.UnityRectTransform.SetParent(_renderer.UnityContainerTransform);

            EnsureZIndex();
        }

        private void OnChildRemoved(object sender, ElementEventArgs e)
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

        private void OnChildrenReordered(object sender, EventArgs e)
        {
            EnsureZIndex();
        }
    }
}