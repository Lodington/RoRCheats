using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RoRCheats
{
    class DrawMenu
    {
        public static void DrawStatsMenu(float x, float y ,float widthSize, int mulY, GUIStyle BGstyle, GUIStyle buttonStyle)
        { 
            GUI.Box(new Rect(x + 0f, y + 0f, 275, 50f + 45 * mulY), "", BGstyle);
            GUI.Label(new Rect(x + 5f, y + 5f, widthSize + 5, 50f), "Stats Menu", buttonStyle);

            GUI.Button(btn.BtnRect(1, "stats"), "Damage: " + Main.Localbody.damage, buttonStyle);
            GUI.Button(btn.BtnRect(2, "stats"), "Crit: " + Main.Localbody.crit, buttonStyle);
            GUI.Button(btn.BtnRect(3, "stats"), "Attack Speed: " + Main.Localbody.attackSpeed, buttonStyle);
            GUI.Button(btn.BtnRect(4, "stats"), "Armor: " + Main.Localbody.armor, buttonStyle);
            GUI.Button(btn.BtnRect(5, "stats"), "Regen: " + Main.Localbody.regen, buttonStyle);
            GUI.Button(btn.BtnRect(6, "stats"), "Move Speed: " + Main.Localbody.moveSpeed, buttonStyle);
            GUI.Button(btn.BtnRect(7, "stats"), "Jump Count: " + Main.Localbody.maxJumpCount, buttonStyle);
            GUI.Button(btn.BtnRect(8, "stats"), "Experience: " + Main.Localbody.experience, buttonStyle);
            GUI.Button(btn.BtnRect(9, "stats"), "Kills: " + Main.Localbody.killCount, buttonStyle);
        }

        public static void DrawManagmentMenu(float x, float y, float widthSize, int mulY, GUIStyle BGstyle, GUIStyle buttonStyle, GUIStyle LabelStyle)
        {

            Util.GetPlayers(Main.Players); //update this asap
            GUI.Box(new Rect(x + 0f, y + 0f, widthSize + 10, 50f + 45 * mulY), "", BGstyle);
            GUI.Label(new Rect(x + 5f, y + 5f, widthSize + 5, 95f), "Lobby Management Menu", LabelStyle);
            int buttonPlacement = 1;
            for (int i = 0; i < Main.Players.Length; i++)
            {
                try
                {
                    if (Main.Players[i] != null)
                    {
                        if (GUI.Button(btn.BtnRect(buttonPlacement, "lobby"), "Kick " + Main.Players[i], buttonStyle))
                        {
                            Chat.AddMessage("<color=#42f5d4>Kicked Player </color>" + "<color=yellow>" + Main.Players[i] + "</color>");
                            Util.kickPlayer(Util.GetNetUserFromString(Main.Players[i].ToString()), Main.LocalNetworkUser);
                        }
                        buttonPlacement++;
                    }
                }
                catch (NullReferenceException)
                {
                    Debug.LogWarning("RoRCheats: There is No Player Selected");
                }
                
            }   
        }
        public static Vector2 scrollPosition = Vector2.zero;
        public static void DrawSpawnMenu(float x, float y, float widthSize, int mulY, GUIStyle BGstyle, GUIStyle buttonStyle, GUIStyle LabelStyle)
        {
            GUI.Box(new Rect(x + 0f, y + 0f, widthSize + 20, 50f + 45 * 15), "", BGstyle);
            GUI.Label(new Rect(x + 5f, y + 5f, widthSize + 10, 95f), "Item Spawn Menu", LabelStyle);

            scrollPosition = GUI.BeginScrollView(new Rect(x + 0f, y + 0f, widthSize + 10, 50f + 45 * 15), scrollPosition, new Rect(x + 0f, y + 0f, widthSize + 10, 50f + 45 * mulY), false, true);
            itemManager.spawnItems(buttonStyle,"spawner");
            GUI.EndScrollView();
        }
        public static void DrawTeleMenu(float x, float y, float widthSize, int mulY, GUIStyle BGstyle, GUIStyle buttonStyle, GUIStyle LabelStyle)
        {
            GUI.Box(new Rect(x + 0f, y + 0f, widthSize + 10, 50f + 45 * 6), "", BGstyle);
            GUI.Label(new Rect(x + 5f, y + 5f, widthSize + 5, 95f), "Teleporter Menu", LabelStyle);

            if (GUI.Button(btn.BtnRect(1, "tele"), "Skip Stage", buttonStyle))
                TeleporterHandler.skipStage();
            if (GUI.Button(btn.BtnRect(2, "tele"), "Spawn Gold Portal", buttonStyle))
                TeleporterHandler.SpawnPortals("gold");
            if (GUI.Button(btn.BtnRect(3, "tele"), "Spawn Blue Portal", buttonStyle))
                TeleporterHandler.SpawnPortals("newt");
            if (GUI.Button(btn.BtnRect(4, "tele"), "Spawn Celestal Portal", buttonStyle))
                TeleporterHandler.SpawnPortals("blue");
            if (GUI.Button(btn.BtnRect(5, "tele"), "Spawn All Portals", buttonStyle))
                TeleporterHandler.SpawnPortals("all");
            if (GUI.Button(btn.BtnRect(6, "tele"), "Insta Charge Teleporter", buttonStyle))
                TeleporterHandler.InstaTeleporter();
        }

        public static void DrawNotCollectedMenu(GUIStyle buttonStyle)
        {
            GUI.Button(btn.BtnRect(1, "full"), "<color=yellow>Please Note buttons will be availble in game.</color>", buttonStyle);
            GUI.Button(btn.BtnRect(14, "full"), "<color=yellow>Created By Lodington#9215.\n Feel Free to Message me on discord</color>", buttonStyle);
            GUI.Button(btn.BtnRect(15, "full"), "<color=yellow>with Bug Reports or suggestions.</color>", buttonStyle);
        }
        
        public static void DrawMainMenu(float x, float y, float widthSize, float mulY, GUIStyle BGstyle, GUIStyle OnStyle, GUIStyle OffStyle, GUIStyle BtnStyle)
        {
            // we dont have a god toggle bool, because we can just ref localhealth
            if (Main.LocalHealth.godMode)
            {
                if (GUI.Button(btn.BtnRect(1, "full"), "God mode: ON", OnStyle))
                {
                    Main.LocalHealth.godMode = false;
                }
            }
            else if (GUI.Button(btn.BtnRect(1, "full"), "God mode: OFF", OffStyle))
            {
                Main.LocalHealth.godMode = true;
            }

            if (Main.skillToggle)
            {
                if (GUI.Button(btn.BtnRect(2, "full"), "Infinite Skills: ON", OnStyle))
                {
                    Main.skillToggle = false;
                }
            }
            else if (GUI.Button(btn.BtnRect(2, "full"), "Infinite Skills: OFF", OffStyle))
            {
                Main.skillToggle = true;
            }

            if (Main.renderInteractables)
            {
                if (GUI.Button(btn.BtnRect(3, "full"), "Interactables ESP: ON", OnStyle))
                {
                    Main.renderInteractables = false;
                }
            }
            else if (GUI.Button(btn.BtnRect(3, "full"), "Interactables ESP: OFF", OffStyle))
            {
                Main.renderInteractables = true;
            }
            if (Main._CharacterToggle)
            {
                if (GUI.Button(btn.BtnRect(4, "full"), "Character Selection: ON", OnStyle))
                {
                    Main._CharacterToggle = false;
                }
            }
            else if (GUI.Button(btn.BtnRect(4, "full"), "Character Selection: OFF", OffStyle))
            {
                Main._CharacterToggle = true;
            }
            if (Main._isStatMenuOpen)
            {
                if (GUI.Button(btn.BtnRect(5, "full"), "Stats Menu: ON", OnStyle))
                {
                    Main._isStatMenuOpen = false;
                }
            }
            else if (GUI.Button(btn.BtnRect(5, "full"), "Stats Menu: OFF", OffStyle))
            {
                Main._isStatMenuOpen = true;
            }
            if (Main._isItemSpawnMenuOpen)
            {
                if (GUI.Button(btn.BtnRect(6, "full"), "Item Spawn Menu: ON", OnStyle))
                {
                    Main._isItemSpawnMenuOpen = false;
                }
            }
            else if (GUI.Button(btn.BtnRect(6, "full"), "Item Spawn Menu: OFF", OffStyle))
            {
                Main._isItemSpawnMenuOpen = true;
            }
            if (Main._isTeleMenuOpen)
            {
                if (GUI.Button(btn.BtnRect(7, "full"), "Teleporter Menu: ON", OnStyle))
                {
                    Main._isTeleMenuOpen = false;
                }
            }
            else if (GUI.Button(btn.BtnRect(7, "full"), "Teleporter Menu: OFF", OffStyle))
            {
                Main._isTeleMenuOpen = true;
            }
            if (Main._isLobbyMenuOpen)
            {
                if (GUI.Button(btn.BtnRect(8, "full"), "Lobby Managment: ON", OnStyle))
                {
                    Main._isLobbyMenuOpen = false;
                }
            }
            else if (GUI.Button(btn.BtnRect(8, "full"), "Lobby Managment: OFF", OffStyle))
            {
                Main._isLobbyMenuOpen = true;
            }
            if (Main.NoclipToggle)
            {
                if (GUI.Button(btn.BtnRect(9, "full"), "Noclip: ON", OnStyle))
                {
                    Main.NoclipToggle = false;
                }
            }
            else if (GUI.Button(btn.BtnRect(9, "full"), "Noclip: Off", OffStyle))
            {
                Main.NoclipToggle = true;
            }

            if (GUI.Button(btn.BtnRect(10, "multiply"), "Give Money: " + Main.moneyToGive.ToString(), BtnStyle))
            {
                Main.GiveMoney();
            }
            if (GUI.Button(new Rect(x + widthSize - 80, y + Main.btnY, 40, 40), "-", OffStyle))
            {
                if (Main.moneyToGive > 100)
                    Main.moneyToGive -= 100;
            }
            if (GUI.Button(new Rect(x + widthSize - 35, y + Main.btnY, 40, 40), "+", OffStyle))
            {
                if (Main.moneyToGive >= 100)
                    Main.moneyToGive += 100;
            }
            if (GUI.Button(btn.BtnRect(11, "multiply"), "Give Lunar Coins: " + Main.coinsToGive.ToString(), BtnStyle))
            {
                Main.GiveLunarCoins();
            }
            if (GUI.Button(new Rect(x + widthSize - 80, y + Main.btnY, 40, 40), "-", OffStyle))
            {
                if (Main.coinsToGive > 10)
                    Main.coinsToGive -= 10;
            }
            if (GUI.Button(new Rect(x + widthSize - 35, y + Main.btnY, 40, 40), "+", OffStyle))
            {
                if (Main.coinsToGive >= 10)
                    Main.coinsToGive += 10;
            }
            if (GUI.Button(btn.BtnRect(12, "multiply"), "Give Experience: " + Main.xpToGive.ToString(), BtnStyle))
            {
                Main.giveXP();
            }
            if (GUI.Button(new Rect(x + widthSize - 80, y + Main.btnY, 40, 40), "-", OffStyle))
            {
                if (Main.xpToGive > 100)
                    Main.xpToGive -= 100;
            }
            if (GUI.Button(new Rect(x + widthSize - 35, y + Main.btnY, 40, 40), "+", OffStyle))
            {
                if (Main.xpToGive >= 100)
                    Main.xpToGive += 100;
            }
            if (GUI.Button(btn.BtnRect(13, "multiply"), "Roll Items: " + Main.itemsToRoll.ToString(), BtnStyle))
            {
                itemManager.RollItems(Main.itemsToRoll.ToString(), Main.LocalPlayerInv);
            }
            if (GUI.Button(new Rect(x + widthSize - 80, y + Main.btnY, 40, 40), "-", OffStyle))
            {
                if (Main.itemsToRoll > 5)
                    Main.itemsToRoll -= 5;
            }
            if (GUI.Button(new Rect(x + widthSize - 35, y + Main.btnY, 40, 40), "+", OffStyle))
            {
                if (Main.itemsToRoll >= 5)
                    Main.itemsToRoll += 5;
            }
            if (GUI.Button(btn.BtnRect(14, "multiply"), "Give All Items: " + Main.allItemsQuantity.ToString(), BtnStyle))
            {
                itemManager.GiveAllItems(Main.LocalPlayerInv);
            }
            if (GUI.Button(new Rect(x + widthSize - 80, y + Main.btnY, 40, 40), "-", OffStyle))
            {
                 if (Main.allItemsQuantity > 1)
                    Main.allItemsQuantity -= 1;
            }
            if (GUI.Button(new Rect(x + widthSize - 35, y + Main.btnY, 40, 40), "+", OffStyle))
            {
                 if (Main.allItemsQuantity >= 1)
                    Main.allItemsQuantity += 1;
            }
            if (Main.damageToggle)
            {
                if (GUI.Button(btn.BtnRect(15, "multiply"), "Damage LvL : " + Main.damagePerLvl.ToString(), OnStyle))
                {
                    Main.damageToggle = false;
                }
            }
            else if (GUI.Button(btn.BtnRect(15, "multiply"), "Damage LvL : " + Main.damagePerLvl.ToString(), OffStyle))
            {
                Main.damageToggle = true;
            }
            if (GUI.Button(new Rect(x + widthSize - 80, y + Main.btnY, 40, 40), "-", OffStyle))
            {
                if (Main.damagePerLvl > 0)
                    Main.damagePerLvl -= 10;
            }
            if (GUI.Button(new Rect(x + widthSize - 35, y + Main.btnY, 40, 40), "+", OffStyle))
            {
                if (Main.damagePerLvl >= 0)
                    Main.damagePerLvl += 10;
            }
            if (Main.critToggle)
            {
                if (GUI.Button(btn.BtnRect(16, "multiply"), "Crit LvL : " + Main.CritPerLvl.ToString(), OnStyle))
                {
                    Main.critToggle = false;
                }
            }
            else if (GUI.Button(btn.BtnRect(16, "multiply"), "Crit LvL : " + Main.CritPerLvl.ToString(), OffStyle))
            {
                Main.critToggle = true;
            }
            if (GUI.Button(new Rect(x + widthSize - 80, y + Main.btnY, 40, 40), "-", OffStyle))
            {
                if (Main.CritPerLvl > 0)
                    Main.CritPerLvl -= 1;
            }
            if (GUI.Button(new Rect(x + widthSize - 35, y + Main.btnY, 40, 40), "+", OffStyle))
            {
                if (Main.CritPerLvl >= 0)
                    Main.CritPerLvl += 1;
            }
            if (GUI.Button(btn.BtnRect(17, "full"), "Unlock All", BtnStyle))
            {
                Main.unlockall();
            }
        }
    }

}
