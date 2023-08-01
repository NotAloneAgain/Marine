using Exiled.API.Extensions;
using Exiled.API.Features;
using YamlDotNet.Serialization;

namespace Marine.Redux.API
{
    public class Message
    {
        public Message()
        {

        }

        public Message(string text, ushort duration, bool show)
        {
            Text = text;
            Duration = duration;
            Show = show;
        }

        [YamlMember(Alias = "text")]
        public string Text { get; set; }

        [YamlMember(Alias = "color")]
        public string Color { get; set; }

        [YamlMember(Alias = "duration")]
        public ushort Duration { get; set; } = 8;

        [YamlMember(Alias = "show")]
        public bool Show { get; set; } = false;

        public void Send(in Player player)
        {
            if (!Show || Duration == 0)
            {
                return;
            }

            string color = string.IsNullOrEmpty(Color) ? player.Role.Type.GetColor().ToHex() : Color;
            string text = $"<line-height=95%><size=95%><voffset=-18em><color={color}>{Text}</color></size></voffset>";

            player.ShowHint(text, Duration);
        }
    }
}
