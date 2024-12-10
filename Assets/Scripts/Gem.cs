using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public GemData gemData; //reference to scriptable object containing gem data
    private AudioSource audioSource;

    //void Start()
    //{
    //    // Get the AudioSource component attached to the GameObject
    //    audioSource = GetComponent<AudioSource>();
    //}

    private void OnTriggerEnter(Collider other) //method is called when another object collides with other object
    {
        if (other.CompareTag("Player")) //if player collides with gem
        {
            CollectibleUI.Instance.AddGemValue(gemData.value); //add gem value to collectible UI
            Destroy(gameObject); //delete gem
        }
    }
}
