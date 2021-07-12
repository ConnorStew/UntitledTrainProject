using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public TextMeshProUGUI dialogueText;
    public Animator animator;
    public float textSpeed;

    private Queue<string> sentences;
    private JObject conversations;
    private JToken conversation;
    private JArray links;

    void Start()
    {
        sentences = new Queue<string>();
        conversations = JObject.Parse(File.ReadAllText("Assets/Conversations.json"));
        StartDialog("opening");
    }

    internal void WordClicked(string lastClickedWord)
    {
        // Debug.Log($"Clicked: {lastClickedWord}, have {links.Count} links.");
        foreach (JToken link in links.Children())
        {
            string word = (string)link["word"];
            string nextConversation = (string)link["goto"];

            if (word.Equals(lastClickedWord))
            {
                Debug.Log($"Found link: {word}, going to {nextConversation}");
                StartDialog(nextConversation);
            }
        }
    }

    public void StartDialog(string dialogueName)
    {
        animator.SetBool("isOpen", true);

        conversation = conversations.SelectToken($"$.conversations[?(@.name == '{dialogueName}')]");
        string conversationName = (string)conversation["name"];
        string characterName = (string)conversation["character"];
        string dialog = (string)conversation["dialog"];
        links = (JArray)conversation["links"];

        nameText.text = characterName;

        sentences.Clear();

        sentences.Enqueue(dialog);

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void EndDialogue()
    {
        animator.SetBool("isOpen", false);
        Debug.Log("End of convo.");
    }
}
