
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SplashScript : MonoBehaviour {

	public float fadeSpeed;
	public float displaySec;

	private bool done = false;
	private CanvasGroup canvasGroup = null;

	void Start() {
		canvasGroup = GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0F;
		StartCoroutine(FadeInOut());
	}

	IEnumerator FadeInOut() {
		while (canvasGroup.alpha < 1F) {
			canvasGroup.alpha += (fadeSpeed * Time.deltaTime);
			yield return null;
		}
		yield return new WaitForSeconds(displaySec);
		while (canvasGroup.alpha > 0F) {
			canvasGroup.alpha -= (fadeSpeed * Time.deltaTime);
			yield return null;
		}
		yield return done = true;
	}

	private void LateUpdate() {
		if (done) {
			Scene curScene = SceneManager.GetActiveScene();
			SceneManager.LoadScene(curScene.buildIndex + 1, LoadSceneMode.Single);
		}
	}

}
