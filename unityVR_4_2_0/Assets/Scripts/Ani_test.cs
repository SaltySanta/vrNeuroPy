using UnityEngine;
using System.Collections;

/** @brief Test for grasping animations
 *
 * @details
 *
 * - Created on: October 2012
 * - Author: Jeannine (jeoe)
 *
 *
 * \ingroup jeoeTest
 */
public class Ani_test : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKey (KeyCode.Y))
			//gameObject.animation.Play ("0_1");
			gameObject.animation.Play ("PY1_1");
		
		if (Input.GetKey (KeyCode.X))
			//gameObject.animation.Play ("0_2");
			gameObject.animation.Play ("P0_2");
		
		if (Input.GetKey (KeyCode.C))
			//gameObject.animation.Play ("0_3");
			gameObject.animation.Play ("PX1_1");
		
		if (Input.GetKey (KeyCode.V))
			//gameObject.animation.Play ("X0_1");
			gameObject.animation.Play ("PX1_2");
		
		if (Input.GetKey (KeyCode.B))
			//gameObject.animation.Play ("X0_2");
			gameObject.animation.Play ("px0_2");
		
		if (Input.GetKey (KeyCode.N))
			gameObject.animation.Play ("X0_3");
		
		if (Input.GetKey (KeyCode.I))
			gameObject.animation.Play ("X1_1");
		
		if (Input.GetKey (KeyCode.U))
			gameObject.animation.Play ("X1_2");
		
		if (Input.GetKey (KeyCode.F))
			gameObject.animation.Play ("X1_3");
		
		if (Input.GetKey (KeyCode.P))
			gameObject.animation.Play ("X2_2");
		
		if (Input.GetKey (KeyCode.O))
			gameObject.animation.Play ("X3_1");
		
		if (Input.GetKey (KeyCode.L))
			gameObject.animation.Play ("Y0_1");
		
		if (Input.GetKey (KeyCode.K))
			gameObject.animation.Play ("Y0_2");
		
		if (Input.GetKey (KeyCode.J))
			gameObject.animation.Play ("Y1_1");
		
		if (Input.GetKey (KeyCode.H))
			gameObject.animation.Play ("Y1_2");
		
		if (Input.GetKey (KeyCode.G))
			gameObject.animation.Play ("Y2_1");
		
		if (Input.GetKey (KeyCode.M)) {
			//float vertical=0;
			//float horizontal=1;
			gameObject.animation.Blend ("X0_3", 1F, 2F);
			//gameObject.animation.Blend ("show_right", 1-horizontal, 2.0F);
			gameObject.animation.Blend ("X0_2",1F, 2F);
			//gameObject.animation.Blend ("s0_1", 1-vertical, 2F);
		}
		
		
	}
}