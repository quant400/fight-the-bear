using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveRoar : MonoBehaviour
{
    BearSFXController bsfx;
    private void OnEnable()
    {
        bsfx = FightModel.currentBear.GetComponent<BearSFXController>();
    }
    public void PlayWaveRoar()
    {
        bsfx.PlayRoar();
        Invoke("StopSfx", 2f);
    }

    void StopSfx()
    {
        bsfx.PauseSfx();
    }
}
