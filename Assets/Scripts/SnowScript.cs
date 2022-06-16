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
        player = MapView.instance.GetPlayer();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(player!=null)
            transform.position = new Vector3(player.position.x, 7f, player.position.z);
    }
}
