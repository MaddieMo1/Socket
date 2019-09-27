using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OfficeOpenXml;
using System.IO;
using UnityEngine.UI;
using System;

/// <summary>
/// 数据表存储
/// </summary>

public class LocationExl_ZH : MonoBehaviour
{
    [Header("警员定位Canvas")]
    public Transform _CopCanvas;

    [Header("数据表最大人数")]
    private int _CopInt = 53;

    [Header("警员定位预制体")]
    public Transform _PrefabricateLocation;

    [Header("警员定位物体数组")]
    public static List<Transform> _ListTra = new List<Transform>();

    [Header("交管巡防大队预制体")]
    public Transform _TraPatrol;

    [Header("第三派出所")]
    public Transform _TraCop;

    [Header("定位器编号")]
    public static string _PositionStr;
    [Header("摩卡托X")]
    public static string _MktX;
    [Header("摩卡托Y")]
    public static string _MktY;

    //行
    int _Row;
    //Y轴高度
    float _PositionY;
    //警员定位按钮布尔
    bool _bCanvas = false;

    //数据表
    ExcelPackage _ExcelPackage;
    //数据表第一页
    ExcelWorksheet _Worksheet;

    void Start()
    {
        // 数据表路径
        string _FilePath = Application.streamingAssetsPath + "/AoTiExl/奥体中心站警员定位涉及到警员信息表.xlsx";

        // string _FilePath = "Assets/Editer/奥体中心站警员定位涉及到警员信息表.xlsx";

        //获取数据表文件
        FileInfo _FileInfo = new FileInfo(_FilePath);



        using (_ExcelPackage = new ExcelPackage(_FileInfo))
        {
            //通过  ExcelPackage  打开数据表
            _ExcelPackage = new ExcelPackage(_FileInfo);

            //获取数据表中第一张表格数据
            _Worksheet = _ExcelPackage.Workbook.Worksheets[1];

            _CopInt = _Worksheet.Cells.Rows;
            //_CopInt = 51;
        }

        //using (_ExcelPackage = new ExcelPackage(_FileInfo))
        //{
        //表格创建
        //ExcelWorksheet _WorksheetNew = _ExcelPackage.Workbook.Worksheets.Add("奥体中心站警员定位涉及到警员信息表");

        //获取数据表中第一张表格数据
        //  _Worksheet = _ExcelPackage.Workbook.Worksheets[1];

        //获取数据表中第一行第一列数据
        //string _Message = _Worksheet.Cells[3, 5].Value.ToString();

        //数据表写入
        //_Worksheet.Cells[3, 5].Value = "1234";

        //保存数据表
        //_ExcelPackage.Save();
        //  }
    }

    void Update()
    {
        //如果警员定位按钮是激活状态
        if (_bCanvas)
        {
            //校验定位器编号数据方法
            VerifyNumber();
        }
    }

    //警员定位按钮方法
    public void ButtonIson()
    {
        _bCanvas = !_bCanvas;

        if (_ListTra.Count > 0)
        {
            for (int i = 0; i < _ListTra.Count; i++)
            {
                if (_bCanvas)
                {
                    _ListTra[i].gameObject.SetActive(_bCanvas);
                }
                else
                {
                    _ListTra[i].gameObject.SetActive(_bCanvas);
                }
            }
        }
    }

    //校验定位器编号数据
    public void VerifyNumber()
    {
        for (int i = 0; i < _CopInt; i++)
        {

            //行 列 
            _Row = i + 3;

            //数据表定位器编号
            string _StrNumber = "";
            //数据表警员编号
            string _CopNumber = "";


            if (_Worksheet.Cells[_Row, 2].Value.ToString() != null)
            {
                //数据表定位器编号
                _StrNumber = _Worksheet.Cells[_Row, 1].Value.ToString();

                //数据表警员编号
                _CopNumber = _Worksheet.Cells[_Row, 2].Value.ToString();
            }
            //Debug.Log("定位器的编号："+_PositionStr+"    列表中的信息编号:"+ _StrNumber);
            //如果在数据表找到相应 定位编号
            if (_PositionStr == _StrNumber)
            {
                //如果警员定位UI下有子物体
                if (_CopCanvas.transform.childCount > 0)
                {
                    //遍历UI数据
                    for (int k = 0; k < _CopCanvas.transform.childCount; k++)
                    {
                        //UI警员编号
                        string _CopNumber00 = _CopCanvas.transform.GetChild(k).Find("警员编号/Text").GetComponent<Text>().text;

                        //如果在子物体下找到相应 警员编号 信息  那就证明存在
                        if (_CopNumber00 == _CopNumber)
                        {
                            //生成物体位置以及数据位置变更 
                            //PositionPer();

                            //开启位置更新协程
                            StartCoroutine(IEPositionPer(_CopNumber, _StrNumber));
                            break;
                        }
                        else
                        {
                            //如果 寻找到UI最后一位 并且当前警员编号不等于传进来的警员编号
                            if (k == _CopCanvas.transform.childCount - 1 && _CopNumber00 != _CopNumber)
                            {
                                //数据确认以及信息变更
                                StartCoroutine(IEBoolEnter());
                            }
                        }
                    }
                }
                else
                {
                    //如果没有子物体
                    //print("没有数据");
                    //数据确认以及信息变更
                    StartCoroutine(IEBoolEnter());
                }

                break;
            }
        }
    }

    //转换X
    public int ReturnX(string _Xstring)
    {
        int _PositionX;
        _PositionX = Convert.ToInt32(_MktX, 16);//将16进制的temp转换成10进制

        //循环遍历警员定位UI下子物体
        for (int i = 0; i < _CopCanvas.childCount; i++)
        {
            //如果在子物体找到和当前不相同的警员编号
            if (_CopCanvas.GetChild(i).Find("警员编号/Text").GetComponent<Text>().text != _Xstring)
            {
                //UI位置Text
                string _StrPo = _CopCanvas.GetChild(i).Find("位置/Text").GetComponent<Text>().text;
                //寻找特定字符索引
                int _Index = _StrPo.IndexOf(",");

                //查看参数是否相同   字符串切分
                if (_StrPo.Substring(0, _Index) == (_PositionX - 127).ToString())
                {
                    _PositionX = _PositionX - 127 + 1;
                    break;
                }
            }
            else
            {
                _PositionX = _PositionX - 127;
                break;
            }
        }
        return _PositionX;
    }

    //转换Z
    private int ReturnZ(string _Zstring)
    {
        int _PositionX;
        _PositionX = Convert.ToInt32(_MktY, 16);//将16进制的temp转换成10进制

        //循环遍历警员定位UI下子物体
        for (int i = 0; i < _CopCanvas.childCount; i++)
        {
            //如果在子物体找到和当前不相同的警员编号
            if (_CopCanvas.GetChild(i).Find("警员编号/Text").GetComponent<Text>().text != _Zstring)
            {
                //UI位置Text
                string _StrPo = _CopCanvas.GetChild(i).Find("位置/Text").GetComponent<Text>().text;
                //寻找特定字符索引
                int _Index = _StrPo.IndexOf(",");

                //查看参数是否相同   字符串切分
                if (_StrPo.Substring(_Index + 1, _StrPo.Length - 1 - _Index) == (_PositionX - 50).ToString())
                {
                    _PositionX = _PositionX - 50 + 1;
                    break;
                }
            }
            else
            {
                _PositionX = _PositionX - 50;
                break;
            }
        }
        return _PositionX;
    }

    //数据确认以及信息录入
    IEnumerator IEBoolEnter()
    {
        //生成警员定位预制体
        Transform _TraPer = Instantiate(_PrefabricateLocation);

        //设置警员定位UI为父类
        _TraPer.SetParent(_CopCanvas);
        _TraPer.localScale = Vector3.one;

        //数据信息录入
        _TraPer.Find("警员编号/Text").GetComponent<Text>().text = _Worksheet.Cells[_Row, 2].Value.ToString();
        _TraPer.Find("所属机构/Text").GetComponent<Text>().text = _Worksheet.Cells[_Row, 3].Value.ToString();
        _TraPer.Find("姓名/Text").GetComponent<Text>().text = _Worksheet.Cells[_Row, 4].Value.ToString();
        _TraPer.Find("位置/Text").GetComponent<Text>().text = _Worksheet.Cells[_Row, 5].Value.ToString();
        _TraPer.Find("警务通号码/Text").GetComponent<Text>().text = _Worksheet.Cells[_Row, 6].Value.ToString();

        yield return StartCoroutine(IECreateModel());
    }

    //模型生成
    IEnumerator IECreateModel()
    {

        //模型生成 位置变更
        Transform _ModleTra;

        //地图坐标赋予
        int _PositionX = Convert.ToInt32(_MktX, 16);//将16进制的temp转换成10进制
        int _PositionZ = Convert.ToInt32(_MktY, 16);//将16进制的temp转换成10进制

        if (UDPClient_ZH._BLayer)
        {
            string _Unit = "";

            if (_Worksheet.Cells[_Row, 3].Value.ToString() != null)
            {
                _Unit = _Worksheet.Cells[_Row, 3].Value.ToString();
                //判断所属机构
                switch (_Unit)
                {
                    case "交管巡防大队":

                        _ModleTra = Instantiate(_TraPatrol);

                        //位置锁定 模型 

                        for (int i = 0; i < _CopCanvas.childCount; i++)
                        {
                            if (_Worksheet.Cells[_Row, 2].Value.ToString() == _CopCanvas.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>().text)
                            {
                                _CopCanvas.GetChild(i).GetChild(0).GetComponent<LockCamera_ZH>().ThisPerson = _ModleTra;
                                break;
                            }
                        }

                        //预制体模型数组存入
                        _ListTra.Add(_ModleTra);

                        //姓名 楼层 警员编号
                        _ModleTra.name = _Worksheet.Cells[_Row, 4].Value.ToString() + ",02" + ":" + _Worksheet.Cells[_Row, 2].Value.ToString();

                        //位置参数变更
                        _ModleTra.position = new Vector3(-21, 27.0f, 11);
                        _ModleTra.eulerAngles = new Vector3(-90.0f, 0.0f, -90.0f);



                        //名字显示
                        WordSpaceToViewSpace.AddPeopleName(_ModleTra, _ModleTra.name.Substring(0, _ModleTra.name.IndexOf(",")));
                        break;

                    case "第三派出所":

                        _ModleTra = Instantiate(_TraCop);

                        //位置锁定 模型 
                        for (int i = 0; i < _CopCanvas.childCount; i++)
                        {
                            if (_Worksheet.Cells[_Row, 2].Value.ToString() == _CopCanvas.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>().text)
                            {
                                _CopCanvas.GetChild(i).GetChild(0).GetComponent<LockCamera_ZH>().ThisPerson = _ModleTra;
                                break;
                            }
                        }

                        //姓名 楼层 警员编号
                        _ModleTra.name = _Worksheet.Cells[_Row, 4].Value.ToString() + ",02" + ":" + _Worksheet.Cells[_Row, 2].Value.ToString();

                        //位置参数变更
                        _ModleTra.position = new Vector3(-21, 27.0f, 11);
                        _ModleTra.eulerAngles = new Vector3(-90.0f, 0.0f, 90.0f);

                        //预制体模型数组存入
                        _ListTra.Add(_ModleTra);

                        //名字显示
                        WordSpaceToViewSpace.AddPeopleName(_ModleTra, _ModleTra.name.Substring(0, _ModleTra.name.IndexOf(",")));

                        break;

                    default:
                        break;
                }
            }
        }
        else
        {
            string _Unit = "";

            if (_Worksheet.Cells[_Row, 3].Value.ToString() != null)
            {
                _Unit = _Worksheet.Cells[_Row, 3].Value.ToString();
                //判断所属机构
                switch (_Unit)
                {
                    case "交管巡防大队":

                        _ModleTra = Instantiate(_TraPatrol);

                        //位置锁定 模型 
                        for (int i = 0; i < _CopCanvas.childCount; i++)
                        {
                            if (_Worksheet.Cells[_Row, 2].Value.ToString() == _CopCanvas.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>().text)
                            {
                                _CopCanvas.GetChild(i).GetChild(0).GetComponent<LockCamera_ZH>().ThisPerson = _ModleTra;
                                break;
                            }
                        }

                        //姓名 楼层 警员编号
                        _ModleTra.name = _Worksheet.Cells[_Row, 4].Value.ToString() + ",03" + ":" + _Worksheet.Cells[_Row, 2].Value.ToString();

                        //位置参数变更
                        _ModleTra.position = new Vector3(-21, 4.5f, 11);
                        _ModleTra.eulerAngles = new Vector3(-90.0f, 0.0f, -90.0f);

                        //预制体模型数组存入
                        _ListTra.Add(_ModleTra);

                        //名字显示
                        WordSpaceToViewSpace.AddPeopleName(_ModleTra, _ModleTra.name.Substring(0, _ModleTra.name.IndexOf(",")));
                        break;

                    case "第三派出所":

                        _ModleTra = Instantiate(_TraCop);

                        //位置锁定 模型 
                        for (int i = 0; i < _CopCanvas.childCount; i++)
                        {
                            if (_Worksheet.Cells[_Row, 2].Value.ToString() == _CopCanvas.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>().text)
                            {
                                _CopCanvas.GetChild(i).GetChild(0).GetComponent<LockCamera_ZH>().ThisPerson = _ModleTra;
                                break;
                            }
                        }

                        //姓名 楼层 警员编号
                        _ModleTra.name = _Worksheet.Cells[_Row, 4].Value.ToString() + ",03" + ":" + _Worksheet.Cells[_Row, 2].Value.ToString();

                        //位置参数变更
                        _ModleTra.position = new Vector3(-21, 4.5f, 11);
                        _ModleTra.eulerAngles = new Vector3(-90.0f, 0.0f, 90.0f);

                        //预制体模型数组存入
                        _ListTra.Add(_ModleTra);

                        //名字显示
                        WordSpaceToViewSpace.AddPeopleName(_ModleTra, _ModleTra.name.Substring(0, _ModleTra.name.IndexOf(",")));

                        break;

                    default:
                        break;
                }
            }
        }

        yield break;
    }

    //位置更新
    IEnumerator IEPositionPer(string _CopNumber, string _StrNumber)
    {
        //地图坐标赋予
        int _PositionX = Convert.ToInt32(_MktX, 16);//将16进制的temp转换成10进制
        int _PositionZ = Convert.ToInt32(_MktY, 16);//将16进制的temp转换成10进制

        //名字显示更新 //TODO 时机？？？
        WordSpaceToViewSpace.UpdateControl = true;

        //如果是F2 _BLayer = True 生成位置 Y轴 就设为6.25f  否则就设为1.0f
        for (int i = 0; i < _ListTra.Count; i++)
        {
            //缓动值
            Vector3 _LerpMove;

            if (_ListTra[i].name.Contains(_CopNumber))
            {
                //传递UI 警员编号
                _PositionX = ReturnX(_CopNumber);
                //_PositionX -= 127;
                //传递UI 警员编号
                _PositionZ = ReturnZ(_CopNumber);
                //_PositionZ -= 50;

                //名称重新赋予
                if (UDPClient_ZH._BLayer)
                {
                    //姓名 楼层 警员编号
                    _ListTra[i].name = _Worksheet.Cells[_Row, 4].Value.ToString() + ",02" + ":" + _Worksheet.Cells[_Row, 2].Value.ToString();
                }
                else
                {
                    //姓名 楼层 警员编号
                    _ListTra[i].name = _Worksheet.Cells[_Row, 4].Value.ToString() + ",03" + ":" + _Worksheet.Cells[_Row, 2].Value.ToString();
                }


                switch (_Worksheet.Cells[_Row, 3].Value.ToString())
                {
                    case "交管巡防大队":

                        //Y轴等于射线打击到的位置Y轴加上本身的高度值
                        _PositionY = _ListTra[i].GetComponent<ModelRay_ZH>()._HitPointY + 1.0f;
                        _ListTra[i].eulerAngles = new Vector3(-90.0f, -90.0f, 0.0f);

                        break;

                    case "第三派出所":
                        //Y轴等于射线打击到的位置Y轴加上本身的高度值
                        _PositionY = _ListTra[i].GetComponent<ModelRay_ZH>()._HitPointY;
                        _ListTra[i].eulerAngles = new Vector3(-90.0f, 0, 90.0f);
                        break;

                    default:
                        break;
                }

                //位置参数变更
                _LerpMove = new Vector3(_PositionX, _PositionY, _PositionZ);

                //缓动
                _ListTra[i].position = Vector3.Lerp(_ListTra[i].position, _LerpMove, 0.35f);
                //ListTra[i].position = _LerpMove;

                _ListTra[i].eulerAngles = new Vector3(-90.0f, -90.0f, 0.0f);

                //数据表位置信息变更
                //_Worksheet.Cells[_Row, 5].Value = _PositionX.ToString() + "," + _PositionZ.ToString();

                //位置实时更新
                for (int j = 0; j < _CopCanvas.transform.childCount; j++)
                {
                    if (_CopNumber == _CopCanvas.transform.GetChild(j).Find("警员编号/Text").GetComponent<Text>().text)
                    {
                        _CopCanvas.transform.GetChild(j).Find("位置/Text").GetComponent<Text>().text = _PositionX.ToString() + "," + _PositionZ.ToString();
                        break;
                    }
                }

                //保存数据表
                // _ExcelPackage.Save();
                break;
            }
        }
        yield return null;
    }
}