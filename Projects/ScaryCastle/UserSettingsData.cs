using Engendro;
using Engendro.Audio;
using Engendro.Input;
using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace ScaryCastle
{
    /// <summary>
    /// UserSettingsData
    /// </summary>
    [Serializable]
    public struct UserSettingsData
    {
        private const string SpeechBubbleTypingAttribute = "SpeechBubbleTyping";
        private const string SpeechBubbleTypingSoundAttribute = "SpeechBubbleTypingSound";

        // AllowVibration
        public bool AllowVibration { get; set; }

        // AmbientVolume
        public float AmbientVolume { get; set; }

        // Apply
        public static void Apply(ScaryCastleGame game, UserSettingsData data)
        {
            GamePadDevice.AllowVibration = data.AllowVibration;

            // Fullscreen & Resolution
            if (game.PlatformBridge.AllowWindowedMode)
            {
                if (data.IsFullScreen)
                    game.SwitchToFullScreen();
                else
                    game.SwitchToWindowedMode();
            }
            else
            {
                game.SwitchToFullScreen();
            }

            // Language
            if (!string.IsNullOrEmpty(data.LanguageTag))
            {
                if (LanguagePackage.FindPackage(data.LanguageTag) is LanguagePackage languagePackage)
                    TextRepository.Load(languagePackage);
            }

            AudioManager.AmbienceCategory.Volume.Master = data.AmbientVolume;
            AudioManager.FXCategory.Volume.Master = data.FXVolume;
            AudioManager.MasterVolume = data.MasterVolume;
            AudioManager.MusicCategory.Volume.Master = data.MusicVolume;

            SpeechBubbleSettings.TextTyping = data.SpeechBubbleTyping;
            SpeechBubbleSettings.TextTypingSound = data.SpeechBubbleTypingSound;
        }

        // FromDefaultValues
        public static UserSettingsData FromDefaultValues()
        {
            LanguagePackage? languagePackage = LanguagePackage.FindPackage(CultureInfo.CurrentCulture.Name, true);
            var ltag = languagePackage == null ? LocalizationManager.English : languagePackage.LanguageTag;

            UserSettingsData result = new()
            {
                AllowVibration = true,
                AmbientVolume = 1,
                FXVolume = 1,
                IsFullScreen = true,
                LanguageTag = ltag,
                MasterVolume = 1,
                MusicVolume = 1,
                SpeechBubbleTyping = true,
                SpeechBubbleTypingSound = true
            };

            return result;
        }

        // FromSystem
        public static UserSettingsData FromSystem(EngendroGame game)
        {
            UserSettingsData result = new()
            {
                AllowVibration = GamePadDevice.AllowVibration,
                AmbientVolume = AudioManager.AmbienceCategory.Volume.Master,
                FXVolume = AudioManager.FXCategory.Volume.Master,
                IsFullScreen = game.IsFullScreen,
                LanguageTag = TextRepository.LanguagePackage?.LanguageTag ?? string.Empty,
                MasterVolume = AudioManager.MasterVolume,
                MusicVolume = AudioManager.MusicCategory.Volume.Master,
                SpeechBubbleTyping = SpeechBubbleSettings.TextTyping,
                SpeechBubbleTypingSound = SpeechBubbleSettings.TextTypingSound
            };

            return result;
        }

        // Load
        public static UserSettingsData Load(ScaryCastleGame game)
        {
            if (game.PlatformBridge.FileSystem.ReadFile(GameSettings.UserSettingsFileName) is Stream input)
            {
                try
                {
                    input.Position = 0;
                    var data = new UserSettingsData();

                    using (var r = XmlReader.Create(input))
                    {
                        r.MoveToContent();

                        if (r.GetAttribute(nameof(AllowVibration)) is string allowVibrationValue)
                            data.AllowVibration = XmlConvert.ToBoolean(allowVibrationValue);

                        if (r.GetAttribute(nameof(AmbientVolume)) is string ambientVolumeValue)
                            data.AmbientVolume = XmlConvert.ToSingle(ambientVolumeValue);

                        if (r.GetAttribute(nameof(FXVolume)) is string fxVolumeValue)
                            data.FXVolume = XmlConvert.ToSingle(fxVolumeValue);

                        if (r.GetAttribute(nameof(IsFullScreen)) is string isFullScreenValue)
                            data.IsFullScreen = XmlConvert.ToBoolean(isFullScreenValue);

                        if (r.GetAttribute(nameof(LanguageTag)) is string languageTagValue)
                            data.LanguageTag = languageTagValue;

                        if (r.GetAttribute(nameof(MasterVolume)) is string masterVolumeValue)
                            data.MasterVolume = XmlConvert.ToSingle(masterVolumeValue);

                        if (r.GetAttribute(nameof(MusicVolume)) is string musicVolumeValue)
                            data.MusicVolume = XmlConvert.ToSingle(musicVolumeValue);

                        if (r.GetAttribute(nameof(SpeechBubbleTypingAttribute)) is string speechBubbleTypingValue)
                            data.SpeechBubbleTyping = XmlConvert.ToBoolean(speechBubbleTypingValue);

                        if (r.GetAttribute(nameof(SpeechBubbleTypingSoundAttribute)) is string speechBubbleTypingSoundValue)
                            data.SpeechBubbleTypingSound = XmlConvert.ToBoolean(speechBubbleTypingSoundValue);
                    }

                    return data;
                }
                catch
                {
                    return FromDefaultValues();
                }
            }
            else
            {
                return FromDefaultValues();
            }
        }

        // LoadAndApply
        public static UserSettingsData LoadAndApply(ScaryCastleGame game)
        {
            var data = Load(game);
            Apply(game, data);
            return data;
        }

        // FXVolume
        public float FXVolume { get; set; }

        // IsFullScreen
        public bool IsFullScreen { get; set; }

        // LanguageTag
        public string LanguageTag { get; set; }

        // MasterVolume
        public float MasterVolume { get; set; }

        // MusicVolume
        public float MusicVolume { get; set; }

        // SaveCurrentSystemSettings
        public static void SaveCurrentSystemSettings(ScaryCastleGame game)
        {
            var userSettings = FromSystem(game);

            using var output = new MemoryStream();
            using var w = XmlWriter.Create(output);

            w.WriteStartDocument();
            w.WriteStartElement("UserSettings");
            w.WriteAttributeString(nameof(AllowVibration), XmlConvert.ToString(userSettings.AllowVibration));
            w.WriteAttributeString(nameof(AmbientVolume), XmlConvert.ToString(userSettings.AmbientVolume));
            w.WriteAttributeString(nameof(FXVolume), XmlConvert.ToString(userSettings.FXVolume));
            w.WriteAttributeString(nameof(IsFullScreen), XmlConvert.ToString(userSettings.IsFullScreen));
            w.WriteAttributeString(nameof(LanguageTag), userSettings.LanguageTag);
            w.WriteAttributeString(nameof(MasterVolume), XmlConvert.ToString(userSettings.MasterVolume));
            w.WriteAttributeString(nameof(MusicVolume), XmlConvert.ToString(userSettings.MusicVolume));
            w.WriteAttributeString(nameof(MusicVolume), XmlConvert.ToString(userSettings.MusicVolume));
            w.WriteAttributeString(SpeechBubbleTypingAttribute, XmlConvert.ToString(userSettings.SpeechBubbleTyping));
            w.WriteAttributeString(SpeechBubbleTypingSoundAttribute, XmlConvert.ToString(userSettings.SpeechBubbleTypingSound));
            w.WriteEndElement();
            w.Flush();

            output.Position = 0;
            game.PlatformBridge.FileSystem.WriteFile(GameSettings.UserSettingsFileName, output);
        }

        // SpeechBubbleTyping
        public bool SpeechBubbleTyping { get; set; }

        // SpeechBubbleTypingSound
        public bool SpeechBubbleTypingSound { get; set; }
    }
}