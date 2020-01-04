using RoR2;
using System;
using UnityEngine;

namespace RoRCheats
{
    internal class NoClip
    {
        public static void DoNoclip()
        {
            try
            {
                var forwardDirection = Main.Localbody.GetComponent<InputBankTest>().moveVector.normalized;
                var aimDirection = Main.Localbody.GetComponent<InputBankTest>().aimDirection.normalized;
                var isForward = Vector3.Dot(forwardDirection, aimDirection) > 0f;

                var isSprinting = Main.LocalNetworkUser.inputPlayer.GetButton("Sprint");
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                var isStrafing = Main.LocalNetworkUser.inputPlayer.GetAxis("MoveVertical") != 0f;

                if (isSprinting)
                {
                    Main.Localbody.characterMotor.velocity = forwardDirection * 100f;
                    if (isStrafing)
                    {
                        if (isForward)
                        {
                            Main.Localbody.characterMotor.velocity.y = aimDirection.y * 100f;
                        }
                        else
                        {
                            Main.Localbody.characterMotor.velocity.y = aimDirection.y * -100f;
                        }
                    }
                }
                else
                {
                    Main.Localbody.characterMotor.velocity = forwardDirection * 50;
                    if (isStrafing)
                    {
                        if (isForward)
                        {
                            Main.Localbody.characterMotor.velocity.y = aimDirection.y * 50;
                        }
                        else
                        {
                            Main.Localbody.characterMotor.velocity.y = aimDirection.y * -50;
                        }
                    }
                }
            }
            catch (NullReferenceException) { }
        }
    }
}