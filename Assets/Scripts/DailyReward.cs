using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DailyReward : MonoBehaviour
{
    public bool initialized;
    public long rewardGivingTimeTicks;
    public GameObject rewardMenu;
    public TMP_Text remainingText;
    public const string LastDailyReward = "lastDailyReward";
  public void InitializedDailyReward(){

    if(PlayerPrefs.HasKey(LastDailyReward)){

        rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString(LastDailyReward)) + 864000000000;
        long currentTime = System.DateTime.Now.Ticks;
        if(currentTime >= rewardGivingTimeTicks){
            GiveReward();
        }
    }else{

        GiveReward();
    }

    initialized= true;


  }

  public void GiveReward(){

        LevelController.Current.GiveMoneyToPlayer(100);
        rewardMenu.SetActive(true);
        PlayerPrefs.SetString(LastDailyReward, System.DateTime.Now.Ticks.ToString());
        rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString(LastDailyReward)) + 864000000000;
  }

    
    void Update()
    {
        if(initialized){

            if(LevelController.Current.startMenu.activeInHierarchy){

                long currentTime = System.DateTime.Now.Ticks;
                long remainingTime = rewardGivingTimeTicks - currentTime;
                if(remainingTime <= 0){

                    GiveReward();
                }else{

                    System.TimeSpan timeSpan = System.TimeSpan.FromTicks(remainingTime);
                    remainingText.text = string.Format("{0},{1},{2}", timeSpan.Hours.ToString("D2"), timeSpan.Minutes.ToString("D2"), timeSpan.Seconds.ToString("D2"));
                }

            }
        }
    }

    public void TapToReturnButton(){

        rewardMenu.SetActive(false);
    }
}
