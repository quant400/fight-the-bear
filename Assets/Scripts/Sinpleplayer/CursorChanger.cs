using UnityEngine;
using System.Collections;

public class CursorChanger : MonoBehaviour
{
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    public void Enter()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    public void Exit()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}