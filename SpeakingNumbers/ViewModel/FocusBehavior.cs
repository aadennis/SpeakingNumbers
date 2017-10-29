using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WpWinNl.Behaviors;

namespace NumberSpeaker.ViewModel
{
    // http://dotnet.dzone.com/articles/writing-behaviors-pcl-windows
    // http://igrali.com/2013/09/18/focus-a-textbox-from-viewmodel-using-a-simple-behavior/
    public class FocusBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.GotFocus += (sender, args) => IsFocused = true;
            AssociatedObject.LostFocus += (sender, a) => IsFocused = false;
            AssociatedObject.TextChanged += (sender, a) => {
                if (!IsFocused) {
                    AssociatedObject.Focus(FocusState.Programmatic);
                    AssociatedObject.Select(AssociatedObject.Text.Length, 0);
                }
                
            };
           
            base.OnAttached();
        }

       

        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register(
                "IsFocused",
                typeof(bool),
                typeof(FocusBehavior),
                new PropertyMetadata(false));

        public bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            set { SetValue(IsFocusedProperty, value); }
        }
    }
}
