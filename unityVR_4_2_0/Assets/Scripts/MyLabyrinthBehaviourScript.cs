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

public class MyLabyrinthBehaviourScript : BehaviourScript
{
	AgentScript ascr = null;
	
 	protected override void AgentInitalization()
    {
        // find all agent OBJECTS  in the scene containing a specific script
		getAllAgentObjects<MyLabyrinthAgentScript>();
		
		// find and init all agent SCRIPTS  in the scene containing a specific script
		getAndInitAllAgentScripts<MyLabyrinthAgentScript>();
		
		ascr = (MyLabyrinthAgentScript)GameObject.Find("Lerpz").GetComponent("MyLabyrinthAgentScript");
		
		moveAgent();
    }
	
	void moveAgent()
	{
		ascr.StartNewMovement (50.0f,90.0f);
		
	}
}

