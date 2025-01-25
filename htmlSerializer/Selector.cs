using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace htmlSerializer
{
   public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        // פונקציה סטטית הממירה מחרוזת של סלקטור לאובייקט Selector
        public static Selector Parse(string query)
        {
            var parts = query.Split(' ');
            var rootSelector = new Selector();
            var currentSelector = rootSelector;

            foreach (var part in parts)
            {
                var newSelector = new Selector();
                var tagNameMatch = Regex.Match(part, @"^[a-zA-Z]+");
                var idMatch = Regex.Match(part, @"^#(\w+)");
                var classMatch = Regex.Match(part, @"^\.([\w\-]+)");

                if (tagNameMatch.Success)
                    newSelector.TagName = tagNameMatch.Value;

                if (idMatch.Success)
                    newSelector.Id = idMatch.Groups[1].Value;

                if (classMatch.Success)
                    newSelector.Classes.Add(classMatch.Groups[1].Value);

                currentSelector.Child = newSelector;
                newSelector.Parent = currentSelector;

                currentSelector = newSelector;
            }

            return rootSelector;
        }
    }
}
