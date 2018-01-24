using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_PickerRenderer))]
	public class Picker : View, ITextElement, IElementConfiguration<Picker>
	{
		public static readonly BindableProperty TextColorProperty = TextElement.TextColorProperty;

		public static readonly BindableProperty TitleProperty =
			BindableProperty.Create(nameof(Title), typeof(string), typeof(Picker), default(string));

		public static readonly BindableProperty SelectedIndexProperty =
			BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(Picker), -1, BindingMode.TwoWay,
									propertyChanged: OnSelectedIndexChanged, coerceValue: CoerceSelectedIndex);

		public static readonly BindableProperty ItemsSourceProperty =
			BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(Picker), default(IList),
									propertyChanged: OnItemsSourceChanged);

		public static readonly BindableProperty SelectedItemProperty =
			BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(Picker), null, BindingMode.TwoWay,
									propertyChanged: OnSelectedItemChanged);

		private readonly Lazy<PlatformConfigurationRegistry<Picker>> _platformConfigurationRegistry;

		public Picker()
		{
			((INotifyCollectionChanged)Items).CollectionChanged += OnItemsCollectionChanged;
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<Picker>>(() => new PlatformConfigurationRegistry<Picker>(this));
		}

		public IList<string> Items { get; } = new LockableObservableListWrapper();

		public IList ItemsSource
		{
			get { return (IList)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public int SelectedIndex
		{
			get { return (int)GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}

		public object SelectedItem
		{
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public Color TextColor
		{
			get { return (Color)GetValue(TextElement.TextColorProperty); }
			set { SetValue(TextElement.TextColorProperty, value); }
		}

		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		private BindingBase _itemDisplayBinding;

		public BindingBase ItemDisplayBinding
		{
			get { return _itemDisplayBinding; }
			set
			{
				if (_itemDisplayBinding == value)
					return;

				OnPropertyChanging();
				var oldValue = value;
				_itemDisplayBinding = value;
				OnItemDisplayBindingChanged(oldValue, _itemDisplayBinding);
				OnPropertyChanged();
			}
		}

		public event EventHandler SelectedIndexChanged;

		private static readonly BindableProperty s_displayProperty =
			BindableProperty.Create("Display", typeof(string), typeof(Picker), default(string));

		private string GetDisplayMember(object item)
		{
			if (ItemDisplayBinding == null)
				return item.ToString();

			ItemDisplayBinding.Apply(item, this, s_displayProperty);
			ItemDisplayBinding.Unapply();
			return (string)GetValue(s_displayProperty);
		}

		private static object CoerceSelectedIndex(BindableObject bindable, object value)
		{
			var picker = (Picker)bindable;
			return picker.Items == null ? -1 : ((int)value).Clamp(-1, picker.Items.Count - 1);
		}

		private void OnItemDisplayBindingChanged(BindingBase oldValue, BindingBase newValue)
		{
			ResetItems();
		}

		private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			SelectedIndex = SelectedIndex.Clamp(-1, Items.Count - 1);
			UpdateSelectedItem();
		}

		private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
		{
			((Picker)bindable).OnItemsSourceChanged((IList)oldValue, (IList)newValue);
		}

		private void OnItemsSourceChanged(IList oldValue, IList newValue)
		{
			var oldObservable = oldValue as INotifyCollectionChanged;
			if (oldObservable != null)
				oldObservable.CollectionChanged -= CollectionChanged;

			var newObservable = newValue as INotifyCollectionChanged;
			if (newObservable != null)
			{
				newObservable.CollectionChanged += CollectionChanged;
			}

			if (newValue != null)
			{
				((LockableObservableListWrapper)Items).IsLocked = true;
				ResetItems();
			}
			else
			{
				((LockableObservableListWrapper)Items).InternalClear();
				((LockableObservableListWrapper)Items).IsLocked = false;
			}
		}

		private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					AddItems(e);
					break;

				case NotifyCollectionChangedAction.Remove:
					RemoveItems(e);
					break;

				default: //Move, Replace, Reset
					ResetItems();
					break;
			}
		}

		private void AddItems(NotifyCollectionChangedEventArgs e)
		{
			int index = e.NewStartingIndex < 0 ? Items.Count : e.NewStartingIndex;
			foreach (object newItem in e.NewItems)
				((LockableObservableListWrapper)Items).InternalInsert(index++, GetDisplayMember(newItem));
		}

		private void RemoveItems(NotifyCollectionChangedEventArgs e)
		{
			int index = e.OldStartingIndex < Items.Count ? e.OldStartingIndex : Items.Count;
			foreach (object _ in e.OldItems)
				((LockableObservableListWrapper)Items).InternalRemoveAt(index--);
		}

		private void ResetItems()
		{
			if (ItemsSource == null)
				return;
			((LockableObservableListWrapper)Items).InternalClear();
			foreach (object item in ItemsSource)
				((LockableObservableListWrapper)Items).InternalAdd(GetDisplayMember(item));
			UpdateSelectedItem();
		}

		private static void OnSelectedIndexChanged(object bindable, object oldValue, object newValue)
		{
			var picker = (Picker)bindable;
			picker.UpdateSelectedItem();
			picker.SelectedIndexChanged?.Invoke(bindable, EventArgs.Empty);
		}

		private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var picker = (Picker)bindable;
			picker.UpdateSelectedIndex(newValue);
		}

		private void UpdateSelectedIndex(object selectedItem)
		{
			if (ItemsSource != null)
			{
				SelectedIndex = ItemsSource.IndexOf(selectedItem);
				return;
			}
			SelectedIndex = Items.IndexOf(selectedItem);
		}

		private void UpdateSelectedItem()
		{
			if (SelectedIndex == -1)
			{
				SelectedItem = null;
				return;
			}

			if (ItemsSource != null)
			{
				SelectedItem = ItemsSource[SelectedIndex];
				return;
			}

			SelectedItem = Items[SelectedIndex];
		}

		public IPlatformElementConfiguration<T, Picker> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		void ITextElement.OnTextColorPropertyChanged(Color oldValue, Color newValue)
		{
		}
	}
}