// Economics manager

using UnityEngine;
using TMPro;

public class InfoDesk : MonoBehaviour
{
    public int loaded = 0;              // Boxes loaded in small truck by player
    public int unloaded = 0;            // Boxes unloaded from big truck by player
    public int delivered = 0;           // Boxes delivered by player
    public int money = 0;              // Money counter

    public int deliveredPrice = 5;      // Price for delivery 1 box

    public TMP_Text loadedText;         // Loaded text
    public TMP_Text unloadedText;       // Unloaded text
    public TMP_Text deliveredText;      // Delivered text
    public TMP_Text moneyText;          // Money text

    void Start()
    {
        money = 0;
    }

    void Update()
    {
        // Update text
        loadedText.text = loaded.ToString();
        unloadedText.text = unloaded.ToString();
        deliveredText.text = delivered.ToString();
        moneyText.text = money.ToString() + " $";
    }
}