
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using System.IO;
using UniRx.Triggers;
using UniRx;
using UniRx.Operators;
public class characterSelectionView : MonoBehaviour
{
    public int currentCharacter;
    [SerializeField]
    Camera cam;
    [SerializeField]
    Transform[] characters;
    [SerializeField]
    Transform characterList;
   
    bool selected;
    [SerializeField]
    float sideCharZdisp;
    [SerializeField]
    Button rightButton, leftButton , select;
    NFTInfo[] myNFT;
    [SerializeField]
    RuntimeAnimatorController controller;
    [SerializeField]
    GameObject buttonsToEnable, ButtonToDisable;
    [SerializeField]
    NFTInfo[] characterNFTMap;

    //for new screen
    [SerializeField]
    Transform[] charButtons;
    int currentStartIndex;
    //for skip
    bool skipping;
    UnityEngine.Object[] info;
    public void Start()
    {
        observeCharacterSelectionBtns();
        observesessionCounter();
        DisablePlay();
    }
    public void observesessionCounter()
    {
      
        gameplayView.instance.dailysessionReactive
            .Do(_ => setPlayButtonDependtoSessions(0))
            .Do(_=>chickenGameModel.currentNFTSession=0)
            .Subscribe()
            .AddTo(this);
    }
    void setPlayButtonDependtoSessions(int sessions)
    {
        if (sessions >= 10)
        {
            select.interactable = false;
            
        }
        else
        {
            select.interactable = true;
        }
    }
    void observeCharacterSelectionBtns()
    {
        rightButton.OnClickAsObservable()
            .Do(_ => MoveRight())
            .Where(_=>PlaySounds.instance!=null)
            .Do(_=> PlaySounds.instance.Play())
            .Subscribe()
            .AddTo(this);
        leftButton.OnClickAsObservable()
           .Do(_ => MoveLeft())
           .Where(_ => PlaySounds.instance != null)
           .Do(_ => PlaySounds.instance.Play())
           .Subscribe()
           .AddTo(this);
        select.OnClickAsObservable()
         .Do(_ => FinalSelectSinglePlayer())
         .Where(_ => PlaySounds.instance != null)
         .Do(_ => PlaySounds.instance.Play())
         .Do(_ => chickenGameModel.gameCurrentStep.Value = chickenGameModel.GameSteps.OnCharacterSelected)
         .Subscribe()
         .AddTo(this);
    }
   

    public void MoveRight()
    {
         /*rightButton.interactable = false;
         leftButton.interactable = false;
         if (currentCharacter < characters.Length - 1)
         {
             if (selected)
             {
                 characters[currentCharacter].GetComponent<Animator>().SetBool("Selected", false);
                 selected = false;
             }
             characters[currentCharacter].transform.localPosition += new Vector3(0, 0, sideCharZdisp);
             currentCharacter++;
             characters[currentCharacter].transform.DOLocalMove(characters[currentCharacter].transform.localPosition + new Vector3(0, 0, -sideCharZdisp), 0.5f);
             cam.transform.DOMoveX(characters[currentCharacter].transform.position.x, 0.5f).OnStepComplete(() =>
             {

                 rightButton.interactable = true;
                 leftButton.interactable = true;
             });
         }
         else
         {
             characters[currentCharacter].transform.localPosition += new Vector3(0, 0, sideCharZdisp);
             currentCharacter = 0;
             characters[currentCharacter].transform.DOLocalMove(characters[currentCharacter].transform.localPosition + new Vector3(0, 0, -sideCharZdisp), 0.5f);
             cam.transform.DOMoveX(characters[currentCharacter].transform.position.x, 0.5f).OnStepComplete(() =>
             {

                 rightButton.interactable = true;
                 leftButton.interactable = true;
             });
         }
         gameplayView.instance.chosenNFT = characterNFTMap[currentCharacter];
         gameplayView.instance.GetScores();*/

        currentStartIndex += 4;
        if (skipping) 
        {
            if (currentStartIndex+4 >info.Length-1)
                rightButton.gameObject.SetActive(false);
            else
                rightButton.gameObject.SetActive(true);
            if(currentStartIndex>0)
                leftButton.gameObject.SetActive(true);
            SkipDisplayChars(currentStartIndex);
        }
        else
        {
            Debug.Log(myNFT.Length);
            if (currentStartIndex+4 > myNFT.Length-1)
                rightButton.gameObject.SetActive(false);
            else
                rightButton.gameObject.SetActive(true);
            if (currentStartIndex > 0)
                leftButton.gameObject.SetActive(true);
            DisplayChar(currentStartIndex);
        }
        
      
    }

    public void MoveLeft()
    {/*
        if (currentCharacter > 0)
        {
            if (selected)
            {
                characters[currentCharacter].GetComponent<Animator>().SetBool("Selected", false);
                selected = false;
            }
            characters[currentCharacter].transform.localPosition += new Vector3(0, 0, sideCharZdisp);
            currentCharacter--;
            characters[currentCharacter].transform.DOLocalMove(characters[currentCharacter].transform.localPosition + new Vector3(0, 0, -sideCharZdisp), 0.5f);

            cam.transform.DOMoveX(characters[currentCharacter].transform.position.x, 0.5f).OnStepComplete(() =>
            {
                rightButton.interactable = true;
                leftButton.interactable = true;
            });
        }
        else
        {
            characters[currentCharacter].transform.localPosition += new Vector3(0, 0, sideCharZdisp);
            currentCharacter = characters.Length - 1;
            characters[currentCharacter].transform.DOLocalMove(characters[currentCharacter].transform.localPosition + new Vector3(0, 0, -sideCharZdisp), 0.5f);
            cam.transform.DOMoveX(characters[currentCharacter].transform.position.x, 0.5f).OnStepComplete(() =>
            {

                rightButton.interactable = true;
                leftButton.interactable = true;
            });
        }
        gameplayView.instance.chosenNFT = characterNFTMap[currentCharacter];
        gameplayView.instance.GetScores();
        */

        currentStartIndex -= 4;
        if (skipping)
        {
            if (currentStartIndex - 4 < 0)
                leftButton.gameObject.SetActive(false);
            else
                leftButton.gameObject.SetActive(true);
            if (currentStartIndex < info.Length)
                rightButton.gameObject.SetActive(true);
            SkipDisplayChars(currentStartIndex);
        }
        else
        {
            if (currentStartIndex - 4 < 0)
                leftButton.gameObject.SetActive(false);
            else
                leftButton.gameObject.SetActive(true);
            if (currentStartIndex < myNFT.Length)
                rightButton.gameObject.SetActive(true);
            DisplayChar(currentStartIndex);
        }
    }
    public void EnablePlay()
    {
        select.interactable = true;
    }
    public void DisablePlay()
    {
        select.interactable = false;
    }

    public void Selected()
    {
       // GameRoom.room.ChooseAvatar("PlayerAvatar" + currentCharacter);
        selected = true;
        characters[currentCharacter].GetComponent<Animator>().SetBool("Selected", true);

    }


    public void UndoFinalSelect()
    {
        rightButton.interactable = true;
        leftButton.interactable = true;
    }

    //added for single player
    public void FinalSelectSinglePlayer()
    {
        gameplayView.instance.chosenNFT = characterNFTMap[currentCharacter];
        selected = true;
        //characters[currentCharacter].GetComponent<Animator>().SetBool("Selected", true);
   

    }

    // foire new character selection
    public void UpdateSelected(int selected)
    {
        currentCharacter = currentStartIndex+selected;
        gameplayView.instance.chosenNFT = characterNFTMap[currentCharacter];
        gameplayView.instance.GetScores();
        
    }


    internal void SetData(NFTInfo[] nFTData)
    {
        //will be respossible for setting up characters according to nft
        myNFT = nFTData;
        SetUpCharacters();

    }


    private void SetUpCharacters()
    {
        if (chickenGameModel.charactersSetted == false)
        {

           


           skipping = false;
           characters = new Transform[myNFT.Length];
           characterNFTMap = new NFTInfo[myNFT.Length];
            
           
        }

        Done();


    }

   
    public void Skip()
    {
        skipping = true;
        info = Resources.LoadAll("SinglePlayerPrefabs/DisplaySprites/HeadShots", typeof(Sprite));
        characterNFTMap = new NFTInfo[info.Length];
        SkipDisplayChars(0);
        Done();
    }
   
    void SkipDisplayChars(int startingindex)
    {
        for (int i = 0; i < 4; i++)
        { 
            
            if (i+startingindex>=info.Length)
                charButtons[i].GetComponent<ButtonInfoHolder>().SetChar("null");
            else
            {
                string name = info[i + startingindex].name;
                charButtons[i].GetComponent<ButtonInfoHolder>().SetChar(name);
                characterNFTMap[i + startingindex] = new NFTInfo { id = 175, name = name };
            }
        }
    }
    void DisplayChar(int startingindex)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i + startingindex >= myNFT.Length)
            {
                charButtons[i].GetComponent<ButtonInfoHolder>().SetChar("null");
            }

            else
            {
                string charName = NameToSlugConvert(myNFT[i+startingindex].name);
                charButtons[i].GetComponent<ButtonInfoHolder>().SetChar(charName);
                characterNFTMap[i+startingindex] = myNFT[i+startingindex];
            }
        }
        chickenGameModel.charactersSetted = true;
    }
    private void Done()
    {
        buttonsToEnable.SetActive(true);
        ButtonToDisable.SetActive(false);
    }



    string NameToSlugConvert(string name)
    {
        string slug;
        slug = name.ToLower().Replace(".", "").Replace("'", "").Replace(" ", "-");
        return slug;

    }
}

