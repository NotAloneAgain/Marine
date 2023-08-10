﻿using CommandSystem;
using Exiled.API.Features;
using Marine.Commands.API.Enums;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace Marine.Commands.API.Abstract
{
    public abstract class CommandBase : ICommand
    {
        [YamlMember(Alias = "name")]
        public abstract string Command { get; set; }

        [YamlMember(Alias = "aliases")]
        public virtual string[] Aliases { get; set; } = new string[0];

        [YamlMember(Alias = "desc")]
        public virtual string Description { get; set; } = string.Empty;

        [YamlMember(Alias = "counts")]
        public virtual List<int> Counts { get; set; } = new List<int> { 0 };

        [YamlMember(Alias = "syntaxes")]
        public virtual Dictionary<int, string> Syntax { get; set; } = new Dictionary<int, string>();

        [YamlMember(Alias = "types")]
        public virtual List<CommandType> Types { get; set; } = new List<CommandType>();

        [YamlMember(Alias = "messages")]
        public virtual Dictionary<CommandResultType, string> Messages { get; set; } = new Dictionary<CommandResultType, string>(3)
        {
            { CommandResultType.PlayerError, "Не удалось получить игрока, вызвавшего команду." },
            { CommandResultType.PermissionError, "У вас недостаточно прав для выполнения данной команды!" },
            { CommandResultType.Error, "Во время выполнения команды произошла ошибка: {0}" },
            { CommandResultType.Syntax, "Синтаксис команды: .{0} {1}" },
            { CommandResultType.Fail, "Неудачно..." },
            { CommandResultType.Success, "Успешно!" }
        };

        [YamlMember(Alias = "permissions")]
        public abstract CommandPermission Permission { get; set; }

        [YamlIgnore]
        public CommandHistory History { get; set; } = new ();

        public void Subscribe()
        {
            foreach (var type in Types)
            {
                switch (type)
                {
                    case CommandType.RemoteAdmin:
                        {
                            CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(this);

                            break;
                        }
                    case CommandType.PlayerConsole:
                        {
                            QueryProcessor.DotCommandHandler.RegisterCommand(this);

                            break;
                        }
                    case CommandType.ServerConsole:
                        {
                            GameCore.Console.singleton.ConsoleCommandHandler.RegisterCommand(this);

                            break;
                        }
                }
            }
        }

        public void Unsubscribe()
        {
            foreach (var type in Types)
            {
                switch (type)
                {
                    case CommandType.RemoteAdmin:
                        {
                            CommandProcessor.RemoteAdminCommandHandler.UnregisterCommand(this);

                            break;
                        }
                    case CommandType.PlayerConsole:
                        {
                            QueryProcessor.DotCommandHandler.UnregisterCommand(this);

                            break;
                        }
                    case CommandType.ServerConsole:
                        {
                            GameCore.Console.singleton.ConsoleCommandHandler.UnregisterCommand(this);

                            break;
                        }
                }
            }
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            CommandUse use = new(DateTime.Now, CommandResultType.Error);

            if (!Counts.Contains(arguments.Count))
            {
                response = string.Format(Messages[CommandResultType.Syntax], Command, Syntax.First().Value);

                return false;
            }

            var list = arguments.ToList();

            if (!ParseSyntax(list, arguments.Count, out var args))
            {
                response = string.Format(Messages[CommandResultType.Syntax], Command, Syntax[arguments.Count]);

                return false;
            }

            try
            {
                Player player = Player.Get(sender);

                if (player == null)
                {
                    use.Result = CommandResultType.PlayerError;

                    response = Messages[CommandResultType.PlayerError];

                    return false;
                }

                History.Add(player, use);

                if (!CheckPermissions(player))
                {
                    use.Result = CommandResultType.PermissionError;

                    response = Messages[CommandResultType.PermissionError];

                    return false;
                }

                use.Result = Handle(args, player, out response);

                if (string.IsNullOrEmpty(response))
                {
                    response = Messages[use.Result];
                }

                return use.Result == CommandResultType.Success;
            }
            catch (Exception ex)
            {
                response = string.Format(Messages[CommandResultType.Error], ex.ToString());

                return false;
            }
        }

        public abstract bool ParseSyntax(List<string> input, int count, out List<object> output);

        public virtual bool CheckPermissions(Player player)
        {
            if (Permission != null && Permission.IsLimited)
            {
                if (Permission.Users.Any() && Permission.Users.Contains(player.UserId) || Permission.Groups.Any() && ServerStatic.PermissionsHandler._members.TryGetValue(player.UserId, out string group) && Permission.Groups.Contains(group))
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        public abstract CommandResultType Handle(List<object> arguments, Player player, out string response);

        protected virtual bool TryParsePlayers(string players, out List<Player> result)
        {
            switch (players)
            {
                case "all":
                    {
                        result = Player.List.ToList();

                        break;
                    }
                case "0":
                case "me":
                    {
                        result = new List<Player>(1) { };

                        break;
                    }
                default:
                    {
                        if (players.Contains("."))
                        {
                            var splitted = players.Split('.');

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

                            if (!Player.TryGet(players, out var player))
                            {
                                return false;
                            }

                            result.Add(player);
                        }

                        break;
                    }
            }

            return result != null;
        }
    }
}
