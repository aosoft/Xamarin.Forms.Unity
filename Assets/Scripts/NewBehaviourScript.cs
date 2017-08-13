using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

	private void Awake()
	{
		var x = new Xamarin.Forms.Platform.Unity.UnityPlatformServices(System.Threading.Thread.CurrentThread);
		var a = x.GetAssemblies();
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
