using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class statUpdater : MonoBehaviour {

    public GameObject earthGO;
    statsManager SM;
    Text tx;
    
    // Use this for initialization
    void Start () {
	SM = earthGO.GetComponent<statsManager>();
        tx = gameObject.GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        tx.text = SM.statPrintString;
    }
}
