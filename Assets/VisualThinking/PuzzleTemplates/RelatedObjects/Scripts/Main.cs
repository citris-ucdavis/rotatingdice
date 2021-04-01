
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace RelatedObjects {

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
		public Button resetButton;
		public Button okButton;
		public Button exitButton;
		public GameObject interactiveContainerPrefab;
		public GameObject questionSpawn;
		public List<GameObject> answerSpawns;

		private static System.Random rand = new System.Random();
		private Puzzle[] puzzles;

		// game state
		private FsmState fsmState = FsmState.START;
		private int puzzleIdx = -1;
		private int puzzleCorrectIdx = -1;
		private int[] puzzleIncorrectIdxs = { -1, -1, -1 };
		private int correctSpawnIdx = -1;
		private int[] incorrectSpawnIdxs = new int[] { -1, -1, -1 };
		private GameObject questionInstance = null;
		private GameObject correctInstance = null;
		private GameObject[] incorrectInstances = new GameObject[] { null, null, null };

		void Start() {
			puzzles = gameObject.GetComponents<Puzzle>();
			ResetPuzzleObjects();
			UpdateState(FsmState.PROMPT_PUZZLE);
		}

		public void HandleObjectClick(GameObject go) {
			if (fsmState != FsmState.PROMPT_PUZZLE)
				return;
			if (go == correctInstance) {
				UpdateState(FsmState.PROMPT_CORRECT);
				return;
			}
			foreach (GameObject ii in incorrectInstances) {
				if (go == ii) {
					UpdateState(FsmState.PROMPT_INCORRECT);
					return;
				}
			}
			// unrelated object; ignore
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
					promptText.text = correctPrompt;
					fsmState = FsmState.PROMPT_CORRECT;
					UpdateButtons();
					break;
				case FsmState.PROMPT_INCORRECT:
					promptText.text = incorrectPrompt;
					fsmState = FsmState.PROMPT_INCORRECT;
					UpdateButtons();
					break;
				default:
					// TODO: throw IllegalStateException
					break;
			}
		}

		private void UpdateButtons() {
			switch(fsmState) {
				case FsmState.PROMPT_PUZZLE:
					resetButton.gameObject.SetActive(true);
					okButton.gameObject.SetActive(false);
					exitButton.gameObject.SetActive(true);
					break;
				case FsmState.PROMPT_CORRECT:
				case FsmState.PROMPT_INCORRECT:
					resetButton.gameObject.SetActive(false);
					okButton.gameObject.SetActive(true);
					exitButton.gameObject.SetActive(false);
					break;
				default:
					// TODO: throw IllegalStateException
					break;
			}
		}

		private void DeterminePuzzle() {
			puzzleIdx = rand.Next(puzzles.Length);
			puzzleCorrectIdx = rand.Next(puzzles[puzzleIdx].correctAnswerPrefabs.Count);
			int[] shuf;
			shuf = ShuffledRange(rand, 0, puzzles[puzzleIdx].incorrectAnswerPrefabs.Count-1);
			puzzleIncorrectIdxs[0] = shuf[0];
			puzzleIncorrectIdxs[1] = shuf[1];
			puzzleIncorrectIdxs[2] = shuf[2];
			shuf = ShuffledRange(rand, 0, 3);
			correctSpawnIdx = shuf[0];
			incorrectSpawnIdxs[0] = shuf[1];
			incorrectSpawnIdxs[1] = shuf[2];
			incorrectSpawnIdxs[2] = shuf[3];
		}

		private void InstantiatePuzzleObjects() {
			if (questionInstance != null)
				Destroy(questionInstance);
			questionInstance = InstantiatePrefab(puzzles[puzzleIdx].questionPrefab, questionSpawn, false);
			if (correctInstance != null)
				Destroy(correctInstance);
			correctInstance = InstantiatePrefab(puzzles[puzzleIdx].correctAnswerPrefabs[puzzleCorrectIdx], answerSpawns[correctSpawnIdx], true);
			for (int i=0; i<3; ++i) {
				if (incorrectInstances[i] != null)
					Destroy(incorrectInstances[i]);
				incorrectInstances[i] = InstantiatePrefab(puzzles[puzzleIdx].incorrectAnswerPrefabs[puzzleIncorrectIdxs[i]], answerSpawns[incorrectSpawnIdxs[i]], true);
			}
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
				container.AddComponent<PuzzleObjectEventTrigger>();
				return container;
			}
			else {
				GameObject widget = Instantiate(prefab, spawn.transform);
				return widget;
			}
		}

		private static int[] ShuffledRange(System.Random rand, int min, int max) {
			int n = max - min + 1;
			int[] buf = new int[n];
			for (int i=min; i<=max; ++i)
				buf[i-min] = i;
			while (n > 1) {
				--n;
				int k = rand.Next(n + 1);
				int temp = buf[k];
				buf[k] = buf[n];
				buf[n] = temp;
			}
			return buf;
		}

	}

}
