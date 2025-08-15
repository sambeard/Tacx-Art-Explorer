using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace TacxArtExplorer.Views
{
    public partial class ArtDetailView : UserControl
    {
        public ArtDetailView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            DataContextChanged += OnDataContextChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Ensure initial layout uses title-only view after first measure
            Dispatcher.BeginInvoke((Action)ResetForCurrent, DispatcherPriority.Loaded);
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyPropertyChanged oldPc)
                oldPc.PropertyChanged -= Vm_PropertyChanged;
            if (e.NewValue is INotifyPropertyChanged pc)
                pc.PropertyChanged += Vm_PropertyChanged;

            // New item (or VM) -> reset to title-only
            Dispatcher.BeginInvoke((Action)ResetForCurrent, DispatcherPriority.Background);
        }

        private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // When the selected item changes (property "Current"), reset the view
            if (string.Equals(e.PropertyName, "Current", StringComparison.Ordinal))
                Dispatcher.BeginInvoke((Action)ResetForCurrent, DispatcherPriority.Background);
        }

        private void ResetForCurrent()
        {
            UpdateTopSpacer();
            MainScroll.ScrollToVerticalOffset(0);  // show only title at the bottom
            UpdateOverlayOpacity();
        }

        private void MainScroll_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTopSpacer();
            UpdateOverlayOpacity();
        }

        private void MainScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            UpdateOverlayOpacity();
        }

        private void UpdateTopSpacer()
        {
            // viewport - title height (clamped >= 0) ensures "title-only" at offset 0, all sizes
            double view = MainScroll.ViewportHeight;
            double title = TitlePanel.ActualHeight;
            double h = Math.Max(0, view - title);
            TopSpacer.Height = h;
        }

        private void UpdateOverlayOpacity()
        {
            // Fade overlay from 0..1 across the spacer; after spacer -> fully opaque
            double h = TopSpacer.Height;
            double y = MainScroll.VerticalOffset;

            double t;
            if (h <= 1)
                t = y > 0 ? 1 : 0;
            else
                t = Math.Max(0, Math.Min(1, y / h));

            ReadingOverlay.Opacity = t;
        }
        private void Description_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Stop the inner viewer from handling it
            e.Handled = true;

            // Re-raise as a normal MouseWheel event on the outer ScrollViewer
            var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = sender
            };
            MainScroll.RaiseEvent(args);
        }
    }
}
