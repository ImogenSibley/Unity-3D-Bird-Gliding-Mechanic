using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public GemData gemData; //reference to scriptable object containing gem data
    
    private void OnTriggerEnter(Collider other) //method is called when another object collides with other object
    {
        if (other.CompareTag("Player"))
        {
            CollectibleUI.Instance.AddGemValue(gemData.value);
            Destroy(gameObject);
        }
    }
}
