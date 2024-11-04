using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gem", menuName = "Collectibles/Gem")]
public class GemData : ScriptableObject
{
    public string gemName; //gems will be names by colour
    public int value; //gem values will be set depending on colour
}
