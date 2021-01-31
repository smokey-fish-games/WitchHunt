using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName="New Item", menuName="World/Item")]
public class Item : ScriptableObject
{
    public int ID;
    private static Item[] allItems = new Item[0];
    new public string name = "New Item";
    public bool isEvidence = false;
    public Sprite icon;

    public override string ToString()
    {
        return ID + ":" + name + ":" + isEvidence + ":" + icon.name;
    }

    public static Item[] GetAll()
    {
        if(allItems.Length == 0)
        {
            lock (allItems)
            {
                if (allItems.Length == 0)
                {
                    allItems = Resources.LoadAll<Item>("ScripptableObjects/Items");
                    allItems = allItems.OrderBy(co => co.ID).ToArray();
                }
            }
        }
        return allItems;
    }

    public static Item GetItem(int ID)
    {
        Item[] fullList = GetAll();

        if(ID < 0 || ID >= fullList.Length)
        {
            Debug.LogError("ID was too something " + ID + " v " +  fullList.Length);
            return null;
        }
        else
        {
            return fullList[ID];
        }
    }
}
