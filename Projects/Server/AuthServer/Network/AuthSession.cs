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
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using AuthServer.Packets;
using AuthServer.Packets.Handlers;
using Framework.Logging;
using Framework.Network.Packets;
using Framework.Objects;

namespace AuthServer.Network
{
    public class AuthSession
    {
        public readonly ManualResetEvent threadManager;
        public Queue PacketQueue = new Queue();
        public ClientInfo Client;

        Socket client;
        byte[] DataBuffer = new byte[0x400];

        public AuthSession()
        {
            threadManager = new ManualResetEvent(false);
            Client = new ClientInfo();
        }

        public void Accept(IAsyncResult state)
        {
            client = (state.AsyncState as TcpListener).EndAcceptSocket(state);

            threadManager.Set();

            var clientInfo = (IPEndPoint)client.RemoteEndPoint;

            // ToDo: Add Account to ClientInfo
            Client.Id = 1;
            Client.IP = clientInfo.Address.ToString();
            Client.Port = clientInfo.Port;

            MiscHandler.OnClientConnection(this);

            client.BeginReceive(DataBuffer, 0, DataBuffer.Length, SocketFlags.None, Process, client);
        }

        public void Process(IAsyncResult state)
        {
            try
            {
                var socket = state.AsyncState as Socket;
                var recievedBytes = client.EndReceive(state);

                if (recievedBytes != 0)
                {
                    while (recievedBytes > 0)
                    {
                        var length = BitConverter.ToUInt16(DataBuffer, 0);

                        var packetData = new byte[length];
                        Buffer.BlockCopy(DataBuffer, 0, packetData, 0, length);

                        Packet packet = new Packet(packetData);
                        PacketQueue.Enqueue(packet);

                        recievedBytes -= length;
                        Buffer.BlockCopy(DataBuffer, length, DataBuffer, 0, recievedBytes);

                        ProcessPacket();
                    }

                    client.BeginReceive(DataBuffer, 0, DataBuffer.Length, SocketFlags.None, Process, socket);
                }
                else
                    client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex.Message);
            }
        }

        void ProcessPacket()
        {
            Packet packet = null;
            if (PacketQueue.Count > 0)
                packet = (Packet)PacketQueue.Dequeue();
            else
                packet = new Packet(DataBuffer);

            PacketManager.InvokeHandler(packet, this);
        }

        public void Send(Packet packet)
        {
            var buffer = packet.GetDataToSend();

            try
            {
                client.Send(buffer, 0, buffer.Length, SocketFlags.None);
            }
            catch (Exception ex)
            {
                Log.Message(LogType.Error, "{0}", ex.Message);
                Log.Message();

                client.Close();
            }
        }
    }
}
