using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        float horizontal = 0;
        float vertical = 0;

        horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        vertical = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, horizontal, 0);
        transform.Translate(0, 0, vertical);

        if (Input.GetKeyDown(KeyCode.Space))
            transform.Translate(0, 1.5f, 0);

        if (Input.GetKeyDown(KeyCode.M))
            Debug.Log("Media: " + PerfomanceReport.getAvg());

        if (Input.GetKeyDown(KeyCode.B))
            Debug.Log("Min: " + PerfomanceReport.getMin());

        if (Input.GetKeyDown(KeyCode.N))
            Debug.Log("Max: " + PerfomanceReport.getMax());

        if (Input.GetKeyDown(KeyCode.V))
            Debug.Log("Total: " + PerfomanceReport.getTotal());
    }
}
