using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Hj.HjUpdaterAPI;

namespace RoRCheats
{
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("com.hijackhornet.HjUpdaterAPI")]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Main : BaseUnityPlugin
    {
        public const string
            NAME = "RoRCheats",
            GUID = "com.Lodington." + NAME,
            VERSION = "3.3.5";

        public static string log = "[" + NAME + "] ";


        #region Player Variables
        public static CharacterMaster LocalPlayer;
        public static Inventory LocalPlayerInv;
        public static HealthComponent LocalHealth;
        public static SkillLocator LocalSkills;
        public static NetworkUser LocalNetworkUser;
        public static CharacterBody Localbody;
        public static CharacterMotor LocalMotor;
        #endregion

        #region Menu Checks
        public static bool _isMenuOpen = false;
        public static bool _ifDragged = false;
        public static bool _CharacterCollected = false;
        public static bool _isStatMenuOpen = false;
        public static bool _isTeleMenuOpen = false;
        public static bool _ItemToggle = false;
        public static bool _CharacterToggle = false;
        public static bool _isLobbyMenuOpen = false;
        public static bool _isEditStatsOpen = false;
        public static bool _isItemSpawnMenuOpen = false;
        public static bool _isPlayerMod = false;
        public static bool _isEquipmentSpawnMenuOpen = false;
        public static bool _isItemManagerOpen = false;
        public static bool enableRespawnButton = false;
        #endregion

        #region Button Styles / Toggles
        public static GUIStyle MainBgStyle, StatBgSytle, TeleBgStyle, OnStyle, OffStyle, LabelStyle, TitleStyle, BtnStyle, ItemBtnStyle, CornerStyle, DisplayStyle; //make new BgStyle for stats menu
        public static GUIStyle BtnStyle1, BtnStyle2, BtnStyle3;
        public static bool skillToggle, renderInteractables, damageToggle, critToggle, attackSpeedToggle, armorToggle, regenToggle, moveSpeedToggle, MouseToggle, NoclipToggle;
        public static float delay = 0, widthSize = 400;
        #endregion

        #region UI Rects
        public static Rect mainRect;
        public static Rect statRect;
        public static Rect teleRect;
        public static Rect lobbyRect;
        public static Rect itemSpawnerRect;
        public static Rect equipmentSpawnerRect;
        public static Rect characterRect;
        public static Rect playerModRect;
        public static Rect itemManagerRect;
        #endregion

        public static Texture2D Image = null, ontexture, onpresstexture, offtexture, offpresstexture, cornertexture, backtexture, btntexture, btnpresstexture, btntexturelabel;
        public static Texture2D NewTexture2D { get { return new Texture2D(1, 1); } }

        #region Stats intervals / toggles
        public static int itemsToRoll = 5;
        public static int damagePerLvl = 10;
        public static int CritPerLvl = 1;
        public static float attackSpeed = 1;
        public static float armor = 0;
        public static float movespeed = 7;
        public static int jumpCount = 1;
        public static bool isDropItems = false;
        public static bool isDropItemForAll = false;
        public static bool alwaysSprint = false;
        public static bool aimBot = false;
        public static int allItemsQuantity = 5;
        public static ulong xpToGive = 100;
        public static uint moneyToGive = 100;
        public static uint coinsToGive = 100;
        #endregion

        public static int PlayerModBtnY, MainMulY, StatMulY, TeleMulY, LobbyMulY, itemSpawnerMulY, equipmentSpawnerMulY, CharacterMulY, PlayerModMulY, ItemManagerMulY, ItemManagerBtnY;

        public static ConfigEntry<string> OpenMenuKey { get; set; }
        public static ConfigEntry<string> GiveMoneyKey { get; set; }
        public static ConfigEntry<string> OpenTeleMenu { get; set; }
        public static ConfigEntry<string> ToggleNoclip { get; set; }
        public static ConfigEntry<bool> ShowUnlockAll { get; set; }
        public static ConfigEntry<bool> CondenseMenu { get; set; }

        public static Dictionary<String, Int32> nameToIndexMap = new Dictionary<String, Int32>();
        public static string[] Players = new string[16];

        private void OnGUI()
        {
            #region GenerateMenus

            mainRect = GUI.Window(0, mainRect, new GUI.WindowFunction(SetMainBG), "", new GUIStyle());
            if (_isMenuOpen)
            {
                DrawAllMenus();
            }
            if (_isStatMenuOpen)
            {
                statRect = GUI.Window(1, statRect, new GUI.WindowFunction(SetStatsBG), "", new GUIStyle());
                DrawMenu.DrawStatsMenu(statRect.x, statRect.y, widthSize, StatMulY, MainBgStyle, LabelStyle);
            }
            if (_isTeleMenuOpen)
            {
                teleRect = GUI.Window(2, teleRect, new GUI.WindowFunction(SetTeleBG), "", new GUIStyle());
                DrawMenu.DrawTeleMenu(teleRect.x, teleRect.y, widthSize, TeleMulY, MainBgStyle, BtnStyle, LabelStyle);
                //Debug.Log("X : " + teleRect.x + " Y : " + teleRect.y);
            }
            if (_isLobbyMenuOpen)
            {
                lobbyRect = GUI.Window(3, lobbyRect, new GUI.WindowFunction(SetLobbyBG), "", new GUIStyle());
                DrawMenu.DrawManagmentMenu(lobbyRect.x, lobbyRect.y, widthSize, LobbyMulY, MainBgStyle, BtnStyle, LabelStyle);
                //Debug.Log("X : " + lobbyRect.x + " Y : " + lobbyRect.y);
            }
            if (_isItemSpawnMenuOpen)
            {
                itemSpawnerRect = GUI.Window(4, itemSpawnerRect, new GUI.WindowFunction(SetSpawnerBG), "", new GUIStyle());
                DrawMenu.DrawSpawnMenu(itemSpawnerRect.x, itemSpawnerRect.y, widthSize, itemSpawnerMulY, MainBgStyle, BtnStyle, LabelStyle);
                //Debug.Log("X : " + itemSpawnerRect.x + " Y : " + itemSpawnerRect.y);
            }

            if (_isPlayerMod)
            {
                playerModRect = GUI.Window(5, playerModRect, new GUI.WindowFunction(SetPlayerModBG), "", new GUIStyle());
                DrawMenu.DrawPlayerModMenu(playerModRect.x, playerModRect.y, widthSize, PlayerModMulY, MainBgStyle, BtnStyle, OnStyle, OffStyle, LabelStyle);
                //Debug.Log("X : " + playerModRect.x + " Y : " + playerModRect.y);
            }
            if (_isEquipmentSpawnMenuOpen)
            {
                equipmentSpawnerRect = GUI.Window(6, equipmentSpawnerRect, new GUI.WindowFunction(SetEquipmentBG), "", new GUIStyle());
                DrawMenu.DrawEquipmentMenu(equipmentSpawnerRect.x, equipmentSpawnerRect.y, widthSize, equipmentSpawnerMulY, MainBgStyle, BtnStyle, LabelStyle, OffStyle);
                //Debug.Log("X : " + equipmentSpawnerRect.x + " Y : " + equipmentSpawnerRect.y);
            }
            if (_CharacterToggle)
            {
                characterRect = GUI.Window(7, characterRect, new GUI.WindowFunction(SetCharacterBG), "", new GUIStyle());
                DrawMenu.CharacterWindowMethod(characterRect.x, characterRect.y, widthSize, CharacterMulY, MainBgStyle, BtnStyle, LabelStyle);
                //Debug.Log("X : " + characterRect.x + " Y : " + characterRect.y);
            }
            if (_isItemManagerOpen)
            {
                itemManagerRect = GUI.Window(8, itemManagerRect, new GUI.WindowFunction(SetItemManagerBG), "", new GUIStyle());
                DrawMenu.DrawItemManagementMenu(itemManagerRect.x, itemManagerRect.y, widthSize, ItemManagerMulY, MainBgStyle, BtnStyle, OnStyle, OffStyle, LabelStyle);
                //Debug.Log("X : " + itemManagerRect.x + " Y : " + itemManagerRect.y);
            }
            if (_CharacterCollected)
            {
                ESPRoutine();
            }
            #endregion
        }

        public void Awake()
        {
            Register(NAME, Flag.UpdateAlways);
            nameToIndexMap = typeof(BodyCatalog).GetFieldValue<Dictionary<String, Int32>>("nameToIndexMap");
            OpenMenuKey = Config.Bind<string>("Main Menu", "Main Menu Keybind", "Insert", "Default Key to open menu is insert");
            GiveMoneyKey = Config.Bind<string>("Give Money", "Give money Keybind", "V", "Default Key to give money is V");
            OpenTeleMenu = Config.Bind<string>("Teleporter Menu", "Open teleporter Keybind", "B", "Default Key to open the Teleporter menu is B");
            ToggleNoclip = Config.Bind<string>("No Clip Toggle", "Toggle No Clip Keybind", "C", "Default Key to toggle Noclip is C");
            ShowUnlockAll = Config.Bind<bool>("Unlock All", "Unlock All Button Enabled", false, "Default Value is false");
            CondenseMenu = Config.Bind<bool>("Condense Menu for smaller resolutions", "Condence Menu", false ,"Default Value is false");
        }

        public void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            #region CondenseMenuValues

            if (!CondenseMenu.Value)
            {
                mainRect = new Rect(10, 10, 20, 20); //start position
                statRect = new Rect(1626, 457, 20, 20); //start position
                teleRect = new Rect(10, 661, 20, 20); //start position
                lobbyRect = new Rect(10, 421, 20, 20); //start position
                itemSpawnerRect = new Rect(1503, 10, 20, 20); //start position
                equipmentSpawnerRect = new Rect(1503, 10, 20, 20); //start position
                characterRect = new Rect(1503, 10, 20, 20); //start position
                playerModRect = new Rect(424, 381, 20, 20); //start position
                itemManagerRect = new Rect(424, 10, 20, 20);
            }
            else if (CondenseMenu.Value)
            {
                mainRect = new Rect(10, 10, 20, 20); //start position
                statRect = new Rect(10, 562, 20, 20); //start position
                teleRect = new Rect(426, 748, 20, 20); //start position
                lobbyRect = new Rect(1263, 10, 20, 20); //start position
                itemSpawnerRect = new Rect(839, 10, 20, 20); //start position
                equipmentSpawnerRect = new Rect(839, 10, 20, 20); //start position
                characterRect = new Rect(426, 10, 20, 20); //start position
                playerModRect = new Rect(426, 10, 20, 20); //start position
                itemManagerRect = new Rect(426, 10, 20, 20);
            }
            else
            {
                Debug.LogError(log+"Condense Menu value is invalid please set it to true or false");
                mainRect = new Rect(10, 10, 20, 20); //start position
                statRect = new Rect(1626, 457, 20, 20); //start position
                teleRect = new Rect(426, 558, 20, 20); //start position
                lobbyRect = new Rect(10, 558, 20, 20); //start position
                itemSpawnerRect = new Rect(1503, 10, 20, 20); //start position
                equipmentSpawnerRect = new Rect(1503, 10, 20, 20); //start position
                characterRect = new Rect(1503, 10, 20, 20); //start position
                playerModRect = new Rect(426, 10, 20, 20); //start position
                itemManagerRect = new Rect(426, 10, 20, 20);
            }

            #endregion

            #region Styles

            if (MainBgStyle == null)
            {
                MainBgStyle = new GUIStyle();
                MainBgStyle.normal.background = BackTexture;
                MainBgStyle.onNormal.background = BackTexture;
                MainBgStyle.active.background = BackTexture;
                MainBgStyle.onActive.background = BackTexture;
                MainBgStyle.normal.textColor = Color.white;
                MainBgStyle.onNormal.textColor = Color.white;
                MainBgStyle.active.textColor = Color.white;
                MainBgStyle.onActive.textColor = Color.white;
                MainBgStyle.fontSize = 18;
                MainBgStyle.fontStyle = FontStyle.Normal;
                MainBgStyle.alignment = TextAnchor.UpperCenter;
            }

            if (CornerStyle == null)
            {
                CornerStyle = new GUIStyle();
                CornerStyle.normal.background = BtnTexture;
                CornerStyle.onNormal.background = BtnTexture;
                CornerStyle.active.background = BtnTexture;
                CornerStyle.onActive.background = BtnTexture;
                CornerStyle.normal.textColor = Color.white;
                CornerStyle.onNormal.textColor = Color.white;
                CornerStyle.active.textColor = Color.white;
                CornerStyle.onActive.textColor = Color.white;
                CornerStyle.fontSize = 18;
                CornerStyle.fontStyle = FontStyle.Normal;
                CornerStyle.alignment = TextAnchor.UpperCenter;
            }

            if (LabelStyle == null)
            {
                LabelStyle = new GUIStyle();
                LabelStyle.normal.textColor = Color.white;
                LabelStyle.onNormal.textColor = Color.white;
                LabelStyle.active.textColor = Color.white;
                LabelStyle.onActive.textColor = Color.white;
                LabelStyle.fontSize = 18;
                LabelStyle.fontStyle = FontStyle.Normal;
                LabelStyle.alignment = TextAnchor.UpperCenter;
            }
            if (TitleStyle == null)
            {
                TitleStyle = new GUIStyle();
                TitleStyle.normal.textColor = Color.white;
                TitleStyle.onNormal.textColor = Color.white;
                TitleStyle.active.textColor = Color.white;
                TitleStyle.onActive.textColor = Color.white;
                TitleStyle.fontSize = 18;
                TitleStyle.fontStyle = FontStyle.Normal;
                TitleStyle.alignment = TextAnchor.UpperCenter;
            }

            if (OffStyle == null)
            {
                OffStyle = new GUIStyle();
                OffStyle.normal.background = OffTexture;
                OffStyle.onNormal.background = OffTexture;
                OffStyle.active.background = OffPressTexture;
                OffStyle.onActive.background = OffPressTexture;
                OffStyle.normal.textColor = Color.white;
                OffStyle.onNormal.textColor = Color.white;
                OffStyle.active.textColor = Color.white;
                OffStyle.onActive.textColor = Color.white;
                OffStyle.fontSize = 18;
                OffStyle.fontStyle = FontStyle.Normal;
                OffStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (OnStyle == null)
            {
                OnStyle = new GUIStyle();
                OnStyle.normal.background = OnTexture;
                OnStyle.onNormal.background = OnTexture;
                OnStyle.active.background = OnPressTexture;
                OnStyle.onActive.background = OnPressTexture;
                OnStyle.normal.textColor = Color.white;
                OnStyle.onNormal.textColor = Color.white;
                OnStyle.active.textColor = Color.white;
                OnStyle.onActive.textColor = Color.white;
                OnStyle.fontSize = 18;
                OnStyle.fontStyle = FontStyle.Normal;
                OnStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (BtnStyle == null)
            {
                BtnStyle = new GUIStyle();
                BtnStyle.normal.background = BtnTexture;
                BtnStyle.onNormal.background = BtnTexture;
                BtnStyle.active.background = BtnPressTexture;
                BtnStyle.onActive.background = BtnPressTexture;
                BtnStyle.normal.textColor = Color.white;
                BtnStyle.onNormal.textColor = Color.white;
                BtnStyle.active.textColor = Color.white;
                BtnStyle.onActive.textColor = Color.white;
                BtnStyle.fontSize = 18;
                BtnStyle.fontStyle = FontStyle.Normal;
                BtnStyle.alignment = TextAnchor.MiddleCenter;
            }
            if (ItemBtnStyle == null)
            {
                ItemBtnStyle = new GUIStyle();
                ItemBtnStyle.normal.background = BtnTexture;
                ItemBtnStyle.onNormal.background = BtnTexture;
                ItemBtnStyle.active.background = BtnPressTexture;
                ItemBtnStyle.onActive.background = BtnPressTexture;
                ItemBtnStyle.normal.textColor = Color.white;
                ItemBtnStyle.onNormal.textColor = Color.white;
                ItemBtnStyle.active.textColor = Color.white;
                ItemBtnStyle.onActive.textColor = Color.white;
                ItemBtnStyle.fontSize = 18;
                ItemBtnStyle.fontStyle = FontStyle.Normal;
                ItemBtnStyle.alignment = TextAnchor.MiddleCenter;
            }
            #endregion
        }

        public void Update()
        {
            try
            {
                AdvancedToolTip.Advancedtooltip();
                CharacterRoutine();
                CheckInputs();
                StatsRoutine();
                ModStatsRoutine();
                NoClipRoutine();
                SprintRoutine();
                AimBotRoutine();
                DropItemRoutine();
            }
            catch (NullReferenceException)
            {
                
            }
        }

        private void CheckInputs()
        {
            if (_isMenuOpen)
            {
                Cursor.visible = true;
            }
            else if (!_isMenuOpen)
            {
                Cursor.visible = false;
            }
            if (InputManager.GetKeyDown("OpenMenu"))
            {
                _isMenuOpen = !_isMenuOpen;
                GetCharacter();
            }
            if (InputManager.GetKeyDown("GiveMoney"))
            {
                PlayerModifiers.GiveMoney();
            }
            if (InputManager.GetKeyDown("OpenTeleMenu"))
            {
                _isTeleMenuOpen = !_isTeleMenuOpen;
            }
            if (InputManager.GetKeyDown("ToggleNoClip"))
            {
                NoclipToggle = !NoclipToggle;
            }
        }

        #region Routines

        private void AimBotRoutine()
        {
            if (aimBot)
                PlayerModifiers.AimBot();
        }
        private void DropItemRoutine()
        {
            DropItem.DropItemMethod();
            if (isDropItems)
            {
                isDropItemForAll = false;
            }
            if (isDropItemForAll)
            {
                isDropItems = false;
            }
        }
        private void SprintRoutine()
        {
            if (alwaysSprint)
                PlayerModifiers.AlwaysSprint();
        }

        private void NoClipRoutine()
        {
            if (NoclipToggle)
            {
                NoClip.DoNoclip();
            }
        }

        private void ModStatsRoutine()
        {
            if (_CharacterCollected)
            {
                if (damageToggle)
                {
                    PlayerModifiers.LevelPlayersDamage();
                }
                if (critToggle)
                {
                    PlayerModifiers.LevelPlayersCrit();
                }
                if (attackSpeedToggle)
                {
                    PlayerModifiers.SetplayersAttackSpeed();
                }
                if (armorToggle)
                {
                    PlayerModifiers.SetplayersArmor();
                }
                if (moveSpeedToggle)
                {
                    PlayerModifiers.SetplayersMoveSpeed();
                }
                Localbody.RecalculateStats();
                
            }
        }
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "title")
            {
                Util.RESETMENU();
                Debug.Log(log+"Resetting Menu");
            }
            else if (!LocalPlayer.alive && scene.name != "title")
            {
                enableRespawnButton = true;
            }
        }

        private void CharacterRoutine()
        {
            if (!_CharacterCollected)
            {
                GetCharacter();
            }
        }

        private void ESPRoutine()
        {
            if (renderInteractables)
            {
                RenderInteractables();
            }
        }

        private void StatsRoutine()
        {
            if (_CharacterCollected)
            {
                if (skillToggle)
                {
                    LocalSkills.ApplyAmmoPack();
                }
            }
        }

        #endregion


        #region SetBG

        public static void SetMainBG(int windowID)
        {
            GUI.Box(new Rect(0f, 0f, widthSize + 10, 50f + 45 * MainMulY), "", CornerStyle);
            if (Event.current.type == EventType.MouseDrag)
            {
                delay += Time.deltaTime;
                if (delay > 0.3f)
                {
                    _ifDragged = true;
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                delay = 0;
                if (!_ifDragged)
                {
                    _isMenuOpen = !_isMenuOpen;
                    GetCharacter();
                }
                _ifDragged = false;
            }
            GUI.DragWindow();
        }

        public static void SetCharacterBG(int windowID)
        {
            GUI.Box(new Rect(0f, 0f, widthSize + 10, 50f + 45 * CharacterMulY), "", CornerStyle);
            if (Event.current.type == EventType.MouseDrag)
            {
                delay += Time.deltaTime;
                if (delay > 0.3f)
                {
                    _ifDragged = true;
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                delay = 0;
                if (!_ifDragged)
                {
                    _CharacterToggle = !_CharacterToggle;
                }
                _ifDragged = false;
            }
            GUI.DragWindow();
        }

        public static void SetEquipmentBG(int windowID)
        {
            GUI.Box(new Rect(0f, 0f, widthSize + 10, 50f + 45 * equipmentSpawnerMulY), "", CornerStyle);
            if (Event.current.type == EventType.MouseDrag)
            {
                delay += Time.deltaTime;
                if (delay > 0.3f)
                {
                    _ifDragged = true;
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                delay = 0;
                if (!_ifDragged)
                {
                    _isEquipmentSpawnMenuOpen = !_isEquipmentSpawnMenuOpen;
                }
                _ifDragged = false;
            }
            GUI.DragWindow();
        }

        public static void SetStatsBG(int windowID)
        {
            GUI.Box(new Rect(0f, 0f, widthSize + 10, 50f + 45 * StatMulY), "", CornerStyle);
            if (Event.current.type == EventType.MouseDrag)
            {
                delay += Time.deltaTime;
                if (delay > 0.3f)
                {
                    _ifDragged = true;
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                delay = 0;
                if (!_ifDragged)
                {
                    _isStatMenuOpen = !_isStatMenuOpen;
                }
                _ifDragged = false;
            }
            GUI.DragWindow();
        }

        public static void SetTeleBG(int windowID)
        {
            GUI.Box(new Rect(0f, 0f, widthSize + 10, 50f + 45 * TeleMulY), "", CornerStyle);
            if (Event.current.type == EventType.MouseDrag)
            {
                delay += Time.deltaTime;
                if (delay > 0.3f)
                {
                    _ifDragged = true;
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                delay = 0;
                if (!_ifDragged)
                {
                    _isTeleMenuOpen = !_isTeleMenuOpen;
                }
                _ifDragged = false;
            }
            GUI.DragWindow();
        }

        public static void SetLobbyBG(int windowID)
        {
            GUI.Box(new Rect(0f, 0f, widthSize + 10, 50f + 45 * LobbyMulY), "", CornerStyle);
            if (Event.current.type == EventType.MouseDrag)
            {
                delay += Time.deltaTime;
                if (delay > 0.3f)
                {
                    _ifDragged = true;
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                delay = 0;
                if (!_ifDragged)
                {
                    _isLobbyMenuOpen = !_isLobbyMenuOpen;
                }
                _ifDragged = false;
            }
            GUI.DragWindow();
        }

        public static void SetEditStatBG(int windowID)
        {
            GUI.Box(new Rect(0f, 0f, widthSize + 10, 50f + 45 * MainMulY), "", CornerStyle);
            if (Event.current.type == EventType.MouseDrag)
            {
                delay += Time.deltaTime;
                if (delay > 0.3f)
                {
                    _ifDragged = true;
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                delay = 0;
                if (!_ifDragged)
                {
                    _isEditStatsOpen = !_isEditStatsOpen;
                }
                _ifDragged = false;
            }
            GUI.DragWindow();
        }

        public static void SetSpawnerBG(int windowID)
        {
            GUI.Box(new Rect(0f, 0f, widthSize + 10, 50f + 45 * MainMulY), "", CornerStyle);
            if (Event.current.type == EventType.MouseDrag)
            {
                delay += Time.deltaTime;
                if (delay > 0.3f)
                {
                    _ifDragged = true;
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                delay = 0;
                if (!_ifDragged)
                {
                    _isItemSpawnMenuOpen = !_isItemSpawnMenuOpen;
                }
                _ifDragged = false;
            }
            GUI.DragWindow();
        }

        public static void SetPlayerModBG(int windowID)
        {
            GUI.Box(new Rect(0f, 0f, widthSize + 10, 50f + 45 * PlayerModMulY), "", CornerStyle);
            if (Event.current.type == EventType.MouseDrag)
            {
                delay += Time.deltaTime;
                if (delay > 0.3f)
                {
                    _ifDragged = true;
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                delay = 0;
                if (!_ifDragged)
                {
                    _isPlayerMod = !_isPlayerMod;
                }
                _ifDragged = false;
            }
            GUI.DragWindow();
        }

        public static void SetItemManagerBG(int windowID)
        {
            GUI.Box(new Rect(0f, 0f, widthSize + 10, 50f + 45 * ItemManagerMulY), "", CornerStyle);
            if (Event.current.type == EventType.MouseDrag)
            {
                delay += Time.deltaTime;
                if (delay > 0.3f)
                {
                    _ifDragged = true;
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                delay = 0;
                if (!_ifDragged)
                {
                    _isItemManagerOpen = !_isItemManagerOpen;
                }
                _ifDragged = false;
            }
            GUI.DragWindow();
        }

        #endregion SetBG

        public static void DrawAllMenus()
        {
            GUI.Box(new Rect(mainRect.x + 0f, mainRect.y + 0f, widthSize + 10, 50f + 45 * MainMulY), "", MainBgStyle);
            GUI.Label(new Rect(mainRect.x + 5f, mainRect.y + 5f, widthSize + 5, 95f), "RoRCheats" + "\n" + VERSION, TitleStyle);

            if (!_CharacterCollected)
            {
                DrawMenu.DrawNotCollectedMenu(LabelStyle, OnStyle, OffStyle);
            }
            if (_CharacterCollected)
            {
                DrawMenu.DrawMainMenu(mainRect.x, mainRect.y, widthSize, MainMulY, MainBgStyle, OnStyle, OffStyle, BtnStyle);
            }
        }

        // Textures

        #region Textures

        public static Texture2D BtnTexture
        {
            get
            {
                if (btntexture == null)
                {
                    btntexture = NewTexture2D;
                    btntexture.SetPixel(0, 0, new Color32(3, 155, 229, 255));
                    //byte[] FileData = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/BepInEx/plugins/RoRCheats/Resources/Images/ButtonStyle.png");
                    //btntexture.LoadImage(FileData);
                    btntexture.Apply();
                }
                return btntexture;
            }
        }

        public static Texture2D BtnTextureLabel
        {
            get
            {
                if (BtnTextureLabel == null)
                {
                    btntexture = NewTexture2D;
                    btntexture.SetPixel(0, 0, new Color32(255, 0, 0, 255));
                    btntexture.Apply();
                }
                return BtnTextureLabel;
            }
        }

        public static Texture2D BtnPressTexture
        {
            get
            {
                if (btnpresstexture == null)
                {
                    btnpresstexture = NewTexture2D;
                    btnpresstexture.SetPixel(0, 0, new Color32(2, 119, 189, 255));
                    btnpresstexture.Apply();
                }
                return btnpresstexture;
            }
        }

        public static Texture2D OnPressTexture
        {
            get
            {
                if (onpresstexture == null)
                {
                    onpresstexture = NewTexture2D;
                    onpresstexture.SetPixel(0, 0, new Color32(62, 119, 64, 255));
                    onpresstexture.Apply();
                }
                return onpresstexture;
            }
        }

        public static Texture2D OnTexture
        {
            get
            {
                if (ontexture == null)
                {
                    ontexture = NewTexture2D;
                    ontexture.SetPixel(0, 0, new Color32(79, 153, 82, 255));
                    ontexture.Apply();
                }
                return ontexture;
            }
        }

        public static Texture2D OffPressTexture
        {
            get
            {
                if (offpresstexture == null)
                {
                    offpresstexture = NewTexture2D;
                    offpresstexture.SetPixel(0, 0, new Color32(79, 79, 79, 255));
                    offpresstexture.Apply();
                }
                return offpresstexture;
            }
        }

        public static Texture2D OffTexture
        {
            get
            {
                if (offtexture == null)
                {
                    offtexture = NewTexture2D;
                    offtexture.SetPixel(0, 0, new Color32(99, 99, 99, 255));
                    //byte[] FileData = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/BepInEx/plugins/RoRCheats/Resources/Images/OffStyle.png");
                    //offtexture.LoadImage(FileData);
                    offtexture.Apply();
                }
                return offtexture;
            }
        }
        public static Texture2D BackTexture
        {
            get
            {
                if (backtexture == null)
                {
                    backtexture = NewTexture2D;
                    backtexture.SetPixel(0, 0, new Color32(42, 42, 42, 200));
                    backtexture.Apply();
                }
                return backtexture;
            }
        }

        public static Texture2D CornerTexture
        {
            get
            {
                if (cornertexture == null)
                {
                    cornertexture = NewTexture2D;
                    //ToHtmlStringRGBA  new Color(33, 150, 243, 1)
                   cornertexture.SetPixel(0, 0, new Color32(42, 42, 42, 0));

                    cornertexture.Apply();
                }
                return cornertexture;
            }
        }

        #endregion Textures

        //Debugtoolkit team

        // try and setup our character, if we hit an error we set it to false
        //TODO: Find a way to stop it from checking whilst in main menu/lobby menu
        public static void GetCharacter()
        {
            try
            {
                LocalNetworkUser = null;
                foreach (NetworkUser readOnlyInstance in NetworkUser.readOnlyInstancesList)
                {
                    //localplayer == you!
                    if (readOnlyInstance.isLocalPlayer)
                    {
                        LocalNetworkUser = readOnlyInstance;
                        LocalPlayer = LocalNetworkUser.master;
                        LocalPlayerInv = LocalPlayer.GetComponent<Inventory>(); //gets player inventory
                        LocalHealth = LocalPlayer.GetBody().GetComponent<HealthComponent>(); //gets players local health numbers
                        LocalSkills = LocalPlayer.GetBody().GetComponent<SkillLocator>(); //gets current for local character skills
                        Localbody = LocalPlayer.GetBody().GetComponent<CharacterBody>(); //gets all stats for local character
                        LocalMotor = LocalPlayer.GetBody().GetComponent<CharacterMotor>();
                        if (LocalPlayer.alive) _CharacterCollected = true;
                        else _CharacterCollected = false;
                    }
                }
            }
            catch (Exception)
            {
                _CharacterCollected = false;
            }
        }

        private static void RenderInteractables()
        {
            try
            {
                foreach (TeleporterInteraction teleporterInteraction in InstanceTracker.GetInstancesList<TeleporterInteraction>())
                {
                    if (teleporterInteraction.isActiveAndEnabled)
                    {
                        float distanceToObject = Vector3.Distance(Camera.main.transform.position, teleporterInteraction.transform.position);
                        Vector3 Position = Camera.main.WorldToScreenPoint(teleporterInteraction.transform.position);
                        var BoundingVector = new Vector3(Position.x, Position.y, Position.z);
                        if (BoundingVector.z > 0.01)
                        {
                            GUI.color = Color.yellow;
                            int distance = (int)distanceToObject;
                            String friendlyName = "Teleporter";
                            string boxText = $"{friendlyName}\n{distance}m";
                            GUI.Label(new Rect(BoundingVector.x - 50f, (float)Screen.height - BoundingVector.y, 100f, 50f), boxText);
                        }
                    }
                }
                foreach (PurchaseInteraction purchaseInteraction in InstanceTracker.GetInstancesList<PurchaseInteraction>())
                {
                    if (purchaseInteraction.available)
                    {
                        float distanceToObject = Vector3.Distance(Camera.main.transform.position, purchaseInteraction.transform.position);
                        Vector3 Position = Camera.main.WorldToScreenPoint(purchaseInteraction.transform.position);
                        var BoundingVector = new Vector3(Position.x, Position.y, Position.z);
                        if (BoundingVector.z > 0.01)
                        {
                            GUI.color = Color.green;
                            int distance = (int)distanceToObject;
                            String friendlyName = purchaseInteraction.GetDisplayName();
                            int cost = purchaseInteraction.cost;
                            string boxText = $"{friendlyName}\n${cost}\n{distance}m";
                            GUI.Label(new Rect(BoundingVector.x - 50f, (float)Screen.height - BoundingVector.y, 100f, 50f), boxText);
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
            }
        }
    }
}