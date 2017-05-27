using System;
using System.Collections;

using SimpleNetwork;

using UnityEngine;

/** @brief Control class for an agent in the "Five Objects At Table" scenario.
 *
 * @details
 * - Created on: August 2012
 * - Author: Michael Schreier
 *
 * \ingroup UnityVRClasses
 */
public class FiveObjectsAtTableAgent : AgentScript
{
    #region Fields

    private int GrapIDExecuting;

    #endregion Fields

	 #region PublicMethods	
	

    	
	#endregion PublicMethods
	
	#region ProtectedMethods		
	
    /// <summary>
    /// Overriden behavior function for MsgAgentPointID: show a pointing to the objects on the table.
    /// </summary>
    /// <param name="msg">MsgAgentGraspID object</param> 	
	protected override void ProcessMsgAgentPointID( MsgAgentPointID msg )
    {
        int animationID = msg.objectID;

        GrapIDExecuting = msg.actionID;
		
		switch( animationID )
        {
        case 1:
            StartCoroutine(StartAnimation( "PY1_1" ));	// left		
            break;
			
        case 2:
            StartCoroutine(StartAnimation( "P0_2" ));   // middle left
            break;
			
        case 3:
            StartCoroutine(StartAnimation ( "PX1_1" )); // middle
            break;
			
        case 4:
            StartCoroutine(StartAnimation( "PX1_2" ));  // middle right
            break;
			
        case 5:
            StartCoroutine(StartAnimation( "px0_2" ));  // right
            break;
			
        default:
			Debug.Log("Unknown point ID: "+animationID);
                break;
        }
    }
	
	/// <summary>
	/// Overwriten here to disable ALL standard animations. 
	/// </summary>
	protected override void PlayAnimation ()
	{

	}

    
	
	 #endregion ProtectedMethods
	
	 #region PrivateMethods	
	
    private IEnumerator StartAnimation( string animationName )
    {
        animation[animationName].wrapMode = WrapMode.Once;
        animation.CrossFade( animationName );

        yield return new WaitForSeconds(4f);
        //it'd be better if the code checks animation.isPlaying, but this didn't work for me.
        //instead it waits 4 seconds
        //yield return WhilePlaying( animationName );
        Debug.Log(String.Format("ID: {0}", MovementIDExecuting));
        MySimpleNet.Send(new MsgActionExecutationStatus() { actionID = GrapIDExecuting, status = MsgActionExecutationStatus.Finished });
    }

    private IEnumerator WhilePlaying( string animationName )
    {
        do
        {
            yield return null;
        } while ( animation.IsPlaying( animationName)  );
    }

    #endregion PrivateMethods
}
