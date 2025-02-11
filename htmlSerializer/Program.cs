

using htmlSerializer;
using System.Text.RegularExpressions;

class Program
{
    static async Task Main()
    {
        var url = "https://www.scaler.com/topics/self-closing-tags-in-html/";
        var html = await Load(url);
        var cleanHtml = new Regex("\\s").Replace(html, "");

        HtmlParser parser = new HtmlParser();
        HtmlElement root = parser.Parse(cleanHtml);

        Console.WriteLine("Parsing completed.");

        // הדפסה של שמות התגיות שנמצאו בעץ
        PrintElementNames(root);

        Console.ReadLine();
    }

    static async Task<string> Load(string url)
    {
        try
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
            return string.Empty;  // או כל פעולה אחרת להחזרת נתונים במקרה של שגיאה
        }
    }

    static void PrintElementNames(HtmlElement root)
    {
        var queue = new Queue<HtmlElement>();  // יצירת תור
        queue.Enqueue(root);  // דחיפת האלמנט הראשוני לתור

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();  // שליפת האלמנט הראשון בתור
            Console.WriteLine($"Element: {current.Name}");  // הדפסת שם התגית

            // דחיפת כל הילדים של האלמנט הנוכחי לתור
            foreach (var child in current.Children)
            {
                queue.Enqueue(child);
            }
        }
    }

}

