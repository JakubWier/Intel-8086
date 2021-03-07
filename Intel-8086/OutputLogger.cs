using System.Windows;
using System.Windows.Controls;
using System.Text;

namespace Intel_8086
{
    public partial class OutputLogger : Window
    {
        TextBlock outputBlock;
        StringBuilder stringBuilder;

        public OutputLogger(TextBlock textBlock)
        {
            outputBlock = textBlock;
            stringBuilder = new StringBuilder();
        }
        public void WriteLog(string text)
        {
            stringBuilder.Append(text);
            outputBlock.Text = "ASD";
        }
    }
}
