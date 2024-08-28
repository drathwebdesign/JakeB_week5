using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemCollector : MonoBehaviour {
    //private int totalGold = 0;
    private PlayerMovement playerMovement;

    //public TextMeshProUGUI goldText;

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnCollisionEnter(Collision collision) {
        ItemCollectable itemPickup = collision.gameObject.GetComponent<ItemCollectable>();
        if (itemPickup != null) {
            CollectItem(itemPickup);
            //Destroy(collision.gameObject);
        }
    }

    private void CollectItem(ItemCollectable itemPickup) {
        Item item = itemPickup.item;

        switch (item.itemType) {
            case Item.ItemType.Gold:
/*                totalGold += item.value;
                UpdateGoldUI(); // Update the UI text*/
                break;

            case Item.ItemType.Health:
                playerMovement.Heal(item.value);
                break;

            case Item.ItemType.PowerUp:
                HandlePowerUp(item);
                break;
        }
    }

    private void HandlePowerUp(Item item) {
        if (item.itemName == "DamageCrystal4") {
            Debug.Log("Collected Damage Crystal, doubling damage!");
            playerMovement.DoubleDamage(); // Apply double damage effect
        }
    }

    /*    private void UpdateGoldUI() {
            goldText.text = "Gold: " + totalGold.ToString();
        }*/
}