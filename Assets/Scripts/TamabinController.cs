using UnityEngine;

public class TamabinController : MonoBehaviour {
	public string tamabinBluetoothName;
	TamabinConnector connector;
	string lastMessage;

	void Start() {
		connector = TamabinConnector.getInstance();
		connector.SetTamabinBluetoothName(tamabinBluetoothName);
		lastMessage = "";
	}

	public void Connect() {
		lastMessage = connector.TryConnect();
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
