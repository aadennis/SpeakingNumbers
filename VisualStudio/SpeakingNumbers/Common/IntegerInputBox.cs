using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System.Windows.Input;

//https://github.com/Microsoft/maps-samples/blob/master/AreaSelector/AreaSelector/NumberInputBox.cs
// http://developer.nokia.com/community/wiki/Implementing_numerical_InputBox_in_Windows_Phone
// Note that the full set of keyboard enumerations is not available on Windows Phone 8.1 (but evidently
// may be on Silverlight for Phone)
// http://msdn.microsoft.com/en-US/library/windows/apps/windows.ui.xaml.input.inputscopenamevalue.aspx
// http://msdn.microsoft.com/en-us/library/system.windows.input.inputscopenamevalue(v=vs.105).aspx
// http://msdn.microsoft.com/en-us/library/windows/apps/xaml/system.windows.input.inputscopenamevalue(v=vs.105).aspx

namespace NumberSpeaker.Common
{
    /// <summary>
    /// TextBox class that only allows integer values. There is no way I know of to suppress visibly the dot
    /// key (although the password box manages it), so anything that is not a 0-9 character is removed in code here.
    /// </summary>
    public class IntegerInputBox : TextBox
    {

        public enum InputType {
                Integer = 0
        }

        public InputType Type { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public bool ValueOk { get; set; }

        public IntegerInputBox() : base() {
            TextChanged += TextChangedEvent;
            InputScope = new InputScope()
            {
                Names = {new InputScopeName() {NameValue = InputScopeNameValue.Number }}
            };
        }

        private void TextChangedEvent(object sender, TextChangedEventArgs e)
        {
            Text = ParseDouble(Text);
        }

        String ParseDouble(string input)
        {
            if (input.Length <= 0) return string.Empty;
            var output = "";

            for (int i = 0; i < input.Length; i++) {
                if ((Type == InputType.Integer) && (input[i] >= '0' && input[i] <= '9')) {
                    output = output + input[i];
                }
            }
            return output;
        }
    }
}
