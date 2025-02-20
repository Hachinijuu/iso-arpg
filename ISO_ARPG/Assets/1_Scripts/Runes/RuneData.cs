using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RuneMathType { ADD, MULTIPLY }

public class RuneData : ItemData
{
    public virtual void ApplyStats(ref PlayerStats stats) { }
}
