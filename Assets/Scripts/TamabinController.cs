using UnityEngine;
using UnityEngine.SceneManagement;

public class TamabinController : MonoBehaviour {
	public string tamabinBluetoothName;
	TamabinConnector connector;
	string lastMessage;
	bool connected;

	void Start() {
		DontDestroyOnLoad(gameObject);
		connector = TamabinConnector.getInstance();
		connector.SetTamabinBluetoothName(tamabinBluetoothName);
		lastMessage = "";
		connected = false;
	}

	public void LoadTamaguichiScene() {
		if (connected) {
			lastMessage = "Tamaguchi scene loaded";
			SceneManager.LoadScene("Tamaguchi");
		} else {
			lastMessage = "Cannot start without connect";
		}
	}

	public void Connect() {
		lastMessage = connector.TryConnect();
		connected = lastMessage == TamabinConnector.CONNECTED || lastMessage == TamabinConnector.ALREADY_CONNECTED;
	}

	public void Disconnect() {
		lastMessage = connector.TryDisconnect();
	}

	public void TamabinCall() {
		string tamabinMessage = connector.TamabinCall();
		lastMessage = tamabinMessage != null ? "Eat" : "Not eat";
	}

	public string GetLastMessage() {
		return lastMessage;
	}
}
