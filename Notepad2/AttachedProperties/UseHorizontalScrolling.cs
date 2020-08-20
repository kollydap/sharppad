using Notepad2.Preferences;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace Notepad2.AttachedProperties
{
    /// <summary>
    /// A class for allowing horizontal scrolling on any control that has a scrollviewer 
    /// </summary>
    public class UseHorizontalScrolling : BaseAttachedProperty<UseHorizontalScrolling, bool>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is UIElement element)
            {
                element.PreviewMouseWheel -= OnPreviewMouseWheel;

                if ((bool)e.NewValue)
                    element.PreviewMouseWheel += OnPreviewMouseWheel;
            }

            else throw new Exception("Attached property must be used with UIElement.");
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs args)
        {
            if (Keyboard.Modifiers != ModifierKeys.Shift)
                return;

            if (!PreferencesG.SCROLL_HORIZONTAL_WITH_SHIFT_MOUSEWHEEL)
                return;

            if (sender is UIElement element)
            {
                ScrollViewer scrollViewer = FindDescendant<ScrollViewer>(element);

                if (scrollViewer == null)
                    return;

                if (args.Delta < 0)
                    for (int i = 1; i <= SystemInformation.MouseWheelScrollLines; i++)
                        scrollViewer.LineRight();
                else
                    for (int i = 1; i <= SystemInformation.MouseWheelScrollLines; i++)
                        scrollViewer.LineLeft();

                args.Handled = true;
            }
        }

        private static T FindDescendant<T>(DependencyObject d) where T : DependencyObject
        {
            if (d == null)
                return null;

            int childCount = VisualTreeHelper.GetChildrenCount(d);

            for (var i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(d, i);
                T result = child as T ?? FindDescendant<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
