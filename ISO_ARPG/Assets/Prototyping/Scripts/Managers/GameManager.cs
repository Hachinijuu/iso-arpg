using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get { return instance; } }

    private static GameManager instance;

    public static PlayerController controller;
    public enum GameState { MENU, PLAYING, PAUSE }
    public GameState currGameState;

    public enum ControlType {  MOUSE_KEYBOARD, CONTROLLER }
    public ControlType controls;

    public PlayerMovement.MoveInput moveType;

    public delegate void moveChanged(PlayerMovement.MoveInput value);
    public event moveChanged onMoveChanged;

    void FireMoveChanged (PlayerMovement.MoveInput value) { if (onMoveChanged != null) onMoveChanged(value); }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (controller == null)
        { 
            controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        if (controller != null)
        { 
            moveType = controller.Movement.moveType;
        }
    }
}
