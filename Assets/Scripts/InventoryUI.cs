using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryUI : MonoBehaviour
{

    public float secondsToRummage = 10f;
    public GameObject rummageUI;
    public GameObject disUI;

    PlayerController callbacke;

    public Slider slider;
    public Image contentImage;

    public void STARTINVENTORY(Inventory theInv, PlayerController callback)
    {
        StopCoroutine("RummageShow");

        callbacke = callback;
        disUI.SetActive(false);
        rummageUI.SetActive(true);

        /* Setup contents of the contents UI*/
        contentImage.sprite = theInv.getItem().icon;

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
        // TODO
        Close();
    }
}
