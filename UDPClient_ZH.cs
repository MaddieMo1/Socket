using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// 客户端
/// </summary>

public class UDPClient_ZH : MonoBehaviour
{
    //[Header("客户端")]
    //public Transform _TraClient;

    [Header("服务器地址")]
    public string _UDPClientIP;
    [Header("服务器端口")]
    public int _ConnectPort;


    private string _Send;
    private string _Receive01;


    public string _Receive;

    string _Str = "Socket01发送消息";
    Socket _Socket;
    EndPoint _ServerEnd;
    IPEndPoint _IpEnd;

    byte[] _RecvData = new byte[1024];
    byte[] _SendData = new byte[1024];
    int _RecvLen = 0;
    Thread _ConnectThread;

    //楼层判断布尔
    public static bool _BLayer;

    UDPClient_ZH _UDPClient;

    void Start()
    {

        //_UDPClientIP = "192.168.102.137";//服务端的IP

        _UDPClientIP = _UDPClientIP.Trim();

        InitSocket();
    }


    void InitSocket()
    {
        //IP、端口赋予
        _IpEnd = new IPEndPoint(IPAddress.Parse(_UDPClientIP), _ConnectPort);
        //_IpEnd = new IPEndPoint(IPAddress.Any, _ConnectPort);

        //通信通道赋予
        _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //客户端赋予
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 9631);
        //IPEndPoint sender = new IPEndPoint(IPAddress.Parse(_UDPClientIP), _ConnectPort);

        _ServerEnd = (EndPoint)sender;

        //绑定端口 IP
        _Socket.Bind(_ServerEnd);

        //print("等待连接");


        //字符串发送
        SocketSend(_Str);

        //print("连接");
        //开启一个线程连接
        _ConnectThread = new Thread(new ThreadStart(SocketReceive));
        _ConnectThread.Start();
    }

    //字符串发送到服务器
    void SocketSend(string sendStr)
    {
        //清空
        _SendData = new byte[1024];
        //数据转换
        _SendData = Encoding.UTF8.GetBytes(sendStr);

        //发送给指定服务端
        _Socket.SendTo(_SendData, _SendData.Length, SocketFlags.None, _IpEnd);
    }

    //接收服务器信息
    void SocketReceive()
    {
        while (true)
        {
            _RecvData = new byte[1024];
            try
            {
                //服务器消息接收
                _RecvLen = _Socket.ReceiveFrom(_RecvData, ref _ServerEnd);

            }
            catch (Exception e)
            {
            }

            //print("信息来自: " + _ServerEnd.ToString());
            if (_RecvLen > 0)
            {

                //转换
                _Receive01 = BitConverter.ToString(_RecvData).Replace("-", " ");

                _Receive = _Receive01;
                //空格去除
                _Receive = _Receive.Replace(" ", "");
                _Receive01 = "";

                //数据
                //print(_Receive);
                //print(_Receive.Length);
            }

            //筛选相应信息
            if (_Receive.Length >= 44 * 2)
            {
                //包头标识
                string baobiaoshi = _Receive.Substring(0, 4);
                if (baobiaoshi == "55AA")
                {
                    //数据长度
                    string dataLengthStr = _Receive.Substring(4, 4);

                    {
                        string temp = dataLengthStr.Substring(2, 2) + dataLengthStr.Substring(0, 2);
                        int dataLength = Convert.ToInt32(temp, 16);//将16进制的temp转换成10进制

                    }
                    //客户端地址
                    string clientIp = _Receive.Substring(8, 8);
                    //服务器地址
                    string serverIp = _Receive.Substring(16, 8);

                    //协议版本号
                    string version = _Receive.Substring(24, 2);
                    //数据类型
                    string dataType = _Receive.Substring(26, 2);
                    //包序号
                    string baoxuhao = _Receive.Substring(28, 4);

                    //摩卡托原点坐标 X
                    string _Mkt_x = _Receive.Substring(32, 16);
                    //摩卡托原点坐标 Y
                    string _Mkt_y = _Receive.Substring(48, 16);

                    //数据域   定位器 编号
                    string _Mac1 = _Receive.Substring(72, 4);

                    //print(_Mac1);

                    //三维坐标
                    string loc_x = _Receive.Substring(76, 4);
                    {
                        string temp = loc_x.Substring(2, 2) + loc_x.Substring(0, 2);
                        int _data = Convert.ToInt32(temp, 16);//将16进制的temp转换成10进制

                        //数据表 X
                        LocationExl_ZH._MktX = temp;
                    }


                    string loc_y = _Receive.Substring(80, 4);
                    {
                        string temp = loc_y.Substring(2, 2) + loc_y.Substring(0, 2);
                        int _data = Convert.ToInt32(temp, 16);//将16进制的temp转换成10进制

                        //数据表Y
                        LocationExl_ZH._MktY = temp;

                    }

                    //////////这一条先判断，值为03或04时此条记录有效，其它值无效，忽视
                    string mapNo = _Receive.Substring(84, 2);
                    if (mapNo == "08")
                    {
                        //定位器编号赋予
                        LocationExl_ZH._PositionStr = _Mac1;

                        _BLayer = true;

                        print("站厅----地下二层");

                    }
                    else if (mapNo == "09")
                    {
                        //定位器编号赋予
                        LocationExl_ZH._PositionStr = _Mac1;

                        _BLayer = false;

                        print("站台----地下三层");

                    }
                    //包结束位
                    string dianliang = _Receive.Substring(86, 2);
                    string other = _Receive.Substring(88);
                }
                else
                {
                    Debug.LogError("数据错误 !");
                }
            }
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
    }
    void OnApplicationQuit()
    {
        SocketQuit();
    }

    private void Update()
    {
        //if (Input.GetKey(KeyCode.KeypadEnter))
        //{
        //    //发送
        //    SocketSend(_RecvStr);
        //    print("发送字符");
        //}
        //SocketSend(_Send);

        //如果警员定位 没隐藏  就执行更新方法
        //if (_TraClient.gameObject.activeInHierarchy)
        //{
        //print("11111111");
        //_TraClient.GetComponent<LocationExl_ZH>().VerifyNumber();
        //}
    }
}
