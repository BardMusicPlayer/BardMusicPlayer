/*
 * Copyright (c) 2017 Kris McGinnes
 * Licensed under the MIT license. See https://github.com/SpicyTaco/SpicyTaco.AutoGrid/blob/master/license for full license information.
 */

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BardMusicPlayer.Ui.Controls.AutoGrid
{
    public class StackPanel : Panel
    {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(StackPanel),
            new FrameworkPropertyMetadata(
                Orientation.Vertical,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty MarginBetweenChildrenProperty = DependencyProperty.Register(
            "MarginBetweenChildren", typeof(double), typeof(StackPanel),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty FillProperty = DependencyProperty.RegisterAttached(
            "Fill", typeof(StackPanelFill), typeof(StackPanel),
            new FrameworkPropertyMetadata(
                StackPanelFill.Auto,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentArrange |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public double MarginBetweenChildren
        {
            get => (double) GetValue(MarginBetweenChildrenProperty);
            set => SetValue(MarginBetweenChildrenProperty, value);
        }

        public Orientation Orientation
        {
            get => (Orientation) GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var children = InternalChildren;

            double parentWidth = 0;
            double parentHeight = 0;
            double accumulatedWidth = 0;
            double accumulatedHeight = 0;

            var isHorizontal = Orientation == Orientation.Horizontal;
            var totalMarginToAdd = CalculateTotalMarginToAdd(children, MarginBetweenChildren);

            for (var i = 0; i < children.Count; i++)
            {
                var child = children[i];

                if (child == null) continue;

                // Handle only the Auto's first to calculate remaining space for Fill's
                if (GetFill(child) != StackPanelFill.Auto) continue;

                // Child constraint is the remaining size; this is total size minus size consumed by previous children.
                var childConstraint = new Size(Math.Max(0.0, constraint.Width - accumulatedWidth),
                    Math.Max(0.0, constraint.Height - accumulatedHeight));

                // Measure child.
                child.Measure(childConstraint);
                var childDesiredSize = child.DesiredSize;

                if (isHorizontal)
                {
                    accumulatedWidth += childDesiredSize.Width;
                    parentHeight     =  Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                }
                else
                {
                    parentWidth       =  Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                    accumulatedHeight += childDesiredSize.Height;
                }
            }

            // Add all margin to accumulated size before calculating remaining space for
            // Fill elements.
            if (isHorizontal)
                accumulatedWidth += totalMarginToAdd;
            else
                accumulatedHeight += totalMarginToAdd;

            var totalCountOfFillTypes = children
                .OfType<UIElement>()
                .Count(x => GetFill(x) == StackPanelFill.Fill
                            && x.Visibility != Visibility.Collapsed);

            var availableSpaceRemaining = isHorizontal
                ? Math.Max(0, constraint.Width - accumulatedWidth)
                : Math.Max(0, constraint.Height - accumulatedHeight);

            var eachFillTypeSize = totalCountOfFillTypes > 0
                ? availableSpaceRemaining / totalCountOfFillTypes
                : 0;

            for (var i = 0; i < children.Count; i++)
            {
                var child = children[i];

                if (child == null) continue;

                // Handle all the Fill's giving them a portion of the remaining space
                if (GetFill(child) != StackPanelFill.Fill) continue;

                // Child constraint is the remaining size; this is total size minus size consumed by previous children.
                var childConstraint = isHorizontal
                    ? new Size(eachFillTypeSize,
                        Math.Max(0.0, constraint.Height - accumulatedHeight))
                    : new Size(Math.Max(0.0, constraint.Width - accumulatedWidth),
                        eachFillTypeSize);

                // Measure child.
                child.Measure(childConstraint);
                var childDesiredSize = child.DesiredSize;

                if (isHorizontal)
                {
                    accumulatedWidth += childDesiredSize.Width;
                    parentHeight     =  Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                }
                else
                {
                    parentWidth       =  Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                    accumulatedHeight += childDesiredSize.Height;
                }
            }

            // Make sure the final accumulated size is reflected in parentSize. 
            parentWidth  = Math.Max(parentWidth, accumulatedWidth);
            parentHeight = Math.Max(parentHeight, accumulatedHeight);
            var parent = new Size(parentWidth, parentHeight);

            return parent;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var children = InternalChildren;
            var totalChildrenCount = children.Count;

            double accumulatedLeft = 0;
            double accumulatedTop = 0;

            var isHorizontal = Orientation == Orientation.Horizontal;
            var marginBetweenChildren = MarginBetweenChildren;

            var totalMarginToAdd = CalculateTotalMarginToAdd(children, marginBetweenChildren);

            var allAutoSizedSum = 0.0;
            var countOfFillTypes = 0;
            foreach (var child in children.OfType<UIElement>())
            {
                var fillType = GetFill(child);
                if (fillType != StackPanelFill.Auto)
                {
                    if (child.Visibility != Visibility.Collapsed && fillType != StackPanelFill.Ignored)
                        countOfFillTypes += 1;
                }
                else
                {
                    var desiredSize = isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
                    allAutoSizedSum += desiredSize;
                }
            }

            var remainingForFillTypes = isHorizontal
                ? Math.Max(0, arrangeSize.Width - allAutoSizedSum - totalMarginToAdd)
                : Math.Max(0, arrangeSize.Height - allAutoSizedSum - totalMarginToAdd);
            var fillTypeSize = remainingForFillTypes / countOfFillTypes;

            for (var i = 0; i < totalChildrenCount; ++i)
            {
                var child = children[i];
                if (child == null) continue;

                var childDesiredSize = child.DesiredSize;
                var fillType = GetFill(child);
                var isCollapsed = child.Visibility == Visibility.Collapsed || fillType == StackPanelFill.Ignored;
                var isLastChild = i == totalChildrenCount - 1;
                var marginToAdd = isLastChild || isCollapsed ? 0 : marginBetweenChildren;

                var rcChild = new Rect(
                    accumulatedLeft,
                    accumulatedTop,
                    Math.Max(0.0, arrangeSize.Width - accumulatedLeft),
                    Math.Max(0.0, arrangeSize.Height - accumulatedTop));

                if (isHorizontal)
                {
                    rcChild.Width = fillType == StackPanelFill.Auto || isCollapsed
                        ? childDesiredSize.Width
                        : fillTypeSize;
                    rcChild.Height  =  arrangeSize.Height;
                    accumulatedLeft += rcChild.Width + marginToAdd;
                }
                else
                {
                    rcChild.Width = arrangeSize.Width;
                    rcChild.Height = fillType == StackPanelFill.Auto || isCollapsed
                        ? childDesiredSize.Height
                        : fillTypeSize;
                    accumulatedTop += rcChild.Height + marginToAdd;
                }

                child.Arrange(rcChild);
            }

            return arrangeSize;
        }

        private static double CalculateTotalMarginToAdd(UIElementCollection children, double marginBetweenChildren)
        {
            var visibleChildrenCount = children
                .OfType<UIElement>()
                .Count(x => x.Visibility != Visibility.Collapsed && GetFill(x) != StackPanelFill.Ignored);
            var marginMultiplier = Math.Max(visibleChildrenCount - 1, 0);
            var totalMarginToAdd = marginBetweenChildren * marginMultiplier;
            return totalMarginToAdd;
        }

        public static void SetFill(DependencyObject element, StackPanelFill value)
        {
            element.SetValue(FillProperty, value);
        }

        public static StackPanelFill GetFill(DependencyObject element) =>
            (StackPanelFill) element.GetValue(FillProperty);
    }

    public enum StackPanelFill
    {
        Auto,
        Fill,
        Ignored
    }
}