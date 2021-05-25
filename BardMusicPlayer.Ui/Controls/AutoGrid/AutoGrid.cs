/*
 * Copyright (c) 2017 Kris McGinnes
 * Licensed under the MIT license. See https://github.com/SpicyTaco/SpicyTaco.AutoGrid/blob/master/license for full license information.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BardMusicPlayer.Ui.Controls.AutoGrid
{
    /// <summary>
    ///     Defines a flexible grid area that consists of columns and rows.
    ///     Depending on the orientation, either the rows or the columns are auto-generated,
    ///     and the children's position is set according to their index.
    ///     Partially based on work at http://rachel53461.wordpress.com/2011/09/17/wpf-grids-rowcolumn-count-properties/
    /// </summary>
    public class AutoGrid : Grid
    {
        // AutoIndex attached property

        public static readonly DependencyProperty AutoIndexProperty = DependencyProperty.RegisterAttached(
            "AutoIndex", typeof(bool), typeof(AutoGrid),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        // RowHeight override attached property

        public static readonly DependencyProperty RowHeightOverrideProperty = DependencyProperty.RegisterAttached(
            "RowHeightOverride", typeof(GridLength?), typeof(AutoGrid),
            new FrameworkPropertyMetadata(default(GridLength?),
                FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty ColumnWidthOverrideProperty = DependencyProperty.RegisterAttached(
            "ColumnWidthOverride", typeof(GridLength?), typeof(AutoGrid),
            new FrameworkPropertyMetadata(default(GridLength?),
                FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty ChildHorizontalAlignmentProperty =
            DependencyProperty.Register("ChildHorizontalAlignment", typeof(HorizontalAlignment?), typeof(AutoGrid),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
                    OnChildHorizontalAlignmentChanged));

        public static readonly DependencyProperty ChildMarginProperty =
            DependencyProperty.Register("ChildMargin", typeof(Thickness?), typeof(AutoGrid),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
                    OnChildMarginChanged));

        public static readonly DependencyProperty ChildVerticalAlignmentProperty =
            DependencyProperty.Register("ChildVerticalAlignment", typeof(VerticalAlignment?), typeof(AutoGrid),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
                    OnChildVerticalAlignmentChanged));

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached("Columns", typeof(string), typeof(AutoGrid),
                new FrameworkPropertyMetadata("",
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
                    ColumnsChanged));

        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.RegisterAttached("ColumnWidth", typeof(GridLength), typeof(AutoGrid),
                new FrameworkPropertyMetadata(GridLength.Auto,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
                    FixedColumnWidthChanged));

        public static readonly DependencyProperty IsAutoIndexingProperty =
            DependencyProperty.Register("IsAutoIndexing", typeof(bool), typeof(AutoGrid),
                new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
                    OnPropertyChanged));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(AutoGrid),
                new FrameworkPropertyMetadata(Orientation.Horizontal,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
                    OnPropertyChanged));

        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.RegisterAttached("RowHeight", typeof(GridLength), typeof(AutoGrid),
                new FrameworkPropertyMetadata(GridLength.Auto,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
                    FixedRowHeightChanged));

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.RegisterAttached("Rows", typeof(string), typeof(AutoGrid),
                new FrameworkPropertyMetadata("",
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
                    RowsChanged));

        private int _rowOrColumnCount;
        private bool _shouldReindex = true;

        /// <summary>
        ///     Gets or sets a value indicating whether the children are automatically indexed.
        ///     <remarks>
        ///         The default is <c>true</c>.
        ///         Note that if children are already indexed, setting this property to <c>false</c> will not remove their indices.
        ///     </remarks>
        /// </summary>
        [Category("Layout")]
        [Description("Set to false to disable the auto layout functionality")]
        public bool IsAutoIndexing
        {
            get => (bool) GetValue(IsAutoIndexingProperty);
            set => SetValue(IsAutoIndexingProperty, value);
        }

        /// <summary>
        ///     Gets or sets the fixed column width
        /// </summary>
        [Category("Layout")]
        [Description("Presets the width of all columns set using the ColumnCount property")]
        public GridLength ColumnWidth
        {
            get => (GridLength) GetValue(ColumnWidthProperty);
            set => SetValue(ColumnWidthProperty, value);
        }

        /// <summary>
        ///     Gets or sets the fixed row height
        /// </summary>
        [Category("Layout")]
        [Description("Presets the height of all rows set using the RowCount property")]
        public GridLength RowHeight
        {
            get => (GridLength) GetValue(RowHeightProperty);
            set => SetValue(RowHeightProperty, value);
        }

        /// <summary>
        ///     Gets or sets the child horizontal alignment.
        /// </summary>
        /// <value>The child horizontal alignment.</value>
        [Category("Layout")]
        [Description("Presets the horizontal alignment of all child controls")]
        public HorizontalAlignment? ChildHorizontalAlignment
        {
            get => (HorizontalAlignment?) GetValue(ChildHorizontalAlignmentProperty);
            set => SetValue(ChildHorizontalAlignmentProperty, value);
        }

        /// <summary>
        ///     Gets or sets the orientation.
        ///     <remarks>The default is Vertical.</remarks>
        /// </summary>
        /// <value>The orientation.</value>
        [Category("Layout")]
        [Description(
            "Defines the directionality of the autolayout. Use vertical for a column first layout, horizontal for a row first layout."
        )]
        public Orientation Orientation
        {
            get => (Orientation) GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        /// <summary>
        ///     Gets or sets the columns
        /// </summary>
        [Category("Layout")]
        [Description("Defines all columns using comma separated grid length notation")]
        public string Columns
        {
            get => (string) GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        /// <summary>
        ///     Gets or sets the rows
        /// </summary>
        [Category("Layout")]
        [Description("Defines all rows using comma separated grid length notation")]
        public string Rows
        {
            get => (string) GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        /// <summary>
        ///     Gets or sets the child margin.
        /// </summary>
        /// <value>The child margin.</value>
        [Category("Layout")]
        [Description("Presets the margin of all child controls")]
        public Thickness? ChildMargin
        {
            get => (Thickness?) GetValue(ChildMarginProperty);
            set => SetValue(ChildMarginProperty, value);
        }

        /// <summary>
        ///     Gets or sets the child vertical alignment.
        /// </summary>
        /// <value>The child vertical alignment.</value>
        [Category("Layout")]
        [Description("Presets the vertical alignment of all child controls")]
        public VerticalAlignment? ChildVerticalAlignment
        {
            get => (VerticalAlignment?) GetValue(ChildVerticalAlignmentProperty);
            set => SetValue(ChildVerticalAlignmentProperty, value);
        }

        public static void SetAutoIndex(DependencyObject element, bool value)
        {
            element.SetValue(AutoIndexProperty, value);
        }

        public static bool GetAutoIndex(DependencyObject element) => (bool) element.GetValue(AutoIndexProperty);

        public static void SetRowHeightOverride(DependencyObject element, GridLength? value)
        {
            element.SetValue(RowHeightOverrideProperty, value);
        }

        public static GridLength? GetRowHeightOverride(DependencyObject element) =>
            (GridLength?) element.GetValue(RowHeightOverrideProperty);

        public static void SetColumnWidthOverride(DependencyObject element, GridLength? value)
        {
            element.SetValue(ColumnWidthOverrideProperty, value);
        }

        public static GridLength? GetColumnWidthOverride(DependencyObject element) =>
            (GridLength?) element.GetValue(ColumnWidthOverrideProperty);

        /// <summary>
        ///     Handles the column count changed event
        /// </summary>
        public static void ColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((int) e.NewValue < 0)
                return;

            var grid = d as AutoGrid;

            // look for an existing column definition for the height
            var width = GridLength.Auto;
            if (grid.ColumnDefinitions.Count > 0)
                width = grid.ColumnDefinitions[0].Width;

            // clear and rebuild
            grid.ColumnDefinitions.Clear();
            for (var i = 0; i < (int) e.NewValue; i++)
            {
                grid.ColumnDefinitions.Add(
                    new() { Width = width });
            }
        }

        /// <summary>
        ///     Handle the columns changed event
        /// </summary>
        public static void ColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((string) e.NewValue == string.Empty)
                return;

            var grid = d as AutoGrid;
            grid.ColumnDefinitions.Clear();

            var defs = Parse((string) e.NewValue);
            foreach (var def in defs)
            {
                grid.ColumnDefinitions.Add(new() { Width = def });
            }
        }

        /// <summary>
        ///     Handle the fixed column width changed event
        /// </summary>
        public static void FixedColumnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as AutoGrid;

            // add a default column if missing
            if (grid.ColumnDefinitions.Count == 0)
                grid.ColumnDefinitions.Add(new());

            // set all existing columns to this width
            for (var i = 0; i < grid.ColumnDefinitions.Count; i++)
                grid.ColumnDefinitions[i].Width = (GridLength) e.NewValue;
        }

        /// <summary>
        ///     Handle the fixed row height changed event
        /// </summary>
        public static void FixedRowHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as AutoGrid;

            // add a default row if missing
            if (grid.RowDefinitions.Count == 0)
                grid.RowDefinitions.Add(new());

            // set all existing rows to this height
            for (var i = 0; i < grid.RowDefinitions.Count; i++)
                grid.RowDefinitions[i].Height = (GridLength) e.NewValue;
        }

        /// <summary>
        ///     Parse an array of grid lengths from comma delim text
        /// </summary>
        public static GridLength[] Parse(string text)
        {
            var tokens = text.Split(',');
            var definitions = new GridLength[tokens.Length];
            for (var i = 0; i < tokens.Length; i++)
            {
                var str = tokens[i];
                double value;

                // ratio
                if (str.Contains('*'))
                {
                    if (!double.TryParse(str.Replace("*", ""), out value))
                        value = 1.0;

                    definitions[i] = new(value, GridUnitType.Star);
                    continue;
                }

                // pixels
                if (double.TryParse(str, out value))
                {
                    definitions[i] = new(value);
                    continue;
                }

                // auto
                definitions[i] = GridLength.Auto;
            }

            return definitions;
        }

        /// <summary>
        ///     Handles the row count changed event
        /// </summary>
        public static void RowCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((int) e.NewValue < 0)
                return;

            var grid = d as AutoGrid;

            // look for an existing row to get the height
            var height = GridLength.Auto;
            if (grid.RowDefinitions.Count > 0)
                height = grid.RowDefinitions[0].Height;

            // clear and rebuild
            grid.RowDefinitions.Clear();
            for (var i = 0; i < (int) e.NewValue; i++)
            {
                grid.RowDefinitions.Add(
                    new() { Height = height });
            }
        }

        /// <summary>
        ///     Handle the rows changed event
        /// </summary>
        public static void RowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((string) e.NewValue == string.Empty)
                return;

            var grid = d as AutoGrid;
            grid.RowDefinitions.Clear();

            var defs = Parse((string) e.NewValue);
            foreach (var def in defs)
            {
                grid.RowDefinitions.Add(new() { Height = def });
            }
        }

        /// <summary>
        ///     Called when [child horizontal alignment changed].
        /// </summary>
        private static void OnChildHorizontalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as AutoGrid;
            foreach (UIElement child in grid.Children)
            {
                if (grid.ChildHorizontalAlignment.HasValue)
                    child.SetValue(HorizontalAlignmentProperty, grid.ChildHorizontalAlignment);
                else
                    child.SetValue(HorizontalAlignmentProperty, DependencyProperty.UnsetValue);
            }
        }

        /// <summary>
        ///     Called when [child layout changed].
        /// </summary>
        private static void OnChildMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as AutoGrid;
            foreach (UIElement child in grid.Children)
            {
                if (grid.ChildMargin.HasValue)
                    child.SetValue(MarginProperty, grid.ChildMargin);
                else
                    child.SetValue(MarginProperty, DependencyProperty.UnsetValue);
            }
        }

        /// <summary>
        ///     Called when [child vertical alignment changed].
        /// </summary>
        private static void OnChildVerticalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as AutoGrid;
            foreach (UIElement child in grid.Children)
            {
                if (grid.ChildVerticalAlignment.HasValue)
                    child.SetValue(VerticalAlignmentProperty, grid.ChildVerticalAlignment);
                else
                    child.SetValue(VerticalAlignmentProperty, DependencyProperty.UnsetValue);
            }
        }

        /// <summary>
        ///     Handled the redraw properties changed event
        /// </summary>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AutoGrid) d)._shouldReindex = true;
        }

        /// <summary>
        ///     Apply child margins and layout effects such as alignment
        /// </summary>
        private void ApplyChildLayout(UIElement child)
        {
            if (ChildMargin != null) child.SetIfDefault(MarginProperty, ChildMargin.Value);
            if (ChildHorizontalAlignment != null)
                child.SetIfDefault(HorizontalAlignmentProperty, ChildHorizontalAlignment.Value);
            if (ChildVerticalAlignment != null)
                child.SetIfDefault(VerticalAlignmentProperty, ChildVerticalAlignment.Value);
        }

        /// <summary>
        ///     Clamp a value to its maximum.
        /// </summary>
        private int Clamp(int value, int max) => value > max ? max : value;

        public void PerformLayout()
        {
            var isVertical = Orientation == Orientation.Vertical;

            if (_shouldReindex || IsAutoIndexing &&
                (isVertical && _rowOrColumnCount != ColumnDefinitions.Count ||
                 !isVertical && _rowOrColumnCount != RowDefinitions.Count))
            {
                _shouldReindex = false;

                if (IsAutoIndexing)
                {
                    _rowOrColumnCount = ColumnDefinitions.Count != 0 ? ColumnDefinitions.Count : RowDefinitions.Count;
                    if (_rowOrColumnCount == 0) _rowOrColumnCount = 1;

                    var cellCount = 0;
                    foreach (UIElement child in Children)
                    {
                        if (GetAutoIndex(child) == false) continue;
                        cellCount += ColumnDefinitions.Count != 0 ? GetColumnSpan(child) : GetRowSpan(child);
                    }

                    //  Update the number of rows/columns
                    if (ColumnDefinitions.Count != 0)
                    {
                        var newRowCount = (int) Math.Ceiling(cellCount / (double) _rowOrColumnCount);
                        while (RowDefinitions.Count < newRowCount)
                        {
                            var rowDefinition = new RowDefinition();
                            rowDefinition.Height = RowHeight;
                            RowDefinitions.Add(rowDefinition);
                        }

                        if (RowDefinitions.Count > newRowCount)
                            RowDefinitions.RemoveRange(newRowCount, RowDefinitions.Count - newRowCount);
                    }
                    else // rows defined
                    {
                        var newColumnCount = (int) Math.Ceiling(cellCount / (double) _rowOrColumnCount);
                        while (ColumnDefinitions.Count < newColumnCount)
                        {
                            var columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = ColumnWidth;
                            ColumnDefinitions.Add(columnDefinition);
                        }

                        if (ColumnDefinitions.Count > newColumnCount)
                            ColumnDefinitions.RemoveRange(newColumnCount, ColumnDefinitions.Count - newColumnCount);
                    }
                }

                //  Update children indices
                var cellPosition = 0;
                var cellsToSkip = new Queue<int>();
                foreach (UIElement child in Children)
                {
                    if (IsAutoIndexing && GetAutoIndex(child))
                    {
                        if (cellsToSkip.Any() && cellsToSkip.Peek() == cellPosition)
                        {
                            cellsToSkip.Dequeue();
                            cellPosition += 1;
                        }

                        if (!isVertical) // horizontal (default)
                        {
                            var rowIndex = cellPosition / ColumnDefinitions.Count;
                            SetRow(child, rowIndex);

                            var columnIndex = cellPosition % ColumnDefinitions.Count;
                            SetColumn(child, columnIndex);

                            var rowSpan = GetRowSpan(child);
                            if (rowSpan > 1)
                            {
                                Enumerable.Range(1, rowSpan).ToList()
                                    .ForEach(x => cellsToSkip.Enqueue(cellPosition + ColumnDefinitions.Count * x));
                            }

                            var overrideRowHeight = GetRowHeightOverride(child);
                            if (overrideRowHeight != null) RowDefinitions[rowIndex].Height = overrideRowHeight.Value;

                            var overrideColumnWidth = GetColumnWidthOverride(child);
                            if (overrideColumnWidth != null)
                                ColumnDefinitions[columnIndex].Width = overrideColumnWidth.Value;

                            cellPosition += GetColumnSpan(child);
                        }
                        else
                        {
                            var rowIndex = cellPosition % RowDefinitions.Count;
                            SetRow(child, rowIndex);

                            var columnIndex = cellPosition / RowDefinitions.Count;
                            SetColumn(child, columnIndex);

                            var columnSpan = GetColumnSpan(child);
                            if (columnSpan > 1)
                            {
                                Enumerable.Range(1, columnSpan).ToList()
                                    .ForEach(x => cellsToSkip.Enqueue(cellPosition + RowDefinitions.Count * x));
                            }

                            var overrideRowHeight = GetRowHeightOverride(child);
                            if (overrideRowHeight != null) RowDefinitions[rowIndex].Height = overrideRowHeight.Value;

                            var overrideColumnWidth = GetColumnWidthOverride(child);
                            if (overrideColumnWidth != null)
                                ColumnDefinitions[columnIndex].Width = overrideColumnWidth.Value;

                            cellPosition += GetRowSpan(child);
                        }
                    }

                    // Set margin and alignment
                    if (ChildMargin != null) child.SetIfDefault(MarginProperty, ChildMargin.Value);
                    if (ChildHorizontalAlignment != null)
                    {
                        child.SetIfDefault(HorizontalAlignmentProperty,
                            ChildHorizontalAlignment.Value);
                    }

                    if (ChildVerticalAlignment != null)
                        child.SetIfDefault(VerticalAlignmentProperty, ChildVerticalAlignment.Value);
                }
            }
        }

        #region Overrides

        /// <summary>
        ///     Measures the children of a <see cref="T:System.Windows.Controls.Grid" /> in anticipation of arranging them during
        ///     the <see cref="M:ArrangeOverride" /> pass.
        /// </summary>
        /// <param name="constraint">Indicates an upper limit size that should not be exceeded.</param>
        /// <returns>
        ///     <see cref="Size" /> that represents the required size to arrange child content.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            PerformLayout();
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        ///     Called when the visual children of a <see cref="Grid" /> element change.
        ///     <remarks>Used to mark that the grid children have changed.</remarks>
        /// </summary>
        /// <param name="visualAdded">Identifies the visual child that's added.</param>
        /// <param name="visualRemoved">Identifies the visual child that's removed.</param>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            _shouldReindex = true;
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        #endregion Overrides
    }
}