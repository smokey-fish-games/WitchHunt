using UnityEngine;
using System.Collections.Generic;
public static class CONSTANTS
{
    // List of Items IDS
    public const int NO_ITEM = -1;
    public const int ITEM_BIBLE = 0;
    public const int ITEM_SPELLBOOK = 1;




    // Helper methods;
    private static Random rng = new Random();  

    public static void Shuffle<T>(this IList<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = Random.Range(0, n + 1);
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }

}