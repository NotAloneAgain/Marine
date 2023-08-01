using YamlDotNet.Serialization;

namespace Marine.Redux.API.Interfaces
{
    public interface IHasName
    {
        [YamlMember(Alias = "name")]
        public string Name { get; }
    }
}
