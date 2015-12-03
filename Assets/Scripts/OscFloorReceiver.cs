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

    public GameObject CanPrefab;

    protected override void ReceiveMessage(OscMessage message)
    {

        Debug.Log("Receive floor");

        if(message.Address == OscAddress[0])
        {
            if (message.Count != 2)
            {
                Debug.LogError(string.Format("Unexpected argument count {0}", message.Count));

                return;
            }

            if (!(message[0] is int) ||
                !(message[1] is string))
            {
                Debug.LogError(string.Format("Unexpected argument type"));

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
            if (texture == "sand")
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
            var canSpawened = Instantiate(CanPrefab);
            canSpawened.transform.parent = tile.transform;
            canSpawened.transform.localPosition = new Vector3(0, 0.6f, 0);
        }
        else if(message.Address == OscAddress[1])
        {
            if((string)message[0] == "add")
            {
                Debug.Log((float)message[2] + " " + (float)message[3]);
                GameObject tile;
                if ((float)message[2] < 1 && (float)message[3] < 1)
                {
                    tile = FloorTL;
                }
                else if ((float)message[2] >= 1 && (float)message[3] < 1)
                {
                    tile = FloorTR;
                }
                else if ((float)message[2] < 1 && (float)message[3] >= 1)
                {
                    tile = FloorBL;
                }
                else if ((float)message[2] >= 1 && (float)message[3] >= 1)
                {
                    tile = FloorBR;
                }
                else
                {
                    return;
                }

                for(int i = 0; i < tile.transform.childCount; i++)
                {
                    Destroy(tile.transform.GetChild(i).gameObject);
                }
            }
        }
    }
}
