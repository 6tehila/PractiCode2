using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace project2
{
    internal class HtmlHelper
    {
        public static readonly HtmlHelper _instance=new HtmlHelper();
        public static HtmlHelper Instance=>_instance;

        public string[] HtmlTags { get; set; }
        public string[] HtmlVoidTags { get; set; }

        private HtmlHelper()
        {
            HtmlTags = LoadTagFromFile("Files/HtmlTags.json");
            HtmlVoidTags = LoadTagFromFile("Files/HtmlVoidTags.json");
        }
        public string[] LoadTagFromFile(string file)
        {
            string allTagJson=File.ReadAllText(file);
            return JsonSerializer.Deserialize<string[]>(allTagJson);
           
        }
    }
}
