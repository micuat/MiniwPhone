using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rug.Osc;

public abstract class ReceiveOscBehaviourBase : MonoBehaviour {

	private OscReceiveController m_ReceiveController;

    public GameObject ReceiveController;

	public List<string> OscAddress;

	public void Awake () {
		
		m_ReceiveController = null; 
		
		GameObject controllerObject = ReceiveController; 
		
		if (controllerObject == null) {
			Debug.LogError(string.Format("A GameObject could not be found")); 
			return; 
		}
		
		OscReceiveController controller = controllerObject.GetComponent<OscReceiveController> (); 
		
		if (controller == null) { 
			Debug.LogError(string.Format("The GameObject does not contain a OscReceiveController component")); 
			return; 
		}
		
		m_ReceiveController = controller; 
	}

	// Use this for initialization
	public virtual void Start () {

		if (m_ReceiveController != null) {
            foreach(var s in OscAddress)
            {
                m_ReceiveController.Manager.Attach(s, ReceiveMessage);
            }
        }
	}

	// Update is called once per frame
	public virtual void Update () {
	
	}

	public virtual void OnDestroy () {

		// detach from the OscAddressManager
		if (m_ReceiveController != null) {
            foreach (var s in OscAddress)
            {
                m_ReceiveController.Manager.Detach(s, ReceiveMessage);
            }
        }
	}

	protected abstract void ReceiveMessage (OscMessage message);
}
