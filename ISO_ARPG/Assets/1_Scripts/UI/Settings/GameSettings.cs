using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public Slider difficultySlider; // might be a segmented button

    public Button mouseKeyboard;
    public Button controller;

    public void SwitchMovement()
    {
        // This will probably be a dropdown / button
        // Determines whether the player should move based on WASD (directional) or click to move (left click / right click driven)
        // Might force certain inputs based on controllers / disable inputs
        // If controller, don't handle click to move, instead drive all movement via joystick
    }

    public void SwitchInput(String inputType)
    {
        if (inputType == "mouseKeyboard")
        {
            // Get the input map and switch it to mouse keyboard controls
        }
        else if (inputType == "controller")
        {
            // Get the input map and switch it to controller controls
        }
    }
    public void UpdateDifficulty(int value)
    {
        // Map this to the GameManager current difficulty
        // Change the GameManager's difficulty values based on the difficulty set here

        // Difficulties can be ScriptableObjects for easy editing
        // The base difficulty class can contain health scaling, damage scaling, enemy quantity scaling
    }
}
