using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Marine.Commands.Patches
{
    public abstract class CustomCommandPatch
    {
        private protected readonly Regex _regex;

        private protected CustomCommandPatch() => _regex = new Regex(@"(\d+)([smhdw])");

        public abstract List<object> ParseArguments(List<string> args, Player sender);

        public virtual List<Player> ParsePlayers(string input, Player sender)
        {
            _ = new List<Player>(Server.MaxPlayerCount);
            List<Player> result;
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
                                if (!Player.TryGet(data, out Player player))
                                {
                                    continue;
                                }

                                result.Add(player);
                            }
                        }
                        else
                        {
                            result = new(1);

                            if (!Player.TryGet(input, out Player player))
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
            MatchCollection matches = _regex.Matches(input);

            foreach (Match match in matches)
            {
                if (!int.TryParse(match.Groups[1].Value, out var duration))
                {
                    throw new ArgumentException("Invalid input format. Duration must be a number followed by 's', 'm', 'h', 'd', or 'w'.");
                }

                var unit = match.Groups[2].Value[0];
                result += unit switch
                {
                    's' => TimeSpan.FromSeconds(duration),
                    'm' => TimeSpan.FromMinutes(duration),
                    'h' => TimeSpan.FromHours(duration),
                    'd' => TimeSpan.FromDays(duration),
                    'w' => TimeSpan.FromDays(duration * 7),
                    _ => throw new ArgumentException("Invalid input format. Duration unit must be 's', 'm', 'h', 'd', or 'w'."),
                };
            }

            return result;
        }
    }
}
