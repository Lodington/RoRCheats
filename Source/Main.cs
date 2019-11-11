using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using BepInEx;
using R2API.Utils;
using BepInEx.Configuration;

namespace RoRCheats
{

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Lodington.RoRCheats", "RoRCheats", "3.0.1")]

    public class Main : BaseUnityPlugin
    {
        public static String _VERSION = "v 3.0.1";

        public static CharacterMaster LocalPlayer;
        public static Inventory LocalPlayerInv;
        public static HealthComponent LocalHealth;
        public static SkillLocator LocalSkills;
        public static NetworkUser LocalNetworkUser;
        public static CharacterBody Localbody;
        public static CharacterMotor LocalMotor;

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

        public static GUIStyle MainBgStyle, StatBgSytle, TeleBgStyle, OnStyle, OffStyle, LabelStyle, BtnStyle, CornerStyle, DisplayStyle; //make new BgStyle for stats menu 
        public static GUIStyle BtnStyle1, BtnStyle2, BtnStyle3;
        public static bool skillToggle, renderInteractables, damageToggle, critToggle, MouseToggle, NoclipToggle;
        public static float delay = 0, widthSize = 400;

        public static Rect mainRect = new Rect(10, 10, 20, 20); //start position
        public static Rect statRect = new Rect(1626, 457, 20, 20); //start position
        public static Rect teleRect = new Rect(426, 10, 20, 20); //start position
        public static Rect lobbyRect = new Rect(426, 335, 20, 20); //start position
        public static Rect editStatRect = new Rect(427, 434, 20, 20); //start position
        public static Rect spawnerRect = new Rect(1076, 10, 20, 20); //start position
        public static Rect characterRect = new Rect(1503, 10, 20, 20); //start position

        public static Texture2D Image = null, ontexture, onpresstexture, offtexture, offpresstexture, cornertexture, backtexture, btntexture, btnpresstexture, btntexturelabel;
        public static Texture2D NewTexture2D { get { return new Texture2D(1, 1); } }

        public static int itemsToRoll = 10;
        public static int damagePerLvl = 10;
        public static int CritPerLvl = 1;
        public static int allItemsQuantity = 5;
        public static ulong xpToGive = 100;
        public static uint moneyToGive = 100;
        public static uint coinsToGive = 10;
        public static int btnY, MainMulY, StatMulY, TeleMulY, LobbyMulY, SpawnerMulY, CharacterMulY;

        public static ConfigWrapper<string> OpenMenuKey;
        public static ConfigWrapper<string> GiveMoneyKey;
        public static ConfigWrapper<string> OpenTeleMenu;
        public static ConfigWrapper<string> ToggleNoclip;
    
        public static Vector2 scrollPosition = Vector2.zero;

        static Dictionary<String, Int32> nameToIndexMap = new Dictionary<String, Int32>();
        public static string[] Players = new string[16];

        private void OnGUI()
        {
            mainRect = GUI.Window(0, mainRect, new GUI.WindowFunction(SetMainBG), "", new GUIStyle());
            if (_isMenuOpen)
            {
                DrawMenu();
            }
            if (_isStatMenuOpen)
            {
                statRect = GUI.Window(1, statRect, new GUI.WindowFunction(SetStatsBG), "", new GUIStyle());
                RoRCheats.DrawMenu.DrawStatsMenu(statRect.x, statRect.y, widthSize, StatMulY, MainBgStyle, LabelStyle);
            }
            if (_isTeleMenuOpen)
            {
                teleRect = GUI.Window(2, teleRect, new GUI.WindowFunction(SetTeleBG), "", new GUIStyle());
                RoRCheats.DrawMenu.DrawTeleMenu(teleRect.x, teleRect.y, widthSize, TeleMulY, MainBgStyle, BtnStyle, LabelStyle);
            }
            if (_isLobbyMenuOpen)
            {
                lobbyRect = GUI.Window(3, lobbyRect, new GUI.WindowFunction(SetLobbyBG), "", new GUIStyle());
                RoRCheats.DrawMenu.DrawManagmentMenu(lobbyRect.x, lobbyRect.y, widthSize, LobbyMulY, MainBgStyle, BtnStyle, LabelStyle);
            }
            if (_isItemSpawnMenuOpen)
            {
                spawnerRect = GUI.Window(4, spawnerRect, new GUI.WindowFunction(SetSpawnerBG), "", new GUIStyle());
                RoRCheats.DrawMenu.DrawSpawnMenu(spawnerRect.x, spawnerRect.y, widthSize, SpawnerMulY, MainBgStyle, BtnStyle, LabelStyle);
            } 
            if (_CharacterCollected)
            {
                ESPRoutine();
            }

            if (_CharacterToggle)
            {
                characterRect = GUI.Window(5, characterRect, new GUI.WindowFunction(SetCharacterBG), "", new GUIStyle());
                CharacterWindowMethod();
            }
        }

        public void Awake()
        {
            nameToIndexMap = typeof(BodyCatalog).GetFieldValue<Dictionary<String, Int32>>("nameToIndexMap");
            OpenMenuKey = Config.Wrap("Main Menu", "Main Menu Keybind", "Default Key to open menu is insert", "Insert");
            GiveMoneyKey = Config.Wrap("Give Money", "Give money Keybind", "Default Key to give money is V", "V");
            OpenTeleMenu = Config.Wrap("Teleporter Menu", "Open teleporter Keybind", "Default Key to open the Teleporter menu is B", "B");
            ToggleNoclip = Config.Wrap("No Clip Toggle", "Toggle No Clip Keybind", "Default Key to toggle Noclip is C", "C");
        }

        public void Start()
        {
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
        }

        public void Update()
        {
            CharacterRoutine();
            CheckInputs();
            StatsRoutine();
            DamageRoutine();
            NoClipRoutine();
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
                GiveMoney();
            }
            if(InputManager.GetKeyDown("OpenTeleMenu"))
            {
                _isTeleMenuOpen = !_isTeleMenuOpen;
            }
            if(InputManager.GetKeyDown("ToggleNoClip"))
            {
                NoclipToggle = !NoclipToggle;
            }
        }

        private void NoClipRoutine()
        {
            if (NoclipToggle)
            {
                NoClip.DoNoclip();
            }
            
        }
        private void DamageRoutine()
        {
            if (damageToggle)
            {
                LevelPlayersDamage();
            }
            if (critToggle)
            {
                LevelPlayersCrit();
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
        #endregion

        public static void DrawMenu()
        {
            GUI.Box(new Rect(mainRect.x + 0f, mainRect.y + 0f, widthSize + 10, 50f + 45 * MainMulY), "", MainBgStyle);
            GUI.Label(new Rect(mainRect.x + 5f, mainRect.y + 5f, widthSize + 5, 95f), "RoRCheat Menu\n" + _VERSION, LabelStyle);

            if (!_CharacterCollected)
            {
                RoRCheats.DrawMenu.DrawNotCollectedMenu(LabelStyle);
            }

            if (_CharacterCollected)
            {
                RoRCheats.DrawMenu.DrawMainMenu(mainRect.x, mainRect.y, widthSize, MainMulY, MainBgStyle, OnStyle, OffStyle, BtnStyle);
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
                    btntexture.SetPixel(0, 0, new Color32(255,0,0,255));
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
                    //ToHtmlStringRGBA  new Color(33, 150, 243, 1)
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
        #endregion
        //Debugtoolkit team
      
        public static void CharacterWindowMethod()
        {
            GUI.Box(new Rect(characterRect.x + 0f, characterRect.y + 0f, widthSize + 10, 50f + 45 * 15), "", MainBgStyle);
            GUI.Label(new Rect(characterRect.x + 5f, characterRect.y + 5f, widthSize + 5, 95f), "Character Menu", LabelStyle);

            
            scrollPosition = GUI.BeginScrollView(new Rect(characterRect.x + 0f, characterRect.y + 0f, widthSize + 10, 50f + 45 * 15), scrollPosition, new Rect(characterRect.x + 0f, characterRect.y + 0f, widthSize + 10, 50f + 45 * CharacterMulY), false, true);
            int buttonPlacement = 1;
            foreach (var body in nameToIndexMap)
            {   
                if (body.Key.Contains("(Clone)"))
                    continue;
                if (GUI.Button(btn.BtnRect(buttonPlacement, "character"), body.Key.Replace("Body", ""), BtnStyle))
                {
                    GameObject newBody = BodyCatalog.FindBodyPrefab(body.Key);
                    if (newBody == null)
                        return;
                    var localUser = RoR2.LocalUserManager.GetFirstLocalUser();
                    if (localUser == null || localUser.cachedMasterController == null || localUser.cachedMasterController.master == null) return;
                    var master = localUser.cachedMasterController.master;
                       
                    master.bodyPrefab = newBody;
                    master.Respawn(master.GetBody().transform.position, master.GetBody().transform.rotation);
                    //DrawMenu();
                    return;
                }
                buttonPlacement++;
            }
            GUI.EndScrollView();
        }

        public static void unlockall()
        {
            var unlockables = typeof(UnlockableCatalog).GetFieldValue<Dictionary<String, UnlockableDef>>("nameToDefTable");
            foreach (var unlockable in unlockables)
                RoR2.Run.instance.GrantUnlockToAllParticipatingPlayers(unlockable.Key);
            foreach (var networkUser in NetworkUser.readOnlyInstancesList)
                networkUser.AwardLunarCoins(100);
            var achievementManager = AchievementManager.GetUserAchievementManager(LocalUserManager.GetFirstLocalUser());
            foreach (var achievement in AchievementManager.allAchievementDefs)
                achievementManager.GrantAchievement(achievement);
            var profile = RoR2.LocalUserManager.GetFirstLocalUser().userProfile;
            foreach (var survivor in RoR2.SurvivorCatalog.allSurvivorDefs)
            {
                if (profile.statSheet.GetStatValueDouble(RoR2.Stats.PerBodyStatDef.totalTimeAlive, survivor.bodyPrefab.name) == 0.0)
                    profile.statSheet.SetStatValueFromString(RoR2.Stats.PerBodyStatDef.totalTimeAlive.FindStatDef(survivor.bodyPrefab.name), "0.1");
            }
            for (int i = 0; i < 150; i++)
            {
                profile.DiscoverPickup(new RoR2.PickupIndex((RoR2.ItemIndex)i));
                profile.DiscoverPickup(new RoR2.PickupIndex((RoR2.EquipmentIndex)i));
            }
        }

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
            catch (Exception e)
            {
                _CharacterCollected = false;
                
            }
        }
        //clears inventory, duh.

       
        // self explanatory
        public static void giveXP()
        {
            LocalPlayer.GiveExperience(xpToGive);
        }
        public static void GiveMoney()
        {
            LocalPlayer.GiveMoney(moneyToGive);
            Debug.Log("RoRCheats: Giving " + moneyToGive + " to the player");
        }
        //uh, duh.
        public static void GiveLunarCoins()
        {
            LocalNetworkUser.AwardLunarCoins(coinsToGive);
        }

        public static void LevelPlayersDamage()
        {
            try
            {
                Localbody.levelDamage = (float)damagePerLvl;
                Localbody.RecalculateStats();
            }
            catch (NullReferenceException)
            {

            }
        }
        public static void LevelPlayersCrit()
        {
            try
            {
                Localbody.levelCrit = (float)CritPerLvl;
                Localbody.RecalculateStats();
            }
            catch (NullReferenceException)
            {

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
            }
            catch (NullReferenceException)
            {

            }
            try
            {

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
