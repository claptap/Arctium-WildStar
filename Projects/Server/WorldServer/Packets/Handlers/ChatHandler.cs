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

using Framework.Constans;
using Framework.Network.Packets;
using WorldServer.Network;
using WorldServer.World.Chat.Commands.Manager;

namespace WorldServer.Packets.Handlers
{
    public class ChatHandler
    {
        [Message(ClientMessage.ChatMessage, "6182")]
        public static void OnChatMessage(Packet packet, WorldSession session)
        {
            var channelId   = packet.Read<uint>();
            var chatId      = packet.Read<ulong>();
            var message     = packet.ReadString();
            var formatCount = packet.Read<uint>();

            // ToDo: Handle server answer, we're still a sandbox...

            if (ChatCommandManager.CheckForCommand(message))
                ChatCommandManager.ExecuteChatHandler(message, session);
        }
    }
}
