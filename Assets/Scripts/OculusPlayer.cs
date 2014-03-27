﻿using UnityEngine;
using System.Collections;

public class OculusPlayer : MonoBehaviour
{
	public float speed = 10;
	public Transform cursor;

	public Transform mapCursor;
	public Transform cursorCamera;
	public Transform cursorPointer;

	public PinManager pinManager;
	public GameObject menuInGame;
	//public AnimationClip menuEnter;
	public AnimationState menuEnter;

	private bool stop = false;
	private AnimationExtras animationMenuEnter;

	public Camera leftCamera;
	public Camera rightCamera;

	private Transform manipulatedObject;

	void Start ()
	{
		switch(PlayerPrefs.GetInt("gameMode"))
		{
			case 0 : // Only standard player
				gameObject.SetActive(false);
				break;
			case 1 : // Only Rift player
				leftCamera.rect = new Rect(0f, 0f, 0.5f, 1f);
				rightCamera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
				break;
			default : // Both players
				leftCamera.rect = new Rect(0.5f, 0f, 0.25f, 1f);
				rightCamera.rect = new Rect(0.75f, 0f, 0.25f, 1f);
				break;
		}
		manipulatedObject = null;
	}

	void Update ()
	{
		// Move
		CharacterController controller = GetComponent<CharacterController>();
		Vector3 moveVector = Vector3.zero;
		if (Input.GetKey(KeyCode.LeftArrow))
			moveVector += Vector3.Cross(transform.forward, transform.up);
		if (Input.GetKey(KeyCode.RightArrow))
			moveVector += -Vector3.Cross(transform.forward, transform.up);
		if (Input.GetKey(KeyCode.UpArrow))
			moveVector += transform.forward;
		if (Input.GetKey(KeyCode.DownArrow))
			moveVector += -transform.forward;
		controller.Move(moveVector.normalized * speed * Time.deltaTime);
		
		//cursor.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
		/*
		Ray ray = new Ray(Camera.main.transform.position, (cursor.position - Camera.main.transform.position).normalized);
		Debug.Log(ray);
		Debug.DrawRay(ray.origin, ray.direction, Color.red, 5f);
		*/

        Vector3 rayOrigin = cursorCamera.position;
		Vector3 rayDirection = Vector3.Normalize(cursorPointer.position - rayOrigin);

		Ray ray = new Ray(rayOrigin, rayDirection);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
        	if(hit.collider.gameObject.CompareTag("Grid"))
        	{
				pinManager.pin(hit.point);
        	}
        	if (hit.collider.gameObject.CompareTag("Grid") || hit.collider.gameObject.CompareTag("wall"))
        	{
	        	if(manipulatedObject)
	        		manipulatedObject.position = hit.point + new Vector3(0f, 2.5f, 0f);
        	}
        	if(hit.collider.gameObject.CompareTag("towerBase"))
        	{
        		if(manipulatedObject != null)
        		{
        			hit.collider.gameObject.GetComponent<TowerBase>().addCube(manipulatedObject);
        			manipulatedObject.gameObject.tag = "Untagged";
        			manipulatedObject = null;
        		}
        	}

        	if(hit.collider.gameObject.CompareTag("towerCube"))
        	{
        		if(manipulatedObject == null)
        			manipulatedObject = hit.collider.transform;
        	}
        }
        else
            mapCursor.gameObject.SetActive(false);

		//Action ();
	}

	void Action ()
	{		
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			if (stop) {
				Time.timeScale = 1;
				menuInGame.SetActive(false);
				stop = false;
			}
			else {
				Time.timeScale = 0;
				menuInGame.SetActive(true);
				stop = true;
			}
		}
	}

	/*void playAnimation() {
		menuInGame.animation.Play ();
	}*/
}
