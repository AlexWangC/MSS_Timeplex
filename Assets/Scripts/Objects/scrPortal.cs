using System;
using TMPro;
using UnityEngine;

public class scrPortal : MonoBehaviour
{
    public int remainingUses;
    public GameObject correspondingPortal;

    // Text that is centered above
    public GameObject textUIPrefab;
    public Vector3 textUIOffset;
    private RectTransform _textRectTransform;
    [HideInInspector] public GameObject textUIInstance;

    private void Start()
    {
        // Set up Text Indication
        textUIInstance = Instantiate(textUIPrefab, FindAnyObjectByType<Canvas>().transform);
        _textRectTransform = textUIInstance.GetComponent<TextMeshProUGUI>().GetComponent<RectTransform>();
        textUIInstance.GetComponent<TextMeshProUGUI>().text = remainingUses.ToString();
        
    }

    private void Update()
    {
        // update text position each frame.
        Vector3 text_pos = Camera.main.WorldToScreenPoint(transform.position + textUIOffset);
        _textRectTransform.position = text_pos;
        textUIInstance.GetComponent<TextMeshProUGUI>().text = remainingUses.ToString();

        if (transform.parent.GetComponent<scrPanel>().Dead || remainingUses <= 0)
        {
            SelfDestruct();
        }
        
    }
    
    // if portal is used up destroy itself
    public void SelfDestruct()
    {
        if (correspondingPortal != null)
        {
            Destroy(correspondingPortal.GetComponent<scrPortal>().textUIInstance);
            Destroy(correspondingPortal);
        }
        Destroy(textUIInstance);
        Destroy(gameObject);
    }
}
