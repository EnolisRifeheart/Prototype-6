using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private float gameDuration = 10f;

    private bool gameEnded;
    private float timeRemaining;
    [SerializeField] Cards cardPrefab;
    [SerializeField] Transform gridTransform;
    [SerializeField] Sprite[] sprites;
    [SerializeField] float previewDuration = 3f;

    private List<Sprite> spritePairs;
    private List<Cards> cards = new List<Cards>();

    Cards firstSelected;
    Cards secondSelected;

    int matchCounts;
    bool isCheckingMatch;
    bool isPreviewing = true;
    private void Start()
    {

        if (resultText != null)
            resultText.gameObject.SetActive(false);

        if (timerText != null)
            timerText.text = "Preview";


        PrepareSprites();
        CreateCards();
        StartCoroutine(HideCardsAfterDelay());
    }

    void CreateCards()
    {
        for (int i = 0; i < spritePairs.Count; i++)
        {
            Cards card = Instantiate(cardPrefab, gridTransform);
            card.SetIconSprite(spritePairs[i]);
            card.controller = this;

            cards.Add(card);
        }
    }

    IEnumerator HideCardsAfterDelay()
    {
        yield return new WaitForSeconds(previewDuration);

        foreach (Cards card in cards)
        {
            card.Hide();
        }

        isPreviewing = false;

        // Once card flips, timer should begin.

        yield return new WaitForSeconds(0.25f);

        isPreviewing = false;

        StartCoroutine(GameTimer());

    }

    IEnumerator GameTimer()
    {
        timeRemaining = gameDuration;

        while (timeRemaining > 0f && !gameEnded)
        {
            timerText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}";

            yield return null;

            timeRemaining -= Time.deltaTime;
        }

        if (!gameEnded)
        {
            timeRemaining = 0f;
            timerText.text = "Time: 0";
            EndGame(false);
        }
    }

    void EndGame(bool won)
    {
        if (gameEnded) return;

        gameEnded = true;
        isCheckingMatch = true;

        resultText.text = won ? "You Win!!!" : "Time is UP!!!!!";
        resultText.gameObject.SetActive(true);
    }

    public void SetSelected(Cards card)
    {
        if (gameEnded || isPreviewing || isCheckingMatch || card.isSelected)
            
            return;

        card.Show();

            if (firstSelected == null)
            {
                firstSelected = card;
                return;
            }

        secondSelected = card;
        StartCoroutine(CheckMatching(firstSelected, secondSelected));

        firstSelected = null;
        secondSelected = null;

    }

    IEnumerator CheckMatching(Cards a, Cards b)
    {

        isCheckingMatch = true;

        yield return new WaitForSeconds(0.3f);

        if (a.iconSprite == b.iconSprite)
        {
            // Match
            matchCounts++;

            if(matchCounts >= spritePairs.Count / 2)
            {
                EndGame(true);

                PrimeTween.Sequence.Create()
                    .Chain(PrimeTween.Tween.Scale(gridTransform, Vector3.one, duration: 0.5f, ease: PrimeTween.Ease.OutBack));

                yield break;
            }
        }
        else
        {  // Flip back
            a.Hide();
            b.Hide();
        }

        isCheckingMatch = false;
    }
    private void PrepareSprites()
    {
        spritePairs = new List<Sprite>();
        for (int i = 0; i < sprites.Length; i++)
        {
            // adding sprite 2 times to make it pair

            spritePairs.Add(sprites[i]);
            spritePairs.Add(sprites[i]);
        }

        ShuffleSprites(spritePairs);

    }

    void ShuffleSprites(List<Sprite> spriteList)
    {
        for (int i = spriteList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);


            // Swap the elements at i and RandomIndex
            Sprite temp = spriteList[i];
            spriteList[i] = spriteList[randomIndex];
            spriteList[randomIndex] = temp;
        }
    }
}
