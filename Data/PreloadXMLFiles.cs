using System.Xml.Linq;

namespace HaloWarsScriptingFramework.Data.PreloadXMLFiles
{
    public class PreloadXMLFiles(List<File> files)
    {
        public List<File> Files { get; set; } = files;

        public static PreloadXMLFiles FromFile(string path)
        {
            XDocument doc = XDocument.Load(path);

            if (doc.Root == null)
                return null;

            List<File> files = doc.Root
                .Elements("File")
                .Select(f => new File(
                    (string)f,
                    (string)f.Attribute("dir")))
                .ToList();

            return new PreloadXMLFiles(files);
        }

        public void ToFile(string path)
        {
            if (Files == null)
                return;

            List<XElement> fileElements = Files.Select(f =>
            {
                XElement fileElement = new("File", f.Name);

                if (!string.IsNullOrEmpty(f.Directory))
                    fileElement.Add(new XAttribute("dir", f.Directory));

                return fileElement;
            }).ToList();

            XElement filesListElement = new("Files", fileElements);
            XDocument doc = new(filesListElement);

            doc.Save(path);
        }
    }

    public class File(string name, string directory = null)
    {
        public string Name { get; set; } = name;
        public string Directory { get; set; } = directory;
    }
}
