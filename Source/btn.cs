using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RoRCheats
{
    class btn
    {
        static public Rect BtnRect(int y, string buttonType)
        {

            if (buttonType.Equals("multiply"))
            {
                Main.btnY = 5 + 45 * y;
                Main.MainMulY = y;
                return new Rect(Main.mainRect.x + 5, Main.mainRect.y + 5 + 45 * y, Main.widthSize - 90, 40);
            }
            else if (buttonType.Equals("stats"))
            {
                Main.StatMulY = y;
                return new Rect(Main.statRect.x + 5, Main.statRect.y + 5 + 45 * y, Main.widthSize - 150, 40);
            }
            else if (buttonType.Equals("tele"))
            {
                Main.TeleMulY = y;
                return new Rect(Main.teleRect.x + 5, Main.teleRect.y + 5 + 45 * y, Main.widthSize, 40);
            }
            else if (buttonType.Equals("spawner"))
            {
                Main.SpawnerMulY = y;
                return new Rect(Main.spawnerRect.x + 5, Main.spawnerRect.y + 5 + 45 * y, Main.widthSize, 40);
            }
            else if (buttonType.Equals("character"))
            {
                Main.CharacterMulY = y;
                return new Rect(Main.characterRect.x + 5, Main.characterRect.y + 5 + 45 * y, Main.widthSize, 40);
            }
            else if (buttonType.Equals("lobby"))
            {
                Main.LobbyMulY = y;
                return new Rect(Main.lobbyRect.x + 5, Main.lobbyRect.y + 5 + 45 * y, Main.widthSize, 40);
            }
            else if (buttonType.Equals("full"))
            {
                Main.MainMulY = y;
                return new Rect(Main.mainRect.x + 5, Main.mainRect.y + 5 + 45 * y, Main.widthSize, 40);
            }
            else
            {

                return new Rect(Main.mainRect.x + 5, Main.mainRect.y + 5 + 45 * y, Main.widthSize, 40);
            }
        }
    }
}
