using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NumberSpeaker.ViewModel
{
    public static class FocusExtension
    {
        public static bool GetIsFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }


        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }


        public static readonly DependencyProperty IsFocusedProperty =
                DependencyProperty.RegisterAttached(
                    "IsFocused", typeof(bool), typeof(FocusExtension),
  new PropertyMetadata(false, OnIsFocusedPropertyChanged));


        private static void OnIsFocusedPropertyChanged(DependencyObject d,
                DependencyPropertyChangedEventArgs e)
        {
            var uie = (TextBox) d;
            if ((bool)e.NewValue) {
                uie.Focus(FocusState.Programmatic);
            }
        }
    }


}
