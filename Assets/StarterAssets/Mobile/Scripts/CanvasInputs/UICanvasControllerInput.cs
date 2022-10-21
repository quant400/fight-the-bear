using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public StarterAssetsInputs starterAssetsInputs;
        /*private void Start()
        {
            Invoke("GetRefrence", 1f);
        }*/

        FighterView fv;
        RockThrowView rtv;
        public void GetRefrence(GameObject player)
        {
            fv = player.GetComponent<FighterView>();
            rtv = player.GetComponent<RockThrowView>();
            starterAssetsInputs = player.GetComponent<StarterAssetsInputs>();
        }

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            starterAssetsInputs.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            starterAssetsInputs.JumpInput(virtualJumpState);
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            starterAssetsInputs.SprintInput(virtualSprintState);
        }

        //specific to fight the bear 
        public void VirtualAttackInput()
        {
            Debug.Log(2);
            fv.Punch();
        }

        public void VirtualBlockInput(bool virtualSprintState)
        {
            if (virtualSprintState)
                VirtualBlockInputS();
            else
            {
                VirtualBlockInputE();
            }
        }
        public void VirtualBlockInputS()
        {
            fv.BlockStart();
        }
        public void VirtualBlockInputE()
        {
            fv.BlockEnd();
        }

        public void VirtualPickInput()
        {
            Debug.Log(1);
            rtv.PickUpRock();
        }
        public void VirtualswapInput(bool virtualSprintState)
        {
            //starterAssetsInputs.SprintInput(virtualSprintState);
        }

    }

}
