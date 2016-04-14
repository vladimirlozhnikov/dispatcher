using System;
using System.Globalization;
using System.Windows.Forms;

namespace Dispatcher.UI.CustomControls
{
    public partial class NumericTextBox : TextBox
    {
        bool _allowSpace;

        // Restricts the entry of characters to digits (including hex), the negative sign,
        // the decimal point, and editing keystrokes (backspace).
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            NumberFormatInfo numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;
            string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
            string groupSeparator = numberFormatInfo.NumberGroupSeparator;
            string negativeSign = numberFormatInfo.NegativeSign;

            string keyInput = e.KeyChar.ToString();

            if (Char.IsDigit(e.KeyChar))
            {
                // Digits are OK
            }
            else if (keyInput.Equals(decimalSeparator) || keyInput.Equals(groupSeparator) ||
             keyInput.Equals(negativeSign))
            {
                // Decimal separator is OK
            }
            else if (e.KeyChar == '\b')
            {
                // Backspace key is OK
            }
            //    else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
            //    {
            //     // Let the edit control handle control and alt key combinations
            //    }
            else if (_allowSpace && e.KeyChar == ' ')
            {

            }
            else
            {
                // Swallow this invalid key and beep
                e.Handled = true;
                //    MessageBeep();
            }
        }

        private void TrimZero()
        {
            Text = Text.TrimStart('0');
        }

        public bool IsValid
        {
            get
            {
                TrimZero();

                int value;
                return int.TryParse(Text, out value);
            }
        }

        public bool IsDecimalValid
        {
            get
            {
                TrimZero();

                decimal value;
                return decimal.TryParse(Text, out value);
            }
        }

        public int IntValue
        {
            get
            {
                TrimZero();
                return Int32.Parse(Text);
            }
        }

        public decimal DecimalValue
        {
            get
            {
                TrimZero();

                CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                ci.NumberFormat.CurrencyDecimalSeparator = ".";

                return Decimal.Parse(Text, NumberStyles.Any, ci);
            }
        }

        public bool AllowSpace
        {
            set
            {
                _allowSpace = value;
            }

            get
            {
                return _allowSpace;
            }
        }
    }
}
