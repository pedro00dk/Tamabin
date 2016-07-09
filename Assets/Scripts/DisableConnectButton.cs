using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DisableConnectButton : MonoBehaviour {

	Button button;
	Image buttonImage;

	TamabinController controller;

	void Start () {
		button = GetComponent<Button>();
		buttonImage = GetComponent<Image>();
		controller = FindObjectOfType<TamabinController>();
	}

	void Update () {
		if (controller.Connected()) {
			button.enabled = false;
			buttonImage.enabled = false;
		} else {
			button.enabled = true;
			buttonImage.enabled = true;
		}
	}
}
