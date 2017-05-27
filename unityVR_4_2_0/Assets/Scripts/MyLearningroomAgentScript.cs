using System;
using System.Collections;
using SimpleNetwork;
using UnityEngine;



public class MyLearningroomAgentScript : AgentScript {

	// Amount already rotated of current rotation
	float totalRotation = 0;

	// Current target rotation
	float targetRotation;

	// Current target position
	Vector3 targetPosition;

	// Is a search currently active
	bool search = true;

	// Controls execution of search
	int searchStep = 0;
	
	
	/*protected override void Update(){
		base.Update ();

		// handle search
		if(search){
			switch(searchStep){
			case 0:
				//random rotation
				targetRotation = UnityEngine.Random.Range(-90f,90f);
				searchStep++;
				break;
			case 1:
				//turn
				if (totalRotation<Mathf.Abs(targetRotation))
					handleSearch (1);
				else{
					searchStep++;
					totalRotation = 0f;
				}
				break;
			case 2:
				//random distance
				float tmp = UnityEngine.Random.Range(1f,5f);
				targetPosition = this.transform.position + this.transform.rotation * new Vector3(0,0,tmp);
				searchStep++;
				break;
			case 3:
				//walk
				float dif = new Vector2(targetPosition.x - transform.position.x,targetPosition.y - transform.position.y).magnitude;
				handleSearch(2);
				break;
			}
		}
  	} */
	         //Update() function removed, because agent is controlled by client

	// Handles rotation and movement during search
	public void handleSearch(int step){
		// Rotation
		if (step == 1) {
			float currentAngle = transform.rotation.eulerAngles.y;
		    transform.rotation = Quaternion.AngleAxis (currentAngle + (Time.deltaTime * targetRotation), Vector3.up);
			totalRotation += Time.deltaTime * Mathf.Abs(targetRotation);
		}
		// Movement
		else if(step==2){
			Vector3 direction = targetPosition - transform.position;
			direction.y = 0;
			Vector3 v= Vector3.ClampMagnitude(direction * 2f, 2f);
			if(v.magnitude <= 0.1)
				searchStep = 0;		
			GetComponent<CharacterController>().SimpleMove(v);
		}
	
	}

	// If Lerpz collides with an object he turns around and contiues the search
	void OnControllerColliderHit(ControllerColliderHit hit){
		if (hit.gameObject.name != "Terrain") {
						targetRotation = 180f;
						searchStep = 1;
				}
	}
}
	