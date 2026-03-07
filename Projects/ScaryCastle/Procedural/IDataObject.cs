using Engendro;

namespace ScaryCastle
{
    // IDataObject
    public interface IDataObject : INamedObject
    {
        void Validate(GameSession session);
    }
}
