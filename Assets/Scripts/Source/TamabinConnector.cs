
/// <summary>
/// Tamabin connector.
/// This class uses a <see cref="BluetoothConnector"/> instance to connect with a Tamabin using the
/// Tamabin bluetooth device name.
/// </summary>
public class TamabinConnector {

	/// <summary>
	/// The bluetooth connector instance.
	/// </summary>
	private BluetoothConnector bluetoothConnector;

	/// <summary>
	/// The name of the tamabin bluetooth device.
	/// </summary>
	private string tamabinBluetoothName;

	/// <summary>
	/// The NOT AVALIABLE message.
	/// This message is returned if the device does not support bluetooth in the
	/// <see cref="TamabinConnector.TryConnect()"/> function.
	/// </summary>
	public const string NOT_AVALIABLE = "Bluetooth not available";

	/// <summary>
	/// The NOT ENABLED message.
	/// This message is returned if the device bluetooth is disabled in the
	/// <see cref="TamabinConnector.TryConnect()"/> function.
	/// </summary>
	public const string NOT_ENABLED = "Bluetooth not enabled";

	/// <summary>
	/// The NAMA NOT SET message.
	/// This message is returned if try call <see cref="TamabinConnector.TryConnect()"/> function
	/// without set the name in the
	/// <see cref="TamabinConnector.SetTamaguchiBluetoothName()"/> function.
	/// </summary>
	public const string NAME_NOT_SET = "Tamabin BT name not set";

	/// <summary>
	/// The NOT FOUND message.
	/// This message is returned if the device does not found a bluetooth device with the received
	/// name in <see cref="TamabinConnector.TryConnect()"/> function.
	/// </summary>
	public const string NOT_FOUND = "Tamabin not found";

	/// <summary>
	/// The CONNECTED message.
	/// This message is returned if the device successfully connects with the Tambin in
	/// <see cref="TamabinConnector.TryConnect()"/> function.
	/// </summary>
	public const string CONNECTED = "Tamabin connected";

	/// <summary>
	/// The CONNECTED message.
	/// This message is returned if the device is already connected with the Tamabin in
	/// <see cref="TamabinConnector.TryConnect()"/> function.
	/// </summary>
	public const string ALREADY_CONNECTED = "Tamabin already connected";

	/// <summary>
	/// The NOT CONNECTED message.
	/// This message is returned if this device is not connected with the Tamabin in
	/// <see cref="TamabinConnector.TryDisconnect()"/> function.
	/// </summary>
	public const string NOT_CONNECTED = "Tamabin not connected";

	/// <summary>
	/// The DISCONNECTED message.
	/// This message is returned if this device was successfully disconnected from the Tamabin in
	/// <see cref="TamabinConnector.TryDisconnect()"/> function.
	/// </summary>
	public const string DISCONNECTED = "Tamabin disconnected";

	/// <summary>
	/// The singleton instance.
	/// This class is a singleton.
	/// </summary>
	private static TamabinConnector instance;

	/// <summary>
	/// Gets the singleton instance.
	/// Only one instance of this object can be used in the program.
	/// Multiple instances can produce java exceptions untreatable in the c# code.
	/// </summary>
	/// <returns>The singleton instance.</returns>
	public static TamabinConnector getInstance() {
		if (instance == null) {
			instance = new TamabinConnector();
		}
		return instance;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TamabinConnector"/> class.
	/// Private construtor to prevent external access, the instance of this object shold be get by
	/// <see cref="TamabinConnector.getInstance()"/> method.  
	/// </summary>
	private TamabinConnector() {
		bluetoothConnector = BluetoothConnector.getInstance();
		tamabinBluetoothName = null;
	}

	/// <summary>
	/// Sets the name of the tamabin bluetooth device name. If connected the name will not change.
	/// </summary>
	/// <param name="name">The name of the Tamabin bluetooth device.</param>
	public void SetTamabinBluetoothName(string name) {
		if (!bluetoothConnector.Connected()) {
			tamabinBluetoothName = name;
		}
	}

	/// <summary>
	/// Tries the connection with the Tamabin using the name set in
	/// <see cref="TamabinConnector.SetTamabinBluetoothName()"/>, if it has not set, the connection
	/// will fail.
	/// </summary>
	/// <returns>The connect message.</returns>
	public string TryConnect() {
		if (!bluetoothConnector.Avaliable()) {
			return NOT_AVALIABLE;
		} else if (!bluetoothConnector.Enabled()) {
			return NOT_ENABLED;
		} else {
			if (tamabinBluetoothName == null) {
				return NAME_NOT_SET;
			}
			if (bluetoothConnector.Connected()) {
				return ALREADY_CONNECTED;
			}
			string[] pairedDevices = bluetoothConnector.GetBondedDevices();
			foreach (string bluetoothDeviceInfo in pairedDevices) {
				if (bluetoothDeviceInfo.Contains(tamabinBluetoothName)) {
					string address = bluetoothDeviceInfo.Split(new char[]{ ',' })[0];
					bluetoothConnector.Connect(address);
					return CONNECTED;
				}
			}
			return NOT_FOUND;
		}
	}

	/// <summary>
	/// Tries the disconnection with the tamabin.
	/// </summary>
	/// <returns>The disconnect message.</returns>
	public string TryDisconnect() {
		if (bluetoothConnector.Connected()) {
			bluetoothConnector.Disconnect();
			return DISCONNECTED;
		}
		return NOT_CONNECTED;
	}

	/// <summary>
	/// Returns the Tamabin sent message, or null if it does not sent anything.
	/// This function too returns null if the Tamabin is not connected.
	/// </summary>
	/// <returns>The call.</returns>
	public string TamabinCall() {
		return bluetoothConnector.Read();
	}
}
