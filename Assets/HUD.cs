using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public InventoryController inventory;

    public Image[] slots;
    public Text money;

    private int occupiedSlot;
    // Start is called before the first frame update
    void Start()
    {
        occupiedSlot = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (inventory.money != int.Parse(money.text) ||
            inventory.items.Count != occupiedSlot) {
            occupiedSlot = inventory.items.Count;
            UpdateInventory();
        }
    }

    public void UpdateInventory() {
        money.text = inventory.money.ToString();
        for (int i = 0; i < slots.Length; i++) {
            if (inventory.items.Count > i) {
                slots[i].gameObject.SetActive(true);
                slots[i].sprite = inventory.items[i].Thumbnail;
            } else {
                slots[i].gameObject.SetActive(false);
                slots[i].sprite = null;
            }
        }
    }
}
