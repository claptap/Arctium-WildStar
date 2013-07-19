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

using AuthServer.Network;
using Framework.Constans;
using Framework.Logging;
using Framework.Network.Packets;

namespace AuthServer.Packets.Handlers
{
    public class RealmHandler
    {
        /// <summary>
        /// Should be send after SendAuthComplete
        /// </summary>
        public static void SendRealmList(AuthSession session)
        {
            var realmList = new Packet(ServerMessage.RealmList);

            realmList.Write<int>(1);       // Count

            realmList.Write<int>(1);       // Id
            realmList.Write("Arctium");    // Name
            realmList.Write<int>(0);       // Flags
            realmList.Write<int>(12);      // RealmStatus

            realmList.Write<int>(0);       // RealmMessageCount
            realmList.Write("");

            session.Send(realmList);
        }

        [Message(ClientMessage.SelectRealm, "6182")]
        public static void OnSelectRealm(Packet packet, AuthSession session)
        {
            var realmId = packet.Read<uint>();

            if (realmId == 1337)
            {
                Log.Message(LogType.Normal, "Selected Realm: {0}", "Three Wood");

                SendConnectToRealm(session, realmId);
            }
        }

        /// <summary>
        /// Answer for OnSelectRealm
        /// </summary>
        static void SendConnectToRealm(AuthSession session, uint realmId)
        {
            var realmList = new Packet(ServerMessage.ConnectToRealm);

            realmList.Write<byte>(1);      // IP Part 4
            realmList.Write<byte>(0);      // IP Part 3
            realmList.Write<byte>(0);      // IP Part 2
            realmList.Write<byte>(127);    // IP Part 1
            realmList.Write<uint>(24000);  // Port
            realmList.Write<ulong>(0);
            realmList.Write<ulong>(0);
            realmList.Write<uint>(1);      // AcccountId

            session.Send(realmList);
        }
    }
}
