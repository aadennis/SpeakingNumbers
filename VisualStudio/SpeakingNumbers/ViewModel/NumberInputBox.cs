using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace NumberSpeaker.ViewModel
{
    public class NumberInputBox : TextBox {
        public InputType Type { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public bool ValueOk { get; set; }


        public NumberInputBox() :base() {
            TextChanged += TextChangedEvent;
            InputScope = new InputScope()
            {
                Names = {new InputScopeName {NameValue = InputScopeNameValue.Number}}
            };
            Type = InputType.Float;
            MaxValue = MinValue = 0;
            ValueOk = false;
        }

        private void TextChangedEvent(object sender, TextChangedEventArgs e)
        {
            Text = ParseDouble(Text);
            if (Text == null) {
                throw new Exception("TextChangedEvent: number could not be parsed as a double");
            }
            SelectionStart = Text.Length;

            if (MaxValue == MinValue) return;
            double d;
            if (!double.TryParse(Text, out d)) return;
            if (d < MinValue || d > MaxValue)
            {
                ValueOk = false;
                Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                ValueOk = true;
                Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        String ParseDouble(string input) {
            if (input.Length <= 0) return "";
            var outBut = "";
            var foundSeparator = false;

            for (var i = 0; i < input.Length; i++)
            {
                if ((Type == InputType.Integer
                     || Type == InputType.Float)
                    && ((i == 0) && input[0] == '-'))
                {
                    outBut = outBut + input[i];
                }
                else if ((input[i] >= '0' && input[i] <= '9'))
                {
                    outBut = outBut + input[i];
                }
                else if (Type == InputType.Float
                         || Type == InputType.FloatPositive)
                {
                    if (foundSeparator) continue;
                    var sepa = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
                    if (sepa.Length <= 0) continue;
                    if (sepa[0] != input[i] && '.' != input[i] && ',' != input[i]) continue;
                    outBut = outBut + sepa[0];
                    foundSeparator = true;
                }
            }
            return outBut;
        }

        public enum InputType
        {
            Integer = 0,
            IntegerPositive = 1,
            Float = 2,
            FloatPositive = 3,
        }
    }

   
}
