using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace RoRCheats
{
    public class itemManager : MonoBehaviour
    {
        public static void GiveItems(GUIStyle buttonStyle, string buttonName)
        {
            int buttonPlacement = 1;
            for (int i = (int)ItemIndex.Syringe; i < (int)ItemIndex.Count; i++)
            {
                var def = ItemCatalog.GetItemDef((ItemIndex)i);
                if (GUI.Button(btn.BtnRect(buttonPlacement, false, buttonName), Language.GetString(def.nameToken),buttonStyle))
                {
                    var localUser = LocalUserManager.GetFirstLocalUser();
                    if (localUser.cachedMasterController && localUser.cachedMasterController.master)
                    {
                        var body = localUser.cachedMasterController.master.GetBody();
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex((ItemIndex)i), body.transform.position + (Vector3.up * 1.5f), Vector3.up * 20f + body.transform.forward * 2f);
                    }
                }
                buttonPlacement++;
            }
        }
        public static void GiveEquipment(GUIStyle buttonStyle, string buttonName)
        {
            int buttonPlacement = 1;
            for (int i = (int)EquipmentIndex.CommandMissile; i < (int)EquipmentIndex.Count; i++)
            {
                var def = EquipmentCatalog.GetEquipmentDef((EquipmentIndex)i);
                if (GUI.Button(btn.BtnRect(buttonPlacement, false, buttonName), Language.GetString(def.nameToken), buttonStyle))
                {
                    var localUser = LocalUserManager.GetFirstLocalUser();
                    if (localUser.cachedMasterController && localUser.cachedMasterController.master)
                    {
                        var body = localUser.cachedMasterController.master.GetBody();
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex((EquipmentIndex)i), body.transform.position + Vector3.up * 1.5f, Vector3.up * 20f + body.transform.forward * 2f);
                    }
                }
                buttonPlacement++;
            }
        }

        public static void RollItems(string amount, Inventory LocalPlayerInv)
        {
            Debug.Log(Main.log+"Rolling " + amount + " items");
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

        public static void GiveAllItems(Inventory LocalPlayerInv)
        {
            if (LocalPlayerInv)
            {
                for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < (ItemIndex)99; itemIndex++)
                {
                    //plantonhit kills you when you pick it up
                    if (itemIndex == ItemIndex.PlantOnHit || itemIndex == ItemIndex.HealthDecay || itemIndex == ItemIndex.TonicAffliction || itemIndex == ItemIndex.BurnNearby || itemIndex == ItemIndex.CrippleWardOnLevel || itemIndex == ItemIndex.Ghost || itemIndex == ItemIndex.ExtraLifeConsumed)
                        continue;
                    //ResetItem sets quantity to 1, RemoveItem removes the last one.
                    LocalPlayerInv.GiveItem(itemIndex, Main.allItemsQuantity);
                }
            }
        }
        public static void ClearInventory(Inventory LocalPlayerInv)
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
        public static void StackInventory(Inventory LocalPlayerInv)
        {
            //Does the same thing as the shrine of order. Orders all your items into stacks of several random items.
            LocalPlayerInv.ShrineRestackInventory(Run.instance.runRNG);
        }

    }
}