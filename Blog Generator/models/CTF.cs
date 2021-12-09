using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Blog_Generator.models
{
    internal class CTF
    {
        public string Name { get; set; }
        public string OriginalUrl { get; set; }

        public List<Writeup> writeups { get; set; } = new List<Writeup>();

        private string[] months = { "January","February","March","April","May","June","July", "August","September","October","November","December" };

        public string PrettyPrint()
        {
            return months[Int32.Parse(this.Name.Split("-")[1]) - 1] + ", " + this.Name.Split("-")[0] + " " + (this.Name.Split("-")[2]);
        }
        public CTF(HtmlNode row)
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

                var files = file_box.Descendants().Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value.Contains("Box-row")).Skip(1);

                if (row.InnerText.Trim().Split("\n")[0].Split("/").Count() == 2)
                    writeups.Add(new Writeup(files.First(), this.OriginalUrl));

                else
                {
                    foreach (var file in files)
                    {
                        writeups.Add(new Writeup(file, this.OriginalUrl));
                    }
                }
            }

        }
    }
}
