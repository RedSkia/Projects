using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Oxide.Core.Configuration;
using ConVar;
using Network;

namespace Oxide.Plugins
{
    [Info("XPve", "Infinitynet.dk/Xray", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XPve2 : RustPlugin
    {
        public const ulong IconID = 76561198389709969;
        public const string TitleColor = "#4baffa";
        public const string MsgColor = "#96c8fa";
        public const int MaxWarnings = 5;
        #region Helpers

        private static Dictionary<ulong, int> WarningCount = new Dictionary<ulong, int>();
        private void AddWarning(BasePlayer player)
        {
            if (!WarningCount.ContainsKey(player.userID)) { WarningCount.Add(player.userID, 0); }
            WarningCount[player.userID]++;
            if (WarningCount[player.userID] > MaxWarnings) 
            {
                WarningCount.Remove(player.userID);
                player.Kick("[PVE] Du brød for mange regler -- Læs server beskrivelsen tak");
            }
        }

        private void CancelHit(HitInfo hitInfo)
        {
            hitInfo.damageTypes.ScaleAll(0);
            hitInfo.DidHit = false;
            hitInfo.DoHitEffects = false;
        }

        public void DirectMessage(BasePlayer player, string message)
        {
            try { player.SendConsoleCommand("chat.add", 0, IconID, $"<size=16>[<color={TitleColor}>System</color>]</size> {message}"); }
            catch { SendReply(player, message, 2, IconID); }
        }

        /// <summary>
        /// <list type="table">Attacker <paramref name="player1"/></list>
        /// <list type="table">Victim <paramref name="player2"/></list>
        /// </summary>
        private bool IsTeam(ulong player1, ulong player2)
        {
            BasePlayer targetPlayer1 = null;
            BasePlayer.TryFindByID(player1, out targetPlayer1);

            BasePlayer targetPlayer2 = null;
            BasePlayer.TryFindByID(player2, out targetPlayer2);
            if (targetPlayer1 != null && targetPlayer2 != null)
            {
                if(targetPlayer1.currentTeam == 0) { DirectMessage(targetPlayer1, ReplyMessages.MakeTeam); }
                if (targetPlayer1.currentTeam != 0 && targetPlayer2.currentTeam != 0)
                {
                    RelationshipManager.PlayerTeam team = RelationshipManager.ServerInstance.FindTeam(targetPlayer1.currentTeam);
                    if (team.members.Contains(targetPlayer2.userID)) { return true; }
                }
            }
            else
            {
                try
                {
                    if(targetPlayer1 != null)
                    {
                        if (targetPlayer1.currentTeam == 0) { DirectMessage(targetPlayer1, ReplyMessages.MakeTeam); }
                    }
                    RelationshipManager.PlayerTeam team = RelationshipManager.ServerInstance.FindTeam(player1);
                    if (team.members.Contains(player2)) { return true; }
                } catch { }
            }

            return false;
        }
        #endregion Helpers

        #region Messages
        private class ReplyMessages
        {
            public const string MakeTeam = "Lav et hold med spilleren for at kunne interagere";
            public const string NoSteal = "Du må ikke stjæle andres spilleres ting!";
            public const string NoKilling = "Du må ikke skade eller dræbe andre spillere!";
            public const string NoBuildingDamage = "Du må ikke skade eller ødelægge andre spillers ting!";
        }
        #endregion Messages




        #region On Loot
        bool CanLootPlayer(BasePlayer target, BasePlayer looter)
        {
            if (target.userID != looter.userID)
            {
                if (!IsTeam(looter.userID, target.userID))
                {
                    AddWarning(looter);
                    DirectMessage(looter, ReplyMessages.NoSteal);
                    return false;
                }
            }
            return true;
        }

        object CanLootEntity(BasePlayer looter, object container)
        {
            if(container != null && (container.GetType() == typeof(PlayerCorpse) || (container.GetType() == typeof(DroppedItemContainer))))
            {
                if(container.GetType() == typeof(PlayerCorpse))
                {
                    PlayerCorpse playerCorpse = (PlayerCorpse)container;
                    if ((playerCorpse.playerSteamID.IsSteamId() && looter.userID.IsSteamId()) && playerCorpse.playerSteamID != looter.userID)
                    {
                        if(!IsTeam(looter.userID, playerCorpse.playerSteamID))
                        {
                            AddWarning(looter);
                            DirectMessage(looter, ReplyMessages.NoSteal);
                            return false;
                        }
   
                    }
                }
                if (container.GetType() == typeof(DroppedItemContainer))
                {
                    DroppedItemContainer itemContainer = (DroppedItemContainer)container;
                    if ((itemContainer.playerSteamID.IsSteamId() && looter.userID.IsSteamId()) && itemContainer.playerSteamID != looter.userID)
                    {
                        if (!IsTeam(looter.userID, itemContainer.playerSteamID))
                        {
                            AddWarning(looter);
                            DirectMessage(looter, ReplyMessages.NoSteal);
                            return false;
                        }
                    }
                }
            }
            return null;
        }
        #endregion On Loot

        #region On Damage
        /*On Player Hurt*/
        object OnEntityTakeDamage(BasePlayer target, HitInfo info)
        {
            BasePlayer attacker = info?.InitiatorPlayer;
            if(attacker != null && target != null && target?.userID != attacker.userID && target.userID.IsSteamId() && attacker.userID.IsSteamId()) 
            {
                if (!IsTeam(attacker.userID, target.userID))
                {
                    AddWarning(attacker);
                    DirectMessage(attacker, ReplyMessages.NoKilling);
                    return false;
                }
            }
            return null;
        }
        object OnEntityTakeDamage(BaseCombatEntity entity, HitInfo info)
        {
            if (entity is BuildingBlock  || entity.name.Contains("deploy") || entity.name.Contains("building"))
            {
                BasePlayer attacker = info?.InitiatorPlayer;
                if(attacker != null && entity != null && attacker.userID != entity.OwnerID && attacker.userID.IsSteamId() && entity.OwnerID.IsSteamId())
                {
                    if (!IsTeam(attacker.userID, entity.OwnerID))
                    {
                        AddWarning(attacker);
                        DirectMessage(attacker, ReplyMessages.NoBuildingDamage);
                        return false;
                    }
                }
            }

            return null;
        }
        #endregion On Damage        
    }
}