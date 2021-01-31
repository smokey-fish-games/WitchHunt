using UnityEngine;
using System.Linq;

public class SpawnMarker : MonoBehaviour
{
    public enum SpawnType
    {
        BOX,
        NPC,
        PLAYER
    }

    public Color spawnMarkerColor = Color.magenta;
    public SpawnType typeToSpawnHere;


    void OnDrawGizmos()
    {
        Gizmos.color = spawnMarkerColor;
        Gizmos.DrawSphere(transform.position, .25f);
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 1f, 1f));
    }

    private static SpawnMarker[] allSpawnMarkers = new SpawnMarker[0];
    public static SpawnMarker[] GetAll()
    {
        if (allSpawnMarkers.Length == 0)
        {
            lock (allSpawnMarkers)
            {
                if (allSpawnMarkers.Length == 0)
                {
                    allSpawnMarkers = FindObjectsOfType<SpawnMarker>();
                }
            }
        }
        return allSpawnMarkers;
    }

    public static SpawnMarker[] GetAll(SpawnType typeFilter)
    {
        SpawnMarker[] all = GetAll();
        return all.Where(c => c.typeToSpawnHere == typeFilter).ToArray();
    }
}
