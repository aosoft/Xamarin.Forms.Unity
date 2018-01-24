using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms
{
	public partial class Grid
	{
		private List<ColumnDefinition> _columns;
		private List<RowDefinition> _rows;

		protected override void LayoutChildren(double x, double y, double width, double height)
		{
			if (!InternalChildren.Any())
				return;

			MeasureGrid(width, height);

			// Make copies so if InvalidateMeasure is called during layout we dont crash when these get nulled
			List<ColumnDefinition> columnsCopy = _columns;
			List<RowDefinition> rowsCopy = _rows;

			for (var index = 0; index < InternalChildren.Count; index++)
			{
				var child = (View)InternalChildren[index];
				if (!child.IsVisible)
					continue;
				int r = GetRow(child);
				int c = GetColumn(child);
				int rs = GetRowSpan(child);
				int cs = GetColumnSpan(child);

				double posx = x + c * ColumnSpacing;
				for (var i = 0; i < c; i++)
					posx += columnsCopy[i].ActualWidth;
				double posy = y + r * RowSpacing;
				for (var i = 0; i < r; i++)
					posy += rowsCopy[i].ActualHeight;

				double w = columnsCopy[c].ActualWidth;
				for (var i = 1; i < cs; i++)
					w += ColumnSpacing + columnsCopy[c + i].ActualWidth;
				double h = rowsCopy[r].ActualHeight;
				for (var i = 1; i < rs; i++)
					h += RowSpacing + rowsCopy[r + i].ActualHeight;

				// in the future we can might maybe optimize by passing the already calculated size request
				LayoutChildIntoBoundingRegion(child, new Rectangle(posx, posy, w, h));
			}
		}

		[Obsolete("OnSizeRequest is obsolete as of version 2.2.0. Please use OnMeasure instead.")]
		protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
		{
			if (!InternalChildren.Any())
				return new SizeRequest(new Size(0, 0));

			MeasureGrid(widthConstraint, heightConstraint, true);

			double columnWidthSum = 0;
			double nonStarColumnWidthSum = 0;
			for (var index = 0; index < _columns.Count; index++)
			{
				ColumnDefinition c = _columns[index];
				columnWidthSum += c.ActualWidth;
				if (!c.Width.IsStar)
					nonStarColumnWidthSum += c.ActualWidth;
			}
			double rowHeightSum = 0;
			double nonStarRowHeightSum = 0;
			for (var index = 0; index < _rows.Count; index++)
			{
				RowDefinition r = _rows[index];
				rowHeightSum += r.ActualHeight;
				if (!r.Height.IsStar)
					nonStarRowHeightSum += r.ActualHeight;
			}

			var request = new Size(columnWidthSum + (_columns.Count - 1) * ColumnSpacing, rowHeightSum + (_rows.Count - 1) * RowSpacing);
			var minimum = new Size(nonStarColumnWidthSum + (_columns.Count - 1) * ColumnSpacing, nonStarRowHeightSum + (_rows.Count - 1) * RowSpacing);

			var result = new SizeRequest(request, minimum);
			return result;
		}

		private void AssignAbsoluteCells()
		{
			for (var index = 0; index < _rows.Count; index++)
			{
				RowDefinition row = _rows[index];
				if (row.Height.IsAbsolute)
					row.ActualHeight = row.Height.Value;
			}

			for (var index = 0; index < _columns.Count; index++)
			{
				ColumnDefinition col = _columns[index];
				if (col.Width.IsAbsolute)
					col.ActualWidth = col.Width.Value;
			}
		}

		private void CalculateAutoCells(double width, double height)
		{
			// this require multiple passes. First process the 1-span, then 2, 3, ...
			// And this needs to be run twice, just in case a lower-span column can be determined by a larger span
			for (var iteration = 0; iteration < 2; iteration++)
			{
				for (var rowspan = 1; rowspan <= _rows.Count; rowspan++)
				{
					for (var i = 0; i < _rows.Count; i++)
					{
						RowDefinition row = _rows[i];
						if (!row.Height.IsAuto)
							continue;
						if (row.ActualHeight >= 0) // if Actual is already set (by a smaller span), skip till pass 3
							continue;

						double actualHeight = row.ActualHeight;
						double minimumHeight = row.MinimumHeight;
						for (var index = 0; index < InternalChildren.Count; index++)
						{
							var child = (View)InternalChildren[index];
							if (!child.IsVisible || GetRowSpan(child) != rowspan || !IsInRow(child, i) || NumberOfUnsetRowHeight(child) > 1)
								continue;
							double assignedWidth = GetAssignedColumnWidth(child);
							double assignedHeight = GetAssignedRowHeight(child);
							double widthRequest = assignedWidth + GetUnassignedWidth(width);
							double heightRequest = double.IsPositiveInfinity(height) ? double.PositiveInfinity : assignedHeight + GetUnassignedHeight(height);

							SizeRequest sizeRequest = child.Measure(widthRequest, heightRequest, MeasureFlags.IncludeMargins);
							actualHeight = Math.Max(actualHeight, sizeRequest.Request.Height - assignedHeight - RowSpacing * (GetRowSpan(child) - 1));
							minimumHeight = Math.Max(minimumHeight, sizeRequest.Minimum.Height - assignedHeight - RowSpacing * (GetRowSpan(child) - 1));
						}
						if (actualHeight >= 0)
							row.ActualHeight = actualHeight;
						if (minimumHeight >= 0)
							row.MinimumHeight = minimumHeight;
					}
				}

				for (var colspan = 1; colspan <= _columns.Count; colspan++)
				{
					for (var i = 0; i < _columns.Count; i++)
					{
						ColumnDefinition col = _columns[i];
						if (!col.Width.IsAuto)
							continue;
						if (col.ActualWidth >= 0) // if Actual is already set (by a smaller span), skip
							continue;

						double actualWidth = col.ActualWidth;
						double minimumWidth = col.MinimumWidth;
						for (var index = 0; index < InternalChildren.Count; index++)
						{
							var child = (View)InternalChildren[index];
							if (!child.IsVisible || GetColumnSpan(child) != colspan || !IsInColumn(child, i) || NumberOfUnsetColumnWidth(child) > 1)
								continue;
							double assignedWidth = GetAssignedColumnWidth(child);
							double assignedHeight = GetAssignedRowHeight(child);
							double widthRequest = double.IsPositiveInfinity(width) ? double.PositiveInfinity : assignedWidth + GetUnassignedWidth(width);
							double heightRequest = assignedHeight + GetUnassignedHeight(height);

							SizeRequest sizeRequest = child.Measure(widthRequest, heightRequest, MeasureFlags.IncludeMargins);
							actualWidth = Math.Max(actualWidth, sizeRequest.Request.Width - assignedWidth - (GetColumnSpan(child) - 1) * ColumnSpacing);
							minimumWidth = Math.Max(minimumWidth, sizeRequest.Minimum.Width - assignedWidth - (GetColumnSpan(child) - 1) * ColumnSpacing);
						}
						if (actualWidth >= 0)
							col.ActualWidth = actualWidth;
						if (minimumWidth >= 0)
							col.MinimumWidth = actualWidth;
					}
				}
			}
		}

		private void CalculateStarCells(double width, double height, double totalStarsWidth, double totalStarsHeight)
		{
			double starColWidth = GetUnassignedWidth(width) / totalStarsWidth;
			double starRowHeight = GetUnassignedHeight(height) / totalStarsHeight;

			for (var index = 0; index < _columns.Count; index++)
			{
				ColumnDefinition col = _columns[index];
				if (col.Width.IsStar)
					col.ActualWidth = col.Width.Value * starColWidth;
			}

			for (var index = 0; index < _rows.Count; index++)
			{
				RowDefinition row = _rows[index];
				if (row.Height.IsStar)
					row.ActualHeight = row.Height.Value * starRowHeight;
			}
		}

		private void ContractColumnsIfNeeded(double width, Func<ColumnDefinition, bool> predicate)
		{
			double columnWidthSum = 0;
			for (var index = 0; index < _columns.Count; index++)
			{
				ColumnDefinition c = _columns[index];
				columnWidthSum += c.ActualWidth;
			}

			double rowHeightSum = 0;
			for (var index = 0; index < _rows.Count; index++)
			{
				RowDefinition r = _rows[index];
				rowHeightSum += r.ActualHeight;
			}

			var request = new Size(columnWidthSum + (_columns.Count - 1) * ColumnSpacing, rowHeightSum + (_rows.Count - 1) * RowSpacing);
			if (request.Width > width)
			{
				double contractionSpace = 0;
				for (var index = 0; index < _columns.Count; index++)
				{
					ColumnDefinition c = _columns[index];
					if (predicate(c))
						contractionSpace += c.ActualWidth - c.MinimumWidth;
				}
				if (contractionSpace > 0)
				{
					// contract as much as we can but no more
					double contractionNeeded = Math.Min(contractionSpace, Math.Max(request.Width - width, 0));
					double contractFactor = contractionNeeded / contractionSpace;
					for (var index = 0; index < _columns.Count; index++)
					{
						ColumnDefinition col = _columns[index];
						if (!predicate(col))
							continue;
						double availableSpace = col.ActualWidth - col.MinimumWidth;
						double contraction = availableSpace * contractFactor;
						col.ActualWidth -= contraction;
						contractionNeeded -= contraction;
					}
				}
			}
		}

		private void ContractRowsIfNeeded(double height, Func<RowDefinition, bool> predicate)
		{
			double columnSum = 0;
			for (var index = 0; index < _columns.Count; index++)
			{
				ColumnDefinition c = _columns[index];
				columnSum += Math.Max(0, c.ActualWidth);
			}
			double rowSum = 0;
			for (var index = 0; index < _rows.Count; index++)
			{
				RowDefinition r = _rows[index];
				rowSum += Math.Max(0, r.ActualHeight);
			}

			var request = new Size(columnSum + (_columns.Count - 1) * ColumnSpacing, rowSum + (_rows.Count - 1) * RowSpacing);
			if (request.Height <= height)
				return;
			double contractionSpace = 0;
			for (var index = 0; index < _rows.Count; index++)
			{
				RowDefinition r = _rows[index];
				if (predicate(r))
					contractionSpace += r.ActualHeight - r.MinimumHeight;
			}
			if (!(contractionSpace > 0))
				return;
			// contract as much as we can but no more
			double contractionNeeded = Math.Min(contractionSpace, Math.Max(request.Height - height, 0));
			double contractFactor = contractionNeeded / contractionSpace;
			for (var index = 0; index < _rows.Count; index++)
			{
				RowDefinition row = _rows[index];
				if (!predicate(row))
					continue;
				double availableSpace = row.ActualHeight - row.MinimumHeight;
				double contraction = availableSpace * contractFactor;
				row.ActualHeight -= contraction;
				contractionNeeded -= contraction;
			}
		}

		private void EnsureRowsColumnsInitialized()
		{
			_columns = ColumnDefinitions == null ? new List<ColumnDefinition>() : ColumnDefinitions.ToList();
			_rows = RowDefinitions == null ? new List<RowDefinition>() : RowDefinitions.ToList();

			int lastRow = -1;
			for (var index = 0; index < InternalChildren.Count; index++)
			{
				Element w = InternalChildren[index];
				lastRow = Math.Max(lastRow, GetRow(w) + GetRowSpan(w) - 1);
			}
			lastRow = Math.Max(lastRow, RowDefinitions.Count - 1);

			int lastCol = -1;
			for (var index = 0; index < InternalChildren.Count; index++)
			{
				Element w = InternalChildren[index];
				lastCol = Math.Max(lastCol, GetColumn(w) + GetColumnSpan(w) - 1);
			}
			lastCol = Math.Max(lastCol, ColumnDefinitions.Count - 1);

			while (_columns.Count <= lastCol)
				_columns.Add(new ColumnDefinition());
			while (_rows.Count <= lastRow)
				_rows.Add(new RowDefinition());

			for (var index = 0; index < _columns.Count; index++)
			{
				ColumnDefinition col = _columns[index];
				col.ActualWidth = -1;
			}
			for (var index = 0; index < _rows.Count; index++)
			{
				RowDefinition row = _rows[index];
				row.ActualHeight = -1;
			}
		}

		private void ExpandLastAutoColumnIfNeeded(double width, bool expandToRequest)
		{
			for (var index = 0; index < InternalChildren.Count; index++)
			{
				Element element = InternalChildren[index];
				var child = (View)element;
				if (!child.IsVisible)
					continue;

				ColumnDefinition col = GetLastAutoColumn(child);
				if (col == null)
					continue;

				double assignedWidth = GetAssignedColumnWidth(child);
				double w = double.IsPositiveInfinity(width) ? double.PositiveInfinity : assignedWidth + GetUnassignedWidth(width);
				SizeRequest sizeRequest = child.Measure(w, GetAssignedRowHeight(child), MeasureFlags.IncludeMargins);
				double requiredWidth = expandToRequest ? sizeRequest.Request.Width : sizeRequest.Minimum.Width;
				double deltaWidth = requiredWidth - assignedWidth - (GetColumnSpan(child) - 1) * ColumnSpacing;
				if (deltaWidth > 0)
				{
					col.ActualWidth += deltaWidth;
				}
			}
		}

		private void ExpandLastAutoRowIfNeeded(double height, bool expandToRequest)
		{
			for (var index = 0; index < InternalChildren.Count; index++)
			{
				Element element = InternalChildren[index];
				var child = (View)element;
				if (!child.IsVisible)
					continue;

				RowDefinition row = GetLastAutoRow(child);
				if (row == null)
					continue;

				double assignedHeight = GetAssignedRowHeight(child);
				double h = double.IsPositiveInfinity(height) ? double.PositiveInfinity : assignedHeight + GetUnassignedHeight(height);
				SizeRequest sizeRequest = child.Measure(GetAssignedColumnWidth(child), h, MeasureFlags.IncludeMargins);
				double requiredHeight = expandToRequest ? sizeRequest.Request.Height : sizeRequest.Minimum.Height;
				double deltaHeight = requiredHeight - assignedHeight - (GetRowSpan(child) - 1) * RowSpacing;
				if (deltaHeight > 0)
				{
					row.ActualHeight += deltaHeight;
				}
			}
		}

		private void MeasureAndContractStarredColumns(double width, double height, double totalStarsWidth)
		{
			double starColWidth;
			starColWidth = MeasuredStarredColumns();

			if (!double.IsPositiveInfinity(width) && double.IsPositiveInfinity(height))
			{
				// re-zero columns so GetUnassignedWidth returns correctly
				for (var index = 0; index < _columns.Count; index++)
				{
					ColumnDefinition col = _columns[index];
					if (col.Width.IsStar)
						col.ActualWidth = 0;
				}

				starColWidth = Math.Max(starColWidth, GetUnassignedWidth(width) / totalStarsWidth);
			}

			for (var index = 0; index < _columns.Count; index++)
			{
				ColumnDefinition col = _columns[index];
				if (col.Width.IsStar)
					col.ActualWidth = col.Width.Value * starColWidth;
			}

			ContractColumnsIfNeeded(width, c => c.Width.IsStar);
		}

		private void MeasureAndContractStarredRows(double width, double height, double totalStarsHeight)
		{
			double starRowHeight;
			starRowHeight = MeasureStarredRows();

			if (!double.IsPositiveInfinity(height) && double.IsPositiveInfinity(width))
			{
				for (var index = 0; index < _rows.Count; index++)
				{
					RowDefinition row = _rows[index];
					if (row.Height.IsStar)
						row.ActualHeight = 0;
				}

				starRowHeight = Math.Max(starRowHeight, GetUnassignedHeight(height) / totalStarsHeight);
			}

			for (var index = 0; index < _rows.Count; index++)
			{
				RowDefinition row = _rows[index];
				if (row.Height.IsStar)
					row.ActualHeight = row.Height.Value * starRowHeight;
			}

			ContractRowsIfNeeded(height, r => r.Height.IsStar);
		}

		private double MeasuredStarredColumns()
		{
			double starColWidth;
			for (var iteration = 0; iteration < 2; iteration++)
			{
				for (var colspan = 1; colspan <= _columns.Count; colspan++)
				{
					for (var i = 0; i < _columns.Count; i++)
					{
						ColumnDefinition col = _columns[i];
						if (!col.Width.IsStar)
							continue;
						if (col.ActualWidth >= 0) // if Actual is already set (by a smaller span), skip
							continue;

						double actualWidth = col.ActualWidth;
						double minimumWidth = col.MinimumWidth;
						for (var index = 0; index < InternalChildren.Count; index++)
						{
							var child = (View)InternalChildren[index];
							if (!child.IsVisible || GetColumnSpan(child) != colspan || !IsInColumn(child, i) || NumberOfUnsetColumnWidth(child) > 1)
								continue;
							double assignedWidth = GetAssignedColumnWidth(child);

							SizeRequest sizeRequest = child.Measure(double.PositiveInfinity, double.PositiveInfinity, MeasureFlags.IncludeMargins);
							actualWidth = Math.Max(actualWidth, sizeRequest.Request.Width - assignedWidth - (GetColumnSpan(child) - 1) * ColumnSpacing);
							minimumWidth = Math.Max(minimumWidth, sizeRequest.Minimum.Width - assignedWidth - (GetColumnSpan(child) - 1) * ColumnSpacing);
						}
						if (actualWidth >= 0)
							col.ActualWidth = actualWidth;

						if (minimumWidth >= 0)
							col.MinimumWidth = minimumWidth;
					}
				}
			}

			//Measure the stars
			starColWidth = 1;
			for (var index = 0; index < _columns.Count; index++)
			{
				ColumnDefinition col = _columns[index];
				if (!col.Width.IsStar)
					continue;
				starColWidth = Math.Max(starColWidth, col.ActualWidth / col.Width.Value);
			}

			return starColWidth;
		}

		private void MeasureGrid(double width, double height, bool requestSize = false)
		{
			EnsureRowsColumnsInitialized();

			AssignAbsoluteCells();

			CalculateAutoCells(width, height);

			if (!requestSize)
			{
				ContractColumnsIfNeeded(width, c => c.Width.IsAuto);
				ContractRowsIfNeeded(height, r => r.Height.IsAuto);
			}

			double totalStarsHeight = 0;
			for (var index = 0; index < _rows.Count; index++)
			{
				RowDefinition row = _rows[index];
				if (row.Height.IsStar)
					totalStarsHeight += row.Height.Value;
			}

			double totalStarsWidth = 0;
			for (var index = 0; index < _columns.Count; index++)
			{
				ColumnDefinition col = _columns[index];
				if (col.Width.IsStar)
					totalStarsWidth += col.Width.Value;
			}

			if (requestSize)
			{
				MeasureAndContractStarredColumns(width, height, totalStarsWidth);
				MeasureAndContractStarredRows(width, height, totalStarsHeight);
			}
			else
			{
				CalculateStarCells(width, height, totalStarsWidth, totalStarsHeight);
			}

			ZeroUnassignedCells();

			ExpandLastAutoRowIfNeeded(height, requestSize);
			ExpandLastAutoColumnIfNeeded(width, requestSize);
		}

		private double MeasureStarredRows()
		{
			double starRowHeight;
			for (var iteration = 0; iteration < 2; iteration++)
			{
				for (var rowspan = 1; rowspan <= _rows.Count; rowspan++)
				{
					for (var i = 0; i < _rows.Count; i++)
					{
						RowDefinition row = _rows[i];
						if (!row.Height.IsStar)
							continue;
						if (row.ActualHeight >= 0) // if Actual is already set (by a smaller span), skip till pass 3
							continue;

						double actualHeight = row.ActualHeight;
						double minimumHeight = row.MinimumHeight;
						for (var index = 0; index < InternalChildren.Count; index++)
						{
							var child = (View)InternalChildren[index];
							if (!child.IsVisible || GetRowSpan(child) != rowspan || !IsInRow(child, i) || NumberOfUnsetRowHeight(child) > 1)
								continue;
							double assignedHeight = GetAssignedRowHeight(child);
							double assignedWidth = GetAssignedColumnWidth(child);

							SizeRequest sizeRequest = child.Measure(assignedWidth, double.PositiveInfinity, MeasureFlags.IncludeMargins);
							actualHeight = Math.Max(actualHeight, sizeRequest.Request.Height - assignedHeight - RowSpacing * (GetRowSpan(child) - 1));
							minimumHeight = Math.Max(minimumHeight, sizeRequest.Minimum.Height - assignedHeight - RowSpacing * (GetRowSpan(child) - 1));
						}
						if (actualHeight >= 0)
							row.ActualHeight = actualHeight;

						if (minimumHeight >= 0)
							row.MinimumHeight = minimumHeight;
					}
				}
			}

			// 3. Star columns:

			//Measure the stars
			starRowHeight = 1;
			for (var index = 0; index < _rows.Count; index++)
			{
				RowDefinition row = _rows[index];
				if (!row.Height.IsStar)
					continue;
				starRowHeight = Math.Max(starRowHeight, row.ActualHeight / row.Height.Value);
			}

			return starRowHeight;
		}

		private void ZeroUnassignedCells()
		{
			for (var index = 0; index < _columns.Count; index++)
			{
				ColumnDefinition col = _columns[index];
				if (col.ActualWidth < 0)
					col.ActualWidth = 0;
			}
			for (var index = 0; index < _rows.Count; index++)
			{
				RowDefinition row = _rows[index];
				if (row.ActualHeight < 0)
					row.ActualHeight = 0;
			}
		}

		#region Helpers

		private static bool IsInColumn(BindableObject child, int column)
		{
			int childColumn = GetColumn(child);
			int span = GetColumnSpan(child);
			return childColumn <= column && column < childColumn + span;
		}

		private static bool IsInRow(BindableObject child, int row)
		{
			int childRow = GetRow(child);
			int span = GetRowSpan(child);
			return childRow <= row && row < childRow + span;
		}

		private int NumberOfUnsetColumnWidth(BindableObject child)
		{
			var n = 0;
			int index = GetColumn(child);
			int span = GetColumnSpan(child);
			for (int i = index; i < index + span; i++)
				if (_columns[i].ActualWidth <= 0)
					n++;
			return n;
		}

		private int NumberOfUnsetRowHeight(BindableObject child)
		{
			var n = 0;
			int index = GetRow(child);
			int span = GetRowSpan(child);
			for (int i = index; i < index + span; i++)
				if (_rows[i].ActualHeight <= 0)
					n++;
			return n;
		}

		private double GetAssignedColumnWidth(BindableObject child)
		{
			var actual = 0d;
			int index = GetColumn(child);
			int span = GetColumnSpan(child);
			for (int i = index; i < index + span; i++)
				if (_columns[i].ActualWidth >= 0)
					actual += _columns[i].ActualWidth;
			return actual;
		}

		private double GetAssignedRowHeight(BindableObject child)
		{
			var actual = 0d;
			int index = GetRow(child);
			int span = GetRowSpan(child);
			for (int i = index; i < index + span; i++)
				if (_rows[i].ActualHeight >= 0)
					actual += _rows[i].ActualHeight;
			return actual;
		}

		private ColumnDefinition GetLastAutoColumn(BindableObject child)
		{
			int index = GetColumn(child);
			int span = GetColumnSpan(child);
			for (int i = index + span - 1; i >= index; i--)
				if (_columns[i].Width.IsAuto)
					return _columns[i];
			return null;
		}

		private RowDefinition GetLastAutoRow(BindableObject child)
		{
			int index = GetRow(child);
			int span = GetRowSpan(child);
			for (int i = index + span - 1; i >= index; i--)
				if (_rows[i].Height.IsAuto)
					return _rows[i];
			return null;
		}

		private double GetUnassignedHeight(double heightRequest)
		{
			double assigned = (_rows.Count - 1) * RowSpacing;
			for (var i = 0; i < _rows.Count; i++)
			{
				double actual = _rows[i].ActualHeight;
				if (actual >= 0)
					assigned += actual;
			}
			return heightRequest - assigned;
		}

		private double GetUnassignedWidth(double widthRequest)
		{
			double assigned = (_columns.Count - 1) * ColumnSpacing;
			for (var i = 0; i < _columns.Count; i++)
			{
				double actual = _columns[i].ActualWidth;
				if (actual >= 0)
					assigned += actual;
			}
			return widthRequest - assigned;
		}

		#endregion Helpers
	}
}