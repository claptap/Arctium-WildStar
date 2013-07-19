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

using System.Threading;
using AuthServer.Network;
using Framework.Constans;
using Framework.Logging;
using Framework.Network.Packets;

namespace AuthServer.Packets.Handlers
{
    public class AuthHandler
    {
        [Message(ClientMessage.AuthRequest, "6182")]
        public static void OnAuthRequest(Packet packet, AuthSession session)
        {
            var accountName = packet.ReadString();
            var clientBuild = packet.Read<uint>();

            Log.Message(LogType.Debug, "Login attempt for Account: {0}, Build: {1}", accountName, clientBuild);

            // ToDo: Check if account exists, we're a sandbox atm :P
            if (accountName != "" && accountName != "''")
                SendPasswordCrypt(session);
        }

        /// <summary>
        /// Answer for OnAuthRequest
        /// </summary>
        static void SendPasswordCrypt(AuthSession session)
        {
            var passwordCrypt = new Packet(ServerMessage.PasswordCrypt);

            // ToDo: Generate a Salt/Random value and store them
            // in the database
            passwordCrypt.Write<long>(0);  // Salt
            passwordCrypt.Write<long>(0);  // Random

            session.Send(passwordCrypt);
        }

        [Message(ClientMessage.PasswordDigest, "6182")]
        public static void OnPasswordDigest(Packet packet, AuthSession session)
        {
            // ToDo: Check password (SHA1 with Salt & Random value)
            if (true)
            {
                // Prevent random disconnects after auth.
                Thread.Sleep(100);

                SendAuthComplete(session);
                RealmHandler.SendRealmList(session);
            }
        }

        /// <summary>
        /// Possible answer for OnPasswordDigest (Success)
        /// </summary>
        static void SendAuthComplete(AuthSession session)
        {
            var authComplete = new Packet(ServerMessage.AuthComplete);

            session.Send(authComplete);
        }
    }
}
