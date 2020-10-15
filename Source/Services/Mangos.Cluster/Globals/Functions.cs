﻿//
//  Copyright (C) 2013-2020 getMaNGOS <https:\\getmangos.eu>
//  
//  This program is free software. You can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation. either version 2 of the License, or
//  (at your option) any later version.
//  
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY. Without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//  
//  You should have received a copy of the GNU General Public License
//  along with this program. If not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//

using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Mangos.Cluster.Handlers;
using Mangos.Cluster.Server;
using Mangos.Common;
using Mangos.Common.Enums.Chat;
using Mangos.Common.Enums.Global;
using Mangos.Common.Enums.Misc;
using Mangos.Common.Enums.Player;
using Mangos.Common.Globals;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Mangos.Cluster.Globals
{
    public class Functions
    {
        public int ToInteger(bool value)
        {
            if (value)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public string ToHex(byte[] bBytes, int start = 0)
        {
            if (bBytes.Length == 0)
                return "''";
            string tmpStr = "0x";
            for (int i = start, loopTo = bBytes.Length - 1; i <= loopTo; i++)
            {
                if (bBytes[i] < 16)
                {
                    tmpStr += "0" + Conversion.Hex(bBytes[i]);
                }
                else
                {
                    tmpStr += Conversion.Hex(bBytes[i]);
                }
            }

            return tmpStr;
        }

        public char[] ByteToCharArray(byte[] bBytes)
        {
            if (bBytes.Length == 0)
                return new char[] { };
            var bChar = new char[bBytes.Length];
            for (int i = 0, loopTo = bBytes.Length - 1; i <= loopTo; i++)
                bChar[i] = (char)bBytes[i];
            return bChar;
        }

        public int[] ByteToIntArray(byte[] bBytes)
        {
            if (bBytes.Length == 0)
                return new int[] { };
            var bInt = new int[(bBytes.Length - 1) / 4 + 1];
            for (int i = 0, loopTo = bBytes.Length - 1; i <= loopTo; i += 4)
                bInt[i / 4] = BitConverter.ToInt32(bBytes, i);
            return bInt;
        }

        public byte[] IntToByteArray(int[] bInt)
        {
            if (bInt.Length == 0)
                return new byte[] { };
            var bBytes = new byte[(bInt.Length * 4)];
            for (int i = 0, loopTo = bInt.Length - 1; i <= loopTo; i++)
            {
                var tmpBytes = BitConverter.GetBytes(bInt[i]);
                Array.Copy(tmpBytes, 0, bBytes, i * 4, 4);
            }

            return bBytes;
        }

        public byte[] Concat(byte[] a, byte[] b)
        {
            var buffer1 = new byte[(a.Length + b.Length)];
            int num1;
            var loopTo = a.Length - 1;
            for (num1 = 0; num1 <= loopTo; num1++)
                buffer1[num1] = a[num1];
            int num2;
            var loopTo1 = b.Length - 1;
            for (num2 = 0; num2 <= loopTo1; num2++)
                buffer1[num2 + a.Length] = b[num2];
            return buffer1;
        }

        public bool HaveFlag(uint value, byte flagPos)
        {
            value >>= (int)(uint)flagPos;
            value = (uint)(value % 2L);
            if (value == 1L)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HaveFlags(int value, int flags)
        {
            return (value & flags) == flags;
        }

        public void SetFlag(ref uint value, byte flagPos, bool flagValue)
        {
            if (flagValue)
            {
                value |= 0x1U << (int)(uint)flagPos;
            }
            else
            {
                value = value & 0x0U << (int)(uint)flagPos & 0xFFFFFFFFU;
            }
        }

        public DateTime GetNextDay(DayOfWeek iDay, int hour = 0)
        {
            int iDiff = (int)iDay - (int)DateAndTime.Today.DayOfWeek;
            if (iDiff <= 0)
                iDiff += 7;
            var nextFriday = DateAndTime.Today.AddDays(iDiff);
            nextFriday = nextFriday.AddHours(hour);
            return nextFriday;
        }

        public DateTime GetNextDate(int days, int hours = 0)
        {
            var nextDate = DateAndTime.Today.AddDays(days);
            nextDate = nextDate.AddHours(hours);
            return nextDate;
        }

        public uint GetTimestamp(DateTime fromDateTime)
        {
            var startDate = DateTime.Parse("1970-01-01");
            TimeSpan timeSpan;
            timeSpan = fromDateTime.Subtract(startDate);
            return (uint)Math.Abs(timeSpan.TotalSeconds);
        }

        public DateTime GetDateFromTimestamp(uint unixTimestamp)
        {
            TimeSpan timeSpan;
            var startDate = DateTime.Parse("1970-01-01");
            if (unixTimestamp == 0L)
                return startDate;
            timeSpan = new TimeSpan(0, 0, (int)unixTimestamp);
            return startDate.Add(timeSpan);
        }

        public string GetTimeLeftString(uint seconds)
        {
            if (seconds < 60L)
            {
                return seconds + "s";
            }
            else if (seconds < 3600L)
            {
                return seconds / 60L + "m " + seconds % 60L + "s";
            }
            else if (seconds < 86400L)
            {
                return seconds / 3600L + "h " + seconds / 60L % 60L + "m " + seconds % 60L + "s";
            }
            else
            {
                return seconds / 86400L + "d " + seconds / 3600L % 24L + "h " + seconds / 60L % 60L + "m " + seconds % 60L + "s";
            }
        }

        public string EscapeString(string s)
        {
            return s.Replace("\"", "").Replace("'", "");
        }

        public string CapitalizeName(string name)
        {
            return name.Length > 1 ? Strings.UCase(Strings.Left(name, 1)) + Strings.LCase(Strings.Right(name, name.Length - 1)) : Strings.UCase(name);
        }

        private readonly Regex Regex_AZ = new Regex("^[a-zA-Z]+$");

        public bool ValidateName(string strName)
        {
            if (strName.Length < 2 || strName.Length > 16)
                return false;
            return Regex_AZ.IsMatch(strName);
        }

        private readonly Regex Regex_Guild = new Regex("^[a-z A-Z]+$");

        public bool ValidateGuildName(string strName)
        {
            if (strName.Length < 2 || strName.Length > 16)
                return false;
            return Regex_Guild.IsMatch(strName);
        }

        public string FixName(string strName)
        {
            return strName.Replace("\"", "'").Replace("<", "").Replace(">", "").Replace("*", "").Replace("/", "").Replace(@"\", "").Replace(":", "").Replace("|", "").Replace("?", "");
        }

        public void RAND_bytes(ref byte[] bBytes, int length)
        {
            if (length == 0)
                return;
            bBytes = (new byte[length]);
            for (int i = 0, loopTo = length - 1; i <= loopTo; i++)
            {
                if (i == bBytes.Length)
                    break;
                bBytes[i] = (byte)new Random().Next(0, 256);
            }
        }

        public float MathLerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public void Ban_Account(string name, string reason)
        {
            var account = new DataTable();
            var bannedAccount = new DataTable();
            ClusterServiceLocator._WorldCluster.GetAccountDatabase().Query(string.Format("SELECT id, username FROM account WHERE username = {0};", name), ref account);
            if (account.Rows.Count > 0)
            {
                int accId = account.Rows[0].As<int>("id");
                ClusterServiceLocator._WorldCluster.GetAccountDatabase().Query(string.Format("SELECT id, active FROM account_banned WHERE id = {0};", accId), ref bannedAccount);
                if (bannedAccount.Rows.Count > 0)
                {
                    ClusterServiceLocator._WorldCluster.GetAccountDatabase().Update("UPDATE account_banned SET active = 1 WHERE id = '" + accId + "';");
                }
                else
                {
                    string tempBanDate = Strings.FormatDateTime(Conversions.ToDate(DateTime.Now.ToFileTimeUtc().ToString()), DateFormat.LongDate) + " " + Strings.FormatDateTime(Conversions.ToDate(DateTime.Now.ToFileTimeUtc().ToString()), DateFormat.LongTime);
                    ClusterServiceLocator._WorldCluster.GetAccountDatabase().Update(string.Format("INSERT INTO `account_banned` VALUES ('{0}', UNIX_TIMESTAMP('{1}'), UNIX_TIMESTAMP('{2}'), '{3}', '{4}', active = 1);", accId, tempBanDate, "0000-00-00 00:00:00", name, reason));
                }

                ClusterServiceLocator._WorldCluster.Log.WriteLine(LogType.INFORMATION, "Account [{0}] banned by server. Reason: [{1}].", name, reason);
            }
            else
            {
                ClusterServiceLocator._WorldCluster.Log.WriteLine(LogType.INFORMATION, "Account [{0}] NOT Found in Database.", name);
            }
        }

        public string GetClassName(int classe)
        {
            string GetClassNameRet;
            switch ((Classes)classe)
            {
                case var @case when @case == Classes.CLASS_DRUID:
                    {
                        GetClassNameRet = "Druid";
                        break;
                    }

                case var case1 when case1 == Classes.CLASS_HUNTER:
                    {
                        GetClassNameRet = "Hunter";
                        break;
                    }

                case var case2 when case2 == Classes.CLASS_MAGE:
                    {
                        GetClassNameRet = "Mage";
                        break;
                    }

                case var case3 when case3 == Classes.CLASS_PALADIN:
                    {
                        GetClassNameRet = "Paladin";
                        break;
                    }

                case var case4 when case4 == Classes.CLASS_PRIEST:
                    {
                        GetClassNameRet = "Priest";
                        break;
                    }

                case var case5 when case5 == Classes.CLASS_ROGUE:
                    {
                        GetClassNameRet = "Rogue";
                        break;
                    }

                case var case6 when case6 == Classes.CLASS_SHAMAN:
                    {
                        GetClassNameRet = "Shaman";
                        break;
                    }

                case var case7 when case7 == Classes.CLASS_WARLOCK:
                    {
                        GetClassNameRet = "Warlock";
                        break;
                    }

                case var case8 when case8 == Classes.CLASS_WARRIOR:
                    {
                        GetClassNameRet = "Warrior";
                        break;
                    }

                default:
                    {
                        GetClassNameRet = "Unknown Class";
                        break;
                    }
            }

            return GetClassNameRet;
        }

        public string GetRaceName(int race)
        {
            string GetRaceNameRet;
            switch ((Races)race)
            {
                case var @case when @case == Races.RACE_DWARF:
                    {
                        GetRaceNameRet = "Dwarf";
                        break;
                    }

                case var case1 when case1 == Races.RACE_GNOME:
                    {
                        GetRaceNameRet = "Gnome";
                        break;
                    }

                case var case2 when case2 == Races.RACE_HUMAN:
                    {
                        GetRaceNameRet = "Human";
                        break;
                    }

                case var case3 when case3 == Races.RACE_NIGHT_ELF:
                    {
                        GetRaceNameRet = "Night Elf";
                        break;
                    }

                case var case4 when case4 == Races.RACE_ORC:
                    {
                        GetRaceNameRet = "Orc";
                        break;
                    }

                case var case5 when case5 == Races.RACE_TAUREN:
                    {
                        GetRaceNameRet = "Tauren";
                        break;
                    }

                case var case6 when case6 == Races.RACE_TROLL:
                    {
                        GetRaceNameRet = "Troll";
                        break;
                    }

                case var case7 when case7 == Races.RACE_UNDEAD:
                    {
                        GetRaceNameRet = "Undead";
                        break;
                    }

                default:
                    {
                        GetRaceNameRet = "Unknown Race";
                        break;
                    }
            }

            return GetRaceNameRet;
        }

        public int GetRaceModel(Races race, int gender)
        {
            switch (race)
            {
                case var @case when @case == Races.RACE_HUMAN:
                    {
                        return 49 + gender;
                    }

                case var case1 when case1 == Races.RACE_ORC:
                    {
                        return 51 + gender;
                    }

                case var case2 when case2 == Races.RACE_DWARF:
                    {
                        return 53 + gender;
                    }

                case var case3 when case3 == Races.RACE_NIGHT_ELF:
                    {
                        return 55 + gender;
                    }

                case var case4 when case4 == Races.RACE_UNDEAD:
                    {
                        return 57 + gender;
                    }

                case var case5 when case5 == Races.RACE_TAUREN:
                    {
                        return 59 + gender;
                    }

                case var case6 when case6 == Races.RACE_GNOME:
                    {
                        return 1563 + gender;
                    }

                case var case7 when case7 == Races.RACE_TROLL:
                    {
                        return 1478 + gender;
                    }

                default:
                    {
                        return 16358;                    // PinkPig? Lol
                    }
            }
        }

        public bool GetCharacterSide(byte race)
        {
            switch ((Races)race)
            {
                case var @case when @case == Races.RACE_DWARF:
                case var case1 when case1 == Races.RACE_GNOME:
                case var case2 when case2 == Races.RACE_HUMAN:
                case var case3 when case3 == Races.RACE_NIGHT_ELF:
                    {
                        return false;
                    }

                default:
                    {
                        return true;
                    }
            }
        }

        public bool IsContinentMap(int map)
        {
            switch (map)
            {
                case 0:
                case 1:
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        public string SetColor(string message, byte red, byte green, byte blue)
        {
            string SetColorRet = "|cFF";
            SetColorRet = red < 16 ? SetColorRet + "0" + Conversion.Hex(red) : SetColorRet + Conversion.Hex(red);
            SetColorRet = green < 16 ? SetColorRet + "0" + Conversion.Hex(green) : SetColorRet + Conversion.Hex(green);
            SetColorRet = blue < 16 ? SetColorRet + "0" + Conversion.Hex(blue) : SetColorRet + Conversion.Hex(blue);
            SetColorRet = SetColorRet + message + "|r";
            return SetColorRet;

            // SetColor = String.Format("|cff{0:x}{1:x}{2:x}{3}|r", Red, Green, Blue, Message)
        }

        public bool RollChance(float chance)
        {
            int nChance = (int)(chance * 100f);
            if (ClusterServiceLocator._WorldCluster.Rnd.Next(1, 10001) <= nChance)
                return true;
            return false;
        }

        public void SendMessageMOTD(WC_Network.ClientClass client, string message)
        {
            var packet = BuildChatMessage(0, message, ChatMsg.CHAT_MSG_SYSTEM, LANGUAGES.LANG_UNIVERSAL);
            client.Send(packet);
        }

        public void SendMessageNotification(WC_Network.ClientClass client, string message)
        {
            var packet = new Packets.PacketClass(Opcodes.SMSG_NOTIFICATION);
            try
            {
                packet.AddString(message);
                client.Send(packet);
            }
            finally
            {
                packet.Dispose();
            }
        }

        public void SendMessageSystem(WC_Network.ClientClass objCharacter, string message)
        {
            var packet = BuildChatMessage(0, message, ChatMsg.CHAT_MSG_SYSTEM, LANGUAGES.LANG_UNIVERSAL, 0, "");
            try
            {
                objCharacter.Send(packet);
            }
            finally
            {
                packet.Dispose();
            }
        }

        public void Broadcast(string message)
        {
            ClusterServiceLocator._WorldCluster.CHARACTERs_Lock.AcquireReaderLock(ClusterServiceLocator._Global_Constants.DEFAULT_LOCK_TIMEOUT);
            foreach (KeyValuePair<ulong, WcHandlerCharacter.CharacterObject> character in ClusterServiceLocator._WorldCluster.CHARACTERs)
            {
                if (character.Value.Client is object)
                    SendMessageSystem(character.Value.Client, "System Message: " + SetColor(message, 255, 0, 0));
            }

            ClusterServiceLocator._WorldCluster.CHARACTERs_Lock.ReleaseReaderLock();
        }

        public void SendAccountMD5(WC_Network.ClientClass client, WcHandlerCharacter.CharacterObject character)
        {
            bool FoundData = false;

            // TODO: How Does Mangos Zero Handle the Account Data For the Characters?
            // Dim AccData As New DataTable
            // _WorldCluster.AccountDatabase.Query(String.Format("SELECT id FROM account WHERE username = ""{0}"";", client.Account), AccData)
            // If AccData.Rows.Count > 0 Then
            // Dim AccID As Integer = CType(AccData.Rows(0).Item("account_id"), Integer)

            // AccData.Clear()
            // _WorldCluster.AccountDatabase.Query(String.Format("SELECT * FROM account_data WHERE account_id = {0}", AccID), AccData)
            // If AccData.Rows.Count > 0 Then
            // FoundData = True
            // Else
            // _WorldCluster.AccountDatabase.Update(String.Format("INSERT INTO account_data VALUES({0}, '', '', '', '', '', '', '', '')", AccID))
            // End If
            // End If

            var smsgAccountDataTimes = new Packets.PacketClass(Opcodes.SMSG_ACCOUNT_DATA_MD5);
            try
            {
                // Dim md5hash As MD5 = MD5.Create()
                for (int i = 0; i <= 7; i++)
                {
                    if (FoundData)
                    {
                    }
                    // Dim tmpBytes() As Byte = AccData.Rows(0).Item("account_data" & i)
                    // If tmpBytes.Length = 0 Then
                    // SMSG_ACCOUNT_DATA_TIMES.AddInt64(0)
                    // SMSG_ACCOUNT_DATA_TIMES.AddInt64(0)
                    // Else
                    // SMSG_ACCOUNT_DATA_TIMES.AddByteArray(md5hash.ComputeHash(tmpBytes))
                    // End If
                    else
                    {
                        smsgAccountDataTimes.AddInt64(0L);
                        smsgAccountDataTimes.AddInt64(0L);
                    }
                }
                // md5hash.Clear()
                // md5hash = Nothing

                client.Send(smsgAccountDataTimes);
            }
            finally
            {
                smsgAccountDataTimes.Dispose();
            }

            ClusterServiceLocator._WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_ACCOUNT_DATA_MD5", client.IP, client.Port);
        }

        public void SendTriggerCinematic(WC_Network.ClientClass client, WcHandlerCharacter.CharacterObject character)
        {
            var packet = new Packets.PacketClass(Opcodes.SMSG_TRIGGER_CINEMATIC);
            try
            {
                if (ClusterServiceLocator._WS_DBCDatabase.CharRaces.ContainsKey((int)character.Race))
                {
                    
                    packet.AddInt32(ClusterServiceLocator._WS_DBCDatabase.CharRaces[(int)character.Race].CinematicID);
                }
                else
                {
                    ClusterServiceLocator._WorldCluster.Log.WriteLine(LogType.WARNING, "[{0}:{1}] SMSG_TRIGGER_CINEMATIC [Error: RACE={2} CLASS={3}]", client.IP, client.Port, character.Race, character.Classe);
                    return;
                }

                client.Send(packet);
            }
            finally
            {
                packet.Dispose();
            }

            ClusterServiceLocator._WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_TRIGGER_CINEMATIC", client.IP, client.Port);
        }

        public void SendTimeSyncReq(WC_Network.ClientClass client)
        {
            // Dim packet As New PacketClass(OPCODES.SMSG_TIME_SYNC_REQ)
            // packet.AddInt32(0)
            // Client.Send(packet)
        }

        public void SendGameTime(WC_Network.ClientClass client, WcHandlerCharacter.CharacterObject character)
        {
            var smsgLoginSettimespeed = new Packets.PacketClass(Opcodes.SMSG_LOGIN_SETTIMESPEED);
            try
            {
                var time = DateTime.Now;
                int year = time.Year - 2000;
                int month = time.Month - 1;
                int day = time.Day - 1;
                int dayOfWeek = (int)time.DayOfWeek;
                int hour = time.Hour;
                int minute = time.Minute;

                // SMSG_LOGIN_SETTIMESPEED.AddInt32(CType((((((Minute + (Hour << 6)) + (DayOfWeek << 11)) + (Day << 14)) + (Year << 18)) + (Month << 20)), Integer))
                smsgLoginSettimespeed.AddInt32(minute + (hour << 6) + (dayOfWeek << 11) + (day << 14) + (month << 20) + (year << 24));
                smsgLoginSettimespeed.AddSingle(0.01666667f);
                client.Send(smsgLoginSettimespeed);
            }
            finally
            {
                smsgLoginSettimespeed.Dispose();
            }

            ClusterServiceLocator._WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_LOGIN_SETTIMESPEED", client.IP, client.Port);
        }

        public void SendProficiency(WC_Network.ClientClass client, byte proficiencyType, int proficiencyFlags)
        {
            var packet = new Packets.PacketClass(Opcodes.SMSG_SET_PROFICIENCY);
            try
            {
                packet.AddInt8(proficiencyType);
                packet.AddInt32(proficiencyFlags);
                client.Send(packet);
            }
            finally
            {
                packet.Dispose();
            }

            ClusterServiceLocator._WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_SET_PROFICIENCY", client.IP, client.Port);
        }

        public void SendCorpseReclaimDelay(WC_Network.ClientClass client, WcHandlerCharacter.CharacterObject character, int seconds = 30)
        {
            var packet = new Packets.PacketClass(Opcodes.SMSG_CORPSE_RECLAIM_DELAY);
            try
            {
                packet.AddInt32(seconds * 1000);
                client.Send(packet);
            }
            finally
            {
                packet.Dispose();
            }

            ClusterServiceLocator._WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_CORPSE_RECLAIM_DELAY [{2}s]", client.IP, client.Port, seconds);
        }

        public Packets.PacketClass BuildChatMessage(ulong senderGuid, string message, ChatMsg msgType, LANGUAGES msgLanguage, byte flag = 0, string msgChannel = "Global")
        {
            var packet = new Packets.PacketClass(Opcodes.SMSG_MESSAGECHAT);
            try
            {
                packet.AddInt8((byte)msgType);
                packet.AddInt32((int)msgLanguage);
                switch (msgType)
                {
                    case var @case when @case == ChatMsg.CHAT_MSG_CHANNEL:
                        {
                            packet.AddString(msgChannel);
                            packet.AddUInt32(0U);
                            packet.AddUInt64(senderGuid);
                            break;
                        }

                    case var case1 when case1 == ChatMsg.CHAT_MSG_YELL:
                    case var case2 when case2 == ChatMsg.CHAT_MSG_SAY:
                    case var case3 when case3 == ChatMsg.CHAT_MSG_PARTY:
                        {
                            packet.AddUInt64(senderGuid);
                            packet.AddUInt64(senderGuid);
                            break;
                        }

                    case var case4 when case4 == ChatMsg.CHAT_MSG_SYSTEM:
                    case var case5 when case5 == ChatMsg.CHAT_MSG_EMOTE:
                    case var case6 when case6 == ChatMsg.CHAT_MSG_IGNORED:
                    case var case7 when case7 == ChatMsg.CHAT_MSG_SKILL:
                    case var case8 when case8 == ChatMsg.CHAT_MSG_GUILD:
                    case var case9 when case9 == ChatMsg.CHAT_MSG_OFFICER:
                    case var case10 when case10 == ChatMsg.CHAT_MSG_RAID:
                    case var case11 when case11 == ChatMsg.CHAT_MSG_WHISPER_INFORM:
                    case var case12 when case12 == ChatMsg.CHAT_MSG_GUILD:
                    case var case13 when case13 == ChatMsg.CHAT_MSG_WHISPER:
                    case var case14 when case14 == ChatMsg.CHAT_MSG_AFK:
                    case var case15 when case15 == ChatMsg.CHAT_MSG_DND:
                    case var case16 when case16 == ChatMsg.CHAT_MSG_RAID_LEADER:
                    case var case17 when case17 == ChatMsg.CHAT_MSG_RAID_WARNING:
                        {
                            packet.AddUInt64(senderGuid);
                            break;
                        }

                    case var case18 when case18 == ChatMsg.CHAT_MSG_MONSTER_SAY:
                    case var case19 when case19 == ChatMsg.CHAT_MSG_MONSTER_EMOTE:
                    case var case20 when case20 == ChatMsg.CHAT_MSG_MONSTER_YELL:
                        {
                            ClusterServiceLocator._WorldCluster.Log.WriteLine(LogType.WARNING, "Use Creature.SendChatMessage() for this message type - {0}!", msgType);
                            break;
                        }

                    default:
                        {
                            ClusterServiceLocator._WorldCluster.Log.WriteLine(LogType.WARNING, "Unknown chat message type - {0}!", msgType);
                            break;
                        }
                }

                packet.AddUInt32((uint)(System.Text.Encoding.UTF8.GetByteCount(message) + 1));
                packet.AddString(message);
                packet.AddInt8(flag);
            }
            catch (Exception)
            {
                ClusterServiceLocator._WorldCluster.Log.WriteLine(LogType.FAILED, "failed chat message type - {0}!", msgType);
            }

            return packet;
        }

        public enum PartyMemberStatsStatus : byte
        {
            STATUS_OFFLINE = 0x0,
            STATUS_ONLINE = 0x1,
            STATUS_PVP = 0x2,
            STATUS_CORPSE = 0x8,
            STATUS_DEAD = 0x10
        }

        public enum PartyMemberStatsBits : byte
        {
            FIELD_STATUS = 0,
            FIELD_LIFE_CURRENT = 1,
            FIElD_LIFE_MAX = 2,
            FIELD_MANA_TYPE = 3,
            FIELD_MANA_CURRENT = 4,
            FIELD_MANA_MAX = 5,
            FIELD_LEVEL = 6,
            FIELD_ZONEID = 7,
            FIELD_POSXPOSY = 8
        }

        public enum PartyMemberStatsFlag : uint
        {
            GROUP_UPDATE_FLAG_NONE = 0x0U, // nothing
            GROUP_UPDATE_FLAG_STATUS = 0x1U, // uint16, flags
            GROUP_UPDATE_FLAG_CUR_HP = 0x2U, // uint16
            GROUP_UPDATE_FLAG_MAX_HP = 0x4U, // uint16
            GROUP_UPDATE_FLAG_POWER_TYPE = 0x8U, // uint8
            GROUP_UPDATE_FLAG_CUR_POWER = 0x10U, // uint16
            GROUP_UPDATE_FLAG_MAX_POWER = 0x20U, // uint16
            GROUP_UPDATE_FLAG_LEVEL = 0x40U, // uint16
            GROUP_UPDATE_FLAG_ZONE = 0x80U, // uint16
            GROUP_UPDATE_FLAG_POSITION = 0x100U, // uint16, uint16
            GROUP_UPDATE_FLAG_AURAS = 0x200U, // uint64 mask, for each bit set uint16 spellid + uint8 unk
            GROUP_UPDATE_FLAG_PET_GUID = 0x400U, // uint64 pet guid
            GROUP_UPDATE_FLAG_PET_NAME = 0x800U, // pet name, NULL terminated string
            GROUP_UPDATE_FLAG_PET_MODEL_ID = 0x1000U, // uint16, model id
            GROUP_UPDATE_FLAG_PET_CUR_HP = 0x2000U, // uint16 pet cur health
            GROUP_UPDATE_FLAG_PET_MAX_HP = 0x4000U, // uint16 pet max health
            GROUP_UPDATE_FLAG_PET_POWER_TYPE = 0x8000U, // uint8 pet power type
            GROUP_UPDATE_FLAG_PET_CUR_POWER = 0x10000U, // uint16 pet cur power
            GROUP_UPDATE_FLAG_PET_MAX_POWER = 0x20000U, // uint16 pet max power
            GROUP_UPDATE_FLAG_PET_AURAS = 0x40000U, // uint64 mask, for each bit set uint16 spellid + uint8 unk, pet auras...
            GROUP_UPDATE_PET = GROUP_UPDATE_FLAG_PET_GUID | GROUP_UPDATE_FLAG_PET_NAME | GROUP_UPDATE_FLAG_PET_MODEL_ID | GROUP_UPDATE_FLAG_PET_CUR_HP | GROUP_UPDATE_FLAG_PET_MAX_HP | GROUP_UPDATE_FLAG_PET_POWER_TYPE | GROUP_UPDATE_FLAG_PET_CUR_POWER | GROUP_UPDATE_FLAG_PET_MAX_POWER | GROUP_UPDATE_FLAG_PET_AURAS,
            GROUP_UPDATE_FULL = GROUP_UPDATE_FLAG_STATUS | GROUP_UPDATE_FLAG_CUR_HP | GROUP_UPDATE_FLAG_MAX_HP | GROUP_UPDATE_FLAG_CUR_POWER | GROUP_UPDATE_FLAG_LEVEL | GROUP_UPDATE_FLAG_ZONE | GROUP_UPDATE_FLAG_MAX_POWER | GROUP_UPDATE_FLAG_POSITION | GROUP_UPDATE_FLAG_AURAS,
            GROUP_UPDATE_FULL_PET = GROUP_UPDATE_FULL | GROUP_UPDATE_PET,
            GROUP_UPDATE_FULL_REQUEST_REPLY = 0x7FFC0BFFU
        }

        public Packets.PacketClass BuildPartyMemberStatsOffline(ulong guid)
        {
            var packet = new Packets.PacketClass(Opcodes.SMSG_PARTY_MEMBER_STATS_FULL);
            packet.AddPackGUID(guid);
            packet.AddUInt32((uint)PartyMemberStatsFlag.GROUP_UPDATE_FLAG_STATUS);
            packet.AddInt8((byte)PartyMemberStatsStatus.STATUS_OFFLINE);
            return packet;
        }
    }
}