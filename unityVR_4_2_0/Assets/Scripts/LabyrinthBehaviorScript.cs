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

/** @brief Control class for the environment in Labyrinth scenario.
 *
 * @details
 * - Created on: August 2012
 * - Author: Michael Schreier, Marcel Richter
 *
 * \ingroup UnityVRClasses
 */
public class LabyrinthBehaviorScript : BehaviourScript
{


    #region ProtectedMethods
	
    /// <summary>
    /// Overridden function for creation of the agent gameObject in the scene: 
    ///   - It could create N agents at certain positions
    /// </summary>
    protected override void AgentInitalization()
    {
        const int numberOfAgents = 3;	
        agents = new GameObject[numberOfAgents];

        for(int i = 0; i < numberOfAgents; i++)
        {
			//Position for agent x_i = (0,-20,20,-40,40,-60,60,....)
			float pos = Mathf.Pow( -1,i ) * 20 * Mathf.Ceil(i/2f);
			
            agents[i] = Instantiate(Resources.Load("Lerpz") as UnityEngine.Object, new Vector3( pos , 0, 0), Quaternion.identity) as GameObject;
            //agents[i].AddComponent("LabyrinthAgentScript");
            agents[i].GetComponent<LabyrinthAgentScript>().AgentID = i;
            Debug.Log("Created " + (i+1) + " at x-pos "+pos);    
						
            //Get the Agent ready
            agents[i].GetComponent<LabyrinthAgentScript>().DefaultAgentPosition = new Vector3( pos , 0, 0);			
        }	
		
		
		// find and init all agent SCRIPTS  in the scene containing a specific script
		getAndInitAllAgentScripts<LabyrinthAgentScript>();
		
		//prevent that several cameras are rendered at top of each other: allow only 0st agent to display camera		
		for(int i = 1; i < numberOfAgents; i++)
			agentScripts[i].setDisplayCameraVR(false);
		
    }
	
	 #endregion ProtectedMethods


	 #region InternalMethods	
	 /// <summary>
    /// Definition and logic of the menu and debuging views of agents eyes.
    /// </summary>
    void OnGUI()
    {
        #region Menu

        int ButtonWidth = 80;
        int ButtonHeight = 20;

        GUI.Box(new Rect(10, 10, 100, 230), "Menu");// Make a background box

        if (GUI.Button(new Rect(20, 40, ButtonWidth, ButtonHeight), "100 Reward"))
        {
            //MsgRewardCodeLocation
            agentScripts[0].ReceiveReward(100);
        }

        if (GUI.Button(new Rect(20, 70, ButtonWidth, ButtonHeight), "New Barrier"))
        {
            //Creates a gameObject from an PreFab
            Instantiate(this.Barrier, new Vector3(0, 30, 0), Quaternion.identity);
        }

        if (GUI.Button(new Rect(20, 100, ButtonWidth, ButtonHeight), "Barriers Del"))
        {
            this.RemoveAllBarriers();
        }

        if (GUI.Button(new Rect(20, 130, ButtonWidth, ButtonHeight), "Screen Save"))
        {
            StartCoroutine(this.SaveScreenPNG());
        }

        if (GUI.Button(new Rect(20, 160, ButtonWidth, ButtonHeight), "View Save"))
        {
            //save to an hdd what the Agent sees
            this.agentScripts[0].SaveViewPNG();
        }

        if (GUI.Button(new Rect(20, 190, ButtonWidth, ButtonHeight), "Quit"))
        {
            Application.Quit();
        }

        if (GUI.Button(new Rect(20, 220, ButtonWidth, ButtonHeight), "Agenttest"))
        {
            agentScripts[0].SendMenuClick(55);
        }

       /* #endregion

        #region show cameraviews of the Agents

        // Left Eye
		
       Texture2D tex = agentScripts[0].GetCameraViewTexture(agentScripts[0].cameraLeft2);

        //Calculate Position on Screen
        int PosY = Screen.height - 210;
        int PosX = 10;
        int WidthAndHeight = 185;
        GUI.Box(new Rect(PosX, PosY, WidthAndHeight, WidthAndHeight + 15), "Left");
        GUI.Label(new Rect(PosX + 8, PosY + 18, WidthAndHeight - 10, WidthAndHeight - 10), tex);
        Destroy(tex);

        // Right Eye

   //     tex = agentScripts[0].GetCameraViewTexture(agentScripts[0].cameraRight2);
        PosX = PosX + WidthAndHeight + 15;
        GUI.Box(new Rect(PosX, PosY, WidthAndHeight, WidthAndHeight + 15), "Right");
        GUI.Label(new Rect(PosX + 8, PosY + 18, WidthAndHeight - 10, WidthAndHeight - 10), tex);

        Destroy(tex);
		*/
        #endregion
    }
	#endregion InternalMethods	

}
