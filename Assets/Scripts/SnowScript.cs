using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowScript : MonoBehaviour
{
    Transform player;
    void Start()
    {
        Invoke("GetPlayer",0.5f);
    }
    void GetPlayer()
    {
        if (FightModel.currentPlayer != null) 
        {
            player = MapView.instance.GetPlayer().transform;
        }
        else
        {
            Invoke("GetPlayer", 2f);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(player!=null)
            transform.position = new Vector3(player.position.x, 7f, player.position.z);
    }
}
