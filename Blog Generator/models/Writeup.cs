using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Blog_Generator.models
{
    internal class Writeup
    {
        public string Name { get; set; }
        public string OriginalUrl { get; set; }

        public string Markdown { get; set; } = "";

        public Writeup(HtmlNode row, string path)
        {
            this.Name = System.Web.HttpUtility.HtmlDecode(row.InnerText.Trim().Split("\n")[0].Split("/")[0]);

            if (this.Name.Equals("Original-Files"))
            {
                this.Name = path.Split("/").Last();
                path = path.Replace(this.Name, "");
            }

            if (!this.Name.Equals("README.md"))
            {
                this.OriginalUrl = row.Descendants().First(x => x.Name == "a").Attributes["href"].Value;

                Console.WriteLine(this.Name);

                using (var client = new WebClient())
                {
                    var contents = client.DownloadString("https://raw.githubusercontent.com" + path.Replace("/tree", "") + "/" + this.Name + "/README.md");

                    this.Markdown = contents.Replace("](./Photos", "](" + "https://raw.githubusercontent.com" + path.Replace("/tree", "") + "/" + this.Name + "/Photos");
                }
            }
        }
    }
}