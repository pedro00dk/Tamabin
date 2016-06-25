using UnityEngine;
using UnityEngine.SceneManagement;

public class TamabinController : MonoBehaviour {

	#if UNITY_ANDROID
	public string tamabinBluetoothName;
	TamabinConnector connector;
	bool connected;
	#endif

	#if UNITY_EDITOR
	bool editorConnected;
	bool tamaguchiEat;
	#endif

	string lastMessage;

	void Start() {
		DontDestroyOnLoad(gameObject);

		#if UNITY_ANDROID
		connector = TamabinConnector.getInstance();
		connector.SetTamabinBluetoothName(tamabinBluetoothName);
		lastMessage = "";
		connected = false;
		#endif

		#if UNITY_EDITOR
		editorConnected = false;
		tamaguchiEat = false;
		#endif
	}

	public void LoadTamaguichiScene() {
		
		#if UNITY_ANDROID
		if (connected) {
			lastMessage = "Tamaguchi scene loaded";
			SceneManager.LoadScene("Tamaguchi");
		} else {
			lastMessage = "Cannot start without connect";
		}
		#endif

		#if UNITY_EDITOR
		if (editorConnected) {
			lastMessage = "Tamaguchi scene loaded";
			SceneManager.LoadScene("Tamaguchi");
		} else {
			lastMessage = "Cannot start without connect";
		}
		#endif
	}

	public void Connect() {
		
		#if UNITY_ANDROID
		lastMessage = connector.TryConnect();
		connected = lastMessage == TamabinConnector.CONNECTED || lastMessage == TamabinConnector.ALREADY_CONNECTED;
		#endif

		#if UNITY_EDITOR
		if (!editorConnected) {
			lastMessage = TamabinConnector.CONNECTED;
			editorConnected = true;
		} else {
			lastMessage = TamabinConnector.ALREADY_CONNECTED;
		}
		#endif
	}

	public void Disconnect() {

		#if UNITY_ANDROID
		lastMessage = connector.TryDisconnect();
		connected = false;
		#endif

		#if UNITY_EDITOR
		if (editorConnected) {
			lastMessage = TamabinConnector.DISCONNECTED;
			editorConnected = false;
		} else {
			lastMessage = TamabinConnector.NOT_CONNECTED;
		}
		#endif
	}

	public void TamabinCall() {

		#if UNITY_ANDROID
		string tamabinMessage = connector.TamabinCall();
		lastMessage = tamabinMessage != null ? "Eat" : "Not eat";
		#endif

		#if UNITY_EDITOR
		lastMessage = tamaguchiEat ? "Eat" : "Not eat";
		tamaguchiEat = false;
		#endif
	}

	void Update() {
		
		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.C)) {
			tamaguchiEat = true;
		}
		#endif
	}

	public string GetLastMessage() {
		return lastMessage;
	}
}
