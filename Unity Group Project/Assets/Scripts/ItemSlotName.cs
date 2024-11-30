using UnityEngine;
using TMPro;

public class ItemSlotName : MonoBehaviour {
    [SerializeField] private Player player;
    private TMP_Text text;

    void Start() {
        text = GetComponent<TMP_Text>();
    }

    void Update() {
        ItemTag tag = player.inventory[player.inventorySelectedIndex];
        text.text = tag.empty ? "" : tag.name;
    }
}