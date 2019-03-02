using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[System.Serializable]
public class PurchasableSpecInfos {
    [SerializeField] private string title;
    public string Title { get { return title; } set { title = value; } }
    [SerializeField] private Sprite thumbnail;
    public Sprite Thumbnail { get { return thumbnail; } set { thumbnail = value; } }
    [SerializeField] private int price;
    public int Price { get { return price; } set { price = value; } }
    [SerializeField] private string desc;
    public string Desc { get { return desc; } set { desc = value; } }
    [SerializeField] private VideoClip preview;
    public VideoClip Preview { get { return preview; } set { preview = value; } }
}

public class PurchasableSpec : MonoBehaviour {

    [SerializeField] private Text title;
    public Text Title { get { return title; } set { title = value; } }
    [SerializeField] private Image thumbnail;
    public Image Thumbnail { get { return thumbnail; } set { thumbnail = value; } }
    [SerializeField] private Text price;
    public Text Price { get { return price; } set { price = value; } }
    [SerializeField] private Text desc;
    public Text Desc { get { return desc; } set { desc = value; } }
    [SerializeField] private VideoPlayer preview;
    public VideoPlayer Preview { get { return preview; } set { preview = value; } }

    public void Setup(PurchasableSpecInfos infos) {
        title.text = infos.Title;
        thumbnail.sprite = infos.Thumbnail;
        price.text = infos.Price.ToString();
        desc.text = infos.Desc;
        preview.clip = infos.Preview;
    }
}
