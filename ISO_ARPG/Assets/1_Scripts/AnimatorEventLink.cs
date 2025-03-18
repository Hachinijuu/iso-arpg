using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventLink : MonoBehaviour
{
    // This class will pass along events from animation events
    public delegate void onStepped();
    public onStepped Stepped;
    public void FireStepped() { if (Stepped != null) Stepped(); }
}
