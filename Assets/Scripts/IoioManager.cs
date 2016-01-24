using UnityEngine;
using System.Collections;
using Rug.Osc;

public class IoioManager : MonoBehaviour {

    private static AndroidJavaObject androidObject;
    public GameObject foot;
    public GameObject floor;
    private OscFloorReceiver floorReceiver;
    bool stepped = false;

    // Use this for initialization
    void Start () {
        floorReceiver = floor.GetComponent<OscFloorReceiver>();

        AndroidJNI.AttachCurrentThread();
        AndroidJavaClass javaClass = new AndroidJavaClass("com.naotohieda.miniwphone.UnityTest");
        // Debug.Log ("Start javaClass: " + javaClass);
        androidObject = javaClass.GetStatic<AndroidJavaObject>("ctx");
    }

    float Map(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        float outVal = ((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin);
        return outVal;
    }

    // Update is called once per frame
    void Update () {
        if (androidObject != null)
        {
            var p = foot.transform.position;
            float fsr = androidObject.Get<float>("fsr");
            if (fsr < 850)
            {
                fsr = 850;
                if(stepped == false)
                    floorReceiver.TriggerMessage(new OscMessage("/niw/client/aggregator/floorcontact", "add", 0, -0.15f, 0.15f));
                stepped = true;
            }
            else
            {
                stepped = false;
            }

            fsr = Map(fsr, 850, 1024, 0, 0.25f);
            p.y = p.y * 0.9f + fsr * 0.1f;
            foot.transform.position = p;

            //Debug.Log("pressed (init) " + androidObject.GetStatic<int>("testVal"));
            //androidObject.Call("initLooper");
        }
    }

    public void ReceiveButtonPressed()
    {
        if(floorReceiver.TileType[floorReceiver.FloorTL] == floorReceiver.IceMaterial)
            floorReceiver.TriggerMessage(new OscMessage("/niw/preset", 4, "snow"));
        else
            floorReceiver.TriggerMessage(new OscMessage("/niw/preset", 4, "ice"));

        if (androidObject != null)
        {
            Debug.Log("pressed (init) " + androidObject.GetStatic<int>("testVal"));
            //androidObject.Call("toggleEffect");
            int b = androidObject.Get<int>("button_");
            if (b == 0) b = 1;
            else b = 0;
            androidObject.Set<int>("button_", b);
        }
        else
        {
            Debug.Log("pressed (no init)");
        }
    }
}
