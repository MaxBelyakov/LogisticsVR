// Economics manager

using UnityEngine;
using TMPro;

public class InfoDesk : MonoBehaviour
{
    public int loaded = 0;              // Boxes loaded in small truck by player
    public int unloaded = 0;            // Boxes unloaded from big truck by player
    public int delivered = 0;           // Boxes delivered by player
    private int money = 0;              // Money counter

    public TMP_Text loadedText;         // Loaded text
    public TMP_Text unloadedText;       // Unloaded text
    public TMP_Text deliveredText;      // Delivered text
    public TMP_Text moneyText;          // Money text

    void Update()
    {
        // Calculate money
        money = loaded + unloaded + delivered * 5;

        // Update text
        loadedText.text = loaded.ToString();
        unloadedText.text = unloaded.ToString();
        deliveredText.text = delivered.ToString();
        moneyText.text = money.ToString() + " $";
    }
}