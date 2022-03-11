using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "melihbahri/IdleArcade/Data/ArcadeData", order = 1)]
public class ArcadeData : ScriptableObject
{
    public IntVariable[] CurrentResources;
    // public int CurrentCoin;
    public int ResourceToCoinMultiply;
}
