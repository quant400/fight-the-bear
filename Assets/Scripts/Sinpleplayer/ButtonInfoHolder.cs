using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class ButtonInfoHolder : MonoBehaviour
{
    [SerializeField]
    Sprite [] bg, selectedCharBG;
    Image background;
    Image charPic;
    int bgIndex;
    string charName;
    [SerializeField]
    Image display;
    Image displayBG;
    [SerializeField]
    Sprite defaultImg;
    [SerializeField]
    TMP_Text nameText, info;
    characterSelectionView CSV;
    [SerializeField]
    ButtonInfoHolder currentSelected;
    Button button;
    private void Awake()
    {
        //bgIndex = Random.Range(0, bg.Length);
        background = gameObject.GetComponent<Image>();
        charPic = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        displayBG = display.transform.parent.GetComponent<Image>();
        CSV = transform.GetComponentInParent<characterSelectionView>();
        ResetSlot();
    }
    private void Start()
    {
        button = GetComponent<Button>();
        observeBtns();
    }

    void observeBtns()
    {
        button.OnClickAsObservable()
            .Do(_ => OnClick())
            .Where(_ => PlaySounds.instance != null)
            .Do(_ => PlaySounds.instance.Play())
            .Subscribe()
            .AddTo(this);
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
            background.sprite = bg[CSV.GetavaliableColor()];
            if (gameplayView.instance.usingFreemint)

                charPic.sprite = Resources.Load(Path.Combine("SinglePlayerPrefabs/DisplaySprites/FreeMint/HeadShots", name), typeof(Sprite)) as Sprite;

            else
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
            displayBG.sprite = selectedCharBG[int.Parse(background.sprite.name)];
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
        if (bearGameModel.currentNFTSession<3)
            CSV.EnablePlay();
        info.text = "PLAYED " + bearGameModel.currentNFTSession + " OUT OF 3 DAILY GAMES";
    }
}
