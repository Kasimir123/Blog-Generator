using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Blog_Generator.models
{
    internal class BlogPost
    {
        public string Name { get; set; }
        public string OriginalUrl { get; set; }
        public string Date { get; set; }
        public string ReadTime { get; set; }
        public string Description { get; set; }
        public string Markdown { get; set; } = "";
        public string Title { get; set; }

        private string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        public BlogPost(HtmlNode row)
        {
            this.Name = row.InnerText.Trim().Split("\n")[0].Split("/")[0];

            this.OriginalUrl = System.Web.HttpUtility.HtmlDecode(row.Descendants().First(x => x.Name == "a").Attributes["href"].Value);

            Console.WriteLine(this.Name);

            using (var client = new WebClient())
            {
                var contents = client.DownloadString("https://github.com" + this.OriginalUrl);

                var doc = new HtmlDocument();
                doc.LoadHtml(contents);

                var file_box = doc.DocumentNode.Descendants().Where(x => x.Attributes.Contains("aria-labelledby") && x.Attributes["aria-labelledby"].Value.Equals("files")).First();

                var files = file_box.Descendants().Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value.Contains("Box-row"));


                foreach (var file in files)
                {
                    var filename = System.Web.HttpUtility.HtmlDecode(file.InnerText.Trim().Split("\n")[0].Split("/")[0]);
                    var url = System.Web.HttpUtility.HtmlDecode(row.Descendants().First(x => x.Name == "a").Attributes["href"].Value);
                    if (filename.Equals("README.md"))
                    {

                        var markdown = client.DownloadString("https://raw.githubusercontent.com" + this.OriginalUrl.Replace("/tree", "") + "/README.md");

                        this.Markdown = markdown.Replace("](./Photos", "](" + "https://raw.githubusercontent.com" + this.OriginalUrl.Replace("/tree", "") + "/Photos");
                    }
                    else if (filename.Equals(".config"))
                    {

                        var configfile = client.DownloadString("https://raw.githubusercontent.com" + this.OriginalUrl.Replace("/tree", "") + "/.config");

                        this.Date = configfile.Split("\n")[0].Split("||")[1].Trim();
                        this.Description = configfile.Split("\n")[1].Split("||")[1].Trim();
                        this.ReadTime = configfile.Split("\n")[2].Split("||")[1].Trim();
                        this.Title = configfile.Split("\n")[4].Split("||")[1].Trim();
                    }
                }

                Console.WriteLine(this.Name);
            }

        }
    }
}

