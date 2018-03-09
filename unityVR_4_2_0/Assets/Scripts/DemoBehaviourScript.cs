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


public class DemoBehaviourScript : BehaviourScript {

	// Spawn point enumeration
	public enum level 
	{
		spawn1 = 1
	};

	// Selected spawn point
	public level select = level.spawn1;


	GameObject agent;

	GameObject[] spawns = null;

	// Initalization
	protected override void AgentInitalization()
	{		
		// find all agent OBJECTS  in the scene containing a specific script
		getAllAgentObjects<DemoAgentScript>();
		
		// find and init all agent SCRIPTS  in the scene containing a specific script
		getAndInitAllAgentScripts<DemoAgentScript>();

		// set default position to selected spawn point
		switch((int)select){
			case 1:
				agentScripts[0].DefaultAgentPosition = new Vector3(2.07f,0.25f,-11.9f); 
				break;
		}

		agentScripts[0].DefaultRotation = 0;

		Reset();
		
	}

}