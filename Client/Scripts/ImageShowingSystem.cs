using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageShowingSystem : MonoBehaviour {

    public static ImageShowingSystem Instance { get; set; }
    public GameObject imagePanel;
    public Item[] items;

    Image image;
    Text name;
    Text description;
    List<Sprite> sprites;
    //Text discription;

    int imageIndex;

    void Start()
    {
        imagePanel.SetActive(false);
        image = imagePanel.transform.Find("Image").GetComponent<Image>();
        description = imagePanel.transform.Find("Text").GetComponent<Text>();
        name = imagePanel.transform.Find("Name").GetChild(0).GetComponent<Text>();



        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ContinueShow();
        }
    }

    public void AddNewImageShow(List<Sprite> spritesToAdd)
    {
        imageIndex = 0;
        sprites = new List<Sprite>();
        sprites = spritesToAdd;
        for(int i = 0; i< 8; i++)
        {
            if (sprites[0] == items[i].sprite)
            {
                imageIndex = i;
                Inventory.Instance.seletedIndex = imageIndex;
                break;
            }
        }
        CreateDialog();
    }

    public void CreateDialog()
    {
        image.sprite = items[imageIndex].sprite;
        name.text = items[imageIndex].Name;
        description.text = items[imageIndex].description[0];
        imagePanel.SetActive(true);
    }

    public void ContinueShow()
    {
            imagePanel.SetActive(false);

    }

}
