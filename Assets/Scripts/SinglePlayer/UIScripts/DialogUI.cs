using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace SFG.WitchHunt.SinglePlayer.UI
{
    public class DialogUI : MonoBehaviour
    {
        private RobLogger rl;
        RobLogger RL
        {
            get
            {
                if (rl != null)
                {
                    return rl;
                }
                return rl = RobLogger.GetRobLogger();
            }
        }

        [SerializeField] Button[] buttonArray;

        PlayerController callbacke;
        NPCController partner;

        [SerializeField] GameObject dialogBox;
        [SerializeField] Sprite malePic;
        [SerializeField] Sprite femalePic;
        [SerializeField] Image ProfilePic;
        [SerializeField] TMP_Text dialogText;
        [SerializeField] TMP_Text nameBox;
        [SerializeField] TMP_Text attitudeText;
        [SerializeField] Image attitudeColourControl;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            RL.writeTraceEntry();
            if (buttonArray.Length == 0)
            {
                RL.writeError("Button array not setup");
            }

            for (int i = 0; i < buttonArray.Length; i++)
            {
                int x = i;
                buttonArray[i].onClick.AddListener(delegate { Speak(x); });
            }
            RL.writeTraceExit(null);
        }


        public void STARTDIALOG(NPCController theNPC, PlayerController callback)
        {
            RL.writeTraceEntry(theNPC, callback);
            callbacke = callback;
            partner = theNPC;

            dialogBox.SetActive(true);
            dialogText.text = "Hello there";
            nameBox.text = theNPC.fullName;
            if (theNPC.male)
            {
                ProfilePic.sprite = malePic;
            }
            else
            {
                ProfilePic.sprite = femalePic;
            }
            for (int i = 0; i < buttonArray.Length; i++)
            {
                buttonArray[i].GetComponentInChildren<Text>().text = partner.GetDialogQuestion(i);
            }
            SetAttitude(theNPC.attitude);
            RL.writeTraceExit(null);
        }

        public void Speak(int option)
        {
            RL.writeTraceEntry(option);
            dialogText.text = partner.GetDialogResponse(option);
            // Rebuild the questions
            for (int i = 0; i < buttonArray.Length; i++)
            {
                buttonArray[i].GetComponentInChildren<Text>().text = partner.GetDialogQuestion(i);
            }
            SetAttitude(partner.attitude);
            RL.writeTraceExit(null);
        }


        public void Close()
        {
            RL.writeTraceEntry();
            dialogBox.SetActive(false);
            if (callbacke != null)
            {
                callbacke.CancelUI();
            }

            callbacke = null;
            partner = null;
            RL.writeTraceExit(null);
        }

        // -100 -> -75  = Hated
        // -74  -> -35  = Disliked
        // -34  ->  34  = Neutral
        //  35  ->  74  = Liked
        //  75  ->  100 = Loved
        void SetAttitude(int attValue)
        {
            string text;
            Color color;

            if (attValue > 74)
            {
                text = "Loved";
                color = Color.blue;
            }
            else if (attValue > 34)
            {
                text = "Liked";
                color = Color.green;
            }
            else if (attValue > -33)
            {
                text = "Neutral";
                color = Color.grey;
            }
            else if (attValue > -75)
            {
                text = "Disliked";
                color = Color.yellow;
            }
            else
            {
                text = "Hated";
                color = Color.red;
            }

            attitudeColourControl.color = color;
            attitudeText.text = text;
        }
    }
}
