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
}
