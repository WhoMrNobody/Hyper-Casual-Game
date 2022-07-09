using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketItem : MonoBehaviour
{
    public int itemId, wearId;
    public int price;
    public Button buyButton, equipButton, unequipButton;
    public TMP_Text priceText;
    public GameObject itemPrefab;
    public const string ItemKey = "item";
    public const string MoneyKey = "money";

    public bool HasItem(){

        /*  0 : Daha satın alınmamış
            1 : Satın alınmamış ama giyilmemiş
            2 : Hem satın alınmış ve giyilmiş
        */
        bool hasItem = PlayerPrefs.GetInt(ItemKey + itemId.ToString()) != 0;
        return hasItem;
    }
    public bool IsEquipped(){

        bool equippedItem = PlayerPrefs.GetInt(ItemKey + itemId.ToString()) == 2;
        return equippedItem;
    }

    public void InitializeItem(){

        priceText.text = price.ToString();

        if(HasItem()){

            buyButton.gameObject.SetActive(false);
            if(IsEquipped()){

                EquipItem();

            }else{

                equipButton.gameObject.SetActive(true);

            }
        }
    }

    public void BuyItem(){

        if(!HasItem()){

            int money = PlayerPrefs.GetInt(MoneyKey);
            if(money >= price){

                PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.buyItemClip, 0.1f);
                LevelController.Current.GiveMoneyToPlayer(-price);
                PlayerPrefs.SetInt(ItemKey + itemId.ToString(), 1);
                buyButton.gameObject.SetActive(false);
                equipButton.gameObject.SetActive(true);
            }
        }
    }

    public void EquipItem(){

        UnequipItem();
        MarketController.Current.equippedItems[wearId] = Instantiate(itemPrefab, PlayerController.Current.wearSpots[wearId].transform).GetComponent<Item>();
        MarketController.Current.equippedItems[wearId].itemId = itemId;
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(true);
        PlayerPrefs.SetInt(ItemKey + itemId.ToString(), 2);

    }

    public void UnequipItem(){

        Item equippedItem = MarketController.Current.equippedItems[wearId];
        if(equippedItem != null){

            MarketItem marketItem = MarketController.Current.Items[equippedItem.itemId];
            PlayerPrefs.SetInt(ItemKey + marketItem.itemId, 1);
            marketItem.equipButton.gameObject.SetActive(true);
            marketItem.unequipButton.gameObject.SetActive(false);
            Destroy(equippedItem.gameObject);
        }


    }

    public void EquipItemButton(){

        PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.equipItemAudioClip, 0.1f);
        EquipItem();

    }

    public void UnequipItemButton(){

        PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.unequipItemAudioClip, 0.1f);
        UnequipItem();
    }
}
