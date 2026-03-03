using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class TestScript : MonoBehaviour
{
   [SerializeField]public string abc { get; set; }
    private string name = "LLLMMM";
    public string Name { get { return name; } set { name = value; } }

    [field: SerializeField] public string _Name { get; private set; }
    private void Start()
    {
        var p = new TestTwo();
        Debug.Log(p.Name);
        p.Name = "Fan";
        Debug.Log(p.Name);

        var p1 = new TestThree();
        Debug.Log(p1.Name);
        // 重新赋值也不会改变
        p1.Name = "FanFan";
        Debug.Log(p1.Name);

        var p2 = new TestFour();
        Debug.Log(p2.Name);
        p2.Name = "FanFan";
        Debug.Log(p2.Name);

        Debug.Log(Name);
        name = "PPPQQQ";
        Debug.Log(Name);
        Name = "CCCSSS";
        Debug.Log(Name);
        Debug.Log(name);
    }
}
public class TestTwo
{
    private string name;
    public string Name { get; set; }
    
}
public class TestThree
{
    private string name = "Lin";
    private string name1 = "SSSS";
    public string Name { get { return name; } set { name1 = value; } }

}
public class TestFour
{
    private string name = "SDDD";
    public string Name { get { return name; } set { name = value; } }

}