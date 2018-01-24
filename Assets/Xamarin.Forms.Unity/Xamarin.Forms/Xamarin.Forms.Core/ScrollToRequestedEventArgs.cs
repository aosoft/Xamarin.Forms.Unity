﻿using System;

namespace Xamarin.Forms
{
	public class ScrollToRequestedEventArgs : EventArgs, ITemplatedItemsListScrollToRequestedEventArgs
	{
		internal ScrollToRequestedEventArgs(double scrollX, double scrollY, bool shouldAnimate)
		{
			ScrollX = scrollX;
			ScrollY = scrollY;
			ShouldAnimate = shouldAnimate;
			Mode = ScrollToMode.Position;
		}

		internal ScrollToRequestedEventArgs(Element element, ScrollToPosition position, bool shouldAnimate)
		{
			Element = element;
			Position = position;
			ShouldAnimate = shouldAnimate;
			Mode = ScrollToMode.Element;
		}

		internal ScrollToRequestedEventArgs(object item, ScrollToPosition position, bool shouldAnimate)
		{
			Item = item;
			Position = position;
			ShouldAnimate = shouldAnimate;
			//Mode = ScrollToMode.Item;
		}

		internal ScrollToRequestedEventArgs(object item, object group, ScrollToPosition position, bool shouldAnimate)
		{
			Item = item;
			Group = group;
			Position = position;
			ShouldAnimate = shouldAnimate;
			//Mode = ScrollToMode.GroupAndIem;
		}

		public Element Element { get; private set; }

		public ScrollToMode Mode { get; private set; }

		public ScrollToPosition Position { get; private set; }

		public double ScrollX { get; private set; }

		public double ScrollY { get; private set; }

		public bool ShouldAnimate { get; private set; }

		internal object Group { get; private set; }

		object ITemplatedItemsListScrollToRequestedEventArgs.Group
		{
			get
			{
				return Group;
			}
		}

		internal object Item { get; private set; }

		object ITemplatedItemsListScrollToRequestedEventArgs.Item
		{
			get
			{
				return Item;
			}
		}
	}
}