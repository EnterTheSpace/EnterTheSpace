using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    //REFS
    public InteractController interacter;
    //DATA
    public int money;
    public List<PurchasableSpecInfos> items;

    private void Update() {
        Shop temp = (interacter.InRangeObject != null) ? interacter.InRangeObject.GetComponent<Shop>() : null;
        //Checking inside interacter class for shop object
        if (temp != null) {
            if (temp.buy) {
                temp.buy = false;
                //Check existence
                if (!ItemPossessed(temp.items[temp.itemIndex].infos)) {
                    if (money >= temp.items[temp.itemIndex].infos.Price) {
                        if (temp.items[temp.itemIndex].infos.Title == "Long Dash") {
                            Persistent.dashLength = 4f;
                            Persistent.items.Add(temp.items[temp.itemIndex].infos.Thumbnail);
                        }
                        if (temp.items[temp.itemIndex].infos.Title == "Laser Gun") {
                            Persistent.weapInfos = "Laser Gun";
                        }
                        money -= temp.items[temp.itemIndex].infos.Price;
                        items.Add(temp.items[temp.itemIndex].infos);
                    }
                }
            }
        }
    }

    public bool ItemPossessed(PurchasableSpecInfos item) {
        for (int i = 0; i < items.Count; i++) {
            if (items[i] == item)
                return true;
        }
        return false;
    }

    public bool ItemPossessed(string name) {
        for (int i = 0; i < items.Count; i++) {
            if (items[i].Title == name)
                return true;
        }
        return false;
    }
}
