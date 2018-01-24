using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[ContentProperty("Content")]
	[RenderWith(typeof(_ScrollViewRenderer))]
	public class ScrollView : Layout, IScrollViewController, IElementConfiguration<ScrollView>
	{
		public static readonly BindableProperty OrientationProperty = BindableProperty.Create("Orientation", typeof(ScrollOrientation), typeof(ScrollView), ScrollOrientation.Vertical);

		private static readonly BindablePropertyKey ScrollXPropertyKey = BindableProperty.CreateReadOnly("ScrollX", typeof(double), typeof(ScrollView), 0d);

		public static readonly BindableProperty ScrollXProperty = ScrollXPropertyKey.BindableProperty;

		private static readonly BindablePropertyKey ScrollYPropertyKey = BindableProperty.CreateReadOnly("ScrollY", typeof(double), typeof(ScrollView), 0d);

		public static readonly BindableProperty ScrollYProperty = ScrollYPropertyKey.BindableProperty;

		private static readonly BindablePropertyKey ContentSizePropertyKey = BindableProperty.CreateReadOnly("ContentSize", typeof(Size), typeof(ScrollView), default(Size));

		public static readonly BindableProperty ContentSizeProperty = ContentSizePropertyKey.BindableProperty;

		private readonly Lazy<PlatformConfigurationRegistry<ScrollView>> _platformConfigurationRegistry;

		private View _content;

		private TaskCompletionSource<bool> _scrollCompletionSource;

		public View Content
		{
			get { return _content; }
			set
			{
				if (_content == value)
					return;

				OnPropertyChanging();
				if (_content != null)
					InternalChildren.Remove(_content);
				_content = value;
				if (_content != null)
					InternalChildren.Add(_content);
				OnPropertyChanged();
			}
		}

		public Size ContentSize
		{
			get { return (Size)GetValue(ContentSizeProperty); }
			private set { SetValue(ContentSizePropertyKey, value); }
		}

		public ScrollOrientation Orientation
		{
			get { return (ScrollOrientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}

		public double ScrollX
		{
			get { return (double)GetValue(ScrollXProperty); }
			private set { SetValue(ScrollXPropertyKey, value); }
		}

		public double ScrollY
		{
			get { return (double)GetValue(ScrollYProperty); }
			private set { SetValue(ScrollYPropertyKey, value); }
		}

		public ScrollView()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<ScrollView>>(() => new PlatformConfigurationRegistry<ScrollView>(this));
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public Point GetScrollPositionForElement(VisualElement item, ScrollToPosition pos)
		{
			ScrollToPosition position = pos;
			double y = GetCoordinate(item, "Y", 0);
			double x = GetCoordinate(item, "X", 0);

			if (position == ScrollToPosition.MakeVisible)
			{
				bool isItemVisible = ScrollX < y && ScrollY + Height > y;
				if (isItemVisible)
					return new Point(ScrollX, ScrollY);
				switch (Orientation)
				{
					case ScrollOrientation.Vertical:
						position = y > ScrollY ? ScrollToPosition.End : ScrollToPosition.Start;
						break;

					case ScrollOrientation.Horizontal:
						position = x > ScrollX ? ScrollToPosition.End : ScrollToPosition.Start;
						break;

					case ScrollOrientation.Both:
						position = x > ScrollX || y > ScrollY ? ScrollToPosition.End : ScrollToPosition.Start;
						break;
				}
			}
			switch (position)
			{
				case ScrollToPosition.Center:
					y = y - Height / 2 + item.Height / 2;
					x = x - Width / 2 + item.Width / 2;
					break;

				case ScrollToPosition.End:
					y = y - Height + item.Height;
					x = x - Width + item.Width;
					break;
			}
			return new Point(x, y);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendScrollFinished()
		{
			if (_scrollCompletionSource != null)
				_scrollCompletionSource.TrySetResult(true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetScrolledPosition(double x, double y)
		{
			if (ScrollX == x && ScrollY == y)
				return;

			ScrollX = x;
			ScrollY = y;

			EventHandler<ScrolledEventArgs> handler = Scrolled;
			if (handler != null)
				handler(this, new ScrolledEventArgs(x, y));
		}

		public event EventHandler<ScrolledEventArgs> Scrolled;

		public IPlatformElementConfiguration<T, ScrollView> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		public Task ScrollToAsync(double x, double y, bool animated)
		{
			var args = new ScrollToRequestedEventArgs(x, y, animated);
			OnScrollToRequested(args);
			return _scrollCompletionSource.Task;
		}

		public Task ScrollToAsync(Element element, ScrollToPosition position, bool animated)
		{
			if (!Enum.IsDefined(typeof(ScrollToPosition), position))
				throw new ArgumentException("position is not a valid ScrollToPosition", "position");

			if (element == null)
				throw new ArgumentNullException("element");

			if (!CheckElementBelongsToScrollViewer(element))
				throw new ArgumentException("element does not belong to this ScrollVIew", "element");

			var args = new ScrollToRequestedEventArgs(element, position, animated);
			OnScrollToRequested(args);
			return _scrollCompletionSource.Task;
		}

		protected override void LayoutChildren(double x, double y, double width, double height)
		{
			if (_content != null)
			{
				SizeRequest size;
				switch (Orientation)
				{
					case ScrollOrientation.Horizontal:
						size = _content.Measure(double.PositiveInfinity, height, MeasureFlags.IncludeMargins);
						LayoutChildIntoBoundingRegion(_content, new Rectangle(x, y, GetMaxWidth(width, size), height));
						ContentSize = new Size(GetMaxWidth(width), height);
						break;

					case ScrollOrientation.Vertical:
						size = _content.Measure(width, double.PositiveInfinity, MeasureFlags.IncludeMargins);
						LayoutChildIntoBoundingRegion(_content, new Rectangle(x, y, width, GetMaxHeight(height, size)));
						ContentSize = new Size(width, GetMaxHeight(height));
						break;

					case ScrollOrientation.Both:
						size = _content.Measure(double.PositiveInfinity, double.PositiveInfinity, MeasureFlags.IncludeMargins);
						LayoutChildIntoBoundingRegion(_content, new Rectangle(x, y, GetMaxWidth(width, size), GetMaxHeight(height, size)));
						ContentSize = new Size(GetMaxWidth(width), GetMaxHeight(height));
						break;
				}
			}
		}

		[Obsolete("OnSizeRequest is obsolete as of version 2.2.0. Please use OnMeasure instead.")]
		protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
		{
			if (Content == null)
				return new SizeRequest();

			switch (Orientation)
			{
				case ScrollOrientation.Horizontal:
					widthConstraint = double.PositiveInfinity;
					break;

				case ScrollOrientation.Vertical:
					heightConstraint = double.PositiveInfinity;
					break;

				case ScrollOrientation.Both:
					widthConstraint = double.PositiveInfinity;
					heightConstraint = double.PositiveInfinity;
					break;
			}

			SizeRequest contentRequest = Content.Measure(widthConstraint, heightConstraint, MeasureFlags.IncludeMargins);
			contentRequest.Minimum = new Size(Math.Min(40, contentRequest.Minimum.Width), Math.Min(40, contentRequest.Minimum.Height));
			return contentRequest;
		}

		internal override void ComputeConstraintForView(View view)
		{
			switch (Orientation)
			{
				case ScrollOrientation.Horizontal:
					LayoutOptions vOptions = view.VerticalOptions;
					if (vOptions.Alignment == LayoutAlignment.Fill && (Constraint & LayoutConstraint.VerticallyFixed) != 0)
					{
						view.ComputedConstraint = LayoutConstraint.VerticallyFixed;
					}
					break;

				case ScrollOrientation.Vertical:
					LayoutOptions hOptions = view.HorizontalOptions;
					if (hOptions.Alignment == LayoutAlignment.Fill && (Constraint & LayoutConstraint.HorizontallyFixed) != 0)
					{
						view.ComputedConstraint = LayoutConstraint.HorizontallyFixed;
					}
					break;

				case ScrollOrientation.Both:
					view.ComputedConstraint = LayoutConstraint.None;
					break;
			}
		}

		private bool CheckElementBelongsToScrollViewer(Element element)
		{
			return Equals(element, this) || element.RealParent != null && CheckElementBelongsToScrollViewer(element.RealParent);
		}

		private void CheckTaskCompletionSource()
		{
			if (_scrollCompletionSource != null && _scrollCompletionSource.Task.Status == TaskStatus.Running)
			{
				_scrollCompletionSource.TrySetCanceled();
			}
			_scrollCompletionSource = new TaskCompletionSource<bool>();
		}

		private double GetCoordinate(Element item, string coordinateName, double coordinate)
		{
			if (item == this)
				return coordinate;
			coordinate += (double)typeof(VisualElement).GetProperty(coordinateName).GetValue(item, null);
			var visualParentElement = item.RealParent as VisualElement;
			return visualParentElement != null ? GetCoordinate(visualParentElement, coordinateName, coordinate) : coordinate;
		}

		private double GetMaxHeight(double height)
		{
			return Math.Max(height, _content.Bounds.Top + Padding.Top + _content.Bounds.Bottom + Padding.Bottom);
		}

		private static double GetMaxHeight(double height, SizeRequest size)
		{
			return Math.Max(size.Request.Height, height);
		}

		private double GetMaxWidth(double width)
		{
			return Math.Max(width, _content.Bounds.Left + Padding.Left + _content.Bounds.Right + Padding.Right);
		}

		private static double GetMaxWidth(double width, SizeRequest size)
		{
			return Math.Max(size.Request.Width, width);
		}

		private void OnScrollToRequested(ScrollToRequestedEventArgs e)
		{
			CheckTaskCompletionSource();
			EventHandler<ScrollToRequestedEventArgs> handler = ScrollToRequested;
			if (handler != null)
				handler(this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<ScrollToRequestedEventArgs> ScrollToRequested;
	}
}