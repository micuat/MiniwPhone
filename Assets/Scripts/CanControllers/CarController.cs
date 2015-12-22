using UnityEngine;
using System.Collections;

public class CarController : CanContoller {

    void Start()
    {

    }

    public override void Crush()
    {
        var material = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        float deform = material.GetFloat("_NoiseAmplitude");
        if (deform < 0.02f)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<MeshRenderer>().material.SetFloat("_NoiseAmplitude", deform + 0.005f);
            }
            var scale = transform.localScale;
            scale.x *= 1.05f;
            scale.y *= 0.8f;
            scale.z *= 1.05f;
            transform.localScale = scale;
        }
    }
}
