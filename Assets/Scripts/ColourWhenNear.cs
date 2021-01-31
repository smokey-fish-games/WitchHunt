using UnityEngine;
using System.Linq;
/* Tint the object when hovered. */

[RequireComponent(typeof(Collider))]
public class ColourWhenNear : MonoBehaviour {

    RobLogger RL;
    void Awake()
    {
        RL = RobLogger.GetRobLogger();
    }

	public Color color;
	public Renderer meshRenderer;

	Color[] originalColours;

	void Start() {
		if (meshRenderer == null) {
			meshRenderer = GetComponent<MeshRenderer> ();
		}
		originalColours = meshRenderer.materials.Select (x => x.color).ToArray ();
	}

	/// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            RL.writeInfo("Player Object: " + other.name + " entered SOI of " + this.name);
            foreach (Material mat in meshRenderer.materials) {
                mat.color *= color;
            }
        }
	}

	/// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            RL.writeInfo("Player Object: " + other.name + " left SOI of " + this.name);
            for (int i = 0; i < originalColours.Length; i++) {
                meshRenderer.materials [i].color = originalColours [i];
            }
        }
	}
}