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
    public class MiscHandler
    {
        public static void OnClientConnection(AuthSession session)
        {
            Log.Message(LogType.Debug, "Received Client Connection from IP: {0}, Port: {1}", session.Client.IP, session.Client.Port);

            // Send server hello message
            SendSHello(session);
        }

        /// <summary>
        /// Answer for OnClientConnection
        /// </summary>
        static void SendSHello(AuthSession session)
        {
            var sHello = new Packet(ServerMessage.SHello);

            sHello.Write<int>(6182);                // BuildNumber
            sHello.Write<int>(0);
            sHello.Write<int>(0);
            sHello.Write<int>(0);
            sHello.Write<long>(0);
            sHello.Write<int>(0);
            sHello.Write<int>(0);
            sHello.WriteDefault<uint>(0xA82B20A8);  // NetworkMessageCRC
            sHello.Write<int>(0);
            sHello.Write<long>(0);

            session.Send(sHello);
        }

        [Message(ClientMessage.State, "6182")]
        public static void OnState(Packet packet, AuthSession session)
        {
            var state = packet.Read<byte>();

            SendStateResponse(session, state);
        }

        /// <summary>
        /// Answer for OnState
        /// </summary>
        static void SendStateResponse(AuthSession session, ushort state)
        {
            var stateResponse = new Packet(ServerMessage.StateResponse);

            stateResponse.Write<ushort>(state);

            session.Send(stateResponse);
        }

        [Message(ClientMessage.State2, "6182")]
        public static void OnState2(Packet packet, AuthSession session)
        {
            var state = packet.Read<byte>();

            SendStateResponse2(session, state);
        }

        /// <summary>
        /// Answer for OnState2
        /// </summary>
        static void SendStateResponse2(AuthSession session, ushort state)
        {
            var stateResponse2 = new Packet(ServerMessage.StateResponse2);

            stateResponse2.Write<ushort>(state);

            session.Send(stateResponse2);
        }
    }
}
