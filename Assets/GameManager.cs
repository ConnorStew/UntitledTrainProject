using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Camera cam;
    public DialogueManager dialogueManager;

    public GameObject dialogPanel;
    public GameObject buttonPrefab;

    public GameObject continueButton;
    public GameObject dialogText;
    public GameObject nameText;

    private Character soldier, alien, hero;

    private Character currentCharacter;

    private Queue<GameObject> buttons;

    void Start()
    {
        soldier = new Character(dialogueManager, this, "Soldier");
        //alien = new Character(dialogueManager, "Alien");
        hero = new Character(dialogueManager, this, "Hero");

        buttons = new Queue<GameObject>();

        buttonPrefab.SetActive(false);

        ChangeCabin("Soldier");
    }
    
    public void ChangeCabin(string character)
    {
        switch (character)
        {
            case "Soldier":
                cam.transform.position = new Vector3(-2000, 50, -100);
                SetCharacter(soldier);
                break;
            case "Alien":
                break;
            case "Hero":
                cam.transform.position = new Vector3(-3000, 70, -200);
                SetCharacter(hero);
                break;
            default:
                break;
        }

    }

    private void SetCharacter(Character character)
    {
        currentCharacter = character;
        character.DisplayConversation();
    }

    public void ContinueConversation()
    {
        currentCharacter.ContinueConversation();
    }

    internal void WordClicked(string lastClickedWord)
    {
        currentCharacter.WordClicked(lastClickedWord);
    }

    internal void SwitchToQuestionUI(JToken choices)
    {
        //hide the other dialog ui components
        dialogText.SetActive(false);
        nameText.SetActive(false);
        continueButton.SetActive(false);

        //destroy any previous buttons.
        while (buttons.Count > 0)
        {
            GameObject button = buttons.Dequeue();
            Destroy(button);
        }

        RectTransform rt = (RectTransform)dialogPanel.transform;

        float x = dialogPanel.transform.position.x;
        float y = dialogPanel.transform.position.y + rt.rect.height / 8;

        int index = 0;
        foreach (JToken choice in choices.Children())
        {
            string choiceText = choice.Value<string>("choice");

            GameObject button = (GameObject)Instantiate(buttonPrefab);
            button.transform.SetParent(dialogPanel.transform);

            button.transform.position = new Vector3(x, y);
            button.GetComponent<Button>().onClick.AddListener(delegate { currentCharacter.ChoiceClicked(choiceText); } );
            button.transform.GetChild(0).GetComponent<Text>().text = choiceText;
            button.SetActive(true);

            buttons.Enqueue(button);

            Debug.Log(button.transform.position.x);
            Debug.Log(button.transform.position.y);

            y -= 40;
            index++;
        }
    }

    internal void SwitchToDialogUI()
    {
        //destroy any previous buttons.
        while (buttons.Count > 0)
        {
            GameObject button = buttons.Dequeue();
            Destroy(button);
        }

        //show dialog UI
        dialogText.SetActive(true);
        nameText.SetActive(true);
        continueButton.SetActive(true);
    }
}
