using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapSelect : MonoBehaviour {
    [SerializeField] private Transform[] maps;
    [SerializeField] private int selectedIndex;
    [SerializeField] private Material textUnselected;
    [SerializeField] private Material textSelected;
    [SerializeField] private Material borderSelected;
    [SerializeField] private Material borderUnselected;

    void Update() {
        Transform selected = maps[selectedIndex];
        Transform borderT = selected.GetChild(0);
        Transform container = selected.GetChild(2);
        Transform nameT = container.GetChild(0);
        Transform descT = container.GetChild(1);
        TMP_Text name = nameT.GetComponent<TMP_Text>();
        TMP_Text desc = descT.GetComponent<TMP_Text>();
        Image border = borderT.GetComponent<Image>();
        name.material = textSelected;
        border.material = borderSelected;
    }
}