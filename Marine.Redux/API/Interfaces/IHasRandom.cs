using YamlDotNet.Serialization;

namespace Marine.Redux.API.Interfaces
{
    public interface IHasRandom
    {
        [YamlMember(Alias = "is_randomable")]
        public bool IsRandomable { get; set; }

        public void Randomize();
    }
}
