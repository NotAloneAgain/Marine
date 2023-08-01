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
        public virtual string[] Aliases { get; set; } = Array.Empty<string>();

        [YamlMember(Alias = "desc")]
        public virtual string Description { get; set; } = string.Empty;

        [YamlMember(Alias = "counts")]
        public virtual List<int> Counts { get; set; } = new List<int> { 0 };

        [YamlMember(Alias = "syntaxes")]
        public Dictionary<int, string> Syntax { get; set; }

        [YamlMember(Alias = "types")]
        public abstract List<CommandType> Types { get; }

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
            if (!Counts.Contains(arguments.Count))
            {
                response = string.Format(Messages[CommandResultType.Syntax], Command, Syntax[0]);

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
                    response = Messages[CommandResultType.PlayerError];

                    return false;
                }

                if (Permission != null && Permission.IsLimited)
                {
                    if (Permission.Custom(player) || Permission.Users.Any() && Permission.Users.Contains(player.UserId) || Permission.Groups.Any() && ServerStatic.PermissionsHandler._members.TryGetValue(player.UserId, out string group) && Permission.Groups.Contains(group))
                    {
                        goto FINAL;
                    }

                    response = Messages[CommandResultType.PermissionError];

                    return false;
                }

                goto FINAL;

            FINAL:
                var result = Handle(args, player, out response);

                if (string.IsNullOrEmpty(response))
                {
                    response = Messages[result];
                }

                CommandUse use = new (DateTime.Now, result);

                History.Add(player, use);

                return result == CommandResultType.Success;
            }
            catch (Exception ex)
            {
                response = string.Format(Messages[CommandResultType.Error], ex.ToString());

                return false;
            }
        }

        public abstract bool ParseSyntax(List<string> input, int count, out List<object> output);

        public abstract CommandResultType Handle(List<object> arguments, Player player, out string response);
    }
}
