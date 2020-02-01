using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitHazard : MonoBehaviour
{
	private GameController gameController;
	private AudioSource audioData;

	private void Awake()
	{
		audioData = GetComponent<AudioSource>();
	}

	// Start is called before the first frame update
	void Start()
	{
		gameController = GameObject.FindWithTag(" ").GetComponent<GameController>();
		if (gameController == null)
		{
			Debug.Log("gameController in HitHazard.cs is null.");
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log(other.name);
		if (other.tag != "Hazard")
			return;

		var hazard = other.GetComponent<Hazard>();
		if (hazard != null)
		{
			gameController.UpdateEnergy(-hazard.energyDrain);
		}
		else
		{
			Debug.Log("Could not find the Hazard object from the given collider in HitHazard.");
		}

		Destroy(other.gameObject);

		//audioData.Play();
	}
}
