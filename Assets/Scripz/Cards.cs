using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class Cards : MonoBehaviour
{

    // Tutorial from: https://www.youtube.com/watch?v=HajiNmv4UNY

    [SerializeField] private Image iconImage;

    public Sprite hiddenIconSprite;
    public Sprite iconSprite;

    public bool isSelected;

    public CardController controller;

    public void onCardClick()
    {
        controller.SetSelected(this);
    }
    public void SetIconSprite(Sprite sp)
    {
        iconSprite = sp;


        //Display icons/symbols during preview

        iconImage.sprite = iconSprite;

        transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        isSelected = true;
    }

    public void Show()
    {
        if (isSelected) return;

        isSelected = true;

        //Selects the target on Transfer, spins the card, animation starts.

        Sequence.Create().Chain(Tween.Rotation(transform, new Vector3(0f, 90f, 0), 0.1f))
            .ChainCallback(() => iconImage.sprite = iconSprite)
            .Chain(Tween.Rotation(transform, new Vector3(0f, 180f, 0f), 0.1f));

        Tween.Delay(0.1f, () => iconImage.sprite = iconSprite);

        iconImage.sprite = iconSprite;
        isSelected = true;
    }

    public void Hide()
    {
        if (!isSelected) return;

        isSelected = false;

        Sequence.Create().Chain(Tween.Rotation(transform, new Vector3(0f, 90f, 0f), 0.1f))
            .ChainCallback(() => iconImage.sprite = hiddenIconSprite)
            .Chain(Tween.Rotation(transform, Vector3.zero, 0.1f));
    }
}

// Assets used: https://gametemplates.itch.io/template-snapy-pairs