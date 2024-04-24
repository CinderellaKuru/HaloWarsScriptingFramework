using System.Xml.Linq;

namespace HaloWarsScriptingFramework.Data
{
    public class WeaponTypes(List<WeaponType> weaponTypes)
    {
        public List<WeaponType> WeaponTypeList { get; set; } = weaponTypes;

        public static WeaponTypes FromFile(string path)
        {
            XDocument doc = XDocument.Load(path);

            if (doc.Root == null)
                return null;

            List<WeaponType> weaponTypes = doc.Root
                .Elements("WeaponType")
                .Select(wt =>
                {
                    string name = (string)wt.Element("Name");
                    if (name == null)
                        return null;

                    List<DamageModifier> damageModifiers = wt.Elements("DamageModifier")
                        .Select(dm =>
                        {
                            string rating = (string)dm.Attribute("rating");
                            string type = (string)dm.Attribute("type");
                            double value = (double)dm;

                            return rating == null || type == null ? null : new DamageModifier(rating, type, value);
                        })
                        .Where(dm => dm != null)
                        .ToList();

                    return new WeaponType(name, (string)wt.Element("DeathAnimation"), damageModifiers);
                })
                .Where(wt => wt != null)
                .ToList();

            return new WeaponTypes(weaponTypes);
        }

        public void ToFile(string path)
        {
            if (WeaponTypeList == null)
                return;

            List<XElement> weaponTypes = [];

            foreach (WeaponType wt in WeaponTypeList)
            {
                XElement weaponTypeElement = new("WeaponType",
                    new XElement("Name", wt.Name));

                if (wt.DeathAnimation != null)
                {
                    weaponTypeElement.Add(new XElement("DeathAnimation", wt.DeathAnimation));
                }

                foreach (System.Reflection.FieldInfo modifierType in typeof(DamageModifier.ModifierTypes).GetFields())
                {
                    string modifierTypeName = (string)modifierType.GetValue(null);

                    DamageModifier existingModifier = wt.DamageModifiers.FirstOrDefault(dm => dm.Type == modifierTypeName);
                    if (existingModifier != null)
                    {
                        weaponTypeElement.Add(new XElement("DamageModifier",
                            new XAttribute("rating", existingModifier.Rating),
                            new XAttribute("type", existingModifier.Type),
                            existingModifier.Value));
                    }
                    else
                    {
                        weaponTypeElement.Add(new XElement("DamageModifier",
                            new XAttribute("rating", "1"),
                            new XAttribute("type", modifierTypeName),
                            1));
                    }
                }

                weaponTypes.Add(weaponTypeElement);
            }

            XElement weaponTypesElement = new("WeaponTypes", weaponTypes);
            XDocument doc = new(weaponTypesElement);

            doc.Save(path);
        }

    }

    public class WeaponType(string name, string deathAnimation, List<DamageModifier> damageModifiers)
    {
        public string Name { get; set; } = name;
        public string DeathAnimation { get; set; } = deathAnimation;
        public List<DamageModifier> DamageModifiers { get; set; } = damageModifiers;
    }

    public class DamageModifier(string rating, string type, double value)
    {
        public string Rating { get; set; } = rating;
        public string Type { get; set; } = type;
        public double Value { get; set; } = value;

        public class ModifierTypes
        {
            public const string Light = "Light";
            public const string LightInCover = "LightInCover";
            public const string LightArmored = "LightArmored";
            public const string CovenantLeader = "CovenantLeader";
            public const string Medium = "Medium";
            public const string MediumAir = "MediumAir";
            public const string Heavy = "Heavy";
            public const string Building = "Building";
            public const string Shielded = "Shielded";
            public const string Flood = "Flood";
        }
    }
}
