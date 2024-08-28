using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject {
    public string itemName;
    public Sprite itemIcon; // Icon to display in the UI
    public int value;
    //public int minValue;
    //public int maxValue; 
    public ItemType itemType;

    public enum ItemType {
        Gold,
        Health,
        PowerUp
    }

    /*    public int GetRandomValue() {
            return Random.Range(minValue, maxValue + 1);
        }*/
}