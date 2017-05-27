using System.Collections;

using UnityEngine;

/**
 * @brief Script for a which turns out a lightswitch on collision
 * \ingroup UnityVRClasses
 */
public class switchLight : MonoBehaviour
{
    #region Fields

	/// <summary>
    /// Light object which the script should control
    /// </summary>
    public Light linkedLight;

    #endregion Fields

    #region Methods

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "LightSwitch")
            linkedLight.enabled = !linkedLight.enabled;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    #endregion Methods
}
