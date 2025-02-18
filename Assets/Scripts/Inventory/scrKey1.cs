using UnityEngine;

public class scrKey1 : MonoBehaviour
{

    [Header("Key Icon Sprite")] public Sprite keyIconSprite;
    
    private GameObject keyIconObject;
    
    void Start()
    {
       Debug.Log("hey key 1 got instantiated"); 
       
       // 1. Create a child object to hold the sprite
       keyIconObject = new GameObject("prefKeyIcon");
       keyIconObject.transform.SetParent(transform, worldPositionStays: false);
       
       // 2. Position the icon a bit above the parent
       keyIconObject.transform.localPosition = new Vector3(0f, 1f, 0f);
       
       // 3. Add a SpriteRenderer for the icon and set the sprite
       SpriteRenderer sr = keyIconObject.AddComponent<SpriteRenderer>();
       sr.sprite = keyIconSprite;
    }
    
   
    private void Update()
    {
        
    }
    
    private void OnDestroy()
    {
        // 4. Clean up the icon when this script is removed/destroyed
        if (keyIconObject != null)
        {
            Destroy(keyIconObject);
        }
    }
}
