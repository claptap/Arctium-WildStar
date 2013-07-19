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
    public enum ServerMessage : ushort
    {
        StateResponse  = 0x000,
        StateResponse2 = 0x001,
        SHello         = 0x002,
        WorldLocation  = 0x048,
        Pong           = 0x051,  // Not used
        GameMode       = 0x07D,
        CharacterList  = 0x09D,
        PlayerChanged  = 0x0FC,
        ChatZoneChange = 0x132,
        ObjectInfo     = 0x163,
        ConnectToRealm = 0x1EB,
        PasswordCrypt  = 0x2F0,
        AuthComplete   = 0x2F2,
        RealmList      = 0x42D,

        Unknown        = 0x429,
        Unknown2       = 0x06A,
        Unknown3       = 0x026,
    }
}
