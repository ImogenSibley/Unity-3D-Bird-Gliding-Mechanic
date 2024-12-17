using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public GemData gemData; //reference to scriptable object containing gem data
    private AudioSource audioSource;

    private void OnTriggerEnter(Collider other) //method is called when another object collides with other object
    {
        if (other.CompareTag("Player")) //if player collides with gem
        {
            CollectibleUI.Instance.AddGemValue(gemData.value); //add gem value to collectible UI
            Destroy(gameObject); //delete gem
        }
    }
}
