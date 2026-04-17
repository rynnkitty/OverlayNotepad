using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace OverlayNotepad.Controls
{
    public class OutlinedTextControl : FrameworkElement
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(OutlinedTextControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(nameof(FontFamily), typeof(FontFamily), typeof(OutlinedTextControl),
                new FrameworkPropertyMetadata(new FontFamily("맑은 고딕"), FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(OutlinedTextControl),
                new FrameworkPropertyMetadata(14.0, FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

        public static readonly DependencyProperty FillBrushProperty =
            DependencyProperty.Register(nameof(FillBrush), typeof(Brush), typeof(OutlinedTextControl),
                new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

        public static readonly DependencyProperty OutlineThicknessProperty =
            DependencyProperty.Register(nameof(OutlineThickness), typeof(double), typeof(OutlinedTextControl),
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

        public static readonly DependencyProperty OutlineBrushProperty =
            DependencyProperty.Register(nameof(OutlineBrush), typeof(Brush), typeof(OutlinedTextControl),
                new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender, OnPropertyChanged));

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register(nameof(VerticalOffset), typeof(double), typeof(OutlinedTextControl),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(nameof(HorizontalOffset), typeof(double), typeof(OutlinedTextControl),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TextPaddingProperty =
            DependencyProperty.Register(nameof(TextPadding), typeof(Thickness), typeof(OutlinedTextControl),
                new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.AffectsRender));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public Brush FillBrush
        {
            get => (Brush)GetValue(FillBrushProperty);
            set => SetValue(FillBrushProperty, value);
        }

        public double OutlineThickness
        {
            get => (double)GetValue(OutlineThicknessProperty);
            set => SetValue(OutlineThicknessProperty, value);
        }

        public Brush OutlineBrush
        {
            get => (Brush)GetValue(OutlineBrushProperty);
            set => SetValue(OutlineBrushProperty, value);
        }

        public double VerticalOffset
        {
            get => (double)GetValue(VerticalOffsetProperty);
            set => SetValue(VerticalOffsetProperty, value);
        }

        public double HorizontalOffset
        {
            get => (double)GetValue(HorizontalOffsetProperty);
            set => SetValue(HorizontalOffsetProperty, value);
        }

        public Thickness TextPadding
        {
            get => (Thickness)GetValue(TextPaddingProperty);
            set => SetValue(TextPaddingProperty, value);
        }

        public OutlinedTextControl()
        {
            IsHitTestVisible = false;
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((OutlinedTextControl)d).InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            string text = Text;
            if (string.IsNullOrEmpty(text))
                return;

            string[] lines = text.Split('\n');
            double lineHeight = FontSize * 1.4;
            double x = TextPadding.Left - HorizontalOffset;
            double y = TextPadding.Top - VerticalOffset;

            Pen outlinePen = OutlineThickness > 0
                ? new Pen(OutlineBrush, OutlineThickness) { LineJoin = PenLineJoin.Round }
                : null;

            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    FormattedText formattedText = new FormattedText(
                        line,
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface(FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                        FontSize,
                        FillBrush,
                        VisualTreeHelper.GetDpi(this).PixelsPerDip);

                    Geometry geometry = formattedText.BuildGeometry(new Point(x, y));
                    if (geometry != null)
                        dc.DrawGeometry(FillBrush, outlinePen, geometry);
                }

                y += lineHeight;
            }
        }
    }
}
