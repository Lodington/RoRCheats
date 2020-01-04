using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoRCheats
{
    public class InputManager
    {
        private static KeyCode[] CustomKeys = new KeyCode[4];
        private static KeyCode[] defaults = new KeyCode[4]
        {
            KeyCode.Insert, //toggle Main menu
            KeyCode.V, //Give money
            KeyCode.B, //open tele menu
            KeyCode.C //toggle no clip
        };

        private static Dictionary<string, KeyCode> keyMapping;
        private static string[] keyMaps = new string[4]
        {
            "OpenMenu",
            "GiveMoney",
            "OpenTeleMenu",
            "ToggleNoClip"
        };
        static InputManager()
        {
            CustomKeys[0] = sanitzeKeybinds(Main.OpenMenuKey.Value);
            CustomKeys[1] = sanitzeKeybinds(Main.GiveMoneyKey.Value);
            CustomKeys[2] = sanitzeKeybinds(Main.OpenTeleMenu.Value);
            CustomKeys[3] = sanitzeKeybinds(Main.ToggleNoclip.Value);
            InitializeDictionary(GetKeybinds(defaults, CustomKeys));
        }

        public static bool GetKeyDown(string keyMap)
        {
            return Input.GetKeyDown(keyMapping[keyMap]);
        }

        public static KeyCode sanitzeKeybinds(String Keybind)
        {
            bool isValid = Enum.TryParse<KeyCode>(Keybind, true, out KeyCode key);
            if (isValid)
            {
                Debug.LogWarning(Main.log+"Registered Key : " + key);
                return key;
            }
            else if (!isValid)
            {
                Debug.LogError(Main.log+"Invalid Keybind in Config File");
                return key;
            }
            return key;
        }

        public static void SetKeyMap(string keyMap, KeyCode key)
        {
            if (!keyMapping.ContainsKey(keyMap))
                Debug.LogError(Main.log+"Invalid KeyMap in SetKeyMap: " + keyMap);
            keyMapping[keyMap] = key;
        }

        private static KeyCode[] GetKeybinds(KeyCode[] DefaultKeybinds, KeyCode[] CustomKeybinds)
        {
            bool isEqual = Enumerable.SequenceEqual(DefaultKeybinds, CustomKeybinds);

            if (!isEqual)
            {
                for (int i = 0; i < CustomKeybinds.Length; i++)
                {
                    if (CustomKeybinds[i] == KeyCode.None)
                    {
                        Debug.Log(Main.log + "Loaded Default Keybinds");
                        return DefaultKeybinds;
                    }
                }
                Debug.Log(Main.log + "Loaded Keybinds From Config File");
                return CustomKeybinds;
            }
            else if (isEqual)
            {
                Debug.Log(Main.log + "Loaded Default Keybinds");
                return DefaultKeybinds;
            }
            Debug.LogError(Main.log + "Something Went Wrong...Loading Default Keybinds");
            return DefaultKeybinds;
        }

        private static void InitializeDictionary(KeyCode[] Keybinds)
        {
            keyMapping = new Dictionary<string, KeyCode>();
            for (int i = 0; i < keyMaps.Length; ++i)
            {
                keyMapping.Add(keyMaps[i], Keybinds[i]);
            }
        }
    }
}