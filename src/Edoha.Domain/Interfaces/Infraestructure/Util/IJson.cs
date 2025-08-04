namespace Edoha.Domain.Interfaces.Infraestructure.Util
{
    public interface IJson
    {
        string Serialize<T>(T obj);
    }
}
