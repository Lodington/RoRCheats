using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using R2API.Utils;
using System.Linq;

namespace RoRCheats
{
    internal class PlayerModifiers
    {
        public static void GiveLunarCoins()
        {
            Main.LocalNetworkUser.AwardLunarCoins(Main.coinsToGive);
        }

        public static void AlwaysSprint()
        {
            var localUser = RoR2.LocalUserManager.GetFirstLocalUser();
            if (localUser == null || localUser.cachedMasterController == null || localUser.cachedMasterController.master == null) return;
            var controller = localUser.cachedMasterController;
            var body = controller.master.GetBody();
            if (body && !body.isSprinting && !localUser.inputPlayer.GetButton("Sprint"))
                controller.SetFieldValue("sprintInputPressReceived", true);
        }

        public static void Respawn()
        {
            Main.LocalPlayer.RespawnExtraLife();
            Debug.Log(Main.log + "Player Respawned");
        }

        public static void AimBot()
        {
            if (Util.CursorIsVisible())
                return;
            var localUser = RoR2.LocalUserManager.GetFirstLocalUser();
            var controller = localUser.cachedMasterController;
            if (!controller)
                return;
            var body = controller.master.GetBody();
            if (!body)
                return;
            var inputBank = body.GetComponent<RoR2.InputBankTest>();
            var aimRay = new Ray(inputBank.aimOrigin, inputBank.aimDirection);
            var bullseyeSearch = new RoR2.BullseyeSearch();
            var team = body.GetComponent<RoR2.TeamComponent>();
            bullseyeSearch.teamMaskFilter = RoR2.TeamMask.all;
            bullseyeSearch.teamMaskFilter.RemoveTeam(team.teamIndex);
            bullseyeSearch.filterByLoS = true;
            bullseyeSearch.searchOrigin = aimRay.origin;
            bullseyeSearch.searchDirection = aimRay.direction;
            bullseyeSearch.sortMode = RoR2.BullseyeSearch.SortMode.Distance;
            bullseyeSearch.maxDistanceFilter = float.MaxValue;
            bullseyeSearch.maxAngleFilter = 20f;// ;// float.MaxValue;
            bullseyeSearch.RefreshCandidates();
            var hurtBox = bullseyeSearch.GetResults().FirstOrDefault();
            if (hurtBox)
            {
                Vector3 direction = hurtBox.transform.position - aimRay.origin;
                inputBank.aimDirection = direction;
            }
        }

        public static void GiveMoney()
        {
            Main.LocalPlayer.GiveMoney(Main.moneyToGive);
            Debug.Log(Main.log+"Giving " + Main.moneyToGive + " to the player");
        }

        public static void giveXP()
        {
            Main.LocalPlayer.GiveExperience(Main.xpToGive);
        }
        public static void LevelPlayersCrit()
        {
            try
            {
                Main.Localbody.levelCrit = (float)Main.CritPerLvl;
            }
            catch (NullReferenceException)
            {
            }
        }

        public static void LevelPlayersDamage()
        {
            try
            {
                Main.Localbody.levelDamage = (float)Main.damagePerLvl;
            }
            catch (NullReferenceException)
            {
            }
        }
        public static void SetplayersAttackSpeed()
        {
            try
            {
                Main.Localbody.baseAttackSpeed = Main.attackSpeed;
            }
            catch (NullReferenceException)
            {
            }
        }
        public static void SetplayersArmor()
        {
            try
            {
                Main.Localbody.baseArmor = Main.armor;
            }
            catch (NullReferenceException)
            {
            }
        }
        public static void SetplayersMoveSpeed()
        {
            try
            {
                Main.Localbody.baseMoveSpeed = Main.movespeed;
            }
            catch (NullReferenceException)
            {
            }
        }

        private static void ClearInventory(Inventory LocalPlayerInv)
        {
            if (LocalPlayerInv)
            {
                for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < (ItemIndex)78; itemIndex++)
                {
                    LocalPlayerInv.ResetItem(itemIndex);
                }
                LocalPlayerInv.SetEquipmentIndex(EquipmentIndex.None);
            }
        }
    }
}