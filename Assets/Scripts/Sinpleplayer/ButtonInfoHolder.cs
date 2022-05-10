using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonInfoHolder : MonoBehaviour
{
    [SerializeField]
    Sprite [] bg;
    Image background;
    Image charPic;
    int bgIndex;
    string charName;
    [SerializeField]
    Image display;
    [SerializeField]
    Sprite defaultImg;
    [SerializeField]
    TMP_Text nameText, info;
    characterSelectionView CSV;
    [SerializeField]
    ButtonInfoHolder currentSelected;
    private void Awake()
    {
        //bgIndex = Random.Range(0, bg.Length);
        background = gameObject.GetComponent<Image>();
        charPic = transform.GetChild(0).GetComponent<Image>();
        CSV = transform.GetComponentInParent<characterSelectionView>();
        ResetSlot();
    }

    public void SetCurrent(Sprite img, int index)
    {
        //background.sprite = bg[index];
        //charPic.sprite = img;
        //charPic.color = new Color(225, 225, 225, 225);
    }
    public void SetChar(string name)
    {
        charName = name;
        if (charName == "null")
        {
            background.sprite = defaultImg;
            charPic.color = new Color(225, 225, 225, 0);
        }
        else
        {
            bgIndex = Random.Range(0, bg.Length);
            background.sprite = bg[bgIndex];
            charPic.sprite = Resources.Load(Path.Combine("SinglePlayerPrefabs/DisplaySprites/HeadShots", name), typeof(Sprite)) as Sprite;
            charPic.color = new Color(225, 225, 225, 225);
        }
        




    }

    public void OnClick()
    {
        if(charName=="null")
        {
            Application.OpenURL("https://app.cryptofightclub.io/mint");
        }
        else
        {
            display.sprite = Resources.Load(Path.Combine("SinglePlayerPrefabs/DisplaySprites/Display", charName), typeof(Sprite)) as Sprite;
            display.color = new Color(225, 225, 225, 225);
            CSV.DisablePlay();
            CSV.UpdateSelected(transform.GetSiblingIndex());
            UpdateInfo();
            //currentSelected.SetCurrent(charPic.sprite, bgIndex);
        }
       
    }

    private void ResetSlot()
    {
        charPic.sprite = defaultImg;
        charPic.color = new Color(225, 225, 225, 0);
    }


    void UpdateInfo()
    {
        nameText.text = charName.ToUpper();
        Invoke("UpdateSessionInfo", 1.5f);
       
    }

    void UpdateSessionInfo()
    {
        if (chickenGameModel.currentNFTSession<10)
            CSV.EnablePlay();
        info.text = "PLAYED " + "<color=red>"+ chickenGameModel.currentNFTSession +"</color>" + " OUT OF <color=red> 10 </color> DAILY GAMES";
    }
}
