using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "DialogueDatas/Dialogue Data")]
public class DialogueDataList : ScriptableObject
{
    [System.Serializable]
    public struct DialogueData
    {
        public int panelIndex;
        public scrPanel panelObject;
        public Vector2Int gridPosition;
        public string[] dialogueText;
    }

    public DialogueData[] dialogueDatas;
}
