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

namespace Framework.Constans
{
    public enum ClientMessage : ushort
    {
        State                  = 0x000,
        State2                 = 0x001,
        RequestWorldEnter      = 0x447,
        Ping                   = 0x052,  // Not used
        RequestCharacterCreate = 0x05C,  // Not used
        MultiPacket            = 0x1D0,
        ChatMessage            = 0x11E,
        AuthRequest            = 0x2EC,
        PasswordDigest         = 0x2EF,
        SelectRealm            = 0x449,
        RequestCharacterList   = 0x44A,
    }
}
