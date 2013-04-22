using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlyingCreatureScript : MonoBehaviour 
{
	enum CreatureStates
	{
		PreInitialize,
		AssemblingParts,
		CollectingPathData,
		FollowingPath,
		Dead
	};
	CreatureStates creatureState = CreatureStates.PreInitialize;

	GameObject[] creaturePartsArray;
	Vector3[] creaturePartsOriginalPositionArray;

	public List<Vector3> positionsRecordingsList;
	public List<Vector3> rotationsRecordingsList;

	PlayerScript playerScript;


	// Use this for initialization
	void Start () 
	{
		playerScript = (PlayerScript)GameObject.FindGameObjectWithTag("Player").GetComponent("PlayerScript");
	
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(creatureState)
		{
			case CreatureStates.AssemblingParts:
				AssembleCreature();
				break;
			case CreatureStates.CollectingPathData:
				CollectPathData();
				break;
		}
	
	}

	public void AquireCreatureParts(GameObject[] partsArray)
	{
		List<GameObject> creaturePartsList = new List<GameObject>();
		List<Vector3> creaturePartsOriginalPositionList = new List<Vector3>();
		for(int i = 0; i < partsArray.Length; i++ )
		{
			partsArray[i].transform.parent = transform;
			creaturePartsList.Add(partsArray[i]);
			creaturePartsOriginalPositionList.Add(partsArray[i].transform.position);
		}
		creaturePartsArray = creaturePartsList.ToArray();
		creaturePartsOriginalPositionArray = creaturePartsOriginalPositionList.ToArray();
		creatureState = CreatureStates.AssemblingParts;

	}

	void AssembleCreature()
	{
		float tempDistance;
		float currentDistance;
		float targetDistance;
		// player hits the ground., need to complete creature formation
		if(playerScript.oldVelocity.y < 0 && playerScript.velocity.y == 0)
		{
			Debug.Log("Hit Ground");
			for(int i = 0; i < creaturePartsArray.Length; i++)
			{
				creaturePartsArray[i].transform.position = Vector3.Lerp(creaturePartsOriginalPositionArray[i], transform.position, 1.0f);
			}
			creatureState = CreatureStates.CollectingPathData;
		}

		// ordinary update
		for(int i = 0; i < creaturePartsArray.Length; i++)
		{
			creaturePartsArray[i].transform.position = Vector3.Lerp(creaturePartsOriginalPositionArray[i], transform.position, playerScript.moveTowardsRatio);
		}
		//Debug.Log(playerScript.moveTowardsRatio);
	}

	void CollectPathData()
	{
		// get the data from the original list in the player
		positionsRecordingsList = new List<Vector3>( playerScript.positionRecordingList );
		rotationsRecordingsList = new List<Vector3>( playerScript.rotationRecordingList );
		// wipe the original
		playerScript.positionRecordingList.Clear();
		playerScript.rotationRecordingList.Clear();
		// change state
		creatureState = CreatureStates.FollowingPath;
	}


}