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
using AuthServer.Network;
using Framework.Constans;
using Framework.Network.Packets;

namespace AuthServer.Packets
{
    public class PacketManager
    {
        static Dictionary<ClientMessage, HandlePacket> MessageHandlers = new Dictionary<ClientMessage, HandlePacket>();
        delegate void HandlePacket(Packet packet, AuthSession session);

        public static void DefineMessageHandler()
        {
            Assembly currentAsm = Assembly.GetExecutingAssembly();
            foreach (var type in currentAsm.GetTypes())
            {
                foreach (var methodInfo in type.GetMethods())
                {
                    foreach (var opcodeAttr in methodInfo.GetCustomAttributes<MessageAttribute>())
                        if (opcodeAttr != null)
                            MessageHandlers[opcodeAttr.Message] = (HandlePacket)Delegate.CreateDelegate(typeof(HandlePacket), methodInfo);
                }
            }
        }

        public static void InvokeHandler(Packet reader, AuthSession session)
        {
            if (MessageHandlers.ContainsKey((ClientMessage)reader.Header.Message))
                MessageHandlers[(ClientMessage)reader.Header.Message].Invoke(reader, session);
        }
    }
}
