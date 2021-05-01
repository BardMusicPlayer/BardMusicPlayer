using LiteDB;

namespace BardMusicPlayer.Catalog
{
    public sealed class LiteDBSchema
    {
        [BsonId]
        public int Id { get; set; } = Constants.SCHEMA_DOCUMENT_ID;
        public byte Version { get; set; } = Constants.SCHEMA_VERSION;
    }
}
