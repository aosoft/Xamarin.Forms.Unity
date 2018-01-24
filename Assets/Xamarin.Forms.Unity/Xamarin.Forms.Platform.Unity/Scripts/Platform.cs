﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
    public class Platform : IPlatform, IDisposable, INavigation
    {
        /*-----------------------------------------------------------------*/

        #region Private Field

        private PlatformRenderer _renderer;
        private UnityFormsApplicationActivity _activity;
        private Canvas _canvas;

        private Page _currentPage;
        private readonly NavigationModel _navModel = new NavigationModel();

        #endregion Private Field

        /*-----------------------------------------------------------------*/

        #region Constructor / Dispose

        internal Platform(UnityFormsApplicationActivity activity, Canvas canvas)
        {
            _renderer = canvas.gameObject.AddComponent<PlatformRenderer>();
            _activity = activity;
            _canvas = canvas;

            var rectTransform = _canvas.GetComponent<RectTransform>();

            rectTransform.ObserveEveryValueChanged(x => new UniRx.Tuple<float, float>(x.rect.width, x.rect.height))
                .Subscribe(_ =>
                {
                    _currentPage?.Layout(ContainerBounds);
                })
                .AddTo(_canvas);
        }

        public void Dispose()
        {
        }

        #endregion Constructor / Dispose

        /*-----------------------------------------------------------------*/

        #region Property

        internal static readonly BindableProperty RendererProperty = BindableProperty.CreateAttached("Renderer",
            typeof(IVisualElementRenderer), typeof(Platform), default(IVisualElementRenderer));

        internal Rectangle ContainerBounds
        {
            get
            {
                var rt = _canvas?.GetComponent<RectTransform>();
                if (rt != null)
                {
                    return new Rectangle(0.0, 0.0, rt.rect.width, rt.rect.height);
                }
                return new Rectangle();
            }
        }

        #endregion Property

        /*-----------------------------------------------------------------*/

        #region Public Method

        public static IVisualElementRenderer CreateRenderer(VisualElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            IVisualElementRenderer renderer = Internals.Registrar.Registered.GetHandler<IVisualElementRenderer>(element.GetType()) ?? new DefaultRenderer();

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

        private void SetCurrent(Page newPage, bool popping = false, Action completedCallback = null)
        {
            if (newPage == _currentPage)
                return;

            newPage.Platform = this;

            if (_currentPage != null)
            {
                Page previousPage = _currentPage;
                var previousRenderer = GetRenderer(previousPage);
                previousRenderer.UnityRectTransform.SetParent(null);

                if (popping)
                    previousPage.Cleanup();
            }

            //	SetParent をすると pageRenderer の RenderTransform が変わってしまうので
            //	(SetParent しても見栄えが変わらないように維持させるため)
            //	先に SetParent してから Layout で補正する。
            var pageRenderer = newPage.GetOrCreateRenderer();
            pageRenderer.UnityRectTransform.SetParent(_canvas.transform);

            newPage.Layout(ContainerBounds);

            //pageRenderer.ContainerElement.Width = _canvas.Width;
            //pageRenderer.ContainerElement.Height = _canvas.Height;

            completedCallback?.Invoke();

            _currentPage = newPage;

            //UpdateToolbarTracker();
            //await UpdateToolbarItems();
        }

        static public UnityEngine.TextAnchor ToUnityTextAnchor(TextAlignment horizonatal, TextAlignment vertical)
        {
            switch (horizonatal)
            {
                case TextAlignment.Start:
                    switch (vertical)
                    {
                        case TextAlignment.Start: return TextAnchor.UpperLeft;
                        case TextAlignment.Center: return TextAnchor.MiddleLeft;
                        case TextAlignment.End: return TextAnchor.LowerLeft;
                    }
                    break;

                case TextAlignment.Center:
                    switch (vertical)
                    {
                        case TextAlignment.Start: return TextAnchor.UpperCenter;
                        case TextAlignment.Center: return TextAnchor.MiddleCenter;
                        case TextAlignment.End: return TextAnchor.LowerCenter;
                    }
                    break;

                case TextAlignment.End:
                    switch (vertical)
                    {
                        case TextAlignment.Start: return TextAnchor.UpperRight;
                        case TextAlignment.Center: return TextAnchor.MiddleRight;
                        case TextAlignment.End: return TextAnchor.LowerRight;
                    }
                    break;
            }
            return TextAnchor.MiddleCenter;
        }

        #endregion Public Method

        /*-----------------------------------------------------------------*/

        #region IPlatform

        public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
        {
            //	暫定
            var viewRenderer = GetRenderer(view);
            if (viewRenderer == null)
            {
                return new SizeRequest();
            }

            widthConstraint = widthConstraint < 0 ? double.PositiveInfinity : widthConstraint;
            heightConstraint = heightConstraint < 0 ? double.PositiveInfinity : heightConstraint;
            var rawResult = viewRenderer.GetDesiredSize(widthConstraint, heightConstraint);
            if (rawResult.Minimum == Size.Zero)
            {
                rawResult.Minimum = rawResult.Request;
            }
            return rawResult;
        }

        #endregion IPlatform

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

        #endregion INavigation
    }
}