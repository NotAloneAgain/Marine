﻿using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Marine.Commands.Patches
{
    public abstract class CustomCommandPatch
    {
        private protected readonly Regex _regex;

        public CustomCommandPatch()
        {
            _regex = new Regex(@"(\d+)([smhdw])");
        }

        public abstract List<object> ParseArguments(List<string> args);

        public virtual List<Player> ParsePlayers(string input, Player sender)
        {
            var result = new List<Player>(Server.MaxPlayerCount);

            switch (input)
            {
                case "all":
                    {
                        result = Player.List.ToList();

                        break;
                    }
                case "0":
                case "me":
                    {
                        result = new List<Player>(1) { sender };

                        break;
                    }
                default:
                    {
                        if (input.Contains("."))
                        {
                            var splitted = input.Split('.');

                            result = new(splitted.Length);

                            foreach (var data in splitted)
                            {
                                if (!Player.TryGet(data, out var player))
                                {
                                    continue;
                                }

                                result.Add(player);
                            }
                        }
                        else
                        {
                            result = new(1);

                            if (!Player.TryGet(input, out var player))
                            {
                                return result;
                            }

                            result.Add(player);
                        }

                        break;
                    }
            }

            return result;
        }

        public virtual TimeSpan ParseDuration(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input string cannot be null or empty.");
            }

            TimeSpan result = TimeSpan.Zero;
            var matches = _regex.Matches(input);

            foreach (Match match in matches)
            {
                if (!int.TryParse(match.Groups[1].Value, out var duration))
                {
                    throw new ArgumentException("Invalid input format. Duration must be a number followed by 's', 'm', 'h', 'd', or 'w'.");
                }

                char unit = match.Groups[2].Value[0];
                switch (unit)
                {
                    case 's':
                        result += TimeSpan.FromSeconds(duration);
                        break;
                    case 'm':
                        result += TimeSpan.FromMinutes(duration);
                        break;
                    case 'h':
                        result += TimeSpan.FromHours(duration);
                        break;
                    case 'd':
                        result += TimeSpan.FromDays(duration);
                        break;
                    case 'w':
                        result += TimeSpan.FromDays(duration * 7);
                        break;
                    default:
                        throw new ArgumentException("Invalid input format. Duration unit must be 's', 'm', 'h', 'd', or 'w'.");
                }
            }

            return result;
        }
    }
}
