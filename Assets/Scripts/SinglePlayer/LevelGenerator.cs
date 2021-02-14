using UnityEngine;
using System.Collections.Generic;

namespace SFG.WitchHunt.SinglePlayer
{
    public class LevelGenerator : MonoBehaviour
    {
        // TODO randomly source these!
        string[] maleFirstNames = new string[] { "Rob", "Paul", "Chris", "Steve", "Joe", "Jim", "Chad", "Mike", "Jake", "Dan" };
        string[] femaleFirstNames = new string[] { "Josie", "Jo", "Olivia", "Kat", "Polina", "Anne-Marie", "Delia", "Sylvia", "Amy", "Ashley" };
        string[] lastNames = new string[] { "Parker", "Messa", "Edmonds", "Cook", "Bean", "Bulmer", "Nash", "Pentalioti", "Pavitt", "Zotova", "Chaloner ", "Cheung", "Carter", "McDonald", "Harris", "Squires", "Kirkpatrick", "Hampton" };


        const int CONSTANT_MIN_NEGATIVE = -100;
        const int CONSTANT_HATED = -75;
        const int CONSTANT_DISLIKED = -35;
        const int CONSTANT_NEUTRAL = 0;
        const int CONSTANT_LIKED = 35;
        const int CONSTANT_LOVED = 75;
        const int CONSTANT_MAX_POSTIVE = 100;

        // -100 -> -75  = Hated
        // -74  -> -35  = Disliked
        // -34  ->  34  = Neutral
        //  35  ->  74  = Liked
        //  75  ->  100 = Loved

        const int MIN_ATTITUDE_PARENT = CONSTANT_HATED;
        const int MAX_ATTITUDE_PARENT = CONSTANT_MAX_POSTIVE;

        const int MIN_ATTITUDE_CHILD = CONSTANT_DISLIKED;
        const int MAX_ATTITUDE_CHILD = CONSTANT_MAX_POSTIVE;

        const int MIN_ATTITUDE_SIBLING = CONSTANT_MIN_NEGATIVE;
        const int MAX_ATTITUDE_SIBLING = CONSTANT_MAX_POSTIVE;

        const int MIN_ATTITUDE_FRIEND = CONSTANT_LIKED;
        const int MAX_ATTITUDE_FRIEND = CONSTANT_LOVED;

        const int MIN_ATTITUDE_ENEMY = CONSTANT_DISLIKED;
        const int MAX_ATTITUDE_ENEMY = CONSTANT_MIN_NEGATIVE;

        const int MIN_ATTITUDE_ACQUAINTANCE = CONSTANT_DISLIKED;
        const int MAX_ATTITUDE_ACQUAINTANCE = CONSTANT_LIKED;

        const int MIN_ATTITUDE_UNKNOWN = CONSTANT_NEUTRAL;
        const int MAX_ATTITUDE_UNKNOWN = CONSTANT_NEUTRAL;

        const int CHANCE_SINGLE = 30;
        const int CHANCE_FAMILY = 100 - CHANCE_SINGLE;
        const int CHANCE_CHILD = 25;

        const int MIN_AGE_ADULT = 40;
        const int MAX_AGE_ADULT = 20;

        const int MIN_AGE_CHILD = 15;
        const int MAX_AGE_CHILD = 8;

        const int MAX_OCCUPANTS = 4;

        public Building[] buildings;
        public GameObject npcPrefab;
        public List<NPCController> allNPCS = new List<NPCController>();

        RobLogger RL;

        struct NPCAttributes
        {
            public string name;
            public string lastname;
            public bool gender;
            public int age;
        }

        void Awake()
        {
            RL = RobLogger.GetRobLogger();
            buildings = FindObjectsOfType<Building>();
            RL.writeInfo("Will setup " + buildings.Length + " buildings!");
        }

        public void GenerateNPCS()
        {
            for (int i = 0; i < buildings.Length; i++)
            {
                List<NPCController> occupants = new List<NPCController>();
                // Family or no?
                int sof = Random.Range(0, 100);
                if (sof < CHANCE_SINGLE)
                {
                    // all by myself
                    NPCAttributes me = createRandomNPC(false, false, "", false);
                    GameObject meGO = GameObject.Instantiate(npcPrefab, buildings[i].transform);
                    NPCController meGONPC = meGO.GetComponent<NPCController>();
                    if (meGONPC == null)
                    {
                        RL.writeError("NOOOOOOOOO NPC Prefab has wrong NPCCONTROLLER COMPIIT");
                        throw new System.Exception("NPC prefab for level generator does not have a NPCController component");
                    }

                    meGONPC.initialize(me.gender, me.name, me.lastname, me.age);
                    occupants.Add(meGONPC);
                    // Relationship setup here
                    allNPCS.Add(meGONPC);
                }
                else
                {
                    // family boi
                    // Man
                    NPCAttributes man = createRandomNPC(true, false, "", false);
                    GameObject manGO = GameObject.Instantiate(npcPrefab, buildings[i].transform);
                    NPCController manGONPC = manGO.GetComponent<NPCController>();
                    if (manGONPC == null)
                    {
                        RL.writeError("NOOOOOOOOO NPC Prefab has wrong NPCCONTROLLER COMPIIT");
                        throw new System.Exception("NPC prefab for level generator does not have a NPCController component");
                    }

                    manGONPC.initialize(man.gender, man.name, man.lastname, man.age);
                    occupants.Add(manGONPC);

                    // Woman
                    NPCAttributes wom = createRandomNPC(false, true, man.lastname, false);
                    GameObject womGO = GameObject.Instantiate(npcPrefab, buildings[i].transform);
                    NPCController womGONPC = womGO.GetComponent<NPCController>();
                    womGONPC.initialize(wom.gender, wom.name, wom.lastname, wom.age);
                    occupants.Add(womGONPC);

                    // Child 1?
                    sof = Random.Range(0, 100);
                    if (sof < CHANCE_CHILD)
                    {
                        // child 1!
                        NPCAttributes child = createRandomNPC(false, true, man.lastname, true);
                        GameObject childGO = GameObject.Instantiate(npcPrefab, buildings[i].transform);
                        NPCController childGONPC = childGO.GetComponent<NPCController>();
                        childGONPC.initialize(child.gender, child.name, child.lastname, child.age);
                        occupants.Add(childGONPC);
                        // Child relationship here
                        allNPCS.Add(childGONPC);
                    }
                    // Child 2?
                    sof = Random.Range(0, 100);
                    if (sof < CHANCE_CHILD)
                    {
                        // child 2!
                        NPCAttributes child = createRandomNPC(false, true, man.lastname, true);
                        GameObject childGO = GameObject.Instantiate(npcPrefab, buildings[i].transform);
                        NPCController childGONPC = childGO.GetComponent<NPCController>();
                        childGONPC.initialize(child.gender, child.name, child.lastname, child.age);
                        occupants.Add(childGONPC);
                        // Child relationship here
                        allNPCS.Add(childGONPC);
                    }

                    // Relationships setup here ?
                    allNPCS.Add(manGONPC);
                    allNPCS.Add(womGONPC);
                }

                // Give over the list of occupants
                buildings[i].initialize(occupants);

                // Now put the NPCs in place
                buildings[i].moveOccupantsToSpawns();
                RL.writeInfo("Building " + buildings[i].name + " initialized with " + occupants.Count + " NPCS.");
            }

            RL.writeInfo("Generated " + allNPCS.Count + " NPCS across " + buildings.Length + " buildings");
        }

        NPCAttributes createRandomNPC(bool forcedmale, bool forcedfemale, string enforcedLastName, bool child)
        {
            NPCAttributes toReturn;
            bool male;
            string fn;
            string ln;
            int age;

            if (forcedmale && forcedfemale)
            {
                male = true;
                RL.writeError("Both fm and ff set!");
            }
            else if (forcedmale)
            {
                male = true;
            }
            else if (forcedfemale)
            {
                male = false;
            }
            else
            {
                if (Random.Range(0, 2) == 0)
                {
                    male = false;
                }
                else
                {
                    male = true;
                }
            }

            if (male)
            {
                fn = maleFirstNames[Random.Range(0, maleFirstNames.Length)];
            }
            else
            {
                fn = femaleFirstNames[Random.Range(0, femaleFirstNames.Length)];
            }

            if (enforcedLastName.Length == 0)
            {
                ln = lastNames[Random.Range(0, lastNames.Length)];
            }
            else
            {
                ln = enforcedLastName;
            }

            if (child)
            {
                age = Random.Range(MIN_AGE_CHILD, MAX_AGE_CHILD);
            }
            else
            {
                age = Random.Range(MIN_AGE_ADULT, MAX_AGE_ADULT);
            }

            toReturn.name = fn;
            toReturn.lastname = ln;
            toReturn.gender = male;
            toReturn.age = age;

            return toReturn;
        }
    }
}