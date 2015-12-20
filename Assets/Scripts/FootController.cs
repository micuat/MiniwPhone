using UnityEngine;
using System.Collections;

public class FootController : MonoBehaviour {

    Quaternion InitialRotation;

	// Use this for initialization
	void Start () {
        Input.gyro.enabled = true;
        InitialRotation = Quaternion.identity;
	}
    // Update is called once per frame
    void Update () {
        transform.rotation = InitialRotation * ConvertRotation(Input.gyro.attitude);

        //float x = Input.GetAxis("Horizontal");
        //float z = Input.GetAxis("Vertical");
        //x *= Time.deltaTime * 0.1f;
        //z *= Time.deltaTime * 0.1f;
        //transform.Translate(x, 0, z);
    }

    private static Quaternion ConvertRotation(Quaternion q)
    {
        return new Quaternion(-q.x, -q.z, -q.y, q.w);
    }

    public void receiveButtonPressed()
    {
        InitialRotation = Quaternion.Inverse(ConvertRotation(Input.gyro.attitude));
        /*
        if (InitialRotation == Quaternion.identity)
        {
            InitialRotation = Quaternion.Inverse(ConvertRotation(Input.gyro.attitude));
        }
        else
        {
            InitialRotation = Quaternion.identity;
        }
        */

        Debug.Log("pressed");
    }
}
