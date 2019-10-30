using System;
using System.Windows;
using System.Windows.Controls;

namespace ImageFilters.Helpers
{
    public static class ContentControlHelper
    {

        private static void SourceResourceKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var element = d as ContentControl;
            if (element != null)
            {

                element.SetResourceReference(ContentControl.ContentProperty, e.NewValue);
            }
        }

        public static readonly DependencyProperty SourceResourceKeyProperty = DependencyProperty.RegisterAttached("SourceResourceKey",
            typeof(object),
            typeof(ContentControlHelper),
            new PropertyMetadata(String.Empty, SourceResourceKeyChanged));

        public static void SetSourceResourceKey(ContentControl element, object value)
        {

            element.SetValue(SourceResourceKeyProperty, value);
        }

        public static object GetSourceResourceKey(ContentControl element)
        {

            return element.GetValue(SourceResourceKeyProperty);
        }
    }

}
