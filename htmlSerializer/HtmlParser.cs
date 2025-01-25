//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace htmlSerializer
//{
//    class HtmlParser
//    {
//        private static readonly Regex TagRegex = new("<(/?[^\\s>]+)(.*?)>");
//        private static readonly Regex AttributeRegex = new("([^\\s]*?)=\"(.*?)\"");

//        public HtmlElement Parse(string html)
//        {
//            var root = new HtmlElement("root");
//            var current = root;
//            var tokens = TagRegex.Split(html).Where(s => s.Length > 1);

//            foreach (var token in tokens)
//            {
//                if (token.StartsWith("/"))
//                {
//                    current = current.Parent ?? root;
//                }
//                else if (TagRegex.IsMatch("<" + token + ">"))
//                {
//                    var match = TagRegex.Match("<" + token + ">");
//                    var tagName = match.Groups[1].Value;
//                    var attributesPart = match.Groups[2].Value;

//                    var newElement = new HtmlElement(tagName) { Parent = current };

//                    foreach (Match attr in AttributeRegex.Matches(attributesPart))
//                    {
//                        newElement.Attributes[attr.Groups[1].Value] = attr.Groups[2].Value;
//                    }

//                    current.Children.Add(newElement);
//                    current = newElement;
//                }
//                else
//                {
//                    current.InnerHtml += token.Trim();
//                }
//            }

//            return root;
//        }
//    }
//}



using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Threading.Tasks;

namespace htmlSerializer
{
    class HtmlParser
    {
        private static readonly Regex TagRegex = new("<(/?[^\\s>]+)(.*?)>");
        private static readonly Regex AttributeRegex = new("([^\\s]*?)=\"(.*?)\"");

        // רשימה של תגיות שאין להן סגירה
        private readonly HashSet<string> selfClosingTags;

        public HtmlParser()
        {
            // טוען את רשימת התגיות מהקובץ JSON
            selfClosingTags = LoadSelfClosingTags();
        }

        private HashSet<string> LoadSelfClosingTags()
        {
            var filePath = @"JSON-Files\tagsJson.json";  // שים לב כאן לשים את הנתיב האמיתי שלך

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file was not found.", filePath);
            }

            var json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<HashSet<string>>(json) ?? new HashSet<string>();
        }

        public HtmlElement Parse(string html)
        {
            var root = new HtmlElement("root");
            var current = root;
            var tokens = TagRegex.Split(html).Where(s => s.Length > 1);

            foreach (var token in tokens)
            {
                if (token.StartsWith("/"))
                {
                    current = current.Parent ?? root;
                }
                else if (TagRegex.IsMatch("<" + token + ">"))
                {
                    var match = TagRegex.Match("<" + token + ">");
                    var tagName = match.Groups[1].Value;
                    var attributesPart = match.Groups[2].Value;

                    var newElement = new HtmlElement(tagName) { Parent = current };

                    // טיפול ב-Attributes
                    foreach (Match attr in AttributeRegex.Matches(attributesPart))
                    {
                        newElement.Attributes[attr.Groups[1].Value] = attr.Groups[2].Value;
                    }

                    // טיפול ב-Class (אם קיים)
                    if (newElement.Attributes.ContainsKey("class"))
                    {
                        newElement.Classes = newElement.Attributes["class"].Split(' ').ToList();
                    }

                    // טיפול ב-Id (אם קיים)
                    if (newElement.Attributes.ContainsKey("id"))
                    {
                        newElement.Id = newElement.Attributes["id"];
                    }

                    current.Children.Add(newElement);
                    current = newElement;

                    // אם מדובר בתגית self-closing
                    if (selfClosingTags.Contains(tagName) || token.EndsWith("/>"))
                    {
                        // אלמנט self-closing - אין צורך להמשיך להוסיף אותו
                        current = current.Parent ?? root;
                    }
                }
                else
                {
                    // טקסט פנימי של האלמנט
                    current.InnerHtml += token.Trim();
                }
            }

            return root;
        }
    }

}
