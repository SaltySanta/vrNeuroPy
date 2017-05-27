
using System.Collections;

using UnityEngine;

/** @brief Spawn and destroy objects at Key Press 
 * \ingroup NotUsed
 **/
public class spawn : MonoBehaviour
{
    #region Fields

    public string DestroyKey;
    public string destroyTag;
    public string InstantiateKey;
    public Transform prefab;

    private Vector3 objPosition;
    private Quaternion objRotation;

    #endregion Fields

    #region Methods

    // Use this for initialization
    void Start()
    {
        //schreibe die Position des Spawnpoints in die Variable meinePosition
        objPosition = transform.position;
        //Schreibe die Richtung des Spawnpoints (Rotation) in die Variable meineRichtung
        objRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown (InstantiateKey)) {
            //füge mein Prefab in der Position und Richtung des Spawnpoints ein
            Instantiate (prefab, objPosition, objRotation);
        }
        if (DestroyKey != "" && destroyTag != "") {
            if (Input.GetKeyDown (DestroyKey)) {
                //lösche allle objekte mit dem tag spawnturm
                Destroy (GameObject.FindWithTag (destroyTag));
            }
        }
    }

    #endregion Methods
}
