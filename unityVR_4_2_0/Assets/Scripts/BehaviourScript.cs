#region Header

/** @mainpage  "Simulating of cognitive agent with active environment interaction in a virtual reality"
 *  This document describes all classes and structures of the virtual reality application, the main-class is BehaviourScript .
 *
 * \authors
 *    - Frederik Beuth - project manager (beuth@hrz.tu-chemnitz.de)
 *    - Marcel Richter (richterthepok@yahoo.de)
 *    - Michael Schreier (michaels@hrz.tu-chemnitz.de)
 *    - Jeaninne Oertel (jeoe@hrz.tu-chemnitz.de)
 *    - Xiaomin Ye
 * \version    2.3
 * \date Created on: November 2011, Modified on: June 2013
 *
 * @section apr1 General idea
 * The BehaviourScript controls the whole VR. It assigns networkinterfaces to the agents, and has
 * a own networkinterface to receive commands concerning the running experiment/trail. The AgentScript controls the
 * agent figure. It sends images with its networkinterface and receives commands from the agentprogramm, that possibley is located on
 * an other computer.
 *
 * @section apr2 How to start
 * - Start Unity, hold ALT to open the project selection dialog and select the Unityprojectfolder. 
 * - With double click on a c#-script, it will be opened in your favourite editor. Alternatively, you can open the Visual Studio 2010 projectfile "unityVR.sln"
 *
 * @subsection apr3 Missing unity-references in the visual studio project:
 * The location of the UnityEditor is from PC to PC different, so you maybe have to re-add
 *  missing references, before you can compile the code:
 * - /Unity/Editor/Data/Mono/lib/mono/unity/Boo.Lang.dll
 * - /Unity/Editor/Data/Managed/UnityEditor.dll
 * - /Unity/Editor/Data/Managed/UnityEngine.dll
 * - /Unity/Editor/Data/Mono/lib/mono/unity/UnityScript.dll
 * - /Unity/Editor/Data/Mono/lib/mono/unity/Boo.Lang.dll
 *
 * @section apr4 Development environment:
 * - Visual Studio 2010
 * - Windows 7.
 * - .NET 3.5
 * - Unity 3.5.2
 *
 * @section apr5 Running the Code:
 * @subsection C# controll programm
 * - Make sure Networktest is the Project that gets executed.
 * - Press f5
 * - Restart the project in the Unity-Editor
 * - Press connect in the Networktesting-app
 * -- Control the Agent and watch the received cammera input :)
 * @subsection ANNarchy/C++ programm
 * - Install ANNarchy 2.3 and follow the instruction in the file INSTALL, section "Install the ANNarchy in connection with the virtual reality".
 *
 * @section apr6 Configuration of the VR before the first start:
 * The VR reads port and IP at which it is listening on the network from the file APPConfig.config in the unityprojectfolder.
 * Copy it from the install-folder, and set your own values. 
 * There a one port for vr-releated messages (standard 1337) and one for agent related messages (standard 1338).
 * The value in the file is for the vr-related port, the agent port has always the next number.
 *
 * See also the documentation for the networkinterface <A HREF="../../network-doc/html/index.html">here</a>.
 *
 * @section apr7 Predefined scenarios
 * A scenario describes a certain (neuronal) simulation and its experimental setup, called scene. Each scenario serves as a test for the VR and should be repeated before the release of the software. At VR-side, the scenario contains normally an derivated behaviour and Agentscript. At agent-side, the source code is in the file networkInterfaceCPP/ProtoTest_ANN2_2_R3/src/main.cpp. At the moment, the following scenarios are defined:
 *	- <B>Children's room:</B> Basic scenario showing a room for a small child (age 2-4). This is a nice demonstration of the VR abilities and also deeply test the childrenroom scenario and the child agent. For the testsetup, see testCasesChildroom.txt. VR-Class: ChildrensRoomAgentScript, Agent-Class: main.cpp, mode 9.
 *	- <B>Five objects at the table:</B> Demonstrate an object recognition scenario. The child has the task to point to the correct object. in each trial, five objects are spawn at 5 random positions on the table. VR-Class: FiveObjectsAtTableAgent, Agent-Class: demoSchueler20120102/src/demoR2.m .
 *	- <B>Brain-Visio-Agent:</B> Showing neuronal firing rates in the VR. The VR has the ability to show online neuronal firing rates of an attached agent. This is demonstrated by this scenario for the neuronalfield example. VR-Class: BrainVisioAgent, Agent-Class: main.cpp, mode 6.
 *	- <B>Labyrith:</B> A reinforcement learning agent must find its way trough a labyrinth. This was mainly intended to demonstrate the available sensor and actions (see testCasesEng.txt). The agent is already very old (January 2011), however it will run with the current VR environment. VR-Class: LabyrinthAgentScript, Agent-Class: main.cpp, mode 3.
 *	- <B>Labyrith (LabyrinthAgentScript) with Sync:</B> A setup to demonstrate synchonised time in the agent and in the scene. In this scenario, a reinforcement learning agent must find its way trough a labyrinth. VR-Class: LabyrinthAgentScript, Agent-Class: main.cpp, mode 7.
 *
 *
 * @section apr9 Synchron mode
 * The virtual reality and the agent can run in asynchron, intended to execute a real time demonstration and synchron mode, intended to simulate a psychological experiment. 
 * In the asynchron mode, the VR is executed a swiftly as possible (called realtime). In synchron mode, the VR and all agent uses the same time scale which is also independent from
 * the real passed time, therefore precise experimental timings can be simulated. To achieve this, a simulation time is setup in the VR and it synchronise all agent with it.
 * For more details about the usage and implementation, see this <A HREF="../synchronDescription.pdf">PDF file</a>.
 *
 * @section apr8 Class diagramm
 *
 * <A HREF="../VR-Classdiagram.pdf">Class diagramm</a>.
 *
 * The several scenarios are implemented as derivated classes from BehaviourScript and AgentScript, for details and how to create a new scenario, see <A HREF="../AgentScript_BehaviorScript.pdf">here</a>.
 *
 * \image html VR-Classdiagram.png
 *
 *
 * \defgroup UnityVRClasses Main classes for the UnityVR
 * \defgroup NotUsed Classes with no usage at the moment
 * \defgroup michaelsTest Test classes of Michael Schreier (michaels)
 * \defgroup jeoeTest Test classes of Jeannine Oertel (jeoe)
 */

#endregion Header

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

/** @brief
 * This class controls the whole VR. 
 *
 * It assigns Networkinterfaces to the Agents, and has a own networkinterface to receive commands that concern the running trail.
 * It's not indented that this class is directly attached to a 3D Object in Unity.
 * Therefore the user must create a derivided class for his scenario,
 * where the behavior to protobuf messages can be overridden.
 *
 * - Created on: November 2011
 * - Author: Marcel Richter
 *
 * - Modified: September 2012
 * - Author: Michael Schreier
 * @details Responsibility of this class
 * - Network object creation
 * - Spans agents
 * - Create and delete barriers
 * - GUI/Button handling (BehaviourScript::OnGUI)
 * - Actions
 * - Save camera images to disk for Debug purposes (BehaviourScript::SaveScreenPNG)
 *
 * \ingroup UnityVRClasses
 */
public partial class BehaviourScript : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Access to the agent GameObjects in scene
    /// </summary>
    protected GameObject[] agents;

    /// <summary>
    /// Access to the script controling the Agent
    /// </summary>
    protected AgentScript[] agentScripts;

    /// <summary>
    /// We make copys of this GameObject (a PreFab), if we want to add new Barriers (GUI function...) 
    /// </summary>
    protected GameObject Barrier;

    /// <summary>
    /// Global configuration object
    /// </summary>
    protected cConfiguration config;

    /// <summary>
    /// Networkinterface used by the VR
    /// </summary>
    protected SimpleNet MySimpleNet;

    /// <summary>
    /// name of the global configuration file.
    /// </summary>
    public string ConfigFileName = "APPConfig.config";

    #endregion Fields

	#region PublicMethods	
	

    /// <summary>
    /// Fired when VR shuts down
    /// </summary>
    public void OnApplicationQuit()
    {
        //shut down the networkinterface gracefully
        if (MySimpleNet != null)
        {
            Debug.Log("Stopping network...");
            MySimpleNet.Stop();
        }
		
		
    }
	 #endregion PublicMethodss
	
	
	
	
	
	 #region ProtectedMethods
    /// <summary>
    /// Creation of the Agent GameObjects in the scene (should be overridden in a derivided class)
    /// </summary>
    protected virtual void AgentInitalization()
    {
    }

    /// <summary>
    /// Behavior function for MsgEnvironmentReset (should be overridden in a derivided class)
    /// </summary>
    /// <param name="msg">MsgEnvironmentReset object</param>
    protected virtual void ProcessMsgEnvironmentReset( MsgEnvironmentReset msg )
    {	
		Reset();
    }

    /// <summary>
    /// Behavior function for MsgTrialReset (should be overridden in a derivided class)
    /// </summary>
    /// <param name="msg">MsgTrialReset object</param>
    protected virtual void ProcessMsgTrialReset( MsgTrialReset msg )
    {
        Reset();
    }

    /// <summary>
    /// Remove all Barrierobjects - Used by the VR- menu and resetfuntion
    /// </summary>
    protected void RemoveAllBarriers()
    {
        //delete everything with the tag "Barrier"
        GameObject[] Barriers = GameObject.FindGameObjectsWithTag("Barrier");
        foreach (GameObject obj in Barriers)
            Destroy(obj);
    }

    /// <summary>
    /// Resets the VR, you have to define your scenarios here!
    /// </summary>
    protected void Reset()
    {
		
		for(int i = 0; i < agentScripts.Length; i++)
		{
			agentScripts[i].AgentReset();
		}
		
        
        RemoveAllBarriers();

        //Want to create some objects on the Fly? learn how:
        //http://unity3d.com/support/documentation/Manual/Instantiating%20Prefabs.html
    }

    /// <summary>
    /// Make a Screenshot of VR-view
    /// </summary>
    /// <returns></returns>
    protected IEnumerator SaveScreenPNG()
    {
        Debug.Log("----SavePNG() called------");

        // We should only read the screen bufferafter rendering is complete
        yield return new WaitForEndOfFrame();
        //yield return 0;

        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        Debug.Log("---SavedScreen.png--- unter : " + Application.dataPath.ToString() + " with " + bytes.Length.ToString());

        // For testing purposes, also write to a file in the project folder
        try
        {
            File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
        }
        catch (Exception exe)
        {
            Debug.Log(exe.ToString());
        }
    }
	#endregion ProtectedMethods	
	
    #region InternalMethods	
	
	/// <summary>
	/// Search for all agent objects in the scene, recognizable by tag "agent".
	/// Have to search only for a specific type T of agentScripts as one object could contain several scripts.
	/// </summary> 
	protected void getAllAgentObjects<T>() where T : AgentScript
	{		        
		GameObject[] unsortedAgents = GameObject.FindGameObjectsWithTag("agent");
				
		//BubbleSort: sort list after AgentID
		GameObject tmpObj;		
		for (int n=unsortedAgents.Length; n>1; n--){
    		for (int i=0; i<n-1; i++){
				GameObject A1 = unsortedAgents[i];
				GameObject A2 = unsortedAgents[i+1];
      			if (A1.GetComponent<T>().AgentID > A2.GetComponent<T>().AgentID){
        	    	tmpObj = unsortedAgents[i];
					unsortedAgents[i] = unsortedAgents[i+1];
					unsortedAgents[i+1] = tmpObj;
      			} 
			} 
  		} 
		agents = unsortedAgents;
		
	}
	
	/// <summary>
	/// Search for all agentScripts of type T, store them in "agentScripts" and init them.
	/// The search is based on the agent stored in list "agents", see call getAllAgentObjects() before this function
	/// Have to use a specific type as one object could contain several scripts.
	/// </summary>
	protected void getAndInitAllAgentScripts<T>() where T : AgentScript
	{
		agentScripts = new AgentScript[agents.Length];
		Debug.Log("Init "+agents.Length+" agents...\n");
		
		for(int i = 0; i < agents.Length; i++)
		{
			agentScripts[i] = agents[i].GetComponent<T>();
			T curAgentScript = (T)agentScripts[i];	 
			curAgentScript.InitializeFromConfiguration( config, GetLocalIP(), agentScripts[i].AgentID );		
        	//Do NOT call agentScripts [i].AgentReset (), here this should be done at runtime (see AgentScript::update() )
		}
	}
	
	/// <summary>
    /// Get local IP
    /// </summary>
	protected string GetLocalIP()
	{
		if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
    	{
        	return null;
    	}

    	IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

  
  		foreach(IPAddress i in host.AddressList)
		{
			if(i.AddressFamily == AddressFamily.InterNetwork)
				return i.ToString();
		}
			
		return null;
	}
	

    /// <summary>
    /// Unity doesnt allow conventional constructors, but provides this function for initialisations
    /// </summary>
    void Awake()
    {
		
		if(this.enabled)
		{
		
	        //load configfile
	        config = new cConfiguration(ConfigFileName);
			
			string  localIP = GetLocalIP();
			
	        Debug.Log("Local IP: " + localIP);
	        Debug.Log("Local environment port: " + config.LocalPort);
			//config.SyncMode = false;
			Debug.Log("SyncMode: " + config.SyncMode);
	
	        //""
	        MySimpleNet = new SimpleNet(localIP, config.LocalPort);
	        AgentInitalization();
	        // Make the game run even when in background
	        Application.runInBackground = true;
	
	        //Simulationspeedup
	        //>1 faster
	        //<1 slower
	        Time.timeScale = 1;
		}
    }

    /// <summary>
    /// Update is called once per frame and handles the VR-logic and messaging
    /// For agent related sensor events and actions, see AgentScript 
    /// </summary>
    void Update()
    {
        #region handle incoming msg
        if(MySimpleNet != null)

        if (MySimpleNet.MsgAvailable())
        {
            MsgObject NextMsg = MySimpleNet.Receive();

            //Enviroment reset in the Moment only one configuration
            //MsgEnvironmentResetCodePosition
            if(NextMsg.msgEnvironmentReset != null)
                ProcessMsgEnvironmentReset( NextMsg.msgEnvironmentReset );

            //MsgTrailResetCodePosition
            //do what ever you want ;D
            //for testing, same as EnviromentReset
            if(NextMsg.msgTrialReset != null)
                ProcessMsgTrialReset( NextMsg.msgTrialReset );

        }
        #endregion
    }

    #endregion InternalMethods
}
