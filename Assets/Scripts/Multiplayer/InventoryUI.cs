using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//  
// Copyright (c) Robert Parker 2021. All rights reserved.  
//  
namespace SFG.WitchHunt.MultiPlayer
{
    public class InventoryUI : MonoBehaviour
    {

        public float secondsToRummage = 10f;
        public GameObject rummageUI;
        public GameObject disUI;

        PlayerController callbacke;

        public Slider slider;
        public Image contentImage;
        public Button TakeButton;
        public Button DestroyButton;

        public void STARTINVENTORY(Inventory theInv, PlayerController callback)
        {
            /* Reset state */
            StopCoroutine("RummageShow");
            callbacke = callback;
            disUI.SetActive(false);
            rummageUI.SetActive(true);
            TakeButton.enabled = true;
            DestroyButton.enabled = true;

            /* Setup contents of the contents UI*/
            Item item = theInv.getItem();
            if (item == null)
            {
                item = Item.GetItem(CONSTANTS.NO_ITEM);
            }
            contentImage.sprite = item.icon;
            if (item.ID == CONSTANTS.NO_ITEM || item.ID == CONSTANTS.ITEM_DESTROYED)
            {
                TakeButton.enabled = false;
                DestroyButton.enabled = false;
            }

            StartCoroutine("RummageShow");
        }

        IEnumerator RummageShow()
        {
            float curSec = 0f;
            while (curSec <= secondsToRummage)
            {
                slider.value = curSec / 10;
                yield return new WaitForSeconds(1f);
                curSec += 1f;
            }

            /* now show the next UI */
            disUI.SetActive(true);
            rummageUI.SetActive(false);
        }

        public void Close()
        {
            StopCoroutine("RummageShow");
            disUI.SetActive(false);
            rummageUI.SetActive(false);
            callbacke.CancelUI();
        }

        public void Take()
        {
            callbacke.TakeItem();
            Close();
        }

        public void DestroyItem()
        {
            callbacke.DestroyItem();
            Close();
        }
    }
}
