using Newtonsoft.Json.Linq;
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

    private Queue<string> sentences = new Queue<string>();

    public void SetCharacter(string characterName)
    {
        nameText.text = characterName;
    }

    public void SetConversation(string dialog)
    {
        animator.SetBool("isOpen", true);

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

    public void EndDialogue()
    {
        animator.SetBool("isOpen", false);
    }
}
