using Exiled.API.Extensions;
using Exiled.API.Features;
using PlayerRoles;
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

        public Message(string text, ushort duration, bool show, string color) : this(text, duration, show)
        {
            Color = color;
        }

        [YamlMember(Alias = "text")]
        public string Text { get; set; } = string.Empty;

        [YamlMember(Alias = "color")]
        public string Color { get; set; } = string.Empty;

        [YamlMember(Alias = "duration")]
        public ushort Duration { get; set; } = 8;

        [YamlMember(Alias = "show")]
        public bool Show { get; set; } = false;

        public void Send(in Player player)
        {
            if (!Show || Duration == 0 || string.IsNullOrEmpty(Text))
            {
                return;
            }

            player.ShowHint(Formate(Text, player.Role.Type), Duration);
        }

        public string Formate(in string text, RoleTypeId role)
        {
            string color = string.IsNullOrEmpty(Color) || Color == default ? role.GetColor().ToHex() : Color;

            return $"<line-height=95%><size=95%><voffset=-18em><color={color}>{text}</color></size></voffset>";
        }
    }
}
