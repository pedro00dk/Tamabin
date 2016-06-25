using UnityEngine;
using UnityEngine.UI;

public class ConnectorInformer : MonoBehaviour {
	public TamabinController controller;
	public Text informer;

	void Update() {
		informer.text = controller.GetLastMessage();
	}
}
