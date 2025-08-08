#if DEBUG

using Engendro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Adberration.Scripting
{
    // ExportLocalizableTextsCommand
    // Arguments: [#plain]
    internal sealed class ExportLocalizableTextsCommand : NonAwaitableCommand
    {
        private const string PlainArg = "#plain";

        // Constructor
        internal ExportLocalizableTextsCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0, PlainArg)
        {
        }

        #region Private members

        // ExportAsPlainText
        private static void ExportAsPlainText(StreamWriter w, IEnumerable<Script> scripts)
        {
            foreach (var script in scripts)
            {
                foreach (var command in script.Statements.OfType<LocalizableCommand>())
                {
                    if (command.Literal)
                    {
                        continue;
                    }

                    var text = command.GetDisplayText();

                    if (!TextRepository.IsKeyReference(text))
                    {
                        w.WriteLine(text);
                    }
                }
            }

            w.Flush();
        }

        // ExportCore
        private static void ExportCore(XmlWriter w, IEnumerable<Script> scripts)
        {
            HashSet<int> localizationIds = [];

            var counter = 0;
            w.WriteStartDocument();
            w.WriteStartElement("Strings");
            foreach (var script in scripts)
            {
                localizationIds.Clear();

                foreach (var command in script.Statements.OfType<LocalizableCommand>())
                {
                    // Command
                    if (command.Literal)
                    {
                        continue;
                    }

                    var key = command.EncodeTextKey();
                    var emitter = command.GetTextEmitterName();

                    // Forces to retrieve text from script
                    var text = command.GetDisplayText(LocalizationSource.Script);

                    // Transient localization id
                    if (command.GetLocalizationId() == -1)
                    {
                        continue;
                    }

                    if (!command.HasLocalizationArg)
                    {
                        if (!TextRepository.IsKeyReference(text))
                        {
                            var message = $"Missing localization id: '{script.Name}' -> '{command.Source}'.";
                            throw new InvalidOperationException(message);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (TextRepository.IsKeyReference(text))
                    {
                        var message = $"The command '{command.Source}' in script '{script.Name}' has a localization id but it is a text repository key.";
                        throw new InvalidOperationException(message);
                    }

                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        var lid = command.GetLocalizationId();

                        if (lid > 0)
                        {
                            if (!localizationIds.Add(lid))
                            {
                                throw new InvalidOperationException("Duplicated id in script: " + script.Name);
                            }

                            WriteNode(w, key, emitter, text);

                            counter++;
                        }
                    }
                }
            }

            w.WriteEndElement();
        }

        // WriteNode
        private static void WriteNode(XmlWriter w, string key, string emitter, string value)
        {
            w.WriteStartElement("String");
            w.WriteAttributeString("Key", key);
            w.WriteAttributeString("Emitter", emitter);
            w.WriteValue(value);
            w.WriteEndElement();
        }

        #endregion

        // OnExecute
        protected override void OnExecute()
        {
            var scriptLibrary = Session.ScriptLibrary;

            if (HasArg(PlainArg))
            {
                using (MemoryStream stm = new())
                {
                    using (StreamWriter w = new(stm))
                    {
                        var fileName = Path.Combine(Session.Game.PlatformBridge.FileSystem.TargetDirectory, "PlainTexts.txt");
                        ExportAsPlainText(w, scriptLibrary.AllScripts);
                        stm.Position = 0;
                        scriptLibrary.Session.Game.PlatformBridge.FileSystem.WriteFile(fileName, stm);
                    }
                }
            }
            else
            {
                using (MemoryStream stm = new())
                {
                    using (XmlWriter w = XmlWriter.Create(stm))
                    {
                        var fileName = Path.Combine(Session.Game.PlatformBridge.FileSystem.TargetDirectory, "LocalizableTexts.xml");
                        ExportCore(w, Session.ScriptLibrary.AllScripts);
                        w.Flush();
                        stm.Flush();
                        stm.Position = 0;
                        Session.ScriptLibrary.Session.Game.PlatformBridge.FileSystem.WriteFile(fileName, stm);
                    }
                }
            }
        }
    }
}

#endif