namespace TechWave_Electronics.Models
{
    public interface IKeyManager
    {
        string GetKey();
        void SetKey(string key);
    }
}
