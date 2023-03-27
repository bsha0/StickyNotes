using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace StickyNotes.UI.Helpers
{
    public class RichTextBoxHelper : DependencyObject
    {
        #region IsUpdating
        public static bool GetIsUpdating(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsUpdatingProperty);
        }

        public static void SetIsUpdating(DependencyObject obj, bool value)
        {
            obj.SetValue(IsUpdatingProperty, value);
        }

        public static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(RichTextBoxHelper), new PropertyMetadata(false));
        #endregion

        #region DocumentXaml
        public static string GetDocumentXaml(DependencyObject obj)
        {
            return (string)obj.GetValue(DocumentXamlProperty);
        }

        public static void SetDocumentXaml(DependencyObject obj, string value)
        {
            obj.SetValue(DocumentXamlProperty, value);
        }

        public static readonly DependencyProperty DocumentXamlProperty =
            DependencyProperty.RegisterAttached(
                "DocumentXaml",
                typeof(string),
                typeof(RichTextBoxHelper),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = string.Empty,
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = (s, e) =>
                    {
                        var richTextBox = (RichTextBox)s;
                        var xaml = e.NewValue as string;
                        var doc = richTextBox.Document;
                        var range = new TextRange(doc.ContentStart, doc.ContentEnd);
                        if (!string.IsNullOrEmpty(xaml))
                        {
                            if (!GetIsUpdating(richTextBox))
                            {
                                range.Load(new MemoryStream(Encoding.UTF8.GetBytes(xaml)), DataFormats.Xaml);
                            }
                        }
                        richTextBox.TextChanged += (s, e) =>
                        {
                            if (GetIsUpdating(richTextBox)) return;

                            SetIsUpdating(richTextBox, true);
                            if (string.IsNullOrWhiteSpace(range.Text))
                            {
                                SetDocumentXaml(richTextBox, string.Empty);
                            }
                            else
                            {
                                MemoryStream buffer = new MemoryStream();
                                range.Save(buffer, DataFormats.Xaml);
                                SetDocumentXaml(richTextBox,
                                    Encoding.UTF8.GetString(buffer.ToArray()));
                            }
                            SetIsUpdating(richTextBox, false);
                        };
                    }
                });
        #endregion
    }
}
