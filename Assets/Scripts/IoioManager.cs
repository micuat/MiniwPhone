using UnityEngine;
using System.Collections;
using Rug.Osc;

public class IoioManager : MonoBehaviour {

    private static AndroidJavaObject androidObject;
    public GameObject foot;
    public GameObject floor;
    private OscFloorReceiver floorReceiver;

    // Use this for initialization
    void Start () {
        floorReceiver = floor.GetComponent<OscFloorReceiver>();

        AndroidJNI.AttachCurrentThread();
        AndroidJavaClass javaClass = new AndroidJavaClass("com.naotohieda.miniwphone.UnityTest");
        // Debug.Log ("Start javaClass: " + javaClass);
        androidObject = javaClass.GetStatic<AndroidJavaObject>("ctx");
    }

    // Update is called once per frame
    void Update () {
        if (androidObject != null)
        {
            var p = foot.transform.position;
            float fsr = androidObject.Get<float>("fsr");
            if (fsr < 512)
            {
                fsr = 512;
                floorReceiver.TriggerMessage(new OscMessage("/niw/client/aggregator/floorcontact", "add", 0, -0.15f, 0.15f));
            }
            fsr -= 512.0f;
            fsr /= 512.0f;
            p.y = p.y * 0.9f + fsr * 0.1f;
            foot.transform.position = p;

            //Debug.Log("pressed (init) " + androidObject.GetStatic<int>("testVal"));
            //androidObject.Call("initLooper");
        }
    }

    public void ReceiveButtonPressed()
    {
        floorReceiver.TriggerMessage(new OscMessage("/niw/preset", 4, "snow"));

        if (androidObject != null)
        {
            Debug.Log("pressed (init) " + androidObject.GetStatic<int>("testVal"));
            //androidObject.Call("toggleEffect");
            androidObject.Set<bool>("button_", !androidObject.Get<bool>("button_"));
        }
        else
        {
            Debug.Log("pressed (no init)");
        }
    }
}
