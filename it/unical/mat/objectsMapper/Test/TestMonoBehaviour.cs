
using System.Collections.Generic;
using UnityEngine;

public class TestMonoBehaviour : MonoBehaviour
{
    enum TestingEnum { TEST1, TEST2};
    int _testInt;
    public int testInt; 
    char _testChar;
    string _testString;
    TestingEnum _testEnum;
    float _testFloat;
    uint _testUInt;
    int[,] _testIntMatrix;
    List<string> _testStringList;
    List<double>[,] _testDoubleListMatrix;
    TestMonoBehaviour[,] _testComplexMatrix;
    List<TestMonoBehaviour> _testComplexList;
    int frame;

    void Start()
    {
        _testInt = 10;
        testInt = 100;
        _testChar = 'a';
        _testString = "testing";
        _testEnum = TestingEnum.TEST1;
        _testFloat = 2.5f;
        _testUInt = 4;
        _testIntMatrix = new int[2, 2];
        _testIntMatrix[0, 0] = 1;
        _testIntMatrix[0, 1] = 2;
        _testIntMatrix[1, 0] = 3;
        _testIntMatrix[1, 1] = 4;
        _testStringList = new List<string> { "string1", "string2", "string3" };
        _testDoubleListMatrix = new List<double> [1,2];
        _testDoubleListMatrix[0, 0] = new List<double>();
        _testDoubleListMatrix[0, 1] = new List<double>();
        _testComplexMatrix = new TestMonoBehaviour[1, 3];
        _testComplexMatrix[0, 0] = this;
        _testComplexMatrix[0, 1] = this;
        _testComplexMatrix[0, 2] = this;
        _testComplexList = new List<TestMonoBehaviour> { this };
    }

    void Update()
    {
        frame++;
        if (frame <=600 && frame % 200 == 0)
        {
            _testInt -= 2;
            testInt += 1;
            _testChar++;
            _testString += frame/200;
            _testFloat = 2.5f;
        }
        if (frame == 200)
        {
            _testIntMatrix = new int[1, 2];
            _testIntMatrix[0, 0] = 5;
            _testIntMatrix[0, 1] = 6;
            _testStringList.Add("string4");
            _testDoubleListMatrix[0, 0].Add(2);
            _testDoubleListMatrix[0, 1] .Add(3.4);
            _testDoubleListMatrix[0, 1] .Add(4);
            _testComplexMatrix = new TestMonoBehaviour[1, 1];
            _testComplexMatrix[0, 0] = this;
            _testComplexList.Add(this);
        }
        if (frame == 400)
        {
            _testIntMatrix = null;
            _testStringList.Remove("string3");
            _testDoubleListMatrix[0, 0].Add(5);
            _testComplexMatrix[0, 0] = null;
            _testComplexList = null;
        }
        if(frame == 600)
        {
            _testDoubleListMatrix = null;
            _testStringList= null;
            _testComplexMatrix= null;
            _testIntMatrix = new int[3, 3];
            _testIntMatrix[0, 0] = 0;
            _testIntMatrix[0, 0] = 1;
            _testIntMatrix[0, 0] = 2;
            _testIntMatrix[0, 0] = 3;
            _testIntMatrix[0, 0] = 4;
            _testIntMatrix[0, 0] = 5;
            _testIntMatrix[0, 0] = 6;
            _testIntMatrix[0, 0] = 7;
            _testIntMatrix[0, 0] = 8;
        }
    }
}