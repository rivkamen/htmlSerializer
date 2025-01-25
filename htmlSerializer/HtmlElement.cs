using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;

namespace htmlSerializer
{

    public class HtmlElement
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new();
        public List<string> Classes { get; set; } = new();
        public string InnerHtml { get; set; } = string.Empty;
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new();

        public HtmlElement(string name)
        {
            Name = name;
        }
        //public IEnumerable<HtmlElement> Descendants()
        //{
        //    var queue = new Queue<HtmlElement>();
        //    queue.Enqueue(this);

        //    while (queue.Count > 0)
        //    {
        //        var current = queue.Dequeue();
        //        yield return current;

        //        foreach (var child in current.Children)
        //            queue.Enqueue(child);
        //    }
        //}

        public IEnumerable<HtmlElement> Descendants()
        {
            var queue = new Queue<HtmlElement>();
            queue.Enqueue(this);  // דחוף את האלמנט הנוכחי לתור

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();  // שלוף את האלמנט הראשון בתור
                yield return current;  // החזר אותו

                foreach (var child in current.Children)  // דחוף את הילדים של האלמנט הנוכחי לתור
                {
                    queue.Enqueue(child);
                }
            }
        }


        // פונקציה למעבר על כל האבות של אלמנט
        public IEnumerable<HtmlElement> Ancestors()
        {
            var current = this.Parent;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        // חיפוש אלמנטים בעץ לפי סלקטור
        public IEnumerable<HtmlElement> FindElements(Selector selector)
        {
            var result = new HashSet<HtmlElement>();
            FindElementsRecursive(this, selector, result);
            return result;
        }

        private void FindElementsRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> result)
        {
            var descendants = element.Descendants();
            var matchingDescendants = descendants.Where(e => MatchesSelector(e, selector));

            foreach (var descendant in matchingDescendants)
            {
                result.Add(descendant);
            }

            // אם יש ילד נוסף, נבצע קריאה ריקורסיבית עבורו
            if (selector.Child != null)
            {
                foreach (var child in element.Children)
                {
                    FindElementsRecursive(child, selector.Child, result);
                }
            }
        }

        // פונקציה לבדוק אם אלמנט תואם לסלקטור
        private bool MatchesSelector(HtmlElement element, Selector selector)
        {
            if (!string.IsNullOrEmpty(selector.TagName) && element.Name != selector.TagName)
                return false;

            if (!string.IsNullOrEmpty(selector.Id) && element.Id != selector.Id)
                return false;

            if (selector.Classes.Any() && !selector.Classes.All(cls => element.Classes.Contains(cls)))
                return false;

            return true;
        }
    }
}
    

    
    

