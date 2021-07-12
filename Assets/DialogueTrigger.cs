using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogueTrigger : MonoBehaviour
{

    public string startingDialog;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialog("startingDialog");
    }

}
