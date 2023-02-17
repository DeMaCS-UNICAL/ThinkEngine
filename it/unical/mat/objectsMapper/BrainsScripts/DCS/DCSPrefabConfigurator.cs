using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.DCS
{
    [DisallowMultipleComponent]
    public class DCSPrefabConfigurator:MonoBehaviour
    {
        public bool isDangerous;
        public bool isWalkable;
        public bool isObstacle;
        public bool canFloat;
        public bool isStackable;
        public int minDistanceFromGround = 0;
        public int neededSpaceUp = 0;
        public int neededSpaceDown = 0;
        public int neededSpaceLeft = 0;
        public int neededSpaceRight = 0;
        internal static string _facts;
        internal static string Facts
        {
            get
            {
                if(_facts == null)
                {
                    _facts = "prefabName(Index,Name)."+Environment.NewLine;
                    _facts += "isDangerous(Index,Bool)." + Environment.NewLine;
                    _facts += "isWalkable(Index,Bool)." + Environment.NewLine;
                    _facts += "isObstacle(Index,Bool)." + Environment.NewLine;
                    _facts += "canFloat(Index,Bool)." + Environment.NewLine;
                    _facts += "isStackable(Index,Bool)." + Environment.NewLine;
                    _facts += "minDistanceFromGround(Index,Int)." + Environment.NewLine;
                    _facts += "neededSpaceUp(Index,Int)." + Environment.NewLine;
                    _facts += "neededSpaceDown(Index,Int)." + Environment.NewLine;
                    _facts += "neededSpaceLeft(Index,Int)." + Environment.NewLine;
                    _facts += "neededSpaceRight(Index,Int)." + Environment.NewLine;
                }
            }
        }
        internal string Mapping()
        {
            List<string> maps = new List<string>();
            maps.Add("prefabName({0}," + gameObject.name+")");
            maps.Add("isDangerous({0}," + isDangerous+")");
            maps.Add("isWalkable({0}," + isWalkable + ")");
            maps.Add("isObstacle({0}," + isObstacle + ")");
            maps.Add("canFloat({0}," + canFloat + ")");
            maps.Add("isStackable({0}," + isStackable + ")");
            maps.Add("minDistanceFromGround({0}," + minDistanceFromGround + ")");
            maps.Add("neededSpaceUp({0}," + neededSpaceUp + ")");
            maps.Add("neededSpaceDown({0}," + neededSpaceDown + ")");
            maps.Add("neededSpaceLeft({0}," + neededSpaceLeft + ")");
            maps.Add("neededSpaceRight({0}," + neededSpaceRight + ")");
            return string.Join("."+Environment.NewLine, maps);
        }

    }
}
