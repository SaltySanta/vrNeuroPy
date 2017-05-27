using System.Collections;

using UnityEngine;

/** @brief Handles collisions of the "Character Controller" component
 *
 * @details
 * - Created on: November 2011
 * - Author: Marcel Richter
 *
 * \ingroup UnityVRClasses
 */
public class ControllerCollider : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// GameObjID of object we collided with
    /// </summary>
    public int HitID;

    /// <summary>
    /// GameObjName of object we collided with
    /// </summary>
    public string HitName = string.Empty;

    /// <summary>
    /// GameObject we collided with
    /// </summary>
    public GameObject HitGameObject;
    public float pushPower = 2.0F;

    /// <summary>
    /// Tag of object we collided with
    /// </summary>
    public string HitTag = string.Empty;

    #endregion Fields

	#region PrivateMethods	

    /// <summary>
    /// CollisionEventHandler
    /// </summary>
    /// <param name="hit"></param>
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit != null)
            if (hit.rigidbody != null)
            {
                this.HitTag = hit.rigidbody.transform.tag.ToString();
                this.HitName = hit.transform.gameObject.name.ToString();
                this.HitID = hit.transform.gameObject.GetInstanceID();
                this.HitGameObject=hit.transform.gameObject;
            }

        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3F)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * pushPower;
    }

    #endregion PrivateMethods
}