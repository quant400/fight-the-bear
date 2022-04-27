using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using System.IO;

public class CharacterSelectionScript : MonoBehaviour
{
    [SerializeField]
    Camera cam;
    [SerializeField]
    Transform[] characters;
    [SerializeField]
    Transform characterList;
    int currentCharacter;
    bool selected;
    [SerializeField]
    float sideCharZdisp;
    [SerializeField]
    Button rightButton,leftButton;
    NFTInfo[] myNFT;
    [SerializeField]
    RuntimeAnimatorController controller;
    [SerializeField]
    GameObject buttonsToEnable, ButtonToDisable;

    NFTInfo[] characterNFTMap;


    //temp flag for skip 
    bool skip=false;
    public void MoveRight()
    {
        rightButton.interactable = false;
        leftButton.interactable = false;
        if (currentCharacter < characters.Length-1)
        {
            if (selected)
            {
                characters[currentCharacter].GetComponent<Animator>().SetBool("Selected", false);
                selected = false;
            }
            characters[currentCharacter].transform.localPosition += new Vector3(0, 0, sideCharZdisp);
            currentCharacter++;
            characters[currentCharacter].transform.DOLocalMove(characters[currentCharacter].transform.localPosition + new Vector3(0, 0, -sideCharZdisp), 0.5f);
            cam.transform.DOMoveX(characters[currentCharacter].transform.position.x,  0.5f).OnStepComplete(()=> 
            {
                
                rightButton.interactable = true;
                leftButton.interactable = true;
            } );
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
       
    }

    public void MoveLeft()
    {
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

            cam.transform.DOMoveX(characters[currentCharacter].transform.position.x,0.5f).OnStepComplete(() =>
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

        
    }

   
    public void Selected()
    {
       // GameRoom.room.ChooseAvatar("PlayerAvatar" + currentCharacter);
        selected = true;
        characters[currentCharacter].GetComponent<Animator>().SetBool("Selected", true);

    }

    /*public void FinalSelect()
    {
        GameRoom.room.ChooseAvatar("PlayerAvatar" + currentCharacter);
        selected = true;
        characters[currentCharacter].GetComponent<Animator>().SetBool("Selected", true);
        rightButton.interactable = false;
        leftButton.interactable = false;

    }*/

    public void UndoFinalSelect()
    {
        rightButton.interactable = true;
        leftButton.interactable = true;
    }

    //added for single player
    public void FinalSelectSinglePlayer()
    {
        SingleplayerGameControler.instance.chosenNFT = characterNFTMap[currentCharacter];
        selected = true;
        characters[currentCharacter].GetComponent<Animator>().SetBool("Selected", true);
        rightButton.interactable = false;
        leftButton.interactable = false;

    }



    internal void SetData(NFTInfo[] nFTData)
    {
        //will be respossible for setting up characters according to nft
        myNFT = nFTData;
        SetUpCharacters();
        
    }

   
    private void SetUpCharacters()
    {
        characters = new Transform[myNFT.Length];
        characterNFTMap = new NFTInfo[myNFT.Length];
        int currentindex=1;
        for(int i = 0; i<myNFT.Length;i++)
        {
            string charName = NameToSlugConvert(myNFT[i].name);
            Debug.Log(charName);
            GameObject charModel = Resources.Load(Path.Combine("SinglePlayerPrefabs/DisplayModels", charName)) as GameObject;
            GameObject temp= Instantiate(charModel, characterList);
            temp.transform.localEulerAngles = new Vector3(0, 180, 0);
            if (i == 0)
            {
                temp.transform.localPosition = new Vector3(0, -0.1f, 0);
                characters[0] = temp.transform;
                characterNFTMap[0] = myNFT[i];
            }
            else if (i % 2 == 0)
            {
                temp.transform.localPosition = new Vector3(-currentindex, -0.1f, 0.2f);
                characters[characters.Length - currentindex] = temp.transform;
                characterNFTMap[characters.Length - currentindex] = myNFT[i];
                currentindex++;
            }
            else if (i % 2 != 0)
            {
                temp.transform.localPosition = new Vector3(currentindex, -0.1f, 0.2f);
                characters[currentindex] = temp.transform;
                characterNFTMap[currentindex] = myNFT[i];

            }
            temp.GetComponent<Animator>().runtimeAnimatorController = controller;

        }

        /*if(myNFT.Length==0)
        {
            GameObject charModel = Resources.Load(Path.Combine("SinglePlayerPrefabs/DisplayModels", "a-rod")) as GameObject;
            GameObject temp = Instantiate(charModel, characterList);
            temp.transform.localPosition = new Vector3(0, -0.1f, 0);
            characters[0] = temp.transform;
            characterNFTMap[0] = new NFTInfo { id = 0000, name = "a-rod" };
        }*/

        Done();
       

    }

    // for skip to test all characters
    public void Skip()
    {
        var info = Resources.LoadAll("SinglePlayerPrefabs/DisplayModels",typeof (GameObject));
        characters = new Transform[info.Length];
        characterNFTMap = new NFTInfo[info.Length];
        int currentindex = 1;
        for (int i = 0; i < characters.Length; i++)
        {
            string name = info[i].name;
            GameObject charModel = Resources.Load(Path.Combine("SinglePlayerPrefabs/DisplayModels", name)) as GameObject;
            GameObject temp = Instantiate(charModel, characterList);
            temp.transform.localEulerAngles = new Vector3(0, 180, 0);
            if (i == 0)
            {
                temp.transform.localPosition = new Vector3(0, -0.1f, 0);
                characters[0] = temp.transform;
                characterNFTMap[0] = new NFTInfo { id = i, name = name};
            }
            else if (i % 2 == 0)
            {
                temp.transform.localPosition = new Vector3(-currentindex, -0.1f, 0.2f);
                characters[characters.Length - currentindex ] = temp.transform;
                characterNFTMap[characters.Length - currentindex] = new NFTInfo { id = i, name = name };
                currentindex++;
            }
            else if (i % 2 != 0)
            {
                temp.transform.localPosition = new Vector3(currentindex, -0.1f, 0.2f);
                characters[currentindex] = temp.transform;
                characterNFTMap[currentindex] = new NFTInfo { id = i, name = name };
            }
            
            temp.GetComponent<Animator>().runtimeAnimatorController = controller;
        }

        Done();
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
