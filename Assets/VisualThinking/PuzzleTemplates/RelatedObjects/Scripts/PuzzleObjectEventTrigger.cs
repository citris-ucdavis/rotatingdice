
using UnityEngine;
using UnityEngine.EventSystems;


namespace RelatedObjects {

	public class PuzzleObjectEventTrigger : EventTrigger {

		Main main = null;

		void Awake() {
			GameObject mainObject = GameObject.Find("MainObject");
			main = mainObject.GetComponent<Main>();
		}

		public override void OnPointerClick(PointerEventData data) {
			main.HandleObjectClick(gameObject);
		}

	}

}
