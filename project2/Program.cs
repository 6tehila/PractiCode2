using project2;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

static HtmlElement Serialize(List<string> htmlLines)
{
    var root = new HtmlElement();
    var currentElement = root;

    foreach (var line in htmlLines)
    {
        var firstWord = line.Split(' ')[0];

        if (firstWord == "/html")
        {
            break; // Reached end of HTML
        }
        else if (firstWord.StartsWith("/"))
        {
            if (currentElement.Parent != null) // Make sure there is a valid parent
            {
                currentElement = currentElement.Parent; // Go to previous level in the tree
            }
        }
        else if (HtmlHelper.Instance.HtmlTags.Contains(firstWord))
        {
            var newElement = new HtmlElement();
            newElement.Name = firstWord;

            // Handle attributes
            var restOfString = line.Remove(0, firstWord.Length);
            var attributes = Regex.Matches(restOfString, "([a-zA-Z]+)=\\\"([^\\\"]*)\\\"")
                .Cast<Match>()
                .Select(m => $"{m.Groups[1].Value}=\"{m.Groups[2].Value}\"")
                .ToList();

            if (attributes.Any(attr => attr.StartsWith("class")))
            {
                var attributesClass = attributes.First(attr => attr.StartsWith("class"));
                var classes = attributesClass.Split('=')[1].Trim('"').Split(' ');
                newElement.Classes.AddRange(classes);
            }

            newElement.Attributes.AddRange(attributes);


            var idAttribute = attributes.FirstOrDefault(a => a.StartsWith("id"));
            if (!string.IsNullOrEmpty(idAttribute))
            {
                newElement.Id = idAttribute.Split('=')[1].Trim('"');
            }

            newElement.Parent = currentElement;
            currentElement.Children.Add(newElement);

            // Check if self-closing tag
            if (line.EndsWith("/") || HtmlHelper.Instance.HtmlVoidTags.Contains(firstWord))
            {
                currentElement = newElement.Parent;
            }
            else
            {
                currentElement = newElement;
            }
        }
        else
        {
            // Text content
            currentElement.InnerHtml = line;
        }
    }

    return root;
}

async Task<string> Load(string url)
{
    using (var client = new HttpClient())
    {
        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }
}


var html = await Load("https://learn.malkabruk.co.il/");


var clean = new Regex("\\s+").Replace(html, " ");
var lines = new Regex("<(.*?>)").Split(clean).Where(l => l.Length > 0);
var root = Serialize(lines.ToList());


string query = "div header";

var selector = Selector.FromQueryString(query);
Console.Write(" selector: ");
Console.WriteLine(selector);
var all = root.FindElements(selector);
foreach (var item in all)
{
    Console.WriteLine(item + "\n \n");
}
foreach (var item in all)
{
    Console.WriteLine("\n \n \n ---Ancestors----");
    var ancestors = item.Ancestors();
    foreach (var i in ancestors)
    {
        Console.WriteLine(i);
    }
}
Console.ReadLine();

