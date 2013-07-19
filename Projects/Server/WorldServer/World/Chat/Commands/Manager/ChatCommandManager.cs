/*
 * Copyright (C) 2012-2013 Arctium Emulation <http://arctium.org>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using WorldServer.Network;

namespace WorldServer.World.Chat.Commands.Manager
{
    class ChatCommandManager
    {
        public static Dictionary<string, HandleChatCommand> ChatCommands = new Dictionary<string, HandleChatCommand>();
        public delegate void HandleChatCommand(string[] args, WorldSession session);

        public static void DefineChatCommands()
        {
            Assembly currentAsm = Assembly.GetExecutingAssembly();
            foreach (var type in currentAsm.GetTypes())
            {
                foreach (var methodInfo in type.GetMethods())
                {
                    var chatAttr = methodInfo.GetCustomAttribute<ChatCommandAttribute>();

                    if (chatAttr != null)
                        ChatCommands[chatAttr.ChatCommand] = (HandleChatCommand)Delegate.CreateDelegate(typeof(HandleChatCommand), methodInfo);
                }
            }
        }

        public static void ExecuteChatHandler(string chatCommand, WorldSession session)
        {
            var args = chatCommand.Split(new string[] { " " }, StringSplitOptions.None);
            var command = args[0].Remove(0, 1);

            if (ChatCommands.ContainsKey(command))
                ChatCommands[command].Invoke(args, session);
        }

        public static bool CheckForCommand(string command)
        {
            return command.StartsWith("!") ? true : false;
        }
    }
}
