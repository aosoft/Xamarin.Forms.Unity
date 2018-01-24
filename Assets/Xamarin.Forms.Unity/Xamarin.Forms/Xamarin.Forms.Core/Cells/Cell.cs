using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
	public abstract class Cell : Element, ICellController
	{
		public const int DefaultCellHeight = 40;
		public static readonly BindableProperty IsEnabledProperty = BindableProperty.Create("IsEnabled", typeof(bool), typeof(Cell), true, propertyChanged: OnIsEnabledPropertyChanged);

		private ObservableCollection<MenuItem> _contextActions;

		private double _height = -1;

		private bool _nextCallToForceUpdateSizeQueued;

		public IList<MenuItem> ContextActions
		{
			get
			{
				if (_contextActions == null)
				{
					_contextActions = new ObservableCollection<MenuItem>();
					_contextActions.CollectionChanged += OnContextActionsChanged;
				}

				return _contextActions;
			}
		}

		public bool HasContextActions
		{
			get { return _contextActions != null && _contextActions.Count > 0 && IsEnabled; }
		}

		public double Height
		{
			get { return _height; }
			set
			{
				if (_height == value)
					return;

				OnPropertyChanging("Height");
				OnPropertyChanging("RenderHeight");
				_height = value;
				OnPropertyChanged("Height");
				OnPropertyChanged("RenderHeight");
			}
		}

		public bool IsEnabled
		{
			get { return (bool)GetValue(IsEnabledProperty); }
			set { SetValue(IsEnabledProperty, value); }
		}

		public double RenderHeight
		{
			get
			{
				var table = RealParent as TableView;
				if (table != null)
					return table.HasUnevenRows && Height > 0 ? Height : table.RowHeight;

				var list = RealParent as ListView;
				if (list != null)
					return list.HasUnevenRows && Height > 0 ? Height : list.RowHeight;

				return DefaultCellHeight;
			}
		}

		public event EventHandler Appearing;

		public event EventHandler Disappearing;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler ForceUpdateSizeRequested;

		public void ForceUpdateSize()
		{
			if (_nextCallToForceUpdateSizeQueued)
				return;

			if ((Parent as ListView)?.HasUnevenRows == true)
			{
				_nextCallToForceUpdateSizeQueued = true;
				OnForceUpdateSizeRequested();
			}
		}

		public event EventHandler Tapped;

		protected internal virtual void OnTapped()
		{
			if (Tapped != null)
				Tapped(this, EventArgs.Empty);
		}

		protected virtual void OnAppearing()
		{
			EventHandler handler = Appearing;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (HasContextActions)
			{
				for (var i = 0; i < _contextActions.Count; i++)
					SetInheritedBindingContext(_contextActions[i], BindingContext);
			}
		}

		protected virtual void OnDisappearing()
		{
			EventHandler handler = Disappearing;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		protected override void OnParentSet()
		{
			if (RealParent != null)
			{
				RealParent.PropertyChanged += OnParentPropertyChanged;
				RealParent.PropertyChanging += OnParentPropertyChanging;
			}

			base.OnParentSet();
		}

		protected override void OnPropertyChanging(string propertyName = null)
		{
			if (propertyName == "Parent")
			{
				if (RealParent != null)
				{
					RealParent.PropertyChanged -= OnParentPropertyChanged;
					RealParent.PropertyChanging -= OnParentPropertyChanging;
				}
			}

			base.OnPropertyChanging(propertyName);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendAppearing()
		{
			OnAppearing();

			var container = RealParent as ListView;
			if (container != null)
				container.SendCellAppearing(this);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendDisappearing()
		{
			OnDisappearing();

			var container = RealParent as ListView;
			if (container != null)
				container.SendCellDisappearing(this);
		}

		private void OnContextActionsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			for (var i = 0; i < _contextActions.Count; i++)
				SetInheritedBindingContext(_contextActions[i], BindingContext);

			OnPropertyChanged("HasContextActions");
		}

		private async void OnForceUpdateSizeRequested()
		{
			// don't run more than once per 16 milliseconds
			await Task.Delay(TimeSpan.FromMilliseconds(16));
			EventHandler handler = ForceUpdateSizeRequested;
			if (handler != null)
				handler(this, null);

			_nextCallToForceUpdateSizeQueued = false;
		}

		private static void OnIsEnabledPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			(bindable as Cell).OnPropertyChanged("HasContextActions");
		}

		private void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// Technically we might be raising this even if it didn't change, but I'm taking the bet that
			// its uncommon enough that we don't want to take the penalty of N GetValue calls to verify.
			if (e.PropertyName == "RowHeight")
				OnPropertyChanged("RenderHeight");
		}

		private void OnParentPropertyChanging(object sender, PropertyChangingEventArgs e)
		{
			if (e.PropertyName == "RowHeight")
				OnPropertyChanging("RenderHeight");
		}
	}
}