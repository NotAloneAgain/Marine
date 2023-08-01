using Exiled.API.Features.Roles;
using Exiled.API.Features;
using System;
using System.Linq;
using UnityEngine;
using YamlDotNet.Serialization;
using Exiled.API.Extensions;

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
        public string Text { get; set; } = "Hi";

        [YamlMember(Alias = "color")]
        public string Color { get; set; } = "#FF0000";

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
