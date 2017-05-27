using System;
using System.Collections;
using System.IO;

using SimpleNetwork;

using UnityEngine;

/** @brief Control class for an agent in Labyrinth scenario.
 *
 * @details
 * - Created on: August 2012
 * - Author: Michael Schreier, Marcel Richter
 * -Camera positions for lerpz: 
 *   Left1 and Left2 = [-0.1, 1.7, 0.45]
 *   Right1 and Right2 = [+0.1, 1.7, 0.45]
 * - Camera nearplanes: 0.3 (standard)
 *
 * \ingroup UnityVRClasses
 */
public class LabyrinthAgentScript : AgentScript
{
    #region Fields

    /// <summary>
    /// This is acomponent from Unity to move game objects physicaly correct arround.
    /// We use it to move the agent.
    /// </summary>
    private CharacterController TheCharacterControler;

    #endregion Fields

	 #region PublicMethods	
	
    
	#endregion PublicMethods

	#region ProtectedMethods	
	
    
    /// <summary>
    /// Unity doesnt allow conventional constructors, but provides this function for initialisations
    /// </summary>
    /*
    void Awake()
    {
        //initialisations
        this.TheCharacterControler = this.gameObject.GetComponent(typeof(CharacterController)) as CharacterController;
        ControllerColliderScript = this.gameObject.GetComponent("ControllerCollider") as ControllerCollider;
        this.headLookScript = this.gameObject.GetComponent("HeadLookController") as HeadLookController;

        // Reset the world rotation
        transform.rotation = Quaternion.identity;

        //intial reset
        this.AgentReset();
		tex_L =  new Texture2D(resWidth, resHeight, TextureFormat.RGB24, true);
		tex_R =  new Texture2D(resWidth, resHeight, TextureFormat.RGB24, true);
		rt_R = new RenderTexture(resWidth, resHeight, 24);
		rt_L = new RenderTexture(resWidth, resHeight, 24);

        rt_R.useMipMap = true;
		rt_L.useMipMap = true;
    }
	*/
	
	/**
	 * Overload init-function from configuration to make movement of the agent faster (at 400%) 
	 */
	public override void InitializeFromConfiguration (cConfiguration config, string localIP, int ipPortOffset)
	{
		base.InitializeFromConfiguration(config, localIP, ipPortOffset);		
		this.MovementSpeed = 4*this.MovementSpeed;
		
		GoodWalkAnimationSpeed = 1f;
	}
	
	#endregion ProtectedMethods	

}
