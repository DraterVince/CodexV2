using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header (" RANDOMIZED COUNTER ")]
    public int counter;

    [Header (" CONTAINERS ")]
    public List<CardListContainer> cardContainer = new List<CardListContainer>();
    public List<CardDisplayContainer> cardDisplayContainer = new List<CardDisplayContainer>();

    private List<Item> chosenCards = new List<Item>();

    [System.Serializable]
    public class CardListContainer
    {
        public List<Item> cards = new List<Item>();
    }
    [System.Serializable]
    public class CardDisplayContainer
    {
        public List<CardDisplay> cardDisplay = new List<CardDisplay>();
    }

    public GameObject grid;
    private void Start()
    {
        Time.timeScale = 1f;
        StartCoroutine(Randomize());
    }

    public IEnumerator Randomize()
    {
        chosenCards = new List<Item>(cardContainer[counter].cards);

        for (int i = 0; i < cardDisplayContainer[counter].cardDisplay.Count; i++)
        {
            int rand = Random.Range(0, chosenCards.Count);

            cardDisplayContainer[counter].cardDisplay[i].cardName = chosenCards[rand].cardName;
            cardDisplayContainer[counter].cardDisplay[i].transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = chosenCards[rand].cardName;
            cardDisplayContainer[counter].cardDisplay[i].cardDesign.sprite = chosenCards[rand].artwork;

            yield return new WaitForSeconds(0.15f);
            cardDisplayContainer[counter].cardDisplay[i].gameObject.SetActive(true);

            chosenCards.RemoveAt(rand);
            
        }
    }

    public void ResetCards()
    {
        chosenCards.Clear();

        for (int i = 0; i < cardDisplayContainer[counter].cardDisplay.Count; i++)
        {
            cardDisplayContainer[counter].cardDisplay[i].gameObject.SetActive(false);
            cardDisplayContainer[counter].cardDisplay[i].gameObject.transform.SetParent(grid.transform);
        }
    }

    //public IEnumerator Randomize1()
    //{
    //    chosenCards = new List<Item>(cards);

    //    for (int i = 0; i < cardDisplays.Count; i++)
    //    {
    //        int randomIndex = Random.Range(0, chosenCards.Count);

    //        cardDisplays[i].cardName = chosenCards[randomIndex].cardName;
    //        cardDisplays[i].cardDesign.sprite = chosenCards[randomIndex].artwork;

    //        yield return new WaitForSeconds(0.15f);
    //        cardDisplays[i].gameObject.SetActive(true);

    //        selectedRandomCard1.Add(chosenCards[randomIndex]);
    //        chosenCards.RemoveAt(randomIndex);
    //    }
    //}
    //public void ResetCards1()
    //{
    //    chosenCards.Clear();
    //    for (int i = 0; i < cardDisplays.Count; i++)
    //    {
    //        cardDisplays[i].gameObject.SetActive(false);
    //        cardDisplays[i].gameObject.transform.SetParent(grid.transform);
    //    }
    //}

}
