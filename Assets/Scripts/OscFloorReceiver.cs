using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rug.Osc;

public class OscFloorReceiver : ReceiveOscBehaviourBase
{
    public Material SandMaterial;
    public Material SnowMaterial;
    public Material IceMaterial;
    public Material CanMaterial;
    public Material NeutralMaterial;

    public GameObject FloorTL;
    public GameObject FloorTR;
    public GameObject FloorBL;
    public GameObject FloorBR;

    public GameObject CanPrefab;
    public GameObject IcePrefab;

    public GameObject FootprintPrefab;
    public Texture2D FootprintTexture;

    Dictionary<GameObject, Material> TileType = new Dictionary<GameObject, Material>();

    Texture2D map;

    public override void Start()
    {
        base.Start();
        TileType.Add(FloorTL, NeutralMaterial);
        TileType.Add(FloorTR, NeutralMaterial);
        TileType.Add(FloorBL, NeutralMaterial);
        TileType.Add(FloorBR, NeutralMaterial);

        map = new Texture2D(1024, 1024);

        for (int i = 0; i < map.height; i++)
        {
            for (int j = 0; j < map.width; j++)
            {
                var color = FootprintTexture.GetPixel(j, i);
                if(color.grayscale < 0.7f)
                {
                    color = new Color(0, 0, 0, 1);
                }
                else
                {
                    color = new Color(0, 0, 0, 0);
                }
                color = ((i/4 & j/4) != 0 ? Color.white : Color.gray);
                map.SetPixel(j, i, color);
            }
        }
        map.Apply();
    }

    float Map(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        float outVal = ((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin);
        return outVal;
    }

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
            GameObject spawnedObject;
            Vector3 position;
            if (texture == "sand")
            {
                material = SandMaterial;
                spawnedObject = Instantiate(CanPrefab);
                position = new Vector3(0, 3, 0);
            }
            else if (texture == "snow")
            {
                material = SnowMaterial;
                spawnedObject = Instantiate(CanPrefab);
                position = new Vector3(0, 3, 0);
            }
            else if (texture == "ice")
            {
                material = IceMaterial;
                spawnedObject = Instantiate(IcePrefab);
                position = new Vector3(0, 0.51f, 0);
            }
            else if (texture == "can")
            {
                material = CanMaterial;
                spawnedObject = Instantiate(CanPrefab);
                position = new Vector3(0, 3, 0);
            }
            else
            {
                Debug.Log("Bad texture!");
                return;
            }

            for (int i = 0; i < tile.transform.childCount; i++)
            {
                var child = tile.transform.GetChild(i).gameObject;
                Destroy(child);
            }

            TileType[tile] = material;
            tile.GetComponent<Renderer>().material = material;
            spawnedObject.transform.parent = tile.transform;
            spawnedObject.transform.localPosition = position;
        }
        else if(message.Address == OscAddress[1])
        {
            if((string)message[0] == "add")
            {
                Debug.Log((float)message[2] + " " + (float)message[3]);
                GameObject tile;
                float x = (float)message[2];
                float z = (float)message[3];
                var localPos = new Vector3(0, 0, 0);
                if (x < 1 && z < 1)
                {
                    tile = FloorTL;
                    localPos.x = Map(x, 0, 1, -0.15f, 0.15f);
                    localPos.z = Map(z, 1, 0, -0.15f, 0.15f);
                }
                else if (x >= 1 && z < 1)
                {
                    tile = FloorTR;
                    localPos.x = Map(x, 1, 2, -0.15f, 0.15f);
                    localPos.z = Map(z, 1, 0, -0.15f, 0.15f);
                }
                else if (x < 1 && z >= 1)
                {
                    tile = FloorBL;
                    localPos.x = Map(x, 0, 1, -0.15f, 0.15f);
                    localPos.z = Map(z, 2, 1, -0.15f, 0.15f);
                }
                else if (x >= 1 && z >= 1)
                {
                    tile = FloorBR;
                    localPos.x = Map(x, 1, 2, -0.15f, 0.15f);
                    localPos.z = Map(z, 2, 1, -0.15f, 0.15f);
                }
                else
                {
                    return;
                }

                for(int i = 0; i < tile.transform.childCount; i++)
                {
                    var child = tile.transform.GetChild(i).gameObject;
                    if (child.GetComponent<VoronoiDemo>() != null)
                        child.GetComponent<VoronoiDemo>().CrackAt(localPos + tile.transform.position);
                    else
                        Destroy(child);
                }

                if (TileType[tile] == SnowMaterial)
                {
                    for(int i = 0; i < map.height; i++)
                    {
                        for (int j = 0; j < map.width; j++)
                        {
                            float f = Mathf.PerlinNoise(i * 0.1f, j * 0.1f);
                            if (f < 0.5f) f = 0;
                            map.SetPixel(j, i, new Color(f, f, f));
                        }

                    }
                    map.Apply();
                    tile.GetComponent<Renderer>().material.SetTexture("_HeightMap", map);
                    //tile.GetComponent<Renderer>().material.SetTexture("_BumpMap", map);
                    var footprintObject = Instantiate(FootprintPrefab);
                    footprintObject.transform.parent = tile.transform;
                    footprintObject.transform.position = new Vector3(Map(x, 0, 2, -0.3f, 0.3f), -0.13f, Map(z, 2, 0, -0.3f, 0.3f));
                }
            }
        }
    }
}
