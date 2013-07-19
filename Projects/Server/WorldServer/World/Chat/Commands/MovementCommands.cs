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

using Framework.Helper;
using WorldServer.Network;
using WorldServer.Packets.Handlers;
using WorldServer.World.Chat.Commands.Manager;

namespace WorldServer.World.Chat.Commands
{
    public class MovementCommands : Command
    {
        [ChatCommand("tele")]
        public static void Go(string[] args, WorldSession session)
        {
            uint worldId = Read<uint>(args, 0);

            if (args.Length == 2)
            {
                ObjectHandler.SendWorldLocation(session, (int)worldId);

                // ToDo: Send correct location in ObjectInfo packet
                ObjectHandler.SendRequiredWorldPackets(session);
            }
        }
    }
}
