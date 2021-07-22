using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// This class manages dialog on the end screen.
/// </summary>
public class EndingDialog : MonoBehaviour
{
    /// <summary> The speed to display text at. </summary>
    public float textSpeed;

    /// <summary> The text object used to display the ending. </summary>
    public TextMeshProUGUI dialogueText;

    /// <summary>
    /// Clears previous sentances and displays a new one.
    /// </summary>
    /// <param name="sentence">The sentence to display.</param>
    public void DisplaySentence(string sentence)
    {
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    /// <summary>
    /// Types a sentance out with delay between characters.
    /// </summary>
    /// <param name="sentence">The sentance to type.</param>
    /// <returns>Internal unity wait time.</returns>
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
