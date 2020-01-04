using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using Console = RoR2.Console;

namespace RoRCheats
{
    public static class Util
    {
        public static void banPlayer(NetworkUser PlayerName, NetworkUser LocalNetworkUser)
        {
            Console.instance.RunClientCmd(LocalNetworkUser, "ban_steam", new string[] { PlayerName.Network_id.steamId.ToString() });
        }

        public static NetworkUser GetNetUserFromString(string playerString)
        {
            if (playerString != "")
            {
                if (int.TryParse(playerString, out int result))
                {
                    if (result < NetworkUser.readOnlyInstancesList.Count && result >= 0)
                    {
                        return NetworkUser.readOnlyInstancesList[result];
                    }
                    return null;
                }
                else
                {
                    foreach (NetworkUser n in NetworkUser.readOnlyInstancesList)
                    {
                        if (n.userName.Equals(playerString, StringComparison.CurrentCultureIgnoreCase))
                        {
                            return n;
                        }
                    }
                    return null;
                }
            }
            return null;
        }

        public static void GetPlayers(string[] Players)
        {
            NetworkUser n;
            for (int i = 0; i < NetworkUser.readOnlyInstancesList.Count; i++)
            {
                n = NetworkUser.readOnlyInstancesList[i];

                Players[i] = n.userName;
            }
        }

        public static Boolean CursorIsVisible()
        {
            foreach (var mpeventSystem in RoR2.UI.MPEventSystem.readOnlyInstancesList)
                if (mpeventSystem.isCursorVisible)
                    return true;
            return false;
        }

        public static void kickPlayer(NetworkUser PlayerName, NetworkUser LocalNetworkUser)
        {
            Console.instance.RunClientCmd(LocalNetworkUser, "kick_steam", new string[] { PlayerName.Network_id.steamId.ToString() });
        }

        public static void RESETMENU()
        {
            Main._ifDragged = false;
            Main._CharacterCollected = false;
            Main._isStatMenuOpen = false;
            Main._isTeleMenuOpen = false;
            Main._ItemToggle = false;
            Main._CharacterToggle = false;
            Main._isLobbyMenuOpen = false;
            Main._isEditStatsOpen = false;
            Main._isItemSpawnMenuOpen = false;
            Main._isPlayerMod = false;
            Main._isEquipmentSpawnMenuOpen = false;
            Main._isItemManagerOpen = false;
            Main.damageToggle = false;
            Main.critToggle = false;
            Main.skillToggle = false;
            Main.renderInteractables = false;
            Main.attackSpeedToggle = false;
            Main.armorToggle = false;
            Main.regenToggle = false;
            Main.moveSpeedToggle = false;
            Main.NoclipToggle = false;
            Main.isDropItemForAll = false;
            Main.alwaysSprint = false;
            Main.aimBot = false;
        }

        public static void unlockall()
        {
            var unlockables = typeof(UnlockableCatalog).GetFieldValue<Dictionary<String, UnlockableDef>>("nameToDefTable");
            foreach (var unlockable in unlockables)
                Run.instance.GrantUnlockToAllParticipatingPlayers(unlockable.Key);
            foreach (var networkUser in NetworkUser.readOnlyInstancesList)
                networkUser.AwardLunarCoins(100);
            var achievementManager = AchievementManager.GetUserAchievementManager(LocalUserManager.GetFirstLocalUser());
            foreach (var achievement in AchievementManager.allAchievementDefs)
                achievementManager.GrantAchievement(achievement);
            var profile = LocalUserManager.GetFirstLocalUser().userProfile;
            foreach (var survivor in SurvivorCatalog.allSurvivorDefs)
            {
                if (profile.statSheet.GetStatValueDouble(RoR2.Stats.PerBodyStatDef.totalTimeAlive, survivor.bodyPrefab.name) == 0.0)
                    profile.statSheet.SetStatValueFromString(RoR2.Stats.PerBodyStatDef.totalTimeAlive.FindStatDef(survivor.bodyPrefab.name), "0.1");
            }
            for (int i = 0; i < 150; i++)
            {
                profile.DiscoverPickup(PickupCatalog.FindPickupIndex((ItemIndex)i));
                profile.DiscoverPickup(PickupCatalog.FindPickupIndex((EquipmentIndex)i));
            }
        }
    }
}