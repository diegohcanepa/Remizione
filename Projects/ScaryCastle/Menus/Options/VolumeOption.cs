using Engendro.Audio;

namespace ScaryCastle.Menus
{
    /// <summary>
    /// VolumeOption
    /// </summary>
    public sealed class VolumeOption : Option<float>
    {
        // Constructor
        internal VolumeOption(ScaryCastleGame game, VolumeCategory category)
            : base(game, $"@Menu.Options.{category}")
        {
            this.VolumeCategory = category;

            var value = 0f;
            for (var i = 0; i < 11; i++)
            {
                var displayValue = i * 10;
                if (displayValue == 0)
                {
                    AddValue("@Misc.Mute", value);
                }
                else
                {
                    AddValue($"{displayValue}%", value);
                }

                value += .1f;
            }

            var volume = GetSystemVolume();

            Value = Values[(int)(volume * 10)];
        }

        #region Private members

        // GetSystemVolume
        private float GetSystemVolume()
        {
            return this.VolumeCategory switch
            {
                VolumeCategory.Ambient => AudioManager.AmbienceCategory.Volume.Master,
                VolumeCategory.FX => AudioManager.FXCategory.Volume.Master,
                VolumeCategory.Music => AudioManager.MusicCategory.Volume.Master,
                VolumeCategory.Voice => AudioManager.VoiceCategory.Volume.Master,
                _ => AudioManager.MasterVolume,
            };
        }

        // SetSystemVolume
        private void SetSystemVolume()
        {
            switch (this.VolumeCategory)
            {
                case VolumeCategory.Ambient:
                    AudioManager.AmbienceCategory.Volume.Master = Value;
                    break;

                case VolumeCategory.FX:
                    AudioManager.FXCategory.Volume.Master = Value;
                    break;

                case VolumeCategory.Music:
                    AudioManager.MusicCategory.Volume.Master = Value;
                    break;

                case VolumeCategory.Voice:
                    AudioManager.VoiceCategory.Volume.Master = Value;
                    break;

                default:
                    AudioManager.MasterVolume = Value;
                    break;
            }
        }

        #endregion

        #region Protected members

        // OnValueChanged
        protected override void OnValueChanged(float newValue)
        {
            base.OnValueChanged(newValue);
            SetSystemVolume();
        }

        #endregion

        // VolumeCategory
        public VolumeCategory VolumeCategory { get; }
    }
}
