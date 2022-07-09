using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelController : MonoBehaviour
{
    public static LevelController Current;
    public bool gameActive;
    public GameObject startMenu, gameMenu, gameOverMenu, finishMenu;
    public TMP_Text scoreText, finishScoreText, currentlevelText, nextLevelText, startMenuMoneyText, gameOverMoneyText, gameFinishMoneyText;
    public Slider levelProgressBar;
    public float maxDistance;
    public GameObject finishLine;
    public AudioSource gameMusicAudioSource;
    public AudioClip victoryAudioClip, gameOverAudioClip;
    public DailyReward dailyReward;

    private int _currentLevel;
    private int score;
    void Start()
    {
        Current = this;
        _currentLevel = PlayerPrefs.GetInt("currentLevel");
        
            PlayerController.Current=GameObject.FindObjectOfType<PlayerController>();
            GameObject.FindObjectOfType<MarketController>().InitializeMarketController();
            dailyReward.InitializedDailyReward();
            currentlevelText.text = (_currentLevel + 1).ToString();
            nextLevelText.text = (_currentLevel + 2).ToString();
            UpdateMoneyTexts();
        

        gameMusicAudioSource = Camera.main.GetComponent<AudioSource>();
    }

    
    void Update()
    {
        if(gameActive){

            PlayerController player = PlayerController.Current;
            float distance = finishLine.transform.position.z - player.transform.position.z;
            levelProgressBar.value = 1 - (distance/maxDistance);

        }
    }

    public void StartLevel(){

        maxDistance = finishLine.transform.position.z - PlayerController.Current.transform.position.z;
        PlayerController.Current.ChangeCharacterSpeed(PlayerController.Current.runningSpeed);
        startMenu.SetActive(false);
        gameMenu.SetActive(true);
        PlayerController.Current.animator.SetBool("running", true);
        gameActive=true;
    }

    public void RestartLevel(){
        
        LevelLoader.Current.ChangeLevel(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel(){

        LevelLoader.Current.ChangeLevel("Level " + (_currentLevel + 1));
    }

    public void GameOver(){
        
        UpdateMoneyTexts();
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(gameOverAudioClip);
        gameMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        gameActive= false;
    }

    public void FinishGame(){

        GiveMoneyToPlayer(score);
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(victoryAudioClip);
        PlayerPrefs.SetInt("currentLevel", _currentLevel + 1);
        finishScoreText.text = score.ToString();
        gameMenu.SetActive(false);
        finishMenu.SetActive(true);
        gameActive=false;
    }

    public void ChangeScore(int increment){

        score += increment;
        scoreText.text= score.ToString();
    }

    public void UpdateMoneyTexts(){

        int money = PlayerPrefs.GetInt("money");
        startMenuMoneyText.text=money.ToString();
        gameOverMoneyText.text=money.ToString();
        gameFinishMoneyText.text=money.ToString();

    }

    public void GiveMoneyToPlayer(int increment){

        int money = PlayerPrefs.GetInt("money");
        money = Mathf.Max(0, money + increment);
        PlayerPrefs.SetInt("money", money);
        UpdateMoneyTexts();
    }
}
