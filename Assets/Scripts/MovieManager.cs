using UnityEngine;
using System.Collections;

public class MovieManager : MonoBehaviour
{
    public MovieTexture TextureFirst;
    public MovieTexture TextureSecond;

    // Use this for initialization
    void Start()
    {
        //var movieTexture = ((MovieTexture)GetComponent<Renderer>().material.mainTexture);
        var movieTexture = TextureFirst;
        movieTexture.loop = true;
        movieTexture.Play();
        movieTexture.wrapMode = TextureWrapMode.Repeat;

        movieTexture = TextureSecond;
        movieTexture.loop = true;
        movieTexture.Play();
        movieTexture.wrapMode = TextureWrapMode.Repeat;

        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", TextureFirst);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Crack()
    {
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", TextureSecond);
    }
}