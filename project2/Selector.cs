using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project2
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public Selector Parent { get; set; }
        public Selector Child { get; set; }


        public static Selector FromQueryString(string queryString)
        {
            var selectors = queryString.Split(' ');
            Selector root = null;
            var newSelector = new Selector();
            var currentSelector = new Selector();
            var parts = queryString.Split(' ');
            var tagName = "";
            foreach (var queryStr in selectors)
            {
                newSelector = new Selector();

                var query = queryStr;

                if (query.IndexOf('#') > 0)
                    tagName = query.Substring(0, query.IndexOf('#'));
                else if (query.IndexOf('.') > 0)
                    tagName = query.Substring(0, query.IndexOf('.'));
                else tagName = query;
                if (HtmlHelper.Instance.HtmlTags.Contains(tagName))
                {
                    currentSelector.TagName = tagName;
                    tagName = "";
                    if (query.IndexOf('#') > 0)
                        query = query.Substring(query.IndexOf('#'));
                    else if (query.IndexOf('.') > 0)
                        query = query.Substring(query.IndexOf('.'));

                }
                if (query.IndexOf('#') >= 0)
                {
                    if (query.IndexOf('.') > 0)
                    {
                        currentSelector.Id = query.Substring(1, query.IndexOf('.') - 1);
                        parts = query.Split('.');
                        currentSelector.Classes = new List<string>(parts.Skip(1));

                    }
                    else
                        currentSelector.Id = query.Substring(1);

                }
                else
                {

                    parts = query.Split('.');
                    currentSelector.Classes = new List<string>(parts.Skip(1));
                }
                if (root == null)
                {
                    root = currentSelector;

                }
                if (newSelector != null)
                {
                    currentSelector.Child = newSelector;
                    newSelector.Parent = currentSelector;
                }

                currentSelector = newSelector;
            }
            if (currentSelector.Parent != null)
                currentSelector.Parent.Child = null;
            return root;
        }
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            if (!string.IsNullOrEmpty(TagName))
            {
                result.Append(TagName);
            }
            if (!string.IsNullOrEmpty(Id))
            {
                result.Append($" id: #{Id} ");
            }
            if (Classes != null && Classes.Count > 0)
            {
                result.Append($" classes: .{string.Join(".", Classes)} ");
            }
            if (Child != null)
            {
                result.Append($" \n child: {Child.ToString()} ");
            }
            return result.ToString();
        }
    }
}