using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog_Generator.configs
{
    internal class General
    {
        public string Title { get; set; }
        public string OutputPath { get; set; }
        public string AboutMe { get; set; }
        public string AboutBlog { get; set; }
        public string SkillsAndExperience { get; set; }
        public string SideProjects { get; set; }


        public General(string Title = "", string OutputPath = "", string AboutMe = "", string AboutBlog = "", string SkillsAndExperience = "", string SideProjects = "")
        {
            this.Title = Title.Trim();
            this.OutputPath = OutputPath.Trim();
            this.AboutMe = AboutMe.Trim();
            this.AboutBlog = AboutBlog.Trim();  
            this.SkillsAndExperience= SkillsAndExperience.Trim();
            this.SideProjects = SideProjects.Trim();
        }

    }
}
