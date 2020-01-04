using RoR2;
using UnityEngine;

namespace RoRCheats
{
    internal class TeleporterHandler
    {
        public static void InstaTeleporter()
        {
            if (TeleporterInteraction.instance)
            {
                TeleporterInteraction.instance.remainingChargeTimer = 0;
                Debug.Log("RoRCheats : Skipping Stage");
            }
        }

        public static void skipStage()
        {
            Run.instance.AdvanceStage(Run.instance.nextStageScene);
            Debug.Log("RoRCheats : Skipped Stage");
        }

        public static void addMountain()
        {
            TeleporterInteraction.instance.AddShrineStack();
        }

        public static void SpawnPortals(string portal)
        {
            if (TeleporterInteraction.instance)
            {
                if (portal.Equals("gold"))
                {
                    Debug.Log("RoRCheats : Spawned Gold Portal");
                    TeleporterInteraction.instance.shouldAttemptToSpawnGoldshoresPortal = true;
                }
                else if (portal.Equals("newt"))
                {
                    Debug.Log("RoRCheats : Spawned Shop Portal");
                    TeleporterInteraction.instance.shouldAttemptToSpawnShopPortal = true;
                }
                else if (portal.Equals("blue"))
                {
                    Debug.Log("RoRCheats : Spawned Celestal Portal");
                    TeleporterInteraction.instance.shouldAttemptToSpawnMSPortal = true;
                }
                else if (portal.Equals("all"))
                {
                    Debug.Log("RoRCheats : Spawned All Portals");
                    TeleporterInteraction.instance.shouldAttemptToSpawnGoldshoresPortal = true;
                    TeleporterInteraction.instance.shouldAttemptToSpawnShopPortal = true;
                    TeleporterInteraction.instance.shouldAttemptToSpawnMSPortal = true;
                }
                else
                {
                    Debug.LogError("Selection was " + portal + " please contact mod developer.");
                }
            }
        }
    }
}