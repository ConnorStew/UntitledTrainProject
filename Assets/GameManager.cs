using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary> This class is responsible for managing the backend of the game. </summary>
public class GameManager : MonoBehaviour
{
    /// <summary> The game's main camera. </summary>
    public Camera cam;

    /// <summary> The game's dialog manager. </summary>
    public DialogueManager dialogueManager;

    /// <summary> The game's sound manager. </summary>
    public SoundManager soundManager;

    /// <summary> The panel that displays the game's dialog, used as a parent for buttons. </summary>
    public GameObject buttonContainer;

    /// <summary> The prefab used to generate the decision buttons. </summary>
    public GameObject buttonPrefab;

    /// <summary> The button used to step through the dialog. </summary>
    public GameObject continueButton;

    /// <summary> The text used to display dialog to the player. </summary>
    public GameObject dialogText;

    /// <summary> The text used to display the speakers name. </summary>
    public GameObject nameText;

    /// <summary> Objects used to represent our characters. </summary>
    private Character soldier, alien, hunter;

    /// <summary> The character currently being displayed to the user. </summary>
    public Character currentCharacter;

    /// <summary> The buttons that have been instantiated. </summary>
    private Queue<GameObject> buttons;

    /// <summary> The animator that controls fading. </summary>
    public Animator fadeAnimator;

    /// <summary> Whether its the first fade this game. </summary>
    private bool firstChange;

    /// <summary> The current character's name. </summary>
    internal string characterName { 
        get { return currentCharacter.name; } 
    }

    void Start()
    {
        soldier = new Character(dialogueManager, this, soundManager, "Soldier");
        alien = new Character(dialogueManager, this, soundManager, "Alien");
        hunter = new Character(dialogueManager, this, soundManager, "Hunter");

        buttons = new Queue<GameObject>();

        soundManager.PlaySound("Train_noise_inside");

        buttonPrefab.SetActive(false);
        currentCharacter = soldier;
        firstChange = true;
        StartCoroutine(ChangeCabin("Soldier"));
    }

    /// <summary>
    /// Used to advance the dialog and play the relevent sound.
    /// </summary>
    public void ContinueButtonClicked()
    {
        soundManager.PlaySound("mouse_click");
        ContinueConversation();
    }

    /// <summary>
    /// Used to end the game.
    /// </summary>
    public void CoinClicked()
    {
        PlayerPrefs.SetString("character", characterName);
        PlayerPrefs.SetInt("points", currentCharacter.readDialogs.Count);
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// Used to change the character/cabin and play the relevent sound.
    /// </summary>
    /// <param name="character">The character your switching to.</param>
    public void ChangeCabinButtonClicked(string character)
    {
        soundManager.PlaySound("mouse_click");

        StartCoroutine(ChangeCabin(character));
    }

    /// <summary>
    /// Stops all themes playing and plays the new theme.
    /// </summary>
    /// <param name="sound">The theme to play, should be one of: soldier_theme, hero_theme, alien_theme.</param>
    private void CharacterSound(string sound)
    {
        soundManager.StopSound("soldier_theme");
        soundManager.StopSound("hero_theme");
        soundManager.StopSound("alien_theme");

        soundManager.PlaySound(sound);
    }

    /// <summary>
    /// Switches the cabin (by changing the camera position) and plays the relevent sound.
    /// </summary>
    /// <param name="character">The character to switch to.</param>
    private IEnumerator ChangeCabin(string character)
    {
        soundManager.StopSound("hunter_voice");
        soundManager.StopSound("alien_voice");
        soundManager.StopSound("soldier_voice");

        if (firstChange == false)
        {
            Debug.Log("Fading out");
            fadeAnimator.SetBool("visible", false);
        }

        firstChange = false;

        yield return new WaitForSeconds(1);

        fadeAnimator.SetBool("visible", true);
        Debug.Log("Fading in");

        switch (character)
        {
            case "Soldier":
                cam.transform.position = new Vector3(141, 94.3f, -109.9f);

                
                if (currentCharacter.Equals(alien))
                    soundManager.PlaySound("alien_to_soldier");

                if (currentCharacter.Equals(hunter))
                    soundManager.PlaySound("hunter_to_soldier");

                CharacterSound("soldier_theme");
                SetCharacter(soldier);
                break;
            case "Alien":
                cam.transform.position = new Vector3(2060.46f, 94.3f, -109.9f);

                
                if (currentCharacter.Equals(soldier))
                    soundManager.PlaySound("soldier_to_alien");

                if (currentCharacter.Equals(hunter))
                    soundManager.PlaySound("hunter_to_alien");

                CharacterSound("alien_theme");
                SetCharacter(alien);
                break;
            case "Hunter":
                cam.transform.position = new Vector3(985.96f, 94.3f, -109.9f);

                
                if (currentCharacter.Equals(soldier))
                    soundManager.PlaySound("soldier_to_hunter");

                if (currentCharacter.Equals(alien))
                    soundManager.PlaySound("alien_to_hunter");

                CharacterSound("hero_theme");
                SetCharacter(hunter);
                break;
            default:
                break;
        }

        yield return null;
    }

    /// <summary>
    /// Sets the current character and begins showing their conversation.
    /// </summary>
    /// <param name="character">The character to switch to.</param>
    private void SetCharacter(Character character)
    {
        currentCharacter = character;
        character.DisplayConversation();
    }

    /// <summary>
    /// Continues the conversation of the current character.
    /// </summary>
    public void ContinueConversation()
    {
        currentCharacter.ContinueConversation();
    }

    /// <summary>
    /// Calls "WordClicked" on the current character.
    /// </summary>
    /// <param name="clickedWord">The word that was clicked</param>
    internal void WordClicked(string clickedWord)
    {
        currentCharacter.WordClicked(clickedWord);
    }

    /// <summary>
    /// Switches the UI to show buttons instead of dialog.
    /// </summary>
    /// <param name="choices">The choices to display to the player.</param>
    internal void SwitchToQuestionUI(JToken choices)
    {
        //hide the other dialog ui components
        dialogText.SetActive(false);
        continueButton.SetActive(false);

        DestoryButtons();

        //We're going to place the buttons relative to the dialog panel, this'll give us the coordiantes we need.
        RectTransform buttonContainerTransform = (RectTransform)buttonContainer.transform;

        int buttonCount = 0;
        foreach (JToken choice in choices.Children())
        {
            string choiceText = choice.Value<string>("choice");

            GameObject button = Instantiate(buttonPrefab);

            RectTransform buttonTransform = (RectTransform)button.transform;

            // add an event for when the button is clicked.
            button.GetComponent<Button>().onClick.AddListener(delegate { currentCharacter.ChoiceClicked(choiceText); });

            // set the buttons text to it's assigned choice.
            TMP_Text text = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            text.text = choiceText;
            text.fontStyle = FontStyles.Bold;

            // set the dialog panel as the parent.
            button.transform.SetParent(buttonContainer.transform);
            
            // make the button active
            button.SetActive(true);

            // set the transform to the correct position
            button.transform.localScale = buttonContainerTransform.localScale;
            buttonTransform.sizeDelta = new Vector2(buttonContainerTransform.sizeDelta.x, buttonContainerTransform.sizeDelta.y / 4);

            float spacing = (buttonCount * (buttonTransform.sizeDelta.y));
            float y = buttonContainerTransform.sizeDelta.y / 2 - buttonTransform.sizeDelta.y / 2 - spacing;
            button.transform.localPosition = new Vector3(0, y, 0);

            // add the button to the queue so it can be destroyed later
            buttons.Enqueue(button);

            buttonCount++;
        }
    }

    /// <summary> Switches to the dialog UI. </summary>
    internal void SwitchToDialogUI()
    {
        DestoryButtons();

        //show dialog UI
        dialogText.SetActive(true);
        continueButton.SetActive(true);
    }

    /// <summary>
    /// Destroys any previously created buttons.
    /// </summary>
    private void DestoryButtons()
    {
        while (buttons.Count > 0)
        {
            GameObject button = buttons.Dequeue();
            Destroy(button);
        }
    }
}
