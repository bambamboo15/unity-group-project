using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour {
    // The player to connect to 
    [SerializeField] private Player player;

    // The color if unselected or selected 
    [SerializeField] private Color unselectedColor;
    [SerializeField] private Color selectedColor;

    // The current index of the item slot 
    [SerializeField] private int index;

    // Properties of the item slot (lazy)
    private Image image;
    private Image itemImage;

    // Initialize properties of the item slot (lazy)
    void Start() {
        image = GetComponent<Image>();
        itemImage = transform.GetChild(0).GetComponent<Image>();
    }

    // Change the look of the item slot based on 
    // the player inventory 
    void Update() {
        if (player.inventorySelectedIndex == index)
            image.color = selectedColor;
        else 
            image.color = unselectedColor;
        
        ItemTag tag = player.inventory[index];
        if (tag.empty) {
            itemImage.color = Color.clear;
        } else {
            itemImage.color = Color.white;
            itemImage.sprite = tag.sprite;
        }
    }
}