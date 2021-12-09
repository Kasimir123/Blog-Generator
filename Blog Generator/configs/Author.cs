using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog_Generator.configs
{
    internal class Author
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Github { get; set; }
        public string Twitter { get; set; }
        public string Linkedin { get; set; }

        public string GetInTouch { get; set; }

        public Author(string Name = "", string Description = "", string Github = "", string Twitter = "", string Linkedin = "", string GetInTouch = "")
        {
            this.Name = Name.Trim();
            this.Description = Description.Trim();
            this.Github = Github.Trim();
            this.Twitter = Twitter.Trim();
            this.Linkedin = Linkedin.Trim();
            this.GetInTouch = GetInTouch.Trim();
        }
    }
}
