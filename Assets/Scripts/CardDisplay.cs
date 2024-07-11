using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Corrected namespace for TextMeshPro

public class CardDisplay : MonoBehaviour
{
    public ScriptableCards card;

    public TMP_Text nameText; 
    public TMP_Text descriptionText; 

    public Image artworkImage;

    public TMP_Text manaText;

    private void Start()
    {
        nameText.text = card.name;
        descriptionText.text = card.description;
        artworkImage.sprite = card.artwork;
        manaText.text = card.manaCost.ToString();
    }
}
