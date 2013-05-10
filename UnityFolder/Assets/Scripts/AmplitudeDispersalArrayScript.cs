using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmplitudeDispersalArrayScript : MonoBehaviour 
{

	public GameObject partPrefab;

	public int partsCount;
	public List<Vector3> positionsList = new List<Vector3>();
	List<Vector3> velocitiesList = new List<Vector3>();

	AudioDirectorScript audioDirector;

	public int frequencyMinIndex; // min 0
	public int frequencyMaxIndex; // max 99 (max final data points)

	public float audioHeightScaling = 1.0f;

	public float dispersalUpdateMinimum = 0.02f;
	float dispersalUpdateCounter = 0;

	float oldHeight;

	// Use this for initialization
	void Start () 
	{

		for(int i = 0; i < partsCount; i++)
		{
			positionsList.Add(new Vector3(10 * i, 0 ,0));
			velocitiesList.Add(new Vector3());
			GameObject tempGameObject = (GameObject)Instantiate(partPrefab, positionsList[i], Quaternion.identity);
			tempGameObject.transform.parent = transform;
			((CreaturePartsGeneralScript)tempGameObject.GetComponent("CreaturePartsGeneralScript")).arrayIndex = i;
			((CreaturePartsGeneralScript)tempGameObject.GetComponent("CreaturePartsGeneralScript")).ownerArrayScript = this;

		}

		audioDirector = (AudioDirectorScript) GameObject.Find("AudioDirector").GetComponent("AudioDirectorScript");

	
	}
	
	// Update is called once per frame
	void Update () 
	{

		dispersalUpdateCounter += Time.deltaTime;
		if(dispersalUpdateCounter > dispersalUpdateMinimum)
		{
			dispersalUpdateCounter -= dispersalUpdateMinimum;

		}
		
		float audioAverage = getAmplitudeAverageOverFrequencyRange();
		//Debug.Log(audioAverage);

		Vector3 tempPosition = positionsList[0];
		if( audioAverage * audioHeightScaling > oldHeight )
			tempPosition.y  = Mathf.Lerp( tempPosition.y, audioAverage * audioHeightScaling, 4.0f * Time.deltaTime);
		else
			tempPosition.y  = Mathf.Lerp( tempPosition.y, audioAverage * audioHeightScaling, 1.0f * Time.deltaTime);
		positionsList[0] = tempPosition;

		oldHeight = positionsList[0].y;

		// disperse vaules

		for(int i = partsCount -1; i > 0 ; i--)
		{
			float newHeight = Mathf.Lerp( positionsList[i].y, positionsList[i-1].y, dispersalUpdateCounter/dispersalUpdateMinimum);
			tempPosition = positionsList[i];
			tempPosition.y = newHeight;
			positionsList[i] = tempPosition;
		}


	}


	float getAmplitudeAverageOverFrequencyRange()
	{
		float tempSum = 0;
		for(int i = frequencyMinIndex; i < frequencyMaxIndex; i++)
			tempSum += audioDirector.pseudoLogArray[i];


		float average = tempSum/(float)(frequencyMaxIndex - frequencyMinIndex);
		return average;
	}



}