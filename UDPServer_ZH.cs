using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// 服务端
/// </summary>

public class UDPServer_ZH : MonoBehaviour
{
    [Header("服务端IP")]
    public string _IpAddress = "192.168.102.137";
    [Header("服务端端口")]
    public int _ConnectPort = 9631;
    [Header("发送字符")]
    public string _Send;
    [Header("接受字符")]
    public string _Receive;

    Socket _Socket;
    EndPoint _ClientEnd;
    IPEndPoint _IpEnd;
    string _SendStr;
    byte[] _RecvData = new byte[1024];
    byte[] _SendData = new byte[1024];
    int _RecvLen;
    Thread _ConnectThread;

    //初始化
    void InitSocket()
    {
        _IpEnd = new IPEndPoint(IPAddress.Parse(_IpAddress), _ConnectPort);
       // _IpEnd = new IPEndPoint(IPAddress.Any, _ConnectPort);

        _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        _Socket.Bind(_IpEnd);

        //定义客户端
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 9631);
        //IPEndPoint sender = new IPEndPoint(IPAddress.Parse(_IpAddress), _ConnectPort);
        _ClientEnd = sender;

        print("等待连接数据");

        //开启一个线程连接
        _ConnectThread = new Thread(new ThreadStart(SocketReceive));

        _ConnectThread.Start();
    }

    //发送
    void SocketSend(string sendStr)
    {
        _SendData = new byte[1024];

        _SendData = Encoding.UTF8.GetBytes(sendStr);

        _Socket.SendTo(_SendData, _SendData.Length, SocketFlags.None, _ClientEnd);
    }

    //服务器接收
    void SocketReceive()
    {
        while (true)
        {
            _RecvData = new byte[1024];

            _RecvLen = _Socket.ReceiveFrom(_RecvData, ref _ClientEnd);

            _Receive = Encoding.UTF8.GetString(_RecvData, 0, _RecvLen);

            Debug.Log("收到得信息 " + _Receive);
        }
    }


    //连接关闭
    void SocketQuit()
    {
        //关闭线程
        if (_ConnectThread != null)
        {
            _ConnectThread.Interrupt();
            _ConnectThread.Abort();
        }
        //最后关闭socket
        if (_Socket != null)
        {
            _Socket.Close();
        }
        Debug.LogWarning("断开连接");
    }

    void Start()
    {
        InitSocket(); //在这里初始化server
    }

    void OnApplicationQuit()
    {
        SocketQuit();
    }


    void Update()
    {
        SocketSend(_Send);
        //if (Input.GetKey(KeyCode.KeypadEnter))
        //{
        //    SocketSend(_RecvStr);
        //}
    }
}
