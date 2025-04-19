using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace ContentProtector
{
    internal class Program
    {
        private const string rootPath = @"C:\Users\DIEGO\Documents\vlad-circus\Projects\Content\Atlases";
        private enum AssetPrefixMatchMode { StartsWith, Contains }

        static void Main(string[] args)
        {
            ClearAtlases();
            ClearFrames("NPCS", "Baby-assets/");
            ClearFrames("NPCS", "Boy-assets/");
            ClearFrames("NPCS", "1921", AssetPrefixMatchMode.Contains);
            ClearFrames("NPCS", "HangingBeheaded-assets/");
            ClearFrames("NPCS", "Hyena-assets/");
            ClearFrames("NPCS", "Josef-assets/");
            ClearFrames("NPCS", "Puppet-assets/");
            ClearFrames("NPCS", "RailEmployee-assets/");
            ClearFrames("UI", "1921", AssetPrefixMatchMode.Contains, "Photo1921");
        }

        // ClearAtlases
        private static void ClearAtlases()
        {
            var files = new List<string>
            {
                "BabyCutscene",
                "Basement",
                "BirthCloseUp",
                "CabinInterior",
                "CircusExterior",
                "CircusInterior",
                "CircusRing",
                "DiaryReal",
                "Floor1Bathroom",
                "Floor1Bedroom",
                "Floor1Hall",
                "ForestNorthReal",
                "ForestSouth",
                "HouseFrontReal",
                "JosefInCabin",
                "Maze",
                "OliverWagon",
                "Sewer01",
                "Sewer02",
                "Sewer03",
                "SewerEntrance",
                "SewerExit",
                "Shed",
                "Stable",
                "TrainStation",
                "TwinsCorpse",
                "TwinsWagon",
                "TwinsWagon1921",
                "VladCorpse",
                "VladWagon1921",
                "WinterGarden",
            };

            for (int i = 0; i < files.Count; i++)
            {
                files[i] = Path.Combine(rootPath, files[i]);
                files[i] = Path.ChangeExtension(files[i], "png");
            }

            foreach (var file in files)
            {
                var image = GetImage(file);

                using (Graphics g = Graphics.FromImage(image))
                {
                    g.Clear(Color.Black);
                }

                image.Save(file, ImageFormat.Png);
            }
        }

        // ClearFrames
        private static void ClearFrames(string atlasName, string assetPrefix, AssetPrefixMatchMode matchMode = AssetPrefixMatchMode.StartsWith, params string[] assetExceptions)
        {
            var xmlFileName = Path.ChangeExtension(Path.Combine(rootPath, atlasName), "xml");
            var imageFileName = Path.ChangeExtension(xmlFileName, "png");

            var doc = XDocument.Load(xmlFileName);
            var image = GetImage(imageFileName);

            var save = false;
            using (Graphics g = Graphics.FromImage(image))
            {
                foreach (var sprite in doc.Descendants("sprite"))
                {
                    if (sprite.Attribute("n")?.Value is not string value)
                        continue;

                    if (matchMode == AssetPrefixMatchMode.StartsWith && !value.StartsWith(assetPrefix))
                        continue;

                    if (matchMode == AssetPrefixMatchMode.Contains && !value.Contains(assetPrefix))
                        continue;

                    var exceptionFound = false;
                    foreach (var exceptionValue in assetExceptions)
                    {
                        if (value.Contains(exceptionValue))
                        {
                            exceptionFound = true;
                            break;
                        }
                    }

                    if (exceptionFound)
                        continue;

                    if (sprite.Attribute("x")?.Value is not string xValue ||
                        sprite.Attribute("y")?.Value is not string yValue ||
                        sprite.Attribute("w")?.Value is not string wValue ||
                        sprite.Attribute("h")?.Value is not string hValue)
                        continue;

                    var x = XmlConvert.ToInt32(xValue);
                    var y = XmlConvert.ToInt32(yValue);
                    var w = XmlConvert.ToInt32(wValue);
                    var h = XmlConvert.ToInt32(hValue);

                    var r = new Rectangle(x, y, w, h);

                    g.FillRectangle(Brushes.Black, r);

                    save = true;
                }
            }

            if (save)
                image.Save(imageFileName, ImageFormat.Png);
        }

        // GetImage
        private static Image GetImage(string fileName)
        {
            using var stm = File.OpenRead(fileName);
            return Image.FromStream(stm);
        }
    }
}
