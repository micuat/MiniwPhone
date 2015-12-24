using UnityEngine;
using System.Collections;

public class FootController : MonoBehaviour {

    Quaternion InitialRotation;

    public Material SkeletonMaterial;
    public GameObject BareModel;

    float FootFade;
    enum TriggerFade {BareToSkel, SkelToBare, Done };
    TriggerFade triggerFade;
    public float SkeletonAlpha = 0.25f;
    public float BareAlpha = 1;
    public float FadeIncrement = 0.005f;
    bool UseGyro = true;

	// Use this for initialization
	void Start () {
        FootFade = 0;
        triggerFade = TriggerFade.Done;
        SkeletonMaterial.color = new Color(255, 255, 255, 0);
        BareModel.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, BareAlpha);

        Input.gyro.enabled = true;
        InitialRotation = Quaternion.identity;
	}
    // Update is called once per frame
    void Update () {
        if(UseGyro)
            transform.rotation = InitialRotation * ConvertRotation(Input.gyro.attitude);
        else
            transform.rotation = Quaternion.identity;

        if (triggerFade == TriggerFade.BareToSkel)
        {
            SkeletonMaterial.color = new Color(255, 255, 255, FootFade * SkeletonAlpha);
            BareModel.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, (1 - FootFade) * BareAlpha);
            FootFade += FadeIncrement;
            if (FootFade >= 1)
                triggerFade = TriggerFade.Done;
        }
        else if (triggerFade == TriggerFade.SkelToBare)
        {
            SkeletonMaterial.color = new Color(255, 255, 255, FootFade * SkeletonAlpha);
            BareModel.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, (1 - FootFade) * BareAlpha);
            FootFade -= FadeIncrement;
            if (FootFade <= 0)
                triggerFade = TriggerFade.Done;
        }
    }

    private static Quaternion ConvertRotation(Quaternion q)
    {
        return new Quaternion(-q.x, -q.z, -q.y, q.w);
    }

    public void receiveButtonPressed()
    {
        InitialRotation = Quaternion.Inverse(ConvertRotation(Input.gyro.attitude));
        UseGyro = !UseGyro;
    }

    public void ReceiveFadeSwitch()
    {
        if (FootFade < 0.5f)
        {
            triggerFade = TriggerFade.BareToSkel;
            FootFade = 0;
        }
        else
        {
            triggerFade = TriggerFade.SkelToBare;
            FootFade = 1;
        }
    }
}
