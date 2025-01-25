using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace htmlSerializer
{
 
    public class HtmlHelper
    {
        private readonly static HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        public string[] AllTags { get; private set; }
        public string[] SelfClosingTags { get; private set; }

        private HtmlHelper()
        {
            AllTags = LoadTagsFromJson("JSON-Files/HtmlTags");
            SelfClosingTags = LoadTagsFromJson("JSON-Files/HtmlVoidTags");
        }

        private string[] LoadTagsFromJson(string filePath)
        {
            if (File.Exists(filePath))
            {
                var jsonContent = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<string[]>(jsonContent) ?? Array.Empty<string>();
            }
            return Array.Empty<string>();
        }
    }
}


