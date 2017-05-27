using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using SimpleNetwork;

using UnityEngine;

/** @brief Control class for the environment in Children Playground scenario.
 *
 * @details
 * - Created on: August 2012
 * - Author: Michael Schreier
 *
 * \ingroup UnityVRClasses
 */
public class ChildrensRoomBehaviorScript : BehaviourScript
{
	/// <summary>
	/// A list of spawned object on the table.
	/// </summary>
	GameObject [] spawnedObjs = null;
	
	
	#region ProtectedMethods		

    /// <summary>
    /// Overridden function for creation of the agent gameObject in the scene: you can spawn N agents at a fixed position.
    /// </summary>
    protected override void AgentInitalization()
    {
		// find all agent OBJECTS  in the scene containing a specific script
		getAllAgentObjects<ChildrensRoomAgentScript>();
		
		// find and init all agent SCRIPTS  in the scene containing a specific script
		getAndInitAllAgentScripts<ChildrensRoomAgentScript>();
		
		spawnObjectOnTable();
		
    }
	
	protected override void ProcessMsgEnvironmentReset( MsgEnvironmentReset msg )
    {			
		switch(msg.Type)
		{
		case 0:
			agentScripts[0].DefaultAgentPosition = new Vector3(0f,0f,10f); 
			agentScripts[0].DefaultRotation = 180;break;
		case 1:
			agentScripts[0].DefaultAgentPosition = new Vector3(2.3f,0f,2.42f); 
			agentScripts[0].DefaultRotation = 180;break;
		case 2:
			GameObject.Find("Grasp1").tag = "Untagged"; break;
		default:
			agentScripts[0].DefaultAgentPosition = new Vector3(0f,0f,10f); break;
			
		}		
		spawnObjectOnTable();
		
		Reset();
    }
	
	protected override void ProcessMsgTrialReset( MsgTrialReset msg )
    {			
		switch(msg.Type)
		{
		case 0:
			agentScripts[0].DefaultAgentPosition = new Vector3(0f,0f,10f); 
			agentScripts[0].DefaultRotation = 180;
			break;
		case 1:
			agentScripts[0].DefaultAgentPosition = new Vector3(2.3f,0f,2.42f); 
			agentScripts[0].DefaultRotation = 180;
			break;
		default:
			agentScripts[0].DefaultAgentPosition = new Vector3(0f,0f,10f); break;
			
		}
		Reset();
    }
	
	protected void spawnObjectOnTable()
	{
		
		//spawn objects on the table
		if(spawnedObjs == null)		
		{
			spawnedObjs = new GameObject[6];
		}
		else
		{
			for(int i=0; i<6; i++)
				UnityEngine.Object.Destroy(spawnedObjs[i]);
		}
		
		spawnedObjs[0] = (GameObject)Instantiate (Resources.Load("BallGrasp") as UnityEngine.Object, new Vector3(2.04f, 1.9f, 1.2f), Quaternion.identity);
		spawnedObjs[0].tag="usable"; spawnedObjs[0].name="Grasp1";
		spawnedObjs[1] = (GameObject)Instantiate (Resources.Load("BallGrasp") as UnityEngine.Object, new Vector3(2.75f, 1.9f, 1.7f), Quaternion.identity);
		spawnedObjs[1].tag="usable"; spawnedObjs[1].name="Grasp2";
		spawnedObjs[2] = (GameObject)Instantiate (Resources.Load("BallGrasp") as UnityEngine.Object, new Vector3(2.3f, 1.9f, 1.5f), Quaternion.identity);
		spawnedObjs[2].tag="usable"; spawnedObjs[2].name="Grasp3";
		
		spawnedObjs[3] = (GameObject)Instantiate (Resources.Load("car_crane_yellow_res") as UnityEngine.Object, new Vector3(1f, 1.9f, 1.16f), Quaternion.Euler(-90,0,0));
		spawnedObjs[3].tag="usable"; spawnedObjs[3].name="car_crane_yellow"; spawnedObjs[3].transform.localScale = new Vector3(0.00066f, 0.00066f, 0.00066f);
		spawnedObjs[4] = (GameObject)Instantiate (Resources.Load("open_top_machine_yellow_res") as UnityEngine.Object, new Vector3(2.75f, 1.9f, 1.13f), Quaternion.Euler(-90,0,0));
		spawnedObjs[4].tag="usable"; spawnedObjs[4].name="open_top_machine_yellow"; 
		
		//not graspable
		spawnedObjs[5] = (GameObject)Instantiate (Resources.Load("jelly_red_res") as UnityEngine.Object, new Vector3(2.18f, 1.9f, 1.45f), Quaternion.identity);
		spawnedObjs[5].tag="Untagged"; spawnedObjs[5].name="jelly_red";
	}

    #endregion ProtectedMethods	
}
