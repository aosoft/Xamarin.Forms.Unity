﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ToolbarTracker
	{
		private int _masterDetails;
		private Page _target;

		public IEnumerable<Page> AdditionalTargets { get; set; }

		public bool HaveMasterDetail
		{
			get { return _masterDetails > 0; }
		}

		public bool SeparateMasterDetail { get; set; }

		public Page Target
		{
			get { return _target; }
			set
			{
				if (_target == value)
					return;

				UntrackTarget(_target);
				_target = value;

				if (_target != null)
					TrackTarget(_target);
				EmitCollectionChanged();
			}
		}

		public IEnumerable<ToolbarItem> ToolbarItems
		{
			get
			{
				if (Target == null)
					return Enumerable.Empty<ToolbarItem>();
				IEnumerable<ToolbarItem> items = GetCurrentToolbarItems(Target);
				if (AdditionalTargets != null)
					items = items.Concat(AdditionalTargets.SelectMany(t => t.ToolbarItems));

				return items.OrderBy(ti => ti.Priority);
			}
		}

		public event EventHandler CollectionChanged;

		private void EmitCollectionChanged()
		{
			if (CollectionChanged != null)
				CollectionChanged(this, EventArgs.Empty);
		}

		private IEnumerable<ToolbarItem> GetCurrentToolbarItems(Page page)
		{
			var result = new List<ToolbarItem>();
			result.AddRange(page.ToolbarItems);

			if (page is MasterDetailPage)
			{
				var masterDetail = (MasterDetailPage)page;
				if (SeparateMasterDetail)
				{
					if (masterDetail.IsPresented)
					{
						if (masterDetail.Master != null)
							result.AddRange(GetCurrentToolbarItems(masterDetail.Master));
					}
					else
					{
						if (masterDetail.Detail != null)
							result.AddRange(GetCurrentToolbarItems(masterDetail.Detail));
					}
				}
				else
				{
					if (masterDetail.Master != null)
						result.AddRange(GetCurrentToolbarItems(masterDetail.Master));
					if (masterDetail.Detail != null)
						result.AddRange(GetCurrentToolbarItems(masterDetail.Detail));
				}
			}
			else if (page is IPageContainer<Page>)
			{
				var container = (IPageContainer<Page>)page;
				if (container.CurrentPage != null)
					result.AddRange(GetCurrentToolbarItems(container.CurrentPage));
			}

			return result;
		}

		private void OnChildAdded(object sender, ElementEventArgs eventArgs)
		{
			var page = eventArgs.Element as Page;
			if (page == null)
				return;

			RegisterChildPage(page);
		}

		private void OnChildRemoved(object sender, ElementEventArgs eventArgs)
		{
			var page = eventArgs.Element as Page;
			if (page == null)
				return;

			UnregisterChildPage(page);
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			EmitCollectionChanged();
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			if (propertyChangedEventArgs.PropertyName == NavigationPage.CurrentPageProperty.PropertyName || propertyChangedEventArgs.PropertyName == MasterDetailPage.IsPresentedProperty.PropertyName ||
				propertyChangedEventArgs.PropertyName == "Detail" || propertyChangedEventArgs.PropertyName == "Master")
			{
				EmitCollectionChanged();
			}
		}

		private void RegisterChildPage(Page page)
		{
			if (page is MasterDetailPage)
				_masterDetails++;

			((ObservableCollection<ToolbarItem>)page.ToolbarItems).CollectionChanged += OnCollectionChanged;
			page.PropertyChanged += OnPropertyChanged;
		}

		private void TrackTarget(Page page)
		{
			if (page == null)
				return;

			if (page is MasterDetailPage)
				_masterDetails++;

			((ObservableCollection<ToolbarItem>)page.ToolbarItems).CollectionChanged += OnCollectionChanged;
			page.Descendants().OfType<Page>().ForEach(RegisterChildPage);

			page.DescendantAdded += OnChildAdded;
			page.DescendantRemoved += OnChildRemoved;
			page.PropertyChanged += OnPropertyChanged;
		}

		private void UnregisterChildPage(Page page)
		{
			if (page is MasterDetailPage)
				_masterDetails--;

			((ObservableCollection<ToolbarItem>)page.ToolbarItems).CollectionChanged -= OnCollectionChanged;
			page.PropertyChanged -= OnPropertyChanged;
		}

		private void UntrackTarget(Page page)
		{
			if (page == null)
				return;

			if (page is MasterDetailPage)
				_masterDetails--;

			((ObservableCollection<ToolbarItem>)page.ToolbarItems).CollectionChanged -= OnCollectionChanged;
			page.Descendants().OfType<Page>().ForEach(UnregisterChildPage);

			page.DescendantAdded -= OnChildAdded;
			page.DescendantRemoved -= OnChildRemoved;
			page.PropertyChanged -= OnPropertyChanged;
		}
	}
}