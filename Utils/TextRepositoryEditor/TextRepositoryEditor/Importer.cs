using System.Xml.Linq;

namespace TextRepositoryEditor
{
    /// <summary>
    /// Importer
    /// </summary>
    internal static class Importer
    {
        // CreateFolderNode
        private static FolderNode? CreateFolderNode(ProjectNode projectNode, string path)
        {
            string[] values = path.Split(".");

            TreeNode node = projectNode;
            FolderNode? folderNode = null;

            for (int i = 0; i < values.Length; i++)
            {
                folderNode = node.FindChild<FolderNode>(values[i]);

                if (folderNode == null)
                {
                    folderNode = new FolderNode(values[i]);
                    node.Nodes.Add(folderNode);
                }

                node = folderNode;
            }

            return folderNode;
        }

        // Import
        internal static void Import(string fileName, LanguagePackageNode languagePackageNode)
        {
            // ApplySecondaryAttributes
            void ApplySecondaryAttributes(XElement element, TextNode textNode)
            {
                if (element.Attribute(AttributeName.Comments.ToString())?.Value is string commentsValue)
                    textNode.Comments = commentsValue;

                if (element.Attribute(AttributeName.Emitter.ToString())?.Value is string emitterValue)
                    textNode.Emitter = emitterValue;
            }

            var document = XDocument.Load(fileName);
            var importedNodes = new HashSet<TreeNode>();

            foreach (XElement element in document.Descendants("String"))
            {
                var key = element.Attribute("Key")?.Value.Split('.');
                if (key == null)
                    continue;

                if (element.Value is not string value)
                    continue;

                if (key[0] == "IMPORTS")
                    key[0] = FolderNode.ImportsNamePrefix;

                var textNodeName = key[^1];
                var folderPath = string.Join(".", key, 0, key.Length - 1);
                if (CreateFolderNode(languagePackageNode.ProjectNode, folderPath) is FolderNode folderNode)
                {
                    if (folderNode.FindChild<TextNode>(textNodeName) is TextNode existingTextNode)
                    {
                        if (existingTextNode.SetText(languagePackageNode.Culture, value))
                        {
                            existingTextNode.ImportResult = ImportResult.Updated;
                            //existingTextNode.ClearTexts();
                        }
                        else
                            existingTextNode.ImportResult = ImportResult.Unchanged;

                        ApplySecondaryAttributes(element, existingTextNode);

                        importedNodes.Add(existingTextNode);
                    }
                    else
                    {
                        var textNode = new TextNode(textNodeName);
                        folderNode.Nodes.Add(textNode);
                        textNode.ImportResult = ImportResult.New;
                        importedNodes.Add(textNode);

                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            textNode.SetText(languagePackageNode.Culture, value);
                            ApplySecondaryAttributes(element, textNode);
                        }
                    }
                }
            }

            // Look for orphans
            var allNodes = new List<TextNode>(languagePackageNode.ProjectNode.GetChildNodesRecursively().OfType<TextNode>());
            foreach (var textNode in allNodes)
            {
                if (textNode.FullPath.StartsWith(AttributeName.Project + @"\" + FolderNode.ImportsNamePrefix))
                {
                    if (!importedNodes.Contains(textNode))
                        textNode.ImportResult = ImportResult.NotFound;
                }
            }
        }

        // PreviewImport
        internal static IDictionary<string, string> PreviewImport(string fileName)
        {
            var result = new Dictionary<string, string>();
            var document = XDocument.Load(fileName);

            foreach (XElement element in document.Descendants("String"))
            {
                if (element.Attribute("Key")?.Value is string id && element.Value is string value)
                    result.Add(id, value);
            }

            return result;
        }
    }
}
