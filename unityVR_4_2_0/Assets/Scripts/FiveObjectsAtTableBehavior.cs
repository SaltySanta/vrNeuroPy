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

/** @brief Control class for the environment in "Five Objects At Table" scenario.
 *
 * @details
 * - Created on: August 2012
 * - Author: Frederik Beuth, Michael Schreier
 * 
 * - 5 Positions to spawn objects (1=> left on table, 5=> right on table)
 * - 5 categories (blocks, cars, pens, sweets, toys), each with 3 different obejcts under 3 different colors
 * - 4 different trials of each 5 objects randomly choosen from all categories:
 *   - Trial 1: 4 random distractors and the target 'car' (race-car-green) at pos 5
 *   - Trial 2: 4 random distractors and the target 'car' (crane-red) at pos 5. It is the another car.
 *   - Trial 3: 4 random distractors and the target 'car' (crane-blue) at pos 1. It is same car, but with different color.
 *   - Trial 4: 4 random distractors and the target 'lego-brick' at pos 3. It is another object.
 * 
 *
 * \ingroup UnityVRClasses
 */
public class FiveObjectsAtTableBehavior : BehaviourScript
{
    #region Fields

    /// <summary>
    /// background texture for the text field.
    /// </summary>
    public Texture2D backTex;

    /// <summary>
    /// List of objects for the first cathegory
    /// </summary>
    public Transform[] Cathegory1;

    /// <summary>
    /// Objects for the second cathegory
    /// </summary>
    public Transform[] Cathegory2;

    /// <summary>
    /// Objects for the third cathegory
    /// </summary>
    public Transform[] Cathegory3;

    /// <summary>
    /// Objects for the fourth cathegory.
    /// </summary>
    public Transform[] Cathegory4;

    /// <summary>
    /// Objects for the fifth cathegory.
    /// </summary>
    public Transform[] Cathegory5;

    /// <summary>
    /// Spawn position for the first cathegory.
    /// </summary>
    public Transform Position1;

    /// <summary>
    /// Spawn position for the second cathegory.
    /// </summary>
    public Transform Position2;

    /// <summary>
    /// Spawn position for the third cathegory.
    /// </summary>
    public Transform Position3;

    /// <summary>
    /// Spawn position for the fourth cathegory.
    /// </summary>
    public Transform Position4;

    /// <summary>
    /// Spawn position for the fifth cathegory.
    /// </summary>
    public Transform Position5;


    private GameObject[] spawnedObjects = null; //list of currently spawned objects

    private int idState = 0;

    #endregion Fields

	#region ProtectedMethods	
	
    /// <summary>
    /// Overridden function for creation of the agent gameObject in the scene: 
    ///   - It will set the Cathegory variables to empty lists
    ///   - It also create one agent
    /// </summary>
    protected override void AgentInitalization()
    {		
        spawnedObjects = new GameObject[5];
        for (int i = 0; i < 5; ++i) {
            spawnedObjects [i] = null;
        }
		
		// find all agent OBJECTS  in the scene containing a specific script
		getAllAgentObjects<FiveObjectsAtTableAgent>();
		
		// find and init all agent SCRIPTS  in the scene containing a specific script
		getAndInitAllAgentScripts<FiveObjectsAtTableAgent>();
		
    }

    /// <summary>
    /// Overriden function for MsgTrialReset whereby the msg-entry "type" define the configuration: 
    ///   - For type == 0: this will place 5 random objects on the table 
    ///   - For type == [1:45]: this will place the object with number type in the middle of the table
    ///   - For type == 46: this will clear all objects on the table 
    ///   - For type == [50:53]: this will place 5 defined object on the table
    /// </summary>
    /// <param name="msg">MsgTrialReset object</param>
    protected override void ProcessMsgTrialReset(MsgTrialReset msg)
    {
        //destroy objects from last trial
        for (int i = 0; i < 5; ++i) {
                if (spawnedObjects [i] != null)
                    Destroy (spawnedObjects [i].gameObject);
                spawnedObjects [i] = null;
            }

        if (msg.Type == 0) {

            //determine which category at which position
            int[] fiveNumbers = { 1,2,3,4,5 };
            ShuffleArray (ref fiveNumbers, 5);

            System.Random rng = new System.Random ();

            for (int i = 0; i < 5; ++i) {

                int cathegoryNumber = fiveNumbers [i];

                Transform[] cathegory;
                switch (cathegoryNumber) {

                case 1:
                    cathegory = Cathegory1;
                    break;
                case 2:
                    cathegory = Cathegory2;
                    break;
                case 3:
                    cathegory = Cathegory3;
                    break;
                case 4:
                    cathegory = Cathegory4;
                    break;
                case 5:
                    cathegory = Cathegory5;
                    break;
                default:
                    continue;
                }

                Vector3 pos;
                switch (i + 1) {

                case 1:
                    pos = Position1.position;
                    break;
                case 2:
                    pos = Position2.position;
                    break;
                case 3:
                    pos = Position3.position;
                    break;
                case 4:
                    pos = Position4.position;
                    break;
                case 5:
                    pos = Position5.position;
                    break;
                default:
                    continue;
                }

                //select one random object from a category
                if (cathegory.Length > 0) {
                    int numberOfObjectsInCathegory = cathegory.Length;
                    int randomIndex = rng.Next (numberOfObjectsInCathegory);
                    spawnedObjects [i] = (GameObject)Instantiate (cathegory [randomIndex], pos, cathegory [randomIndex].rotation);
                }

            }
        }
        if( msg.Type >= 1 && msg.Type < 46)
        {
            int id = msg.Type;
            int offset;        //helper variable to calculate index
            Vector3 pos = Position3.position;

            //category 1
            if( id <= Cathegory1.Length )
            {
                id -= 1;
                spawnedObjects [0] = (GameObject)Instantiate (Cathegory1 [id], pos, Cathegory1 [id].rotation);
            }
            //category 2
            else if (id > Cathegory1.Length && id <= Cathegory1.Length +  Cathegory2.Length)
            {
                offset = Cathegory1.Length;
                id -= (offset + 1);
                spawnedObjects [0] = (GameObject)Instantiate (Cathegory2 [id], pos, Cathegory2 [id].rotation);
            }
            //category 3
            else if(id > Cathegory1.Length +  Cathegory2.Length && id <= Cathegory1.Length +  Cathegory2.Length + Cathegory3.Length)
            {
                offset = Cathegory1.Length +  Cathegory2.Length;
                id -= (offset + 1);
                spawnedObjects [0] = (GameObject)Instantiate (Cathegory3 [id], pos, Cathegory3 [id].rotation);

            }
            //category 4
            else if(id > Cathegory1.Length +  Cathegory2.Length + Cathegory3.Length && id <= Cathegory1.Length +  Cathegory2.Length + Cathegory3.Length + Cathegory4.Length)
            {
                offset = Cathegory1.Length +  Cathegory2.Length + Cathegory3.Length;
                id -= (offset + 1);
                spawnedObjects [0] = (GameObject)Instantiate (Cathegory4 [id], pos, Cathegory4 [id].rotation);
            }
            //category 5
            else if(id > Cathegory1.Length +  Cathegory2.Length + Cathegory3.Length + Cathegory4.Length && id <= Cathegory1.Length +  Cathegory2.Length + Cathegory3.Length + Cathegory4.Length + Cathegory5.Length)
            {
                offset = Cathegory1.Length +  Cathegory2.Length + Cathegory3.Length + Cathegory4.Length;
                id -= (offset + 1);
                spawnedObjects [0] = (GameObject)Instantiate (Cathegory5 [id], pos, Cathegory5 [id].rotation);
            }

        }
        //display background without object
        if( msg.Type == 46)
        {

        }

        idState = 0;

        if( msg.Type == 50 )
        {
            idState = 50;
			// Trial 1: 4 random distractors and the target 'car' (race-car-green) at pos 5
            spawnedObjects [0] = (GameObject)Instantiate (Cathegory3 [0].gameObject, Position1.position, Cathegory3 [0].rotation);
            spawnedObjects [4] = (GameObject)Instantiate (Cathegory5 [0].gameObject, Position2.position, Cathegory5 [0].rotation);
            spawnedObjects [2] = (GameObject)Instantiate (Cathegory1 [0].gameObject, Position3.position, Cathegory1 [0].rotation);
            spawnedObjects [3] = (GameObject)Instantiate (Cathegory4 [0].gameObject, Position4.position, Cathegory4 [0].rotation);
            spawnedObjects [1] = (GameObject)Instantiate (Cathegory2 [7].gameObject, Position5.position, Cathegory2 [7].rotation);
        }
        if( msg.Type == 51 )
        {
            idState = 51;
			// Trial 2: 4 random distractors and the target 'car' (crane-red) at pos 5. It is the another car.
            spawnedObjects [0] = (GameObject)Instantiate (Cathegory3 [1].gameObject, Position1.position, Cathegory3 [1].rotation);
            spawnedObjects [4] = (GameObject)Instantiate (Cathegory5 [1].gameObject, Position2.position, Cathegory5 [1].rotation);
            spawnedObjects [2] = (GameObject)Instantiate (Cathegory1 [3].gameObject, Position3.position, Cathegory1 [3].rotation);
            spawnedObjects [3] = (GameObject)Instantiate (Cathegory4 [1].gameObject, Position4.position, Cathegory4 [1].rotation);
            spawnedObjects [1] = (GameObject)Instantiate (Cathegory2 [1].gameObject, Position5.position, Cathegory2 [3].rotation);
        }
        if( msg.Type == 52 )
        {
            idState = 52;
			// Trial 3: 4 random distractors and the target 'car' (crane-blue) at pos 1. It is same car, but with different color.
			spawnedObjects [2] = (GameObject)Instantiate (Cathegory2 [0].gameObject, Position1.position, Cathegory2 [4].rotation);
            spawnedObjects [1] = (GameObject)Instantiate (Cathegory3 [2].gameObject, Position2.position, Cathegory3 [2].rotation);
            spawnedObjects [4] = (GameObject)Instantiate (Cathegory4 [2].gameObject, Position3.position, Cathegory4 [2].rotation);
			spawnedObjects [3] = (GameObject)Instantiate (Cathegory5 [3].gameObject, Position4.position, Cathegory5 [3].rotation);
			spawnedObjects [0] = (GameObject)Instantiate (Cathegory1 [4].gameObject, Position5.position, Cathegory1 [4].rotation);
            
        }
        if( msg.Type == 53 )
        {
            idState = 53;
			// Trial 4: 4 random distractors and the target 'brick' at pos 3. It is another object.
            spawnedObjects [2] = (GameObject)Instantiate (Cathegory2 [3].gameObject, Position1.position, Cathegory2 [3].rotation);
			spawnedObjects [1] = (GameObject)Instantiate (Cathegory5 [4].gameObject, Position2.position, Cathegory5 [4].rotation);
            spawnedObjects [0] = (GameObject)Instantiate (Cathegory1 [5].gameObject, Position3.position, Cathegory1 [5].rotation);            
            spawnedObjects [3] = (GameObject)Instantiate (Cathegory3 [4].gameObject, Position4.position, Cathegory3 [4].rotation);
            spawnedObjects [4] = (GameObject)Instantiate (Cathegory1 [2].gameObject, Position5.position, Cathegory1 [2].rotation);
        }
		
		
		
		
    }
	
	
	#endregion ProtectedMethods	
	
	#region PrivateMethods
    
    private static void ShuffleArray(ref int[] list, int length)
    {
        System.Random rng = new System.Random ();
        int n = length;
        while (n > 1) {
            n--;
            int k = rng.Next (n + 1);
            int value = list [k];
            list [k] = list [n];
            list [n] = value;
        }
    }
	#endregion PrivateMethods
	
	#region InternalMethods	
	
    void OnGUI()
    {
        GUIStyle gs = new GUIStyle();

        gs.normal.textColor = Color.black;
        gs.fontSize = 30;
        gs.normal.background = backTex;

        switch ( idState ) {

        case 50: 	GUI.Label(new Rect(10,10,300,35), "Zeige mir das Auto", gs);
            break;
        case 51: 	GUI.Label(new Rect(10,10,300,35), "Zeige mir das Auto", gs);
            break;
        case 52: 	GUI.Label(new Rect(10,10,300,35), "Zeige mir das Auto", gs);
            break;
        case 53: 	GUI.Label(new Rect(10,10,330,35), "Zeige mir den Baustein", gs);
            break;

        }
    }

   #endregion InternalMethods	
}
