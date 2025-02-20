using System;
using System.Collections.Generic;
using UnityEngine;

public class scrInventory : MonoBehaviour
{
    public Dictionary<string, bool> inventory;
    public Sprite key1Sprite;
    public Sprite key2Sprite;
    public Sprite key3Sprite;
    
    public GameObject keyPickUp1;
    public GameObject keyPickUp2;
    public GameObject keyPickUp3;
    
    private void Start()
    {
        inventory = new Dictionary<string, bool>();
        inventory.Add("key1", false);
        inventory.Add("key2", false);
        inventory.Add("key3", false);
    }

    private void Update()
    {
        
    }

    public void addToInventory(string _what_to_add)
    {
        inventoryDropEverything();

        if (!inventory.ContainsKey(_what_to_add))
        {
            throw new ArgumentException(_what_to_add + " is not a pickup-able object");
        }
        
        // then add this to inventory. Should be the only thing tuned to true.
        inventory[_what_to_add] = true;
        switch (_what_to_add)
        {
            case "key1":
                Debug.Log("what's to be added to inventory: key1");
                scrKey1 newKey1Script = gameObject.AddComponent<scrKey1>();
                newKey1Script.keyIconSprite = key1Sprite;
                break;
            
            case "key2":
                Debug.Log("what's to be added to inventory: key2");
                scrKey2 newKey2Script = gameObject.AddComponent<scrKey2>();
                newKey2Script.keyIconSprite = key2Sprite;
                break;
            
            case "key3":
                Debug.Log("what's to be added to inventory: key3");
                scrKey3 newKey3Script = gameObject.AddComponent<scrKey3>();
                newKey3Script.keyIconSprite = key3Sprite;
                break;
        }
    }

    public void inventoryDropEverything()
    {
        // clear everything first
        List<string> _dic_keys = new List<string>(inventory.Keys);
        foreach (string item in _dic_keys)
        {
            inventory[item] = false;
        }

        if (GetComponent<scrKey1>() != null)
        {
            // drop it on the ground first
            GameObject _new_key_1_pick_up =Instantiate(keyPickUp1, transform.parent);
            _new_key_1_pick_up.GetComponent<GridObject>().gridPosition = this.GetComponent<GridObject>().gridPosition;
            
            // then delete from backpack.
            Destroy(GetComponent<scrKey1>());
        }
        
        
        if (GetComponent<scrKey2>() != null)
        {
            // drop it on the ground first
            GameObject _new_key_2_pick_up =Instantiate(keyPickUp2, transform.parent);
            _new_key_2_pick_up.GetComponent<GridObject>().gridPosition = this.GetComponent<GridObject>().gridPosition;
            
            Destroy(GetComponent<scrKey1>());
        }
        
        if (GetComponent<scrKey3>() != null)
        {
            // drop it on the ground first
            GameObject _new_key_3_pick_up =Instantiate(keyPickUp3, transform.parent);
            _new_key_3_pick_up.GetComponent<GridObject>().gridPosition = this.GetComponent<GridObject>().gridPosition;
            
            Destroy(GetComponent<scrKey1>());
        }
        
    }
}
