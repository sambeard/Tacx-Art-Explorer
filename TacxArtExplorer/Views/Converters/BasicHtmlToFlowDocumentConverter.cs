using HtmlAgilityPack;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Xml;

namespace TacxArtExplorer.Views.Converters
{

    public sealed class BasicHtmlToFlowDocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var html = value as string ?? string.Empty;

            var hap = new HtmlDocument();
            hap.LoadHtml(html);
            var root = hap.DocumentNode.SelectSingleNode("//body") ?? hap.DocumentNode;

            var doc = new FlowDocument
            {
                FontFamily = new FontFamily("Segoe UI"),
                Foreground = Brushes.White,
                PagePadding = new System.Windows.Thickness(0)
            };

            foreach (var node in root.ChildNodes)
            {
                if (IsWhitespace(node)) continue;

                // paragraphs
                if (node.Name.Equals("p", StringComparison.OrdinalIgnoreCase))
                {
                    var p = NewParagraph();
                    AddInlines(p.Inlines, node);
                    doc.Blocks.Add(p);
                }
                else if (node.Name.Equals("br", StringComparison.OrdinalIgnoreCase))
                {
                    doc.Blocks.Add(NewParagraph()); // blank line
                }
                else if (node.Name == "#text")
                {
                    var p = NewParagraph();
                    p.Inlines.Add(new Run(HtmlEntity.DeEntitize(node.InnerText)));
                    doc.Blocks.Add(p);
                }
                else
                {
                    // any other element: wrap its content as a paragraph
                    var p = NewParagraph();
                    AddInlines(p.Inlines, node);
                    if (p.Inlines.Count > 0) doc.Blocks.Add(p);
                }
            }

            return doc;
        }

        static Paragraph NewParagraph() => new Paragraph { Margin = new System.Windows.Thickness(0, 0, 0, 12) };

        static bool IsWhitespace(HtmlNode n) =>
            n.NodeType == HtmlNodeType.Text && string.IsNullOrWhiteSpace(n.InnerText);

        static void AddInlines(InlineCollection inlines, HtmlNode node)
        {
            foreach (var child in node.ChildNodes)
            {
                switch (child.Name.ToLowerInvariant())
                {
                    case "#text":
                        var text = HtmlEntity.DeEntitize(child.InnerText);
                        if (!string.IsNullOrWhiteSpace(text))
                            inlines.Add(new Run(text));
                        break;

                    case "em":
                    case "i":
                        var em = new Span { FontStyle = FontStyles.Italic };
                        AddInlines(em.Inlines, child);
                        inlines.Add(em);
                        break;

                    case "a":
                        var href = child.GetAttributeValue("href", null);
                        var link = new Hyperlink();
                        if (Uri.TryCreate(href, UriKind.Absolute, out var uri))
                            link.NavigateUri = uri;

                        link.RequestNavigate += (s, e) =>
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(
                                    new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                            }
                            catch { /* ignore */ }
                            e.Handled = true;
                        };

                        AddInlines(link.Inlines, child);
                        inlines.Add(link);
                        break;

                    case "br":
                        inlines.Add(new LineBreak());
                        break;

                    case "p": // nested p: treat as line-broken content
                        inlines.Add(new LineBreak());
                        AddInlines(inlines, child);
                        inlines.Add(new LineBreak());
                        break;

                    default:
                        // ignore scripts/styles; recurse otherwise
                        if (child.Name is "script" or "style") break;
                        AddInlines(inlines, child);
                        break;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }

}
