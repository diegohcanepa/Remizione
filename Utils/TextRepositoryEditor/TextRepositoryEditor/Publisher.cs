using System.Globalization;
using System.Xml;

namespace TextRepositoryEditor
{
    internal static class Publisher
    {
        private static readonly XmlWriterSettings writerSettings = new()
        {
            Indent = true
        };

        #region Private members

        // PublishCulture
        private static void PublishCulture(ProjectNode projectNode, CultureInfo culture)
        {
            var textNodes = from text in projectNode.GetChildNodesRecursively().OfType<TextNode>()
                            orderby text.GetPath()
                            select text;

            var stm = new MemoryStream();

            using (XmlWriter w = XmlWriter.Create(stm, writerSettings))
            {
                w.WriteStartDocument();
                w.WriteStartElement("Strings");
                //w.WriteAttributeString("Language", culture.Name);
                //w.WriteAttributeString("PublishVersion", XmlConvert.ToString(projectNode.PublishVersion));

                foreach (TextNode textNode in textNodes)
                {
                    string nodeName = textNode.GetPath(true);
                    if (string.IsNullOrWhiteSpace(nodeName))
                        continue;

                    nodeName = nodeName.Replace("<", string.Empty);
                    nodeName = nodeName.Replace(">", string.Empty);

                    var value = textNode.IsLiteral ? textNode.LiteralText : textNode.GetText(culture);
                    //if (string.IsNullOrWhiteSpace(value))
                    //  continue;

                    if (textNode.HasContext)
                    {
                        w.WriteStartElement("String");
                        w.WriteAttributeString("Key", "Context: " + textNode.Context);
                        w.WriteEndElement();
                    }

                    w.WriteStartElement("String");
                    w.WriteAttributeString("Key", nodeName);

                    if (!string.IsNullOrWhiteSpace(textNode.Emitter))
                        w.WriteAttributeString("Emitter", textNode.Emitter);

                    w.WriteValue(value);
                    w.WriteEndElement();
                }
                w.WriteEndElement();

                w.Flush();
            }

            stm.Position = 0;
            var fileName = Path.Combine(projectNode.OutputFolder, culture.Name.ToLower());
            fileName = Path.ChangeExtension(fileName, "xml");
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                stm.CopyTo(fs);
                fs.Flush();
            }

            if (!string.IsNullOrWhiteSpace(projectNode.EncryptionKey))
            {
                stm.Position = 0;
                var text = XOREncryptor.AsString(stm, projectNode.EncryptionKey);
                fileName = Path.ChangeExtension(fileName, "lpkg");
                using var fs = new FileStream(fileName, FileMode.Create);
                using var sr = new StreamWriter(fs);
                sr.Write(text);
                sr.Flush();
            }
        }

        #endregion

        // Publish
        internal static bool Publish(ProjectNode projectNode)
        {
            if (string.IsNullOrWhiteSpace(projectNode.OutputFolder))
            {
                var dialog = new FolderBrowserDialog()
                {
                    Description = "Select output folder",
                    ShowNewFolderButton = true,
                    AutoUpgradeEnabled = true,
                    UseDescriptionForTitle = true
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    projectNode.OutputFolder = dialog.SelectedPath;
                }
                else
                    return false;
            }

            projectNode.PublishVersion++;

            foreach (var languagePackage in projectNode.GetLanguagePackageNodes())
            {
                PublishCulture(projectNode, languagePackage.Culture);
            }

            return true;
        }

    }
}
