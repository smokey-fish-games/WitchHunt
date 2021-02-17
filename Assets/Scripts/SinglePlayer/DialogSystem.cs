namespace SFG.WitchHunt.SinglePlayer
{
    public class DialogSystem
    {
        public struct dialogResponse
        {
            public string answer;
            public int attitudeChange;

            public dialogResponse(string Q, int A)
            {
                answer = Q;
                attitudeChange = A;
            }
        }

        struct dialog
        {
            public string question;
            public dialogResponse answer;

            public dialog(string Q, dialogResponse A)
            {
                question = Q;
                answer = A;
            }
        }
        struct dialogTree
        {
            int currentDialog;
            dialog[] branch;

            public dialogTree(dialog[] inputs)
            {
                currentDialog = 0;
                branch = inputs;
            }

            public bool moveBack()
            {
                if (currentDialog != 0)
                {
                    currentDialog--;
                    return true;
                }
                return false;
            }

            public string GetDialogQ()
            {
                return branch[currentDialog].question;
            }

            public dialogResponse GetDialogA()
            {
                dialogResponse toReturn = branch[currentDialog].answer;
                if (currentDialog + 1 < branch.Length)
                {
                    currentDialog++;
                }
                return toReturn;
            }
        }

        public DialogSystem()
        {
            this.Initialize();
        }

        // Fixed for now. Likely will be different later and dialog options build off this
        dialogTree[] dialogs = new dialogTree[5];

        public int GetNumDialogTrees()
        {
            return dialogs.Length;
        }

        public void ResetDialogs()
        {
            for (int i = 0; i < dialogs.Length; i++)
            {
                ResetBranch(i);
            }
        }

        public void ResetBranch(int branch)
        {
            while (dialogs[branch].moveBack()) ;
        }

        public string GetDialogTreeQuestion(int branch)
        {
            return dialogs[branch].GetDialogQ();
        }

        public dialogResponse GetDialogTreeResponse(int branch)
        {
            // Also pushes the response on if possible and resets other branches
            for (int i = 0; i < dialogs.Length; i++)
            {
                if (i != branch)
                {
                    ResetBranch(i);
                }
            }

            return dialogs[branch].GetDialogA();
        }

        // All NPCS will use same for now
        public void Initialize()
        {
            //1. How are you?
            dialogs[0] = new dialogTree(new dialog[] { new dialog("How are you?", new dialogResponse("Good Thanks", 1)), new dialog("How are you Really?", new dialogResponse("I'm fine....", 0)), new dialog("Are you sure?", new dialogResponse("Yes. Stop asking", -10)) });

            // 2.Seen anything suspicious?
            dialogs[1] = new dialogTree(new dialog[] { new dialog("Seen anything suspicious?", new dialogResponse("Not that i can think of", 0)), new dialog("Nothing suspicious at all?", new dialogResponse("I told you no", -1)) });

            // 3.Who's in your family
            dialogs[2] = new dialogTree(new dialog[] { new dialog("Tell me about your family?", new dialogResponse("No", 0)) });

            // 4.Where are you going ?
            dialogs[3] = new dialogTree(new dialog[] { new dialog("Where are you off to?", new dialogResponse("My AI is too stupid so i'm just wandering around aimlessly", 10)) });

            // 5.Who are your friends and enemies
            dialogs[4] = new dialogTree(new dialog[] { new dialog("Got any friends or enemies?", new dialogResponse("I am entirely neutral", 0)) });
        }
    }
}
