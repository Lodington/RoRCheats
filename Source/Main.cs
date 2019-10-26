using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RoR2;
using BepInEx;
using R2API.Utils;

namespace RoRCheats
{

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Lodington.RoR2Cheats", "RoR2Cheats", "2.4.1")]

    public class Main : BaseUnityPlugin
    {
        private static String _VERSION = "v 2.4.1";

        private static RoR2.CharacterMaster LocalPlayer;
        private static RoR2.Inventory LocalPlayerInv;
        private static RoR2.HealthComponent LocalHealth;
        private static RoR2.SkillLocator LocalSkills;
        private static RoR2.NetworkUser LocalNetworkUser;
        private static RoR2.CharacterBody Localbody;
        private static RoR2.CharacterMotor LocalMotor;

        private static bool _isMenuOpen = false;
        private static bool _ifDragged = false;
        private static bool _CharacterCollected = false;
        private static bool _isStatMenuOpen = false;
        private static bool _isTeleMenuOpen = false;
        private static bool _ItemToggle = false;
        private static bool _CharacterToggle = false;

        public static GUIStyle MainBgStyle, StatBgSytle, TeleBgStyle, OnStyle, OffStyle, LabelStyle, BtnStyle, CornerStyle, DisplayStyle; //make new BgStyle for stats menu 
        public static GUIStyle BtnStyle1, BtnStyle2, BtnStyle3;
        public static bool skillToggle, renderInteractables, damageToggle, critToggle, MouseToggle, NoclipToggle;
        public static float delay = 0, widthSize = 400;

        public static Rect mainRect = new Rect(10, 10, 20, 20); //start position
        public static Rect statRect = new Rect(427, 200, 20, 20); //start position
        public static Rect teleRect = new Rect(10, 736, 20, 20); //start position

        public static Texture2D Image = null, ontexture, onpresstexture, offtexture, offpresstexture, cornertexture, backtexture, btntexture, btnpresstexture, btntexturelabel;
        public static Texture2D NewTexture2D { get { return new Texture2D(1, 1); } }

        public static int itemsToRoll = 10;
        public static int damagePerLvl = 10;
        public static int CritPerLvl = 1;
        public static int allItemsQuantity = 5;
        private static ulong xpToGive = 100;
        public static uint moneyToGive = 100;
        public static uint coinsToGive = 10;
        public static int btnY, mulY;

        Dictionary<string, int> nameToIndexMap = new Dictionary<string, int>();
        static Int32 Margin = 5;
        static Int32 ItemWidth = 400;
        static Int32 CharacterWidth = 400;
        Int32 ItemSelectorId, CharacterSelectId;
        Vector2 ItemScrollPos, CharacterScrollPos;

        Rect CharacterWindow = new Rect(Screen.width - CharacterWidth - Margin, Margin, CharacterWidth, Screen.height - (Margin * 2));
        Rect ItemSelectorWindow = new Rect(Screen.width - ItemWidth - Margin, Margin, ItemWidth, Screen.height - (Margin * 2));

        private void OnGUI()
        {
            mainRect = GUI.Window(0, mainRect, new GUI.WindowFunction(SetBG), "", new GUIStyle());
            if (_isMenuOpen)
            {
                DrawMenu();

            }
            if (_isStatMenuOpen)
            {
                statRect = GUI.Window(1, statRect, new GUI.WindowFunction(SetBG), "", new GUIStyle());
                DrawStatsMenu();
            }
            if (_isTeleMenuOpen)
            {
                teleRect = GUI.Window(2, teleRect, new GUI.WindowFunction(SetBG), "", new GUIStyle());
                DrawTeleMenu();
            }
            if (_CharacterCollected)
            {
                ESPRoutine();
            }
            if (_ItemToggle)
            {
                ItemSelectorWindow = GUILayout.Window(ItemSelectorId, ItemSelectorWindow, SpawnItem, "Item Select");
            }
            if (_CharacterToggle)
            {
                CharacterWindow = GUILayout.Window(CharacterSelectId, CharacterWindow, CharacterWindowMethod, "Character Select");
            }
        }
        public void Awake()
        {
            nameToIndexMap = typeof(BodyCatalog).GetFieldValue<Dictionary<string, int>>("nameToIndexMap");
            CharacterSelectId = GetHashCode();
            ItemSelectorId = GetHashCode();

            //Here we are subscribing to the SurvivorCatalogReady event, which is run when the subscriber catalog can be modified.
            //We insert Bandit as a new character here, which is then automatically added to the internal game catalog and reconstructed.
            /* R2API.SurvivorAPI.SurvivorCatalogReady += (s, e) =>
             {
                 var survivor = new SurvivorDef
                 {
                     bodyPrefab = BodyCatalog.FindBodyPrefab("BanditBody"),
                     descriptionToken = "BANDIT_DESCRIPTION",
                     displayPrefab = Resources.Load<GameObject>("Prefabs/Characters/BanditDisplay"),
                     primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f),
                     unlockableName = "Bandit"
                 };

                 R2API.SurvivorAPI.AddSurvivorOnReady(survivor);
                 Debug.Log("Loaded Bandit");
             };*/
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
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                _isMenuOpen = !_isMenuOpen;
                Debug.Log("Menu toggled");
                GetCharacter();
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                GiveMoney();
                Debug.Log("V Key Pressed");
            }
            if (MouseToggle)
            {
                if (Input.GetKeyDown(KeyCode.Mouse4))
                {
                    _isMenuOpen = !_isMenuOpen;
                    Debug.Log("Menu toggled");
                    GetCharacter();
                }
            }
        }


        private void NoClipRoutine()
        {
            if (_CharacterCollected) { 
                if (NoclipToggle)
                {
                    LocalMotor.isFlying = false;
                    LocalMotor.useGravity = false;
                }
                else if (!NoclipToggle)
                {
                    LocalMotor.isFlying = false;
                    LocalMotor.useGravity = true;
                }
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
      

        public static void SetBG(int windowID)
        {
            GUI.Box(new Rect(0f, 0f, widthSize + 10, 50f + 45 * mulY), "", CornerStyle);
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
                    _isStatMenuOpen = !_isStatMenuOpen;
                }
                _ifDragged = false;
            }
            GUI.DragWindow();
        }

        public static void DrawTeleMenu()
        {
            int AmountOfButtons = 4;
            GUI.Box(new Rect(teleRect.x + 0f, teleRect.y + 0f, widthSize + 10, 50f + 45 * AmountOfButtons), "", MainBgStyle);
            GUI.Label(new Rect(teleRect.x + 5f, teleRect.y + 5f, widthSize + 5, 95f), "Teleporter Menu", LabelStyle);

            if (_CharacterCollected)
            {
                if (GUI.Button(BtnRect(1, "tele"), "Spawn Gold Portal", BtnStyle))
                    SpawnPortals("gold");
                if (GUI.Button(BtnRect(2, "tele"), "Spawn Blue Portal", BtnStyle))
                    SpawnPortals("newt");
                if (GUI.Button(BtnRect(3, "tele"), "Spawn Celestal Portal", BtnStyle))
                    SpawnPortals("blue");
                if (GUI.Button(BtnRect(4, "tele"), "Spawn All Portals", BtnStyle))
                    SpawnPortals("all");
                
            }
            else
            {
                _isStatMenuOpen = false;
            }

        }

        public static void DrawStatsMenu()
        {
            int AmountOfButtons = 4;
            GUI.Box(new Rect(statRect.x + 0f, statRect.y + 0f, 275, 50f + 45 * AmountOfButtons), "", MainBgStyle);
            GUI.Label(new Rect(statRect.x + 5f, statRect.y + 5f, widthSize + 5, 50f), "Stats Menu", LabelStyle);
          
            GUI.Button(BtnRect(1, "stats"), "Damage Per LvL = " + Localbody.levelDamage, LabelStyle);
            GUI.Button(BtnRect(2, "stats"), "Crit Per Level = " + Localbody.levelCrit, LabelStyle);
            GUI.Button(BtnRect(3, "stats"), "Current Damage = " + Localbody.damage, LabelStyle);
            GUI.Button(BtnRect(4, "stats"), "Current Crit = " + Localbody.crit, LabelStyle);
           
        }

        public static void DrawMenu()
        {
            int AmountOfButtons = 16;
            GUI.Box(new Rect(mainRect.x + 0f, mainRect.y + 0f, widthSize + 10, 50f + 45 * AmountOfButtons), "", MainBgStyle);
            GUI.Label(new Rect(mainRect.x + 5f, mainRect.y + 5f, widthSize + 5, 95f), "RoRCheat Menu\n" + _VERSION, LabelStyle);
           if(!_CharacterCollected)
            {
                if (_CharacterToggle)
                {
                    if (GUI.Button(BtnRect(1, "full"), "Character Selection: ON", OnStyle))
                    {
                        _CharacterToggle = false;
                    }
                }
                else if (GUI.Button(BtnRect(1, "full"), "Character Selection: OFF", OffStyle))
                {
                    _CharacterToggle = true;
                }
                if (MouseToggle)
                {
                    if (GUI.Button(BtnRect(2, "full"), "Mouse 4 button Toggle : ON", OnStyle))
                    {
                        MouseToggle = false;
                    }
                }
                else if (GUI.Button(BtnRect(2, "full"), "Mouse 4 button Toggle : OFF", OffStyle))
                {
                    MouseToggle = true;
                }
            }


            if (_CharacterCollected)
            {
               
                // we dont have a god toggle bool, because we can just ref localhealth
                if (LocalHealth.godMode)
                {
                    if (GUI.Button(BtnRect(1, "full"), "God mode: ON", OnStyle))
                    {
                        LocalHealth.godMode = false;
                    }
                }
                else if (GUI.Button(BtnRect(1, "full"), "God mode: OFF", OffStyle))
                {
                    LocalHealth.godMode = true;
                }

                if (skillToggle)
                {
                    if (GUI.Button(BtnRect(2, "full"), "Infinite Skills: ON", OnStyle))
                    {
                        skillToggle = false;
                    }
                }
                else if (GUI.Button(BtnRect(2, "full"), "Infinite Skills: OFF", OffStyle))
                {
                    skillToggle = true;
                }

                if (renderInteractables)
                {
                    if (GUI.Button(BtnRect(3, "full"), "Interactables ESP: ON", OnStyle))
                    {
                        renderInteractables = false;
                    }
                }
                else if (GUI.Button(BtnRect(3, "full"), "Interactables ESP: OFF", OffStyle))
                {
                    renderInteractables = true;
                }
                if (_CharacterToggle)
                {
                    if (GUI.Button(BtnRect(4, "full"), "Character Selection: ON", OnStyle))
                    {
                        _CharacterToggle = false;
                    }
                }
                else if (GUI.Button(BtnRect(4, "full"), "Character Selection: OFF", OffStyle))
                {
                    _CharacterToggle = true;
                }
                if (_isStatMenuOpen)
                {
                    if (GUI.Button(BtnRect(5, "full"), "Stats Menu: ON", OnStyle))
                    {
                        _isStatMenuOpen = false;
                    }
                }
                else if (GUI.Button(BtnRect(5, "full"), "Stats Menu: OFF", OffStyle))
                {
                    _isStatMenuOpen = true;
                }
                if (_ItemToggle)
                {
                    if (GUI.Button(BtnRect(6, "full"), "Item Spawn Menu: ON", OnStyle))
                    {
                        _ItemToggle = false;
                    }
                }
                else if (GUI.Button(BtnRect(6, "full"), "Item Spawn Menu: OFF", OffStyle))
                {
                    _ItemToggle = true;
                }
                if (_isTeleMenuOpen)
                {
                    if (GUI.Button(BtnRect(7, "full"), "Teleporter Menu: ON", OnStyle))
                    {
                        _isTeleMenuOpen = false;
                    }
                }
                else if (GUI.Button(BtnRect(7, "full"), "Teleporter Menu: OFF", OffStyle))
                {
                    _isTeleMenuOpen = true;
                }
                if(NoclipToggle)
                {
                    if (GUI.Button(BtnRect(8, "full"), "Noclip: ON", OnStyle))
                    {
                        NoclipToggle = false;
                    }
                }
                else if (GUI.Button(BtnRect(8, "full"), "Noclip: ON", OffStyle))
                {
                    NoclipToggle = true;
                }
                if (MouseToggle)
                {
                    if (GUI.Button(BtnRect(9, "full"), "Mouse 4 button Toggle : ON", OnStyle)) {
                        MouseToggle = false;
                    }
                }
                else if (GUI.Button(BtnRect(9, "full"), "Mouse 4 button Toggle : OFF", OffStyle)) {
                    MouseToggle = true;
                }

                if (GUI.Button(BtnRect(10, "multiply"), "Give Money: " + moneyToGive.ToString(), BtnStyle))
                {
                    GiveMoney();
                }
                if (GUI.Button(new Rect(mainRect.x + widthSize - 80, mainRect.y + btnY, 40, 40), "-", OffStyle))
                {
                    if (moneyToGive > 100)
                        moneyToGive -= 100;
                }
                if (GUI.Button(new Rect(mainRect.x + widthSize - 35, mainRect.y + btnY, 40, 40), "+", OffStyle))
                {
                    if (moneyToGive >= 100)
                        moneyToGive += 100;
                }
                if (GUI.Button(BtnRect(11, "multiply"), "Give Lunar Coins: " + coinsToGive.ToString(), BtnStyle))
                {
                    GiveLunarCoins();
                }
                if (GUI.Button(new Rect(mainRect.x + widthSize - 80, mainRect.y + btnY, 40, 40), "-", OffStyle))
                {
                    if (coinsToGive > 10)
                        coinsToGive -= 10;
                }
                if (GUI.Button(new Rect(mainRect.x + widthSize - 35, mainRect.y + btnY, 40, 40), "+", OffStyle))
                {
                    if (coinsToGive >= 10)
                        coinsToGive += 10;
                }
                if (GUI.Button(BtnRect(12, "multiply"), "Give Experience: " + xpToGive.ToString(), BtnStyle))
                {
                    giveXP();
                }
                if (GUI.Button(new Rect(mainRect.x + widthSize - 80, mainRect.y + btnY, 40, 40), "-", OffStyle))
                {
                    if (xpToGive > 100)
                        xpToGive -= 100;
                }
                if (GUI.Button(new Rect(mainRect.x + widthSize - 35, mainRect.y + btnY, 40, 40), "+", OffStyle))
                {
                    if (xpToGive >= 100)
                        xpToGive += 100;
                }
                if (GUI.Button(BtnRect(13, "multiply"), "Roll Items: " + itemsToRoll.ToString(), BtnStyle))
                {
                    RollItems(itemsToRoll.ToString());
                }
                if (GUI.Button(new Rect(mainRect.x + widthSize - 80, mainRect.y + btnY, 40, 40), "-", OffStyle))
                {
                    if (itemsToRoll > 5)
                        itemsToRoll -= 5;
                }
                if (GUI.Button(new Rect(mainRect.x + widthSize - 35, mainRect.y + btnY, 40, 40), "+", OffStyle))
                {
                    if (itemsToRoll >= 5)
                        itemsToRoll += 5;
                }
                /*if (GUI.Button(BtnRect(11, "multiply"), "Give All Items: " + allItemsQuantity.ToString(), BtnStyle))
                {
                    GiveAllItems();
                }
                if (GUI.Button(new Rect(mainRect.x + widthSize - 80, mainRect.y + btnY, 40, 40), "-", OffStyle))
                {
                     if (allItemsQuantity > 1)
                         allItemsQuantity -= 1;
                }
                if (GUI.Button(new Rect(mainRect.x + widthSize - 35, mainRect.y + btnY, 40, 40), "+", OffStyle))
                {
                     if (allItemsQuantity >= 1)
                         allItemsQuantity += 1;
                }*/
                if (damageToggle)
                {
                    if (GUI.Button(BtnRect(14, "multiply"), "Damage LvL : " + damagePerLvl.ToString(), OnStyle))
                    {
                        damageToggle = false;
                    }
                }
                else if (GUI.Button(BtnRect(14, "multiply"), "Damage LvL : " + damagePerLvl.ToString(), OffStyle))
                {
                    damageToggle = true;
                }
                if (GUI.Button(new Rect(mainRect.x + widthSize - 80, mainRect.y + btnY, 40, 40), "-", OffStyle))
                {
                    if (damagePerLvl > 0)
                        damagePerLvl -= 10;
                }
                if (GUI.Button(new Rect(mainRect.x + widthSize - 35, mainRect.y + btnY, 40, 40), "+", OffStyle))
                {
                    if (damagePerLvl >= 0)
                        damagePerLvl += 10;
                }
                if (critToggle)
                {
                    if (GUI.Button(BtnRect(15, "multiply"), "Crit LvL : " + CritPerLvl.ToString(), OnStyle))
                    {
                        critToggle = false; 
                    }
                }
                else if (GUI.Button(BtnRect(15, "multiply"), "Crit LvL : " + CritPerLvl.ToString(), OffStyle))
                {
                    critToggle = true;
                }
                if (GUI.Button(new Rect(mainRect.x + widthSize - 80, mainRect.y + btnY, 40, 40), "-", OffStyle))
                {
                    if (CritPerLvl > 0)
                        CritPerLvl -= 1;
                }
                if (GUI.Button(new Rect(mainRect.x + widthSize - 35, mainRect.y + btnY, 40, 40), "+", OffStyle))
                {
                    if (CritPerLvl >= 0)
                        CritPerLvl += 1;
                }

                if (GUI.Button(BtnRect(16 , "full"), "Skip Stage", BtnStyle))
                    skipStage();

            }
            else
            {
                GUI.Box(new Rect(mainRect.x, mainRect.y + 95f * mulY, widthSize + 10, 50f), "", MainBgStyle);
                GUI.Label(new Rect(mainRect.x, mainRect.y + 95f * mulY, widthSize + 10, 50f), "<color=yellow>Note: Buttons Will Appear in Match Only</color>", LabelStyle);
            }
        }


        // Rect for buttons
        // It automatically auto position buttons. There is no need to change it
        public static Rect BtnRect(int y, String buttonType)
        {
            mulY = y;
            if (buttonType.Equals("multiply"))
            {
                btnY = 5 + 45 * y;
                return new Rect(mainRect.x + 5, mainRect.y + 5 + 45 * y, widthSize - 90, 40);
            }
            else if (buttonType.Equals("stats"))
            {
                return new Rect(statRect.x + 5, statRect.y + 5 + 45 * y, widthSize - 150, 40);
            }
            else if (buttonType.Equals("tele"))
            {
                return new Rect(teleRect.x + 5, teleRect.y + 5 + 45 * y, widthSize, 40);
            }
            else if (buttonType.Equals("full"))
            {
                return new Rect(mainRect.x + 5, mainRect.y + 5 + 45 * y, widthSize, 40);
            }
            else
            {
                return new Rect(mainRect.x + 5, mainRect.y + 5 + 45 * y, widthSize, 40);
            }
        }

        // Textures
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

        // random items
        private static void RollItems(string amount)
        {
            Debug.Log("RoRCheats : Rolling " + amount + " items");
            try
            {
                int num;
                TextSerialization.TryParseInvariant(amount, out num);
                if (num > 0)
                {
                    WeightedSelection<List<PickupIndex>> weightedSelection = new WeightedSelection<List<PickupIndex>>(8);
                    weightedSelection.AddChoice(Run.instance.availableTier1DropList, 80f);
                    weightedSelection.AddChoice(Run.instance.availableTier2DropList, 19f);
                    weightedSelection.AddChoice(Run.instance.availableTier3DropList, 1f);
                    for (int i = 0; i < num; i++)
                    {
                        List<PickupIndex> list = weightedSelection.Evaluate(UnityEngine.Random.value);
                        LocalPlayerInv.GiveItem(list[UnityEngine.Random.Range(0, list.Count)].itemIndex, 1);
                    }
                }
            }
            catch (ArgumentException)
            {
            }
        }
        public static void skipStage()
        {
            Run.instance.AdvanceStage(Run.instance.nextStageScene);
            Debug.Log("RoRCheats : Skipped Stage");
        }
        private static void SpawnPortals(string portal)
        {
            if (TeleporterInteraction.instance)
            {
                if (portal.Equals("gold"))
                {
                    Debug.Log("RoRCheats : Spawned Gold Portal");
                    TeleporterInteraction.instance.Network_shouldAttemptToSpawnGoldshoresPortal = true;
                }
                else if (portal.Equals("newt"))
                {
                    Debug.Log("RoRCheats : Spawned Shop Portal");
                    TeleporterInteraction.instance.Network_shouldAttemptToSpawnShopPortal = true;
                }
                else if (portal.Equals("blue"))
                {
                    Debug.Log("RoRCheats : Spawned Celestal Portal");
                    TeleporterInteraction.instance.Network_shouldAttemptToSpawnMSPortal = true;
                }
                else if (portal.Equals("all"))
                {
                    Debug.Log("RoRCheats : Spawned All Portals");
                    TeleporterInteraction.instance.Network_shouldAttemptToSpawnGoldshoresPortal = true;
                    TeleporterInteraction.instance.Network_shouldAttemptToSpawnShopPortal = true;
                    TeleporterInteraction.instance.Network_shouldAttemptToSpawnMSPortal = true;
                }
                else
                {
                    Debug.LogError("Selection was " + portal + " please contact mod developer.");
                }
                  

            }
        }
        void CharacterWindowMethod(Int32 id)
        {
            CharacterScrollPos = GUILayout.BeginScrollView(CharacterScrollPos);
            {
                
                foreach (var body in nameToIndexMap)
                {
                    if (body.Key.Contains("(Clone)"))
                        continue;
                    if (GUILayout.Button(body.Key.Replace("Body", "")))
                    {
                        GameObject newBody = RoR2.BodyCatalog.FindBodyPrefab(body.Key);
                        if (newBody == null)
                            return;
                        var localUser = RoR2.LocalUserManager.GetFirstLocalUser();
                        if (localUser == null || localUser.cachedMasterController == null || localUser.cachedMasterController.master == null) return;
                        var master = localUser.cachedMasterController.master;
                        if (master == null)
                        {
                            var user = ((RoR2.UI.MPEventSystem)UnityEngine.EventSystems.EventSystem.current).localUser;
                            if (user.eventSystem == UnityEngine.EventSystems.EventSystem.current)
                            {
                                if (user.currentNetworkUser == null)
                                    return;
                                user.currentNetworkUser.CallCmdSetBodyPreference(body.Value);
                            }
                            return;
                        }
                        master.bodyPrefab = newBody;
                        master.Respawn(master.GetBody().transform.position, master.GetBody().transform.rotation);
                        enabled = false;
                        DrawMenu();
                        return;

                    }
                }
                
            }
            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        // try and setup our character, if we hit an error we set it to false
        //TODO: Find a way to stop it from checking whilst in main menu/lobby menu
        private static void GetCharacter()
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
        private static void ClearInventory()
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
        //TODO:
        //Seems like after giving all items and removing all items, something breaks.
        //throws ArgumentException: Destination array was not long enough. Look into later.
        private static void GiveAllItems()
        {
            if (LocalPlayerInv)
            {
                for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < (ItemIndex)78; itemIndex++)
                {
                    //plantonhit kills you when you pick it up
                    if (itemIndex == ItemIndex.PlantOnHit)
                        continue;
                    //ResetItem sets quantity to 1, RemoveItem removes the last one.
                    LocalPlayerInv.GiveItem(itemIndex, allItemsQuantity);
                }
            }
        }

        void SpawnItem(Int32 id)
        {
            int width = 170;
            if (_CharacterCollected)
            { 

            ItemScrollPos = GUILayout.BeginScrollView(ItemScrollPos);

           
                for (int i = (int)ItemIndex.Syringe; i < (int)ItemIndex.Count; i++)
                {
                    GUILayout.BeginHorizontal("box") ;
                    var def = ItemCatalog.GetItemDef((ItemIndex)i);
                    if (GUILayout.Button(Language.GetString(def.nameToken), BtnStyle, GUILayout.Width(width), GUILayout.ExpandHeight(true)))
                    {
                        GUILayout.Space(10);
                        var localUser = LocalUserManager.GetFirstLocalUser();
                        if (localUser.cachedMasterController && localUser.cachedMasterController.master)
                        {
                            var body = localUser.cachedMasterController.master.GetBody();
                            PickupDropletController.CreatePickupDroplet(new PickupIndex((ItemIndex)i), body.transform.position + (Vector3.up * 1.5f), Vector3.up * 20f + body.transform.forward * 2f);
                            
                        }
                    }
                    if(GUILayout.Button("x10", BtnStyle, GUILayout.Width(width), GUILayout.ExpandHeight(true)))
                    {
                       
                        for(int counter = 0; counter <= 9; counter++)
                        {
                            var localUser = LocalUserManager.GetFirstLocalUser();
                            if (localUser.cachedMasterController && localUser.cachedMasterController.master)
                            {
                                var body = localUser.cachedMasterController.master.GetBody();
                                PickupDropletController.CreatePickupDroplet(new PickupIndex((ItemIndex)i), body.transform.position + (Vector3.up * 1.5f), Vector3.up * 20f + body.transform.forward * 2f);
                            
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
            GUI.DragWindow();
        }
       
        private static void StackInventory()
        {
            //Does the same thing as the shrine of order. Orders all your items into stacks of several random items.
            LocalPlayerInv.ShrineRestackInventory(Run.instance.runRNG);
        }
        // self explanatory
        private static void giveXP()
        {
            LocalPlayer.GiveExperience(xpToGive);
        }
        private static void GiveMoney()
        {
            LocalPlayer.GiveMoney(moneyToGive);
        }
        //uh, duh.
        private static void GiveLunarCoins()
        {
            LocalNetworkUser.AwardLunarCoins(coinsToGive);
        }

        public static void LevelPlayersDamage()
        { 
            Localbody.levelDamage = (float)damagePerLvl;
            Localbody.RecalculateStats();
        }
        public static void LevelPlayersCrit()
        {
            Localbody.levelCrit = (float)CritPerLvl;
            Localbody.RecalculateStats();
        }

        
        private static void RenderInteractables()
        {

            foreach (PurchaseInteraction purchaseInteraction in PurchaseInteraction.FindObjectsOfType(typeof(PurchaseInteraction)))
            {
                if(purchaseInteraction.available)
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
    }
}
