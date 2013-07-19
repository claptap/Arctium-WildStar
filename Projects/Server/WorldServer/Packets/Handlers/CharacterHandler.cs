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
    public class CharacterHandler
    {
        [Message(ClientMessage.RequestCharacterList, "6182")]
        public static void OnRequestCharacterList(Packet packet, WorldSession session)
        {
            SendCharacterList(session);
        }

        /// <summary>
        /// Answer for OnRequestCharacterList
        /// </summary>
        static void SendCharacterList(WorldSession session)
        {
            var characterList = new Packet(ServerMessage.CharacterList);

            // Empty character list
            characterList.Write<int>(0, 0, false);  // Count
            characterList.Write<int>(1);            // AdditionalCount (Allowed char creations)
            characterList.Write<int>(0, 0, false);

            session.Send(characterList);
        }

        [Message(ClientMessage.RequestCharacterCreate, "6182")]
        public static void OnRequestCharacterCreate(Packet packet, WorldSession session)
        {
            

            SendCharacterCreate(session);
        }

        /// <summary>
        /// Answer for OnRequestCharacterCreate
        /// </summary>
        static void SendCharacterCreate(WorldSession session)
        {
            
        }
    }
}
