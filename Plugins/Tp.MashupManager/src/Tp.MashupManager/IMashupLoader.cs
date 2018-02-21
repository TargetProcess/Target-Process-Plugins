namespace Tp.MashupManager
{
    public interface IMashupLoader
    {
        Mashup Load(string mashupFolderPath, string mashupName);
    }
}
