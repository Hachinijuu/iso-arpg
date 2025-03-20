using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public Slider difficultySlider; // might be a segmented button

    public Toggle mouseKeyboard;
    public Toggle controller;

    public Toggle directional;
    public Toggle clickToMove;

    public void Awake()
    {
        difficultySlider.value = (float)GoX_Difficulties.GOD;
        SwitchMovement();
    }
    public void SwitchMovement()
    {
        if (directional.isOn)
        {
            PlayerManager.Instance.moveType = MoveInput.DIRECTIONAL;
        }
        if (clickToMove.isOn)
        {
            PlayerManager.Instance.moveType = MoveInput.CLICK;
        }
        // This will probably be a dropdown / button
        // Determines whether the player should move based on WASD (directional) or click to move (left click / right click driven)
        // Might force certain inputs based on controllers / disable inputs
        // If controller, don't handle click to move, instead drive all movement via joystick
    }

    public void SwitchInput()
    {
        // THIS WILL SWITCH BETWEEN CONTROL SCHEMES, IT IS EASY TO SETUP

        
        //if (inputType == "mouseKeyboard")
        //{
        //    // Get the input map and switch it to mouse keyboard controls
        //}
        //else if (inputType == "controller")
        //{
        //    // Get the input map and switch it to controller controls
        //}
    }
    public void UpdateDifficulty(float value)
    {
        // Map this to the GameManager current difficulty
        // Change the GameManager's difficulty values based on the difficulty set here

        // Difficulties can be ScriptableObjects for easy editing
        // The base difficulty class can contain health scaling, damage scaling, enemy quantity scaling
        GameManager.Instance.currDifficulty = GameManager.Instance.difficulties[(int)value];
        GameManager.Instance.FireDifficultyChanged();
    }
}
