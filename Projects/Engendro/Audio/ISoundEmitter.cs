namespace Engendro.Audio
{
    // ISoundEmitter
    public interface ISoundEmitter
    {
        bool IsAvailable { get; }
        void Update(SoundInstance soundInstance, float baseVolume);
    }
}
