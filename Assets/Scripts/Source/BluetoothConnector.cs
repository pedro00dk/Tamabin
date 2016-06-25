using UnityEngine;
using System;

/// <summary>
/// Bluetooth connector.
/// Class responsible for connecting to bluetooth devices.
/// This class is not able to turn on or off bluetooth or search for Bluetooth devices, it is able
/// to connect to already paired devices, check if the Bluetooth adapter is available on the
/// device, if is turned on or off (enabled), get list of paired devices, connect or disconnect
/// with some of them and send or receive text messages for these devices.
/// </summary>
public class BluetoothConnector {

	/// <summary>
	/// The bluetooth adapter class.
	/// Represents an class instance of the android.bluetooth.BluetoothAdapter.
	/// Used to access static methods (create the bluetoothAdapter object).
	/// </summary>
	private AndroidJavaClass bluetoothAdapterClass;

	/// <summary>
	/// The bluetooth adapter.
	/// Represents an object instance of the android.bluetooth.BluetoothAdapter. 
	/// Used in several functions of this class.
	/// </summary>
	private AndroidJavaObject bluetoothAdapter;

	/// <summary>
	/// The UUID class.
	/// Represents an class instance of the java.util.UUID.
	/// Used to access static methods (create the connectionSocketUUID object).
	/// </summary>
	private AndroidJavaClass uuidClass;

	/// <summary>
	/// The connection socket UUID.
	/// Represents an object instance of the java.util.UUID.
	/// Used to create bluetooth sockets.
	/// </summary>
	private AndroidJavaObject connectionSocketUUID;

	/// <summary>
	/// The connected device.
	/// Represents an object instance of the android.bluetooth.BluetoothDevice.
	/// This object is the current connected bluetooth device, null if disconnected.
	/// </summary>
	private AndroidJavaObject connectedDevice;

	/// <summary>
	/// The connection socket.
	/// Represents an object instance of the android.bluetooth.BluetoothSocket.
	/// This object is the current connected socket, null if disconnected.
	/// </summary>
	private AndroidJavaObject connectionSocket;

	// Used to create the bufferedREader and use the function avaliable() to check when is
	// possible call read from the buffered reader without block.
	private AndroidJavaObject inputStream;

	/// <summary>
	/// The buffered reader.
	/// Represents an object instance of the java.io.BufferedReader.
	/// The reader of the connection, created using the InputStream object obtained from the
	/// socket.
	/// This reader is closed and receives null when the bluetooth connector is disconnected.
	/// </summary>
	private AndroidJavaObject bufferedReader;

	/// <summary>
	/// The buffered writer.
	/// Represents an object instance of the java.io.BufferedWriter.
	/// The writer of the connection, created using the OutputStream object obtained from the
	/// socket.
	/// This writer is closed and receives null when the bluetooth connector is disconnected.
	/// </summary>
	private AndroidJavaObject bufferedWriter;

	/// <summary>
	/// The singleton instance.
	/// This class is a singleton.
	/// </summary>
	private static BluetoothConnector instance = null;

	/// <summary>
	/// Gets the singleton instance.
	/// Only one instance of this object can be used in the program.
	/// Multiple instances can produce java exceptions untreatable in the c# code.
	/// </summary>
	/// <returns>The singleton instance.</returns>
	public static BluetoothConnector getInstance() {
		if (instance == null) {
			instance = new BluetoothConnector();
		}
		return instance;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="BluetoothConnector"/> class.
	/// Private construtor to prevent external access, the instance of this object shold be get by
	/// <see cref="BluetoothConnector.getInstance()"/> method.  
	/// </summary>
	private BluetoothConnector() {
		try {
			bluetoothAdapterClass = new AndroidJavaClass("android.bluetooth.BluetoothAdapter");
			bluetoothAdapter = bluetoothAdapterClass.CallStatic<AndroidJavaObject>("getDefaultAdapter");
		} catch (Exception) {
			bluetoothAdapter = null;
		}

		uuidClass = new AndroidJavaClass("java.util.UUID");
		// UUID for serail boards
		connectionSocketUUID = uuidClass.CallStatic<AndroidJavaObject>("fromString", "00001101-0000-1000-8000-00805F9B34FB");

		connectedDevice = null;
		connectionSocket = null;

		bufferedReader = null;
		bufferedWriter = null;
	}

	/// <summary>
	/// Returns if the current device has a bluetooth device avaliable.
	/// </summary>
	public bool Avaliable() {
		return bluetoothAdapter != null;
	}

	/// <summary>
	/// Returns if the current device has an enabled bluetooth device.
	/// </summary>
	public bool Enabled() {
		return Avaliable() && bluetoothAdapter.Call<bool>("isEnabled");
	}

	/// <summary>
	/// Returns if the current device is connected with other device.
	/// </summary>
	public bool Connected() {
		return Enabled() && connectedDevice != null && connectionSocket.Call<bool>("isConnected");
	}

	/// <summary>
	/// Gets the bonded (paired) devices, if the device is not enabled
	/// <see cref="BluetoothConnector.Enabled()"/> returns null, if there is no paired devices,
	/// return a zero sized string array.
	/// The form of the each device is: ADDRESS, NAME -> 00:11:22:33:AA:BB, Name
	/// </summary>
	/// <returns>The bonded devices.</returns>
	public string[] GetBondedDevices() {
		if (!Enabled()) {
			return null;
		}
		AndroidJavaObject bluetoothDevicesSet = bluetoothAdapter.Call<AndroidJavaObject>("getBondedDevices");
		AndroidJavaObject bluetoothDevicesList = new AndroidJavaObject("java.util.ArrayList", bluetoothDevicesSet);

		int bluetoothDevicesCount = bluetoothDevicesList.Call<int>("size");
		string[] bluetoothDevicesArray = new string[bluetoothDevicesCount];

		for (int i = 0; i < bluetoothDevicesCount; i++) {
			AndroidJavaObject bluetoothDevice = bluetoothDevicesList.Call<AndroidJavaObject>("get", i);

			string address = bluetoothDevice.Call<string>("getAddress");
			string name = bluetoothDevice.Call<string>("getName");

			bluetoothDevicesArray[i] = address + ", " + name;
		}
		return bluetoothDevicesArray;
	}

	/// <summary>
	/// Connect with the specified address.
	/// If the address is invalid of if the target device is not avaliable (bluetooth disabled or
	/// far of this device) the connection fail and throws notting, de connection state can be
	/// checked using the <see cref="BluetoothConnector.Connected()"/> function.
	/// The received address should be like 00:11:22:33:AA:BB.
	/// </summary>
	/// <param name="address">Mac address of the target device.</param>
	public void Connect(string address) {
		if (!Enabled()) {
			return;
		}
		if (Connected()) {
			Disconnect();
		}
		connectedDevice = bluetoothAdapter.Call<AndroidJavaObject>("getRemoteDevice", address);
		connectionSocket = connectedDevice.Call<AndroidJavaObject>("createRfcommSocketToServiceRecord", connectionSocketUUID);

		connectionSocket.Call("connect");

		inputStream = connectionSocket.Call<AndroidJavaObject>("getInputStream");
		bufferedReader = new AndroidJavaObject("java.io.BufferedReader", new AndroidJavaObject("java.io.InputStreamReader", inputStream));
		bufferedWriter = new AndroidJavaObject("java.io.BufferedWriter", new AndroidJavaObject("java.io.OutputStreamWriter", connectionSocket.Call<AndroidJavaObject>("getOutputStream")));
	}

	/// <summary>
	/// Disconnect if connected with another bluetooth device, if not connected do notting.
	/// </summary>
	public void Disconnect() {
		if (Connected()) {
			if (bufferedReader != null) {
				try {
					bufferedReader.Call("close");
				} catch (Exception) {
				}
				bufferedReader = null;
				inputStream = null;
			}
			if (bufferedWriter != null) {
				try {
					bufferedWriter.Call("close");
				} catch (Exception) {
				}
				bufferedWriter = null;
			}
			if (connectionSocket != null) {
				try {
					connectionSocket.Call("close");
				} catch (Exception) {
				}
				connectionSocket = null;
			}
			connectedDevice = null;
		}
	}

	/// <summary>
	/// Read the message sent by another device, if nothind was sent returns null, this function
	/// does not block the current thread.
	/// If is disconnected, returns null.
	/// </summary>
	public string Read() {
		if (Connected() && inputStream.Call<int>("available") > 0) {
			return bufferedReader.Call<string>("readLine");
		}
		return null;
	}

	/// <summary>
	/// Write the specified message and a new line. If is disconnected do nothing.
	/// </summary>
	/// <param name="message">Message.</param>
	public void Write(string message) {
		if (Connected()) {
			bufferedWriter.Call("write", message, 0, message.Length);
			bufferedReader.Call("newLine");
		}
	}
}
