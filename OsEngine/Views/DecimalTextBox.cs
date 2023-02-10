using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OsEngine.Views
{
    public class DecimalTextBox:TextBox
    {
        public DecimalTextBox()
        {
            this.PreviewTextInput += DecimalTextBox_PreviewTextInput;
            this.TextChanged += DecimalTextBox_TextChanged;
        }

        private void DecimalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            tb.Select(tb.Text.Length, 0);
        }

        private void DecimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0) && !e.Text.Contains("."))
            {
                e.Handled = true;
            }
        }
    }
}
