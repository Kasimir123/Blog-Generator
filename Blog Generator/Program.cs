using Blog_Generator.configs;
using Blog_Generator.models;
using HtmlAgilityPack;
using Markdig;
using Markdig.SyntaxHighlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;




namespace Blog_Generator
{

    public static class Utils
    {

        // From MSDN https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                if (!File.Exists(tempPath))
                    file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        public static void LogStatus(string msg)
        {
            Console.WriteLine(msg);
        }
    }

    public class Program
    {
        internal Author AuthorData { get; set; } = new Author();
        internal General GeneralConfig { get; set; } = new General();

        public string header { get; set; } = "";
        public string footer { get; set; } = "";

        internal List<CTF> ctfs = new List<CTF>();

        public void GetAuthorData()
        {
            var lines = File.ReadLines("Templates/config/AuthorData.txt", Encoding.UTF8).ToArray();

            AuthorData = new Author(lines[0].Split("::")[1], lines[1].Split("::")[1], lines[2].Split("::")[1], lines[3].Split("::")[1], 
                lines[4].Split("::")[1], lines[5].Split("::")[1]);

            Utils.LogStatus("Querying author data");
        }

        public void GetGeneralData()
        {
            var lines = File.ReadLines("Templates/config/GeneralConfig.txt", Encoding.UTF8).ToArray();

            GeneralConfig = new General(lines[0].Split("::")[1], lines[1].Split("::")[1], lines[2].Split("::")[1], lines[3].Split("::")[1], 
                lines[4].Split("::")[1], lines[5].Split("::")[1]);

            Utils.LogStatus("Querying general data");
        }

        public void GenerateOutputFolder()
        {

            if (Directory.Exists(GeneralConfig.OutputPath))
            {
                if (!Directory.Exists(GeneralConfig.OutputPath + "\\blog"))
                {
                    Directory.CreateDirectory(GeneralConfig.OutputPath + "\\blog");
                }

                Utils.DirectoryCopy("Templates/assets/", GeneralConfig.OutputPath + "\\blog\\assets", true);

                Utils.LogStatus("Directory made and assets copied");
            }
        }

        public void GenerateHeader()
        {
            header = File.ReadAllText("Templates/html/partials/header.html");
            header = header.Replace(Constants.AUTHOR_NAME, AuthorData.Name);
            header = header.Replace(Constants.AUTHOR_DESCRIPTION, AuthorData.Description);
            header = header.Replace(Constants.GITHUB, AuthorData.Github);
            header = header.Replace(Constants.TWITTER, AuthorData.Twitter);
            header = header.Replace(Constants.LINKEDIN, AuthorData.Linkedin);
            header = header.Replace(Constants.GET_IN_TOUCH, AuthorData.GetInTouch);

            Utils.LogStatus("Header file generated");
        }

        public void GenerateFooter()
        {

            footer = File.ReadAllText("Templates/html/partials/footer.html");

            Utils.LogStatus("Footer file generated");
        }

        public string GenerateBlogPosts()
        {
            var content = File.ReadAllText("Templates/html/partials/blog-post.html");

            content += content;
            content += content;
            content += content;

            return content;
        }

        public string GenerateUpdates()
        {
            var content = File.ReadAllText("Templates/html/partials/update.html");
            var output = new StringBuilder();
            var lines = File.ReadLines("Templates/config/updates.txt", Encoding.UTF8).ToArray();

            Array.Reverse(lines);

            foreach (var line in lines)
            {
                var line_content = content.Replace(Constants.BLOG_TITLE, line.Split("||")[1]);
                line_content = line_content.Replace(Constants.BLOG_DATE, line.Split("||")[0]);
                line_content = line_content.Replace(Constants.BLOG_DESC, line.Split("||")[2]);

                output.Append(line_content);
            }

            return output.ToString();
        }

        public void WriteIndexFile()
        {
            var content = File.ReadAllText("Templates/html/index.html");

            content = content.Replace(Constants.TITLE, GeneralConfig.Title);
            content = content.Replace(Constants.AUTHOR_NAME, AuthorData.Name);
            content = content.Replace(Constants.HEADER, header);
            content = content.Replace(Constants.FOOTER, footer);
            content = content.Replace(Constants.NEWS, GenerateUpdates());

            File.WriteAllText(GeneralConfig.OutputPath + "\\blog\\index.html", content);
        }

        public void WriteAboutFile()
        {
            var content = File.ReadAllText("Templates/html/about.html");

            content = content.Replace(Constants.TITLE, GeneralConfig.Title);
            content = content.Replace(Constants.AUTHOR_NAME, AuthorData.Name);
            content = content.Replace(Constants.HEADER, header);
            content = content.Replace(Constants.FOOTER, footer);
            content = content.Replace(Constants.ABOUT_ME, GeneralConfig.AboutMe);
            content = content.Replace(Constants.ABOUT_BLOG, GeneralConfig.AboutBlog);
            content = content.Replace(Constants.SKILLS_AND_EXPERIENCE, GeneralConfig.SkillsAndExperience);
            content = content.Replace(Constants.SIDE_PROJECTS, GeneralConfig.SideProjects);

            File.WriteAllText(GeneralConfig.OutputPath + "\\blog\\about.html", content);
        }

        public void WriteBlogFile()
        {
            var content = File.ReadAllText("Templates/html/blog-posts.html");

            content = content.Replace(Constants.TITLE, GeneralConfig.Title);
            content = content.Replace(Constants.AUTHOR_NAME, AuthorData.Name);
            content = content.Replace(Constants.HEADER, header);
            content = content.Replace(Constants.FOOTER, footer);

            File.WriteAllText(GeneralConfig.OutputPath + "\\blog\\blog-posts.html", content);
        }

        

        public void LoadWriteups()
        {
            using (var client = new WebClient())
            {
                var contents = client.DownloadString("https://github.com/Kasimir123/CTFWriteUps");

                var doc = new HtmlDocument();
                doc.LoadHtml(contents);

                var file_box = doc.DocumentNode.Descendants().Where(x => x.Attributes.Contains("aria-labelledby") && x.Attributes["aria-labelledby"].Value.Equals("files")).First();

                var files = file_box.Descendants().Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value.Contains("Box-row")).SkipLast(1);

                foreach (var file in files)
                {
                    ctfs.Add(new CTF(file));
                }
            }
        }

        internal string GenerateWriteupLinks(CTF ctf)
        {
            var content = File.ReadAllText("Templates/html/partials/ctf-writeups.html");
            var output = new StringBuilder();

            foreach (var writeup in ctf.writeups)
            {
                var ctf_content = content.Replace(Constants.WRITEUP_URL, GeneralConfig.OutputPath + "\\blog\\writeups\\" + ctf.Name + "\\" + writeup.Name + ".html");
                ctf_content = ctf_content.Replace(Constants.WRITEUP_TITLE, writeup.Name);
                output.Append(ctf_content);
            }

            return output.ToString();
        }

        public string GenerateCTFs()
        {
            var content = File.ReadAllText("Templates/html/partials/ctf.html");
            var output = new StringBuilder();

            foreach (var ctf in ctfs)
            {
                var ctf_content = content.Replace(Constants.CTF_NAME, ctf.PrettyPrint());
                ctf_content = ctf_content.Replace(Constants.WRITEUPS, GenerateWriteupLinks(ctf));
                output.Append(ctf_content);
            }

            return output.ToString();
        }

        public void WriteWriteupFile()
        {

            LoadWriteups();

            var content = File.ReadAllText("Templates/html/writeups.html");

            content = content.Replace(Constants.TITLE, GeneralConfig.Title);
            content = content.Replace(Constants.AUTHOR_NAME, AuthorData.Name);
            content = content.Replace(Constants.HEADER, header);
            content = content.Replace(Constants.FOOTER, footer);

            content = content.Replace(Constants.CTFS, GenerateCTFs());

            File.WriteAllText(GeneralConfig.OutputPath + "\\blog\\writeups.html", content);
        }

        public void WriteAllWriteups()
        {
            var content = File.ReadAllText("Templates/html/writeup.html");
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseSyntaxHighlighting().Build();

            if (!Directory.Exists(GeneralConfig.OutputPath + "\\blog\\writeups\\"))
            {
                Directory.CreateDirectory(GeneralConfig.OutputPath + "\\blog\\writeups\\");
            }

            foreach (var ctf in ctfs)
            {
                if (!Directory.Exists(GeneralConfig.OutputPath + "\\blog\\writeups\\" + ctf.Name))
                {
                    Directory.CreateDirectory(GeneralConfig.OutputPath + "\\blog\\writeups\\" + ctf.Name);
                }

                foreach (var writeup in ctf.writeups)
                {
                    var writeup_content = content;
                    writeup_content = writeup_content.Replace(Constants.TITLE, GeneralConfig.Title);
                    writeup_content = writeup_content.Replace(Constants.AUTHOR_NAME, AuthorData.Name);
                    writeup_content = writeup_content.Replace(Constants.HEADER, header.Replace("./", "../../").Replace("assets", "../../assets"));
                    writeup_content = writeup_content.Replace(Constants.FOOTER, footer);
                    writeup_content = writeup_content.Replace(Constants.CTF_NAME, ctf.Name);
                    writeup_content = writeup_content.Replace(Constants.WRITEUP_TITLE, writeup.Name);
                    writeup_content = writeup_content.Replace(Constants.WRITEUP, Markdown.ToHtml(writeup.Markdown, pipeline));

                    File.WriteAllText(GeneralConfig.OutputPath + "\\blog\\writeups\\" + ctf.Name + "\\" + writeup.Name + ".html", writeup_content);
                }
            }
        }

        public static void Main(string[] args)
        {
            Program p = new Program();

            p.GetAuthorData();
            p.GetGeneralData();

            p.GenerateHeader();
            p.GenerateFooter();

            p.GenerateOutputFolder();

            p.WriteIndexFile();
            p.WriteAboutFile();
            p.WriteBlogFile();
            p.WriteWriteupFile();
            p.WriteAllWriteups();

        }
    }
}