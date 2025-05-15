using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayCardButton : MonoBehaviour
{
    public int counter = 0;

    public int nextLevel;
 
    public CardManager cardManager;
    public EnemyManager enemyManager;
    public OutputManager outputManager;

    [SerializeField] GameObject GameOverScreen;
    [SerializeField] GameObject YouWinScreen;

    [SerializeField] TextMeshProUGUI playerHP;
    [SerializeField] TextMeshProUGUI enemyHP;

    public Item card;

    //public List<GameObject> Answers = new List<GameObject>();

    public List <CorrectAnswerContainer> correctAnswersContainer = new List<CorrectAnswerContainer>();
    [System.Serializable]
    public class CorrectAnswerContainer
    {
        public List<string> correctAnswers = new List<string>();
    }

    public Image playerHealthBar;    
    public List<GameObject> enemyHealthBarObject = new List<GameObject>();
    public List<Image> enemyHealthBar = new List<Image>();


    public List<float> enemyHealthAmount = new List<float>();
    public List<float> enemyHealthTotal = new List<float>();

    [SerializeField] public float playerHealthAmount;
    [SerializeField] public float playerHealthTotal;
    //[SerializeField] public float enemyHealthAmount;
    //[SerializeField] public float enemyHealthTotal;

    Image cardDesign;

    Transform parent;
    [SerializeField] Transform playedCard;

    private void Start()
    {
        cardManager = FindAnyObjectByType<CardManager>();
    }
    private void Update()
    {
        if (playerHealthAmount == 0f)
        {
            GameOverScreen.SetActive(true);
        }
        if (enemyManager.counter == enemyManager.enemies.Count)
        {
            nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextLevel > PlayerPrefs.GetInt("levelAt"))
            {
                PlayerPrefs.SetInt("levelAt", nextLevel);
            }
            YouWinScreen.SetActive(true);
        }
        playerHP.text = playerHealthAmount.ToString() + " / " + playerHealthTotal.ToString();
        if (enemyManager.counter < enemyHealthAmount.Count)
        {
            enemyHP.text = enemyHealthAmount[enemyManager.counter].ToString() + " / " + enemyHealthTotal[enemyManager.counter].ToString();
        }


    }
    public void PlayButton()
    {
        
        parent = transform.Find("PlayedCard");
        playedCard = parent.GetChild(0);

        for (int i = 0; i < correctAnswersContainer[outputManager.counter].correctAnswers.Count; i++)
        {
            if (counter == i)
            {

                if (playedCard.name == correctAnswersContainer[outputManager.counter].correctAnswers[counter])
                {
                    EnemyTakeDamage(1);
                    outputManager.answerListContainer[outputManager.counter].answers[i].SetActive(true);
                    cardManager.counter++;
                    counter++;
                    if (enemyHealthAmount[enemyManager.counter] == 0f)
                    {
                        DeactivateEnemy();
                        DeactivateOutput();
                        DeactivateAnswer();
                        enemyManager.counter++;
                        outputManager.counter++;
                        counter = 0;

                        if (enemyManager.counter < enemyHealthAmount.Count)
                        {
                            ActivateEnemy();
                            ActivateOutput();
                            ActivateAnswer();
                        }
                    }
                }
                else
                {
                    PlayerTakeDamage(1);
                }
                cardManager.ResetCards();
                cardManager.StartCoroutine(cardManager.Randomize());
                break;
            }
        }
    }
    public void PlayerTakeDamage(float damage)
    {
        playerHealthAmount -= damage;
        playerHealthBar.fillAmount = playerHealthAmount / playerHealthTotal;
    }

    public void EnemyTakeDamage(float damage)
    {
        enemyHealthAmount[enemyManager.counter] -= damage;
        enemyHealthBar[enemyManager.counter].fillAmount = enemyHealthAmount[enemyManager.counter] / enemyHealthTotal[enemyManager.counter];
    }
    public void DeactivateEnemy()
    {
        enemyManager.enemies[enemyManager.counter].SetActive(false);
        enemyHealthBarObject[enemyManager.counter].SetActive(false);
    }
    public void ActivateEnemy()
    {
        enemyManager.enemies[enemyManager.counter].SetActive(true);
        enemyHealthBarObject[enemyManager.counter].SetActive(true);
    }
    public void DeactivateOutput()
    {
            outputManager.codes[outputManager.counter].SetActive(false);
            outputManager.outputs[outputManager.counter].SetActive(false);
    }
    public void ActivateOutput()
    {
        outputManager.codes[outputManager.counter].SetActive(true);
        outputManager.outputs[outputManager.counter].SetActive(true);
    }

    public void DeactivateAnswer()
    {
        if (enemyManager.counter < enemyHealthAmount.Count)
        {
            outputManager.answerList[outputManager.counter].SetActive(false);
        }
    }
    public void ActivateAnswer()
    {
        outputManager.answerList[outputManager.counter].SetActive(true);
    }
}