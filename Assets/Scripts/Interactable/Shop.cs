using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : Interactable {

    public PurchasableItem[] items;
    public PurchasableSpec spec;
    public Scrollbar scroll;
    public Canvas canvasRef;
    public int itemIndex;
    private int lastItem;

    public bool buy;

    private void Start() {
        lastItem = 0;
        itemIndex = 0;
        spec.Setup(items[itemIndex].infos);
    }

    private void Update() {
        if(lastItem != itemIndex) {
            items[lastItem].Highlight(false);
            lastItem = itemIndex;
            spec.Setup(items[itemIndex].infos);
        }
        if (itemIndex == 0)
            scroll.value = 1f;
        else
            scroll.value =  1f-((float)(itemIndex+1) / items.Length);
        items[itemIndex].Highlight(true);
    }

    public override void Interact() {
        if (beingUsed) {
            buy = true;
        } else {
            base.Interact();
            canvasRef.gameObject.SetActive(true);
        }
    }

    public override void Release() {
        if (beingUsed) {
            base.Release();
            itemIndex = 0;
            canvasRef.gameObject.SetActive(false);
        } else {

        }
    }

    public void Navigate(float dir){
        if (dir > 0.1f)
            itemIndex++;
        else if(dir < -0.1f)
            itemIndex--;

        itemIndex = Mathf.Clamp(itemIndex, 0, items.Length - 1);
    }
}
