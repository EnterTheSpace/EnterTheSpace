using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


public class PurchasableItem : MonoBehaviour {

    [SerializeField] private Text title;
    public Text Title { get { return title; } set { title = value; } }
    [SerializeField] private Image thumbnail;
    public Image Thumbnail { get { return thumbnail; } set { thumbnail = value; } }
    [SerializeField] private Text price;
    public Text Price { get { return price; } set { price = value; } }
    [SerializeField] private Image hightlight;
    public bool isHighlighted { get; private set; }

    public PurchasableSpecInfos infos;

    private void Awake() {
        Setup(infos);
    }

    public void Setup(PurchasableSpecInfos itemInfos) {
        infos = itemInfos;

        title.text = itemInfos.Title;
        thumbnail.sprite = itemInfos.Thumbnail;
        price.text = itemInfos.Price.ToString();
    }

    public void Highlight(bool yes) {
        if (yes) {
            hightlight.color = Color.green;
            isHighlighted = true;
        } else {
            hightlight.color = Color.white;
            isHighlighted = false;
        }
    }
}
