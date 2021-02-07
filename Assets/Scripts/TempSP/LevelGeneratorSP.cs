using UnityEngine;

public class LevelGeneratorSP : MonoBehaviour
{
    // TODO randomly source these!
    string[] maleFirstNames = new string[] { "Rob", "Paul", "Chris", "Steve", "Joe", "Jim", "Chad", "Mike", "Jake", "Dan" };
    string[] femaleFirstNames = new string[] { "Josie", "Jo", "Olivia", "Kat", "Polina", "Anne-Marie", "Delia", "Sylvia", "Amy", "Ashley" };
    string[] lastNames = new string[] { "Parker", "Messa", "Edmonds", "Cook", "Bean", "Bulmer", "Nash", "Pentalioti", "Pavitt", "Zotova", "Chaloner ", "Cheung", "Carter", "McDonald", "Harris", "Squires", "Kirkpatrick", "Hampton" };
    public const int CONSTANT_MIN_NEGATIVE = -100;
    public const int CONSTANT_HATED = -75;
    public const int CONSTANT_DISLIKED = -35;
    public const int CONSTANT_NEUTRAL = 0;
    public const int CONSTANT_LIKED = 35;
    public const int CONSTANT_LOVED = 75;
    public const int CONSTANT_MAX_POSTIVE = 100;

    // -100 -> -75  = Hated
    // -74  -> -35  = Disliked
    // -34  ->  34  = Neutral
    //  35  ->  74  = Liked
    //  75  ->  100 = Loved

    public int MIN_ATTITUDE_PARENT = CONSTANT_HATED;
    public int MAX_ATTITUDE_PARENT = CONSTANT_MAX_POSTIVE;

    public int MIN_ATTITUDE_CHILD = CONSTANT_DISLIKED;
    public int MAX_ATTITUDE_CHILD = CONSTANT_MAX_POSTIVE;

    public int MIN_ATTITUDE_SIBLING = CONSTANT_MIN_NEGATIVE;
    public int MAX_ATTITUDE_SIBLING = CONSTANT_MAX_POSTIVE;

    public int MIN_ATTITUDE_FRIEND = CONSTANT_LIKED;
    public int MAX_ATTITUDE_FRIEND = CONSTANT_LOVED;

    public int MIN_ATTITUDE_ENEMY = CONSTANT_DISLIKED;
    public int MAX_ATTITUDE_ENEMY = CONSTANT_MIN_NEGATIVE;

    public int MIN_ATTITUDE_ACQUAINTANCE = CONSTANT_DISLIKED;
    public int MAX_ATTITUDE_ACQUAINTANCE = CONSTANT_LIKED;

    public int MIN_ATTITUDE_UNKNOWN = CONSTANT_NEUTRAL;
    public int MAX_ATTITUDE_UNKNOWN = CONSTANT_NEUTRAL;


    public int numberOfBuildings = 10;

    public int CHANCE_SINGLE = 30;
    public int CHANCE_FAMILY = 70;

    public int CHANCE_CHILD = 25;

    public NPCControllerSP[] GenerateNPCS()
    {
        return null;
    }
}
