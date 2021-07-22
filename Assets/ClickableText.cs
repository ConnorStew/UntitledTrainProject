using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class is used to allow certain text to be clickable. For example links in dialog.
/// </summary>
public class ClickableText : MonoBehaviour, IPointerClickHandler
{

    /// <summary> The current game manager. </summary>
    public GameManager gameManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();

        //continue if we get a left click
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //get the index of the word clicked
            int wordIndex = TMP_TextUtilities.FindIntersectingWord(text, Input.mousePosition, null);
            if (wordIndex != -1)
            {
                //get the word and notify the game manager.
                string lastClickedWord = text.textInfo.wordInfo[wordIndex].GetWord();
                gameManager.WordClicked(lastClickedWord);
            }
        }
    }
}
