using System.Xml.Linq;

namespace HaloWarsScriptingFramework.Data
{
    public class TipList(List<Tip> tips)
    {
        public List<Tip> Tips { get; set; } = tips;

        public static TipList FromFile(string path)
        {
            XDocument doc = XDocument.Load(path);

            if (doc.Root == null)
                return null;

            List<Tip> tips = doc.Root
                .Elements("Tip")
                .Select(t => new Tip((string)t.Attribute("_locID"), (bool?)t.Attribute("LoadingScreen")))
                .ToList();

            return new TipList(tips);
        }

        public void ToFile(string path)
        {
            if (Tips == null)
                return;

            List<XElement> tipElements = Tips.Select(t =>
            {
                XElement tipElement = new("Tip",
                    new XAttribute("_locID", t.LocID));

                if (t.LoadingScreen != null)
                    tipElement.Add(new XAttribute("LoadingScreen", t.LoadingScreen));

                return tipElement;
            }).ToList();

            XElement tipListElement = new("TipList", tipElements);
            XDocument doc = new(tipListElement);

            doc.Save(path);
        }
    }

    public class Tip(string locID, bool? loadingScreen = null)
    {
        public string LocID { get; set; } = locID;
        public bool? LoadingScreen { get; set; } = loadingScreen;
    }
}
