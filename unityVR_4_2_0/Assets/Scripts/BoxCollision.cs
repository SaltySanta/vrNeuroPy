#region Header

/** @} */
/** \ingroup NotUsed
 * @{ */

#endregion Header

using System.Collections;

using UnityEngine;

/**@brief Destroys object with the tag "box", when an object collides with it
 */
public class BoxCollision : MonoBehaviour
{
    #region Fields

    public Transform replacementObj;

    #endregion Fields

    #region Methods

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "box") {
            Destroy (collision.gameObject);
            Instantiate (replacementObj, transform.position, transform.rotation);
        }
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    #endregion Methods
}

/** @} */
