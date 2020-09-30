using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class GlobalIndexer
{
    private static object toLock = new object();
    static internal int maxIndexUsed;

    internal static int assignIndex()
    {
        lock (toLock)
        {
            return ++maxIndexUsed;
        }
    }
}
