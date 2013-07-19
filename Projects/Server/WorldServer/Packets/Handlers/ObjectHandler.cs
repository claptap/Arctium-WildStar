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

namespace WorldServer.Packets.Handlers
{
    public class ObjectHandler
    {
        [Message(ClientMessage.RequestWorldEnter, "6182")]
        public static void OnRequestWorldEnter(Packet packet, WorldSession session)
        {
            SendWorldLocation(session, -1);
            SendRequiredWorldPackets(session);
        }

        public static void SendRequiredWorldPackets(WorldSession session)
        {
            SendObjectInfo(session);
            SendPlayerChanged(session);
            SendGameMode(session);
            SendUnknown(session);
            SendUnknown2(session);
            SendUnknown3(session);
            SendChatZoneChange(session);
        }

        /// <summary>
        /// Answer for OnRequestWorldEnter
        /// </summary>
        public static void SendWorldLocation(WorldSession session, int worldId = -1)
        {
            var worldLocation = new Packet(ServerMessage.WorldLocation);

            if (worldId <= -1)
                worldLocation.Write<uint>(426);
            else
                worldLocation.Write<uint>((uint)worldId);

            // ToDo: Use location from character database
            // These coords doesn't handle the real position
            // Real location is sent in ObjectInfo packet, except WorldId
            worldLocation.Write(3800.97f);
            worldLocation.Write(-800.516f);
            worldLocation.Write(-1360.65f);
            worldLocation.Write(1f);         // Yaw
            worldLocation.Write(0f);         // Pitch
            worldLocation.Write<uint>(0);    // TileId

            session.Send(worldLocation);
        }

        /// <summary>
        /// Answer for OnRequestWorldEnter
        /// </summary>
        public static void SendObjectInfo(WorldSession session)
        {
            var objectInfo = new Packet(ServerMessage.ObjectInfo, true);

            // Hehe coming soon :)

            session.Send(objectInfo);
        }

        /// <summary>
        /// Answer for OnRequestWorldEnter
        /// </summary>
        static void SendPlayerChanged(WorldSession session)
        {
            var playerChanged = new Packet(ServerMessage.PlayerChanged, true);

            // Hehe coming soon :)

            session.Send(playerChanged);
        }

        /// <summary>
        /// Answer for OnRequestWorldEnter
        /// </summary>
        static void SendGameMode(WorldSession session)
        {
            var gameMode = new Packet(ServerMessage.GameMode);

            session.Send(gameMode);
        }

        /// <summary>
        /// Answer for OnRequestWorldEnter
        /// </summary>
        static void SendUnknown(WorldSession session)
        {
            var unknown = new Packet(ServerMessage.Unknown);

            session.Send(unknown);
        }

        /// <summary>
        /// Answer for OnRequestWorldEnter
        /// </summary>
        static void SendUnknown2(WorldSession session)
        {
            var unknown2 = new Packet(ServerMessage.Unknown2, true);

            // Hehe coming soon :)

            session.Send(unknown2);
        }

        /// <summary>
        /// Answer for OnRequestWorldEnter
        /// </summary>
        static void SendUnknown3(WorldSession session)
        {
            var unknown3 = new Packet(ServerMessage.Unknown3);

            session.Send(unknown3);
        }

        /// <summary>
        /// Answer for OnRequestWorldEnter
        /// </summary>
        static void SendChatZoneChange(WorldSession session)
        {
            var chatZoneChange = new Packet(ServerMessage.ChatZoneChange);

            // ToDo: Use zone from character database
            chatZoneChange.Write<uint>(1);  // WorldZoneId

            session.Send(chatZoneChange);
        }
    }
}
