using UnityEngine;

public class TamabinController : MonoBehaviour {

	public string tamabinBluetoothName;

	#if UNITY_ANDROID
	TamabinConnector connector;
	bool connected;
	#endif

	#if UNITY_EDITOR
	bool editorConnected;
	bool tamaguchiEat;
	#endif

	string lastMessage;

	void Start() {

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

	public bool Connected() {

		#if UNITY_ANDROID
		return connected;
		#endif

		#if UNITY_EDITOR
		return editorConnected;
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

	public const string EAT = "Eat";
	public const string NOT_EAT = "Not Eat";

	public void TamabinCall() {

		#if UNITY_ANDROID
		string tamabinMessage = connector.TamabinCall();
		lastMessage = tamabinMessage != null ? "Eat" : "Not eat";
		#endif

		#if UNITY_EDITOR
		lastMessage = tamaguchiEat ? EAT : NOT_EAT;
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
