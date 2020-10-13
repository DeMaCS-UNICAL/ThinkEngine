using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class MyListString:List<string>
{
    public MyListString():base()
    {
    }

    public MyListString(List<string> clone):base()
    {
        this.AddRange(clone);
    }
    public override bool Equals(object obj)
    {
        return this.SequenceEqual((MyListString)obj);
    }
    public new MyListString GetRange(int start, int count)
    {
        return new MyListString(base.GetRange(start, count));
    }
}