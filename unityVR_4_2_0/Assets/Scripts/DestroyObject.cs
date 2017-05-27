using UnityEngine;
using System.Collections;

public class DestroyObject : MonoBehaviour {

	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.name=="Lerpz"){

			Destroy (gameObject);
			
		}
	}
}
