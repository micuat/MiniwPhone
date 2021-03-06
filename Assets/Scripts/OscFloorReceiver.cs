﻿using UnityEngine;
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

    public GameObject CanPrefab0;
    public GameObject CanPrefab1;
    public GameObject CanPrefab2;
    public GameObject CanPrefab3;
    public GameObject IcePrefab0;
    public GameObject IcePrefab1;

    public GameObject FootprintPrefab;
    public Texture2D FootprintTextureSnow;
    public Texture2D FootprintTextureSand;

    public GameObject FootObject;

    public Dictionary<GameObject, Material> TileType = new Dictionary<GameObject, Material>();

    Dictionary<GameObject, Texture2D> Maps = new Dictionary<GameObject, Texture2D>();

    public int MapDimensions = 256;
    
    void InitNoiseMap(Texture2D map)
    {
        for (int i = 0; i < map.height; i++)
        {
            for (int j = 0; j < map.width; j++)
            {
                float f = Mathf.PerlinNoise(i * 0.1f, j * 0.1f) + Mathf.PerlinNoise(i * 0.25f, j * 0.25f) * 0.25f + Mathf.PerlinNoise(i * 0.5f, j * 0.5f) * 0.5f;
                f = Map(f, 0, 1, 0.66f, 1.0f) * 0.25f;
                map.SetPixel(j, i, new Color(f, f, f));
            }

        }
        map.Apply();
    }

    public override void Start()
    {
        base.Start();
        TileType.Add(FloorTL, NeutralMaterial);
        TileType.Add(FloorTR, NeutralMaterial);
        TileType.Add(FloorBL, NeutralMaterial);
        TileType.Add(FloorBR, NeutralMaterial);


        Maps.Add(FloorTL, new Texture2D(MapDimensions, MapDimensions));
        Maps.Add(FloorTR, new Texture2D(MapDimensions, MapDimensions));
        Maps.Add(FloorBL, new Texture2D(MapDimensions, MapDimensions));
        Maps.Add(FloorBR, new Texture2D(MapDimensions, MapDimensions));

        foreach(var MapPair in Maps)
        {
            var map = MapPair.Value;
            InitNoiseMap(map);
        }
    }

    public void TriggerMessage(OscMessage m)
    {
        ReceiveMessage(m);
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
            GameObject spawnedObject = null;
            if (texture == "sand")
            {
                material = SandMaterial;
            }
            else if (texture == "snow")
            {
                material = SnowMaterial;
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

            for (int i = 0; i < tile.transform.childCount; i++)
            {
                var child = tile.transform.GetChild(i).gameObject;
                Destroy(child);
            }

            TileType[tile] = material;
            tile.GetComponent<Renderer>().material = material;
            if(material == SnowMaterial || material == SandMaterial)
            {
                InitNoiseMap(Maps[tile]);
                tile.GetComponent<Renderer>().material.SetTexture("_HeightMap", Maps[tile]);
            }

            Vector3 position = Vector3.zero;
            if (texture == "ice")
            {
                int IceCount = 0;
                foreach (var type in TileType)
                {
                    if (type.Value == IceMaterial)
                    {
                        IceCount++;
                    }
                }
                if (IceCount == 1)
                    spawnedObject = Instantiate(IcePrefab0);
                else
                    spawnedObject = Instantiate(IcePrefab1);
                position = new Vector3(0, 0.51f, 0);
            }
            else if (texture == "can")
            {
                int CanCount = 0;
                foreach (var type in TileType)
                {
                    if (type.Value == CanMaterial)
                    {
                        CanCount++;
                    }
                }
                if (CanCount == 1)
                    spawnedObject = Instantiate(CanPrefab0);
                else if (CanCount == 2)
                    spawnedObject = Instantiate(CanPrefab1);
                else if (CanCount == 3)
                    spawnedObject = Instantiate(CanPrefab2);
                else if (CanCount >= 4)
                    spawnedObject = Instantiate(CanPrefab3);
                position = new Vector3(0, 0.7f, 0);
            }

            if (spawnedObject != null)
            {
                spawnedObject.transform.parent = tile.transform;
                spawnedObject.transform.localPosition = position;
            }
        }
        else if(message.Address == OscAddress[1])
        {
            if((string)message[0] == "add" || (string)message[0] == "update")
            {
                Debug.Log((float)message[2] + " " + (float)message[3]);
                GameObject tile;
                float x = FootObject.transform.position.x;
                float z = FootObject.transform.position.z;
                var localPos = new Vector3(0, 0, 0);
                if (x < 0 && z > 0)
                {
                    tile = FloorTL;
                    localPos.x = Map(x, -0.3f, 0, -0.15f, 0.15f);
                    localPos.z = Map(z, 0, 0.3f, -0.15f, 0.15f);
                }
                else if (x >= 0 && z > 0)
                {
                    tile = FloorTR;
                    localPos.x = Map(x, 0, 0.3f, -0.15f, 0.15f);
                    localPos.z = Map(z, 0, 0.3f, -0.15f, 0.15f);
                }
                else if (x < 0 && z <= 0)
                {
                    tile = FloorBL;
                    localPos.x = Map(x, -0.3f, 0, -0.15f, 0.15f);
                    localPos.z = Map(z, -0.3f, 0, -0.15f, 0.15f);
                }
                else if (x >= 0 && z <= 0)
                {
                    tile = FloorBR;
                    localPos.x = Map(x, 0, 0.3f, -0.15f, 0.15f);
                    localPos.z = Map(z, -0.3f, 0, -0.15f, 0.15f);
                }
                else
                {
                    return;
                }

                // crack/crush only "add"
                if ((string)message[0] == "add")
                {
                    for (int i = 0; i < tile.transform.childCount; i++)
                    {
                        var child = tile.transform.GetChild(i).gameObject;
                        if (child.GetComponent<VoronoiDemo>() != null)
                            child.GetComponent<VoronoiDemo>().CrackAt(localPos + tile.transform.position);
                        else if (TileType[tile] == CanMaterial)
                        {
                            child.GetComponent<CanContoller>().Crush();
                        }
                    }
                }

                if (TileType[tile] == SnowMaterial || TileType[tile] == SandMaterial)
                {
                    Texture2D FootprintTexture;
                    if (TileType[tile] == SnowMaterial)
                        FootprintTexture = FootprintTextureSnow;
                    else
                        FootprintTexture = FootprintTextureSand;

                    var map = Maps[tile];
                    int texPosX = (int)((-localPos.x + 0.15f) / 0.3f * map.width);
                    int texPosY = (int)((-localPos.z + 0.15f) / 0.3f * map.height);
                    int xDim = 100;
                    int yDim = 200;
                    for (int i = 0; i < yDim; i++)
                    {
                        for (int j = 0; j < xDim; j++)
                        {
                            int mapX = j + texPosX - xDim / 2;
                            int mapY = i + texPosY - yDim / 2;
                            if (mapX < 0 || mapY < 0 || mapX >= map.width || mapY >= map.height) continue;
                            var footPixel = FootprintTexture.GetPixel((int)Map(j, 0, xDim, FootprintTexture.width, 0), (int)Map(i, 0, yDim, FootprintTexture.height, 0));
                            //var footPixel = FootprintTexture.GetPixelBilinear(FootprintTexture.width - (float)j / xDim, FootprintTexture.height - (float)i / yDim);
                            float depth = footPixel.r - 0.5f;
                            //if (depth < 0.1f) continue;
                            footPixel.r = depth;
                            footPixel.g = depth;
                            footPixel.b = depth;
                            footPixel.a = depth;
                            map.SetPixel(mapX, mapY, map.GetPixel(mapX, mapY) - 0.1f * footPixel);
                        }

                    }
                    map.Apply();
                    tile.GetComponent<Renderer>().material.SetTexture("_HeightMap", map);
                }
            }
        }
        else if (message.Address == OscAddress[2])
        {
            FootObject.GetComponent<FootController>().receiveButtonPressed();
            FootObject.GetComponent<OscPositionReceiver>().receiveButtonPressed();
        }
        else if (message.Address == OscAddress[3])
        {
            FootObject.GetComponent<FootController>().ReceiveFadeSwitch();
        }
    }
}
