using System.Windows;

namespace Daekage.Behavior
{
    public class FocusVisualTreeChanger
    {
        public static readonly DependencyProperty IsChangedProperty =
            DependencyProperty.RegisterAttached("IsChanged", typeof(bool), typeof(FocusVisualTreeChanger),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, IsChangedCallback));

        public static bool GetIsChanged(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsChangedProperty);
        }

        public static void SetIsChanged(DependencyObject obj, bool value)
        {
            obj.SetValue(IsChangedProperty, value);
        }

        private static void IsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!true.Equals(e.NewValue)) return;
            switch (d)
            {
                case FrameworkContentElement contentElement:
                    contentElement.FocusVisualStyle = null;
                    return;
                case FrameworkElement element:
                    element.FocusVisualStyle = null;
                    break;
            }
        }
    }
}
