using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SimpleDialogueManager : MonoBehaviour
{
    // when player move, check it's panel index and grid position with struct.
    // if has it in list, start dialogue; automatically update dialogue text after 1 sec.

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //public TextMeshProUGUI dialogueText;
    bool isDialogueActive = false;
    int currentDialogueIndex = 0;
    private DialogueDataList dialogueData;
    private DialogueDataList.DialogueData[] dialogueDatas;
    public GameObject dialogueTextPrefab;

    void Start()
    {
        // Load dialogue data from Resources folder
        dialogueData = Resources.Load<DialogueDataList>("DialogueDatas/" + SceneManager.GetActiveScene().name);
        if (dialogueData != null)
        {
            dialogueDatas = dialogueData.dialogueDatas;
            
            // Set panelObject for each dialogue entry based on designer's input
            for (int i = 0; i < dialogueDatas.Length; i++)
            {
                DialogueDataList.DialogueData data = dialogueDatas[i];
                foreach (scrPanel pn in FindObjectsByType<scrPanel>(FindObjectsSortMode.None))
                {
                    if (pn.Time_index == data.panelIndex)
                    {
                        data.panelObject = pn;
                        dialogueDatas[i] = data;
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.LogError($"Could not find dialogue data for scene: {SceneManager.GetActiveScene().name}");
            dialogueDatas = new DialogueDataList.DialogueData[0];
        }
        currentDialogueIndex = 0;
    }

    public void StartDialogue(GameObject panelObject, scrPlayer player)
    {
        if (isDialogueActive) return;
        foreach (DialogueDataList.DialogueData data in dialogueDatas)
        {
            if (data.panelObject != null && 
                data.panelObject.gameObject == panelObject && 
                data.gridPosition == player.GetComponent<GridObject>().gridPosition)
            {
                isDialogueActive = true;
                currentDialogueIndex = 0;
                GameObject dialogueText = Instantiate(dialogueTextPrefab);
                dialogueText.transform.SetParent(GameObject.Find("Canvas").transform);
                dialogueText.transform.position = Camera.main.WorldToScreenPoint(player.transform.position) + new Vector3(1, 1, 0);
                StartCoroutine(UpdateDialogueText(data, player, dialogueText));
                break;
            }
        }
    }

    IEnumerator UpdateDialogueText(DialogueDataList.DialogueData data, scrPlayer player, GameObject dialogueText)
    {
        string currentText = data.dialogueText[currentDialogueIndex];
        TextMeshProUGUI textComponent = dialogueText.GetComponent<TextMeshProUGUI>();
        textComponent.text = ""; // Clear the text initially
        
        // Type out the text character by character
        foreach (char c in currentText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(1f);
        currentDialogueIndex++;
        if (currentDialogueIndex < data.dialogueText.Length)
        {
            StartCoroutine(UpdateDialogueText(data, player, dialogueText));
        }
        else
        {
            EndDialogue(dialogueText);
        }
    }

    void EndDialogue(GameObject dialogueText)
    {
        isDialogueActive = false;
        currentDialogueIndex = 0;
        Destroy(dialogueText);
    }
}
