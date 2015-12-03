using UnityEngine;
using System.Collections;
using Rug.Osc;

public class OscFloorReceiver : ReceiveOscBehaviourBase
{
    public Material SandMaterial;
    public Material WaterMaterial;
    public Material IceMaterial;
    public Material CanMaterial;

    public GameObject FloorTL;
    public GameObject FloorTR;
    public GameObject FloorBL;
    public GameObject FloorBR;

    protected override void ReceiveMessage(OscMessage message)
    {

        Debug.Log("Receive floor");

        if (message.Count != 2)
        {
            //Debug.LogError(string.Format("Unexpected argument count {0}", message.Count));  

            return;
        }

        if (!(message[0] is int) ||
            !(message[1] is string))
        {
            //Debug.LogError(string.Format("Unexpected argument type"));  

            return;
        }

        // get the position from the message arguments 
        int id = (int)message[0];
        string texture = (string)message[1];
        Debug.Log(id + " " + texture);

        GameObject tile;
        if (id == 1)
        {
            tile = FloorBR;
        }
        else if (id == 2)
        {
            tile = FloorTR;
        }
        else if (id == 3)
        {
            tile = FloorBL;
        }
        else if (id == 4)
        {
            tile = FloorTL;
        }
        else
        {
            Debug.Log("Bad tile id!");
            return;
        }

        Material material;
        if(texture == "sand")
        {
            material = SandMaterial;
        }
        else if (texture == "water")
        {
            material = WaterMaterial;
        }
        else if (texture == "ice")
        {
            material = IceMaterial;
        }
        else if (texture == "can")
        {
            material = CanMaterial;
        }
        else
        {
            Debug.Log("Bad texture!");
            return;
        }

        tile.GetComponent<Renderer>().material = material;
    }
}
