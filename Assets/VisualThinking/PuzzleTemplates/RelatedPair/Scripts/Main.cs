
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace RelatedPair {

	public class Main : MonoBehaviour {

		enum FsmState {
			START,
			PROMPT_PUZZLE,
			PROMPT_CORRECT,
			PROMPT_INCORRECT
		}

		public string puzzlePrompt;
		public string correctPrompt;
		public string incorrectPrompt;
		public Text promptText;
		public Button noButton;
		public Button yesButton;
		public Button resetButton;
		public Button okButton;
		public Button exitButton;
		public GameObject interactiveContainerPrefab;
		public GameObject leftSpawn;
		public GameObject rightSpawn;

		private static System.Random rand = new System.Random();
		private Puzzle[] puzzles;

		//Game state
		private FsmState fsmState = FsmState.START;
		private int puzzleIdx = -1;
		private bool spawnLR;
		private GameObject object1Instance = null;
		private GameObject object2Instance = null;

		void Start() {
			puzzles = gameObject.GetComponents<Puzzle>();
			ResetPuzzleObjects();
			UpdateState(FsmState.PROMPT_PUZZLE);
            Debug.Log("Object 1 Euler Angles" + object1Instance.transform.eulerAngles);
            Debug.Log("Object 2 Euler Angles" + object2Instance.transform.eulerAngles);
            Debug.Log("Object 1 Local Euler Angles" + object1Instance.transform.localEulerAngles);
            Debug.Log("Object 2 Local Euler Angles" + object2Instance.transform.localEulerAngles);
            Debug.Log("Rotation Needed: " + (object1Instance.transform.eulerAngles - object2Instance.transform.eulerAngles));
            //Debug Method
            //RotateDiceB(object1Instance.transform.eulerAngles, object1Instance.transform.eulerAngles - object2Instance.transform.eulerAngles);
        }

		public void HandleNoButtonClick() {
			if (fsmState != FsmState.PROMPT_PUZZLE)
				return;
			if (puzzles[puzzleIdx].match)
				UpdateState(FsmState.PROMPT_INCORRECT);
			else
				UpdateState(FsmState.PROMPT_CORRECT);
		}

		public void HandleYesButtonClick() {
			if (fsmState != FsmState.PROMPT_PUZZLE)
				return;
			if (puzzles[puzzleIdx].match)
				UpdateState(FsmState.PROMPT_CORRECT);
			else
				UpdateState(FsmState.PROMPT_INCORRECT);
		}

		public void HandleResetButtonClick() {
			ResetPuzzleObjects();
			UpdateState(FsmState.PROMPT_PUZZLE);
		}

		public void HandleOkButtonClick() {
			if (fsmState == FsmState.PROMPT_CORRECT) {
				ResetPuzzleObjects();
				UpdateState(FsmState.PROMPT_PUZZLE);
			}
			else if (fsmState == FsmState.PROMPT_INCORRECT) {
				UpdateState(FsmState.PROMPT_PUZZLE);
			}
			else {
				return;
			}
		}

		public void HandleExitButtonClick() {
			Application.Quit();
		}

		private void ResetPuzzleObjects() {
			DeterminePuzzle();
			InstantiatePuzzleObjects();
		}

		private void UpdateState(FsmState state) {
			switch(state) {
				case FsmState.PROMPT_PUZZLE:
					promptText.text = puzzlePrompt;
					fsmState = FsmState.PROMPT_PUZZLE;
					UpdateButtons();
					break;
				case FsmState.PROMPT_CORRECT:
                    //Correct answer
					promptText.text = correctPrompt;
					fsmState = FsmState.PROMPT_CORRECT;
					UpdateButtons();
                    if (puzzles[puzzleIdx].match)
                    {
                        RotateDice();
                    }
                    break;
				case FsmState.PROMPT_INCORRECT:
					promptText.text = incorrectPrompt;
					fsmState = FsmState.PROMPT_INCORRECT;
					UpdateButtons();
					break;
			}
		}

		private void UpdateButtons() {
			switch(fsmState) {
				case FsmState.PROMPT_PUZZLE:
					noButton.gameObject.SetActive(true);
					yesButton.gameObject.SetActive(true);
					resetButton.gameObject.SetActive(true);
					okButton.gameObject.SetActive(false);
					exitButton.gameObject.SetActive(true);
					break;
				case FsmState.PROMPT_CORRECT:
				case FsmState.PROMPT_INCORRECT:
					noButton.gameObject.SetActive(false);
					yesButton.gameObject.SetActive(false);
					resetButton.gameObject.SetActive(false);
					okButton.gameObject.SetActive(true);
					exitButton.gameObject.SetActive(false);
					break;
			}
		}

		private void DeterminePuzzle() {
            puzzleIdx = rand.Next(puzzles.Length);
			spawnLR = (rand.Next(2) == 0);
		}

        //Rotate dice to show how they match using a coroutine
        private void RotateDice() {
            //initial orientation of object2
            Vector3 initialObject2 = object2Instance.transform.localEulerAngles;

            //start CoRoutine to annimate the rotation
            StartCoroutine(RotateDiceA(initialObject2, "x"));
        }

        private void RotateDiceY() {
            //initial orientation of object2
            Vector3 initialObject2 = object2Instance.transform.localEulerAngles;

            //start CoRoutine to annimate the rotation
            StartCoroutine(RotateDiceA(initialObject2, "y"));
        }

        private void RotateDiceZ() {
            //initial orientation of object2
            Vector3 initialObject2 = object2Instance.transform.localEulerAngles;

            //start CoRoutine to annimate the rotation
            StartCoroutine(RotateDiceA(initialObject2, "z"));
        }

        //Take object 2 orientation (vec 3), and string for the axis.
        //Operates on object 1
        public IEnumerator RotateDiceA(Vector3 object2, string axis) {
            //Placeholder
            int rotationNeeded = 0;
            //Initial orientation
            Vector3 initialObject1a = object1Instance.transform.localEulerAngles;
            //Initial orientation in X,Y,Z
            int X = (int)initialObject1a.x;
            int Y = (int)initialObject1a.y;
            int Z = (int)initialObject1a.z;

            //Determine rotation needed depending on axis
            if (axis == "x"){rotationNeeded = (int)initialObject1a.x - (int)object2.x;}
            if (axis == "y"){rotationNeeded = (int)initialObject1a.y - (int)object2.y;}
            if (axis == "z"){rotationNeeded = (int)initialObject1a.z - (int)object2.z;}

            //If greater than 180 then rotate the other way
            if (Mathf.Abs(rotationNeeded) >= 180)
            {
                //update rotation needed
                rotationNeeded = 360 - Mathf.Abs(rotationNeeded);
            }

            //Zero out small rotations to filter out drift
            if (Mathf.Abs(rotationNeeded) <= 40)
            {
                //Update rotation needed
                rotationNeeded = 0;
            }

            //While rotation is needed
            while (rotationNeeded != 0)
            {
                //If the angle needed to rotate is positive subtract 1 each frame
                if (rotationNeeded >= 0)
                {
                    //Increment correct axis
                    if (axis == "x") { X--; }
                    if (axis == "y") { Y--; }
                    if (axis == "z") { Z--; }
                    //Update counter
                    rotationNeeded = rotationNeeded - 1;
                }
                else //Angle needed is negative, add 1 each frame
                {
                    //Increment x value
                    if (axis == "x") { X++; }
                    if (axis == "y") { Y++; }
                    if (axis == "z") { Z++; }
                    //Update counter
                    rotationNeeded = rotationNeeded + 1;
                }
                //Rotate object
                object1Instance.transform.localRotation = Quaternion.Euler(X, Y, Z);
                yield return null;
            }

            //Start next axis rotation
            if (axis == "x") { RotateDiceY(); }
            if (axis == "y") { RotateDiceZ(); }
        }

        private void InstantiatePuzzleObjects() {
			GameObject object1Spawn = spawnLR ? leftSpawn : rightSpawn;
			GameObject object2Spawn = spawnLR ? rightSpawn : leftSpawn;
			if (object1Instance != null)
				Destroy(object1Instance);
			object1Instance = InstantiatePrefab(
				puzzles[puzzleIdx].object1Prefab,
				object1Spawn,
				true
			);
			if (object2Instance != null)
				Destroy(object2Instance);
			object2Instance = InstantiatePrefab(
				puzzles[puzzleIdx].object2Prefab,
				object2Spawn,
				true
			);
		}

		private GameObject InstantiatePrefab(GameObject prefab, GameObject spawn, bool interactive) {
			if (interactive) {
				GameObject widget = Instantiate(prefab, spawn.transform);
				widget.transform.localPosition = Vector3.zero;
				GameObject container = Instantiate(interactiveContainerPrefab, spawn.transform);
				container.transform.localPosition = Vector3.zero;
				container.transform.localRotation = widget.transform.localRotation;
				container.transform.localScale = widget.transform.localScale;
				BoxCollider bc = container.GetComponent<BoxCollider>();
				MeshFilter mf = widget.GetComponent<MeshFilter>();
				bc.size = mf.mesh.bounds.size;
				bc.center = mf.mesh.bounds.center;
				widget.transform.parent = container.transform;
				return container;
			}
			else {
				GameObject widget = Instantiate(prefab, spawn.transform);
				return widget;
			}
		}
	}
}
