using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class NetworkConnector : MonoBehaviour {
	public delegate void OnReadType(string input);

	public const int Port = 25000;

	public InputField IpText;
	public OnReadType OnRead;

	public bool[] LocalPlayer;
	
	private bool IsConnected;
	private byte[] SocketBuffer;
	private Socket MySocket;
	private Socket YourSocket;

	void Start() {
		this.LocalPlayer = new bool[2] {false, false};
		this.IsConnected = false;
		SocketBuffer = new byte[4096];
		DontDestroyOnLoad(this.transform.gameObject);
	}
	
	void Update() {
		if(this.IsConnected) {
			Application.LoadLevel("sceneBattle");
			this.IsConnected = false;
		}
	}

	public void SelfConnect() {
		LocalPlayer[0] = true;
		LocalPlayer[1] = true;
		IsConnected = true;
	}

	public void CreateServer() {
		MySocket = new Socket(AddressFamily.InterNetwork,
							  SocketType.Stream,
							  ProtocolType.IP);
		MySocket.Bind(new IPEndPoint(IPAddress.Any, NetworkConnector.Port));
		MySocket.Listen(10);
		MySocket.BeginAccept(new System.AsyncCallback(this.OnSocketConnect),
							 null);
	}

	public void JoinServer() {
		YourSocket = new Socket(AddressFamily.InterNetwork,
								SocketType.Stream,
								ProtocolType.IP);
		try {
			YourSocket.Connect(IpText.text,
							   NetworkConnector.Port);
			this.LocalPlayer[0] = false;
			this.LocalPlayer[1] = true;
			this.IsConnected = true;
			YourSocket.BeginReceive(this.SocketBuffer,
									0,
									this.SocketBuffer.Length,
									SocketFlags.None,
									new System.AsyncCallback(this.OnSocketRead),
									null);
		}
		catch{
			Debug.Log("Join Failed!");
		}
	}

	public void SendString(string input) {
		byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes(input);
		YourSocket.BeginSend(sendBuffer,
							 0,
							 sendBuffer.Length,
							 SocketFlags.None,
							 new System.AsyncCallback(this.OnSocketWrite),
							 null);
	}

	private void OnSocketConnect(System.IAsyncResult ar) {
		Debug.Log("Wow! Such Connection!");
		YourSocket = MySocket.EndAccept(ar);
		YourSocket.BeginReceive(this.SocketBuffer,
								0,
								this.SocketBuffer.Length,
								SocketFlags.None,
								this.OnSocketRead,
								null);
		this.LocalPlayer[0] = true;
		this.LocalPlayer[1] = false;
		this.IsConnected = true;
	}

	private void OnSocketRead(System.IAsyncResult ar) {
		if(YourSocket.EndReceive(ar) > 0) {
			Debug.Log("Read : " + System.Text.Encoding.ASCII.GetString(this.SocketBuffer));
			OnRead(System.Text.Encoding.ASCII.GetString(this.SocketBuffer));
			YourSocket.BeginReceive(this.SocketBuffer,
									0,
									this.SocketBuffer.Length,
									SocketFlags.None,
									this.OnSocketRead,
									null);
		}
		else {
			YourSocket.Close();
			MySocket.Close();
		}
	}

	private void OnSocketWrite(System.IAsyncResult ar) {
		if(MySocket.EndSend(ar) > 0) {
			//Debug.Log("Send Data!");
		}
		else {
			YourSocket.Close();
			MySocket.Close();
		}
	}

	public void OnDestroy() {
		try { YourSocket.Close(); } catch {}
		try { MySocket.Close(); } catch {}
	}
}