using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableText : MonoBehaviour, IPointerClickHandler
{

    public GameManager gameManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            int wordIndex = TMP_TextUtilities.FindIntersectingWord(text, Input.mousePosition, null);
            if (wordIndex != -1)
            {
                string lastClickedWord = text.textInfo.wordInfo[wordIndex].GetWord();
                gameManager.WordClicked(lastClickedWord);
            }
        }
    }
}
