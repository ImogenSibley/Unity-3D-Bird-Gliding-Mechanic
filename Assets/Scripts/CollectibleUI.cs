using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectibleUI : MonoBehaviour
{
    public static CollectibleUI Instance;
    public TextMeshProUGUI gemValueText;
    public TextMeshProUGUI percentageText;
    public Image progressBar;
    private int gemCount = 0;
    private int gemValue = 0;
    private int totalGems; //total number of gems

    public AudioClip gemSound; //stores audio clip
    public AudioSource audioSource; //stores audio source

    private void Awake()
    {
        if (Instance == null) //set this to the UI manager
        {
            Instance = this;
        }
        else //ensure only one instance of collectible UI manager
        {
            Destroy(gameObject);
        }
        totalGems = GameObject.FindGameObjectsWithTag("Gem").Length; //finds total number of gems in the game and sets variable

        if (progressBar != null) 
        {
            progressBar.type = Image.Type.Filled; //set progress bar to "filled" type
            progressBar.fillMethod = Image.FillMethod.Horizontal; //bar will fill up horizontally
        }
    }

    public void AddGemValue(int value)
    {
        gemValue += value; //adds gem value to score variable
        gemCount++; //adds gem count for percentage calculations
        if (audioSource != null && gemSound != null)
        {
            audioSource.PlayOneShot(gemSound); //play gem sound if one exists
        }
        else
        {
            Debug.LogError("No AudioSource found on this GameObject."); //else print error log
        }
        UpdateGemCountUI();
    }

    private void UpdateGemCountUI()
    {
        gemValueText.text = "Gems Value: " + gemValue; //text displayed on in game UI
        float percentage = (float)gemCount / totalGems * 100; //calculate percentage of gems collected
        percentageText.text = $"{Mathf.RoundToInt(percentage)}%"; //update percentage UI text, rounded to the nearest integer
        progressBar.fillAmount = percentage / 100; //update progress bar
        
        if (progressBar != null) //update progress bar fill amount based on percentage
        {
            progressBar.fillAmount = percentage / 100f;
        }
        
        if (gemCount >= totalGems) //if all gems are collected
        {
            DisplayCompletion(); //show completion message
        }
    }

    private void DisplayCompletion()
    {
        percentageText.text = "100% Level Complete!"; //completion message
    }
}
