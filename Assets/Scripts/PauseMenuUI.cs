using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject pauseMenuUI; //UI objects
    public GameObject controlsMenuUI;
    public GameObject optionsMenuUI;
    public Player playerScript; //reference to player script

    public TextMeshProUGUI resumeText;
    public TextMeshProUGUI levelSelectText;
    public TextMeshProUGUI optionsText;
    public TextMeshProUGUI controlsText;
    public TextMeshProUGUI quitText;
    public TextMeshProUGUI controlsBackText;
    public TextMeshProUGUI optionsBackText;

    public Slider moveSpeedSlider; //sliders for adjusting player settings in options
    public Slider sprintModifierSlider;
    public Slider jumpForceSlider;
    public Slider dropForceSlider;

    private bool isPaused = false; //initial state of the game is unpaused

    void Start()
    {
        pauseMenuUI.SetActive(false); //makes sure pause menu is hidden on start
        controlsMenuUI.SetActive(false); //makes sure control menu is hidden on start
        optionsMenuUI.SetActive(false); //makes options hidden on start

        if (playerScript != null) //if class can find player script on start, set the values as set in the player script
        {
            moveSpeedSlider.value = playerScript.moveSpeed;
            sprintModifierSlider.value = playerScript.sprintModifier;
            jumpForceSlider.value = playerScript.jumpForce;
            dropForceSlider.value = playerScript.diveForce;
        }

        moveSpeedSlider.onValueChanged.AddListener(UpdateMoveSpeed); //add listeners for slider value changes
        sprintModifierSlider.onValueChanged.AddListener(UpdateSprintModifier);
        jumpForceSlider.onValueChanged.AddListener(UpdateJumpForce);
        dropForceSlider.onValueChanged.AddListener(UpdateDropForce);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //if player presses escape, pause the game
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        if (isPaused)
        {
            if (Input.GetMouseButtonDown(0)) //get players mouse input
            {
                Vector3 mousePosition = Input.mousePosition; //get players mouse position
                if (RectTransformUtility.RectangleContainsScreenPoint(resumeText.rectTransform, mousePosition)) //interact with pause menu with mouse
                {
                    ResumeGame();
                }
                else if (RectTransformUtility.RectangleContainsScreenPoint(levelSelectText.rectTransform, mousePosition))
                {
                    LevelSelect();
                }
                else if (RectTransformUtility.RectangleContainsScreenPoint(optionsText.rectTransform, mousePosition))
                {
                    OpenOptionsMenu();
                }
                else if (RectTransformUtility.RectangleContainsScreenPoint(controlsText.rectTransform, mousePosition))
                {
                    OpenControlsMenu();
                }
                else if (RectTransformUtility.RectangleContainsScreenPoint(quitText.rectTransform, mousePosition))
                {
                    QuitGame();
                }
            }
        }
        if (controlsMenuUI.activeSelf && Input.GetKeyDown(KeyCode.Escape)) //logic to close controls menu if escape is pressed
        {
            CloseControlsMenu();
        }
        else if (controlsMenuUI.activeSelf && Input.GetMouseButtonDown(0)) //logic to close controls menu if mouse clicks on back button
        {
            Vector3 mousePosition = Input.mousePosition;
            if (RectTransformUtility.RectangleContainsScreenPoint(controlsBackText.rectTransform, mousePosition))
            {
                CloseControlsMenu();
            }
        }
        
        if (optionsMenuUI.activeSelf && Input.GetKeyDown(KeyCode.Escape)) //logic to close options menu if escape is pressed
        {
            CloseOptionsMenu();
        }
        else if (optionsMenuUI.activeSelf && Input.GetMouseButtonDown(0)) //logic to close options menu if mouse clicks on back button
        {
            Vector3 mousePosition = Input.mousePosition;
            if (RectTransformUtility.RectangleContainsScreenPoint(optionsBackText.rectTransform, mousePosition))
            {
                CloseOptionsMenu();
            }
        }

    }
    
    void PauseGame() //pause game and show menu
    {
        pauseMenuUI.SetActive(true); //show pause menu UI
        Time.timeScale = 0f; //freeze the game
        isPaused = true; //set gamemode bool to is paused
        if (playerScript != null)
        {
            playerScript.enabled = false; //disable player movement mouse rotation
        }
    }
    
    void ResumeGame()
    {
        pauseMenuUI.SetActive(false); //set pause menu UI inactive
        Time.timeScale = 1f; //unfreeze the game
        isPaused = false; //set gamemode bool to not paused
        if (playerScript != null)
        {
            playerScript.enabled = true; //enable player movement mouse rotation
        }
    }
    
    void LevelSelect() //load level select scene
    {
        Debug.Log("Level Select Pressed.");
        //Time.timeScale = 1f;
        //SceneManager.LoadScene("LevelSelect");
    }

    void OpenOptionsMenu() //add logic for adjusting speed, jumpforce, gravity ect. 
    {
        Debug.Log("Options Pressed.");
        pauseMenuUI.SetActive(false); //set pause menu UI inactive
        optionsMenuUI.SetActive(true); //set control menu UI active
        Time.timeScale = 0f; //ensure game is frozen while navigating menu UIs
        if (playerScript != null)
        {
            playerScript.enabled = false; //disable player movement mouse rotation
        }
    }

    void CloseOptionsMenu()
    {
        optionsMenuUI.SetActive(false); // Hide the controls menu
        pauseMenuUI.SetActive(true);     // Show the pause menu
        Time.timeScale = 0f; //ensure game is frozen while navigating menu UIs
        if (playerScript != null)
        {
            playerScript.enabled = false; //disable player movement mouse rotation
        }
    }

    void OpenControlsMenu() //add a UI screen to show the controls, WASD and arrow keys to move, mouse to rotate, shift to sprint, Q to dive, space to jump, space x2 to glide.
    {
        Debug.Log("Controls Pressed.");
        pauseMenuUI.SetActive(false); //set pause menu UI inactive
        controlsMenuUI.SetActive(true); //set control menu UI active
        Time.timeScale = 0f; //ensure game is frozen while navigating menu UIs
        if (playerScript != null)
        {
            playerScript.enabled = false; //disable player movement mouse rotation
        }
    }

    void CloseControlsMenu()
    {
        controlsMenuUI.SetActive(false); // Hide the controls menu
        pauseMenuUI.SetActive(true);     // Show the pause menu
        Time.timeScale = 0f; //ensure game is frozen while navigating menu UIs
        if (playerScript != null)
        {
            playerScript.enabled = false; //disable player movement mouse rotation
        }
    }

    void QuitGame()
    {
        Debug.Log("Quitting game..."); 
        Application.Quit(); //only works in built game not unity editor
    }

    void UpdateMoveSpeed(float value)
    {
        if (playerScript != null) //if player script is found
        {
            Time.timeScale = 0f; //ensure game stays frozen while navigating UI
            playerScript.moveSpeed = value; //set move speed to input value
            playerScript.enabled = false; //disable player movement mouse rotation
        }
    }

    void UpdateSprintModifier(float value)
    {
        if (playerScript != null) //if player script is found
        {
            Time.timeScale = 0f; //ensure game stays frozen while navigating UI
            playerScript.sprintModifier = value; //set to input value
            playerScript.enabled = false; //disable player movement mouse rotation
        }
    }

    void UpdateJumpForce(float value)
    {
        if (playerScript != null) //if player script is found
        {
            Time.timeScale = 0f; //ensure game stays frozen while navigating UI
            playerScript.jumpForce = value; //set to input value
            playerScript.enabled = false; //disable player movement mouse rotation
        }
    }

    void UpdateDropForce(float value)
    {
        if (playerScript != null) //if player script is found
        {
            Time.timeScale = 0f; //ensure game stays frozen while navigating UI
            playerScript.diveForce = value; //set to input value
            playerScript.enabled = false; //disable player movement mouse rotation
        }
    }
}
