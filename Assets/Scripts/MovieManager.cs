using UnityEngine;
using System.Collections.Generic;

public class MovieManager : MonoBehaviour
{
    public List<Texture2D> TextureFirst;
    public List<Texture2D> TextureSecond;

    List<Texture2D> CurTextures;
    IEnumerator<Texture2D> CurTexture;

    // Use this for initialization
    void Start()
    {
        CurTextures = TextureFirst;
        CurTexture = CurTextures.GetEnumerator();
        CurTexture.MoveNext();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", CurTexture.Current);
        if (CurTexture.MoveNext() == false)
        {
            CurTexture = CurTextures.GetEnumerator();
            CurTexture.MoveNext();
        }
    }

    public void Crack()
    {
        CurTextures = TextureSecond;
        CurTexture.MoveNext();
    }
}