using UnityEngine;
using System.Collections;

/** @brief Test for a single animation
 *
 * @details
 *
 * - Created on: October 2012
 * - Author: Jeannine (jeoe)
 *
 *
 * \ingroup jeoeTest
 */
public class animation_test : MonoBehaviour {
	
	public Transform Cathegory1;
	
	// Use this for initialization
	void Start () {
	
	Instantiate (Cathegory1, new Vector3(2.903169f,2.000727f,1.050051f), Cathegory1 .rotation);	
		
		
	}
	
	// Update is called once per frame
	void Update () {
					
	}
}
