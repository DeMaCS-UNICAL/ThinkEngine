using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkEngine.Mappers;
using UnityEngine;

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.DCS
{
    [DisallowMultipleComponent, ExecuteInEditMode, Serializable]
    public class DCSPrefabConfigurator:MonoBehaviour
    {
        internal static Dictionary<int, DCSPrefabConfigurator> instances = new Dictionary<int, DCSPrefabConfigurator>();
        public int index;
        public bool isDangerous;
        public bool isWalkable;
        public bool isObstacle;
        public bool canFloat;
        public bool isStackable;
        public bool cannotBeTopped;
        public bool cannotBeTop;
        public List<DCSPrefabConfigurator> canBeToppedBy;
        public List<DCSPrefabConfigurator> canBeSidedBy;
        public List<DCSPrefabConfigurator> bestSidedBy;
        internal static string _facts;

        void Awake()
        {
            
            if (canBeSidedBy == null)
            {
                canBeSidedBy = new List<DCSPrefabConfigurator>();
                canBeToppedBy = new List<DCSPrefabConfigurator>();
                bestSidedBy = new List<DCSPrefabConfigurator>();
            }
        }
        internal void Init()
        {
            if (gameObject.activeInHierarchy)
            {
                enabled = false;
            }
            index = gameObject.GetInstanceID();
            instances[index] = this;
        }
        internal static string Facts
        {
            get
            {
                if(_facts == null)
                {
                    _facts = "%prefabName(Index,Name)."+Environment.NewLine;
                    _facts += "%isDangerous(Index)." + Environment.NewLine;
                    _facts += "%isWalkable(Index)." + Environment.NewLine;
                    _facts += "%isObstacle(Index)." + Environment.NewLine;
                    _facts += "%canFloat(Index)." + Environment.NewLine;
                    _facts += "%isStackable(Index)." + Environment.NewLine;
                    _facts += "%cannotBeTopped(Index)." + Environment.NewLine;
                    _facts += "%cannotBeTop(Index)." + Environment.NewLine;
                    _facts += "%canBeToppedBy(BottomIndex,PossibleTopperIndex)." + Environment.NewLine;
                    _facts += "%canBeSidedBy(Index,PossibleSiderIndex)." + Environment.NewLine;
                    _facts += "%bestSidedBy(Index,PossibleSiderIndex)." + Environment.NewLine;
                }
                return _facts;
            }
        }
        internal string Mapping()
        {
            List<string> maps = new List<string>();
            maps.Add("prefabName("+index+"," + PropertyMap(gameObject.name)+")");
            if (isDangerous)
            {
                maps.Add("isDangerous("+index+")");
            }
            if (isWalkable)
            {
                maps.Add("isWalkable(" + index + ")");
            }
            if (isObstacle)
            {
                maps.Add("isObstacle(" + index + ")");
            }
            if (canFloat)
            {
                maps.Add("canFloat(" + index + ")");
            }
            if (isStackable)
            {
                maps.Add("isStackable(" + index + ")");
            }
            if (cannotBeTopped)
            {
                maps.Add("cannotBeTopped(" + index + ")");
            }
            if (cannotBeTop)
            {
                maps.Add("cannotBeTop(" + index + ")");
            }
            if (canBeToppedBy.Count>0)
            {
                foreach (DCSPrefabConfigurator prefab in canBeToppedBy)
                {
                    maps.Add("cannotBeTopped("+index+","+prefab.index+")");
                }
            }
            if (canBeSidedBy.Count > 0)
            {
                foreach (DCSPrefabConfigurator prefab in canBeSidedBy)
                {
                    maps.Add("canBeSidedBy(" + index + "," + prefab.index + ")");
                }
            }
            if (bestSidedBy.Count > 0)
            {
                foreach (DCSPrefabConfigurator prefab in bestSidedBy)
                {
                    maps.Add("bestSidedBy(" + index + "," + prefab.index + ")");
                }
            }
            return string.Join("."+Environment.NewLine, maps)+"."+Environment.NewLine;
        }

        private string PropertyMap(object obj)
        {
            return BasicTypeMapper.GetMapper(obj.GetType()).BasicMap(obj);
        }

    }
}
