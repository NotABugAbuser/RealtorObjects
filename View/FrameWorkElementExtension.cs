using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace RealtorObjects.View
{
    public class FrameworkElementExtension
    {
        public static readonly DependencyProperty IsPressedProperty = DependencyProperty.RegisterAttached("IsPressed", typeof(bool),
        typeof(FrameworkElementExtension), new PropertyMetadata(false));

        public static readonly DependencyProperty AttachIsPressedProperty = DependencyProperty.RegisterAttached("AttachIsPressed", typeof(bool), typeof(FrameworkElementExtension), new PropertyMetadata(false, PropertyChangedCallback));

        public static void PropertyChangedCallback(DependencyObject depObj, DependencyPropertyChangedEventArgs args) {
            FrameworkElement element = (FrameworkElement)depObj;
            if (element != null) {
                if ((bool)args.NewValue) {
                    element.MouseDown += new MouseButtonEventHandler(element_MouseDown);
                    element.MouseUp += new MouseButtonEventHandler(element_MouseUp);
                    element.MouseLeave += new MouseEventHandler(element_MouseLeave);
                } else {
                    element.MouseDown -= new MouseButtonEventHandler(element_MouseDown);
                    element.MouseUp -= new MouseButtonEventHandler(element_MouseUp);
                    element.MouseLeave -= new MouseEventHandler(element_MouseLeave);
                }
            }
        }

        static void element_MouseLeave(object sender, MouseEventArgs e) {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null) {
                element.SetValue(IsPressedProperty, false);
            }
        }
        static void element_MouseUp(object sender, MouseButtonEventArgs e) {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null) {
                element.SetValue(IsPressedProperty, false);
            }
        }
        static void element_MouseDown(object sender, MouseButtonEventArgs e) {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null) {
                element.SetValue(IsPressedProperty, true);
            }
        }
        public static bool GetIsPressed(UIElement element) {
            return (bool)element.GetValue(IsPressedProperty);
        }
        public static void SetIsPressed(UIElement element, bool val) {
            element.SetValue(IsPressedProperty, val);
        }
        public static bool GetAttachIsPressed(UIElement element) {
            return (bool)element.GetValue(AttachIsPressedProperty);
        }
        public static void SetAttachIsPressed(UIElement element, bool val) {
            element.SetValue(AttachIsPressedProperty, val);
        }
    }
}
