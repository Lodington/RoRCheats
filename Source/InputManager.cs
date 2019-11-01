using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RoRCheats
{

    public class InputManager
    {

        static Dictionary<string, KeyCode> keyMapping;
        static KeyCode[] CustomKeys = new KeyCode[4];

        static string[] keyMaps = new string[4]
        {
            "OpenMenu",
            "GiveMoney",
            "OpenTeleMenu",
            "ToggleNoClip"
        };
        static KeyCode[] defaults = new KeyCode[4]
        {
            KeyCode.Insert, //toggle Main menu
            KeyCode.V, //Give money
            KeyCode.B, //open tele menu
            KeyCode.C //toggle no clip
        };
        
        static InputManager()
        {
            CustomKeys[0] = sanitzeKeybinds(MainMenu.OpenMenuKey.Value);
            CustomKeys[1] = sanitzeKeybinds(MainMenu.GiveMoneyKey.Value);
            CustomKeys[2] = sanitzeKeybinds(MainMenu.OpenTeleMenu.Value);
            CustomKeys[3] = sanitzeKeybinds(MainMenu.ToggleNoclip.Value);
            InitializeDictionary(GetKeybinds(defaults, CustomKeys));
        }

        public static KeyCode sanitzeKeybinds(String Keybind)
        {
            bool isValid = Enum.TryParse<KeyCode>(Keybind, true ,out KeyCode key);
            if (isValid)
            {
                Debug.LogWarning(key);
                return key;
            }
            else if (!isValid)
            {
                Debug.LogError("Invalid Keybind in Config File");
                return key;
            }
            return key;

        }

        private static KeyCode[] GetKeybinds(KeyCode[] DefaultKeybinds, KeyCode[] CustomKeybinds)
        {

            bool isEqual = Enumerable.SequenceEqual(DefaultKeybinds, CustomKeybinds);

            if(!isEqual)
            {
                for(int i = 0; i < CustomKeybinds.Length; i++)
                {
                    if (CustomKeybinds[i] == KeyCode.None)
                    {
                        Debug.Log("Loaded Default Keybinds");
                        return DefaultKeybinds;
                    }
                }
                Debug.Log("Loaded Keybinds From Config File");
                return CustomKeybinds;
            } 
            else if(isEqual)
            {
                Debug.Log("Loaded Default Keybinds");
                return DefaultKeybinds;
            }
            Debug.LogError("Something Went Wrong Loading Default Keybinds");
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

        public static void SetKeyMap(string keyMap, KeyCode key)
        {
            if (!keyMapping.ContainsKey(keyMap))
                Debug.LogError("Invalid KeyMap in SetKeyMap: " + keyMap);
            keyMapping[keyMap] = key;
        }

        public static bool GetKeyDown(string keyMap)
        {
            return Input.GetKeyDown(keyMapping[keyMap]);
        }
            
    }
}

