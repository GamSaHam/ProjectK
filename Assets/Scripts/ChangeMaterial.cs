using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour {
	public void setColor(float r,float g,float b,float a)
	{
		gameObject.GetComponent<Renderer>().material.color = new Color (r,g,b,a);
	}

	public float r=1;
	public float g=1;
	public float b=1;
	public float a=1;
	void Start()
	{
		setColor (r, g, b, a);

	}
}
