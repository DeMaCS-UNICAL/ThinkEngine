using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ThinkEngine.Mappers;
using UnityEngine;

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.DCS
{
    [DisallowMultipleComponent, ExecuteInEditMode, Serializable]
    public class ContentPrefabConfigurator:MonoBehaviour
    {
        [AttributeUsage(AttributeTargets.Field)]
        public class Mappable : Attribute
        {
        }

        internal static Dictionary<int, ContentPrefabConfigurator> instances = new Dictionary<int, ContentPrefabConfigurator>();
        private static bool first=true;
        public int index;
        /*public bool isDangerous;
        public bool isWalkable;
        public bool isObstacle;
        public bool canFloat;
        public bool isStackable;
        public bool cannotBeTopped;
        public bool cannotBeTop;
        public List<ContentPrefabConfigurator> canBeToppedBy;
        public List<ContentPrefabConfigurator> canBeSidedBy;
        public List<ContentPrefabConfigurator> bestSidedBy;*/
        internal static string _facts;
        internal bool debug;

        void Awake()
        {
            
            /*if (canBeSidedBy == null)
            {
                canBeSidedBy = new List<ContentPrefabConfigurator>();
                canBeToppedBy = new List<ContentPrefabConfigurator>();
                bestSidedBy = new List<ContentPrefabConfigurator>();
            }*/
        }

        internal static GameObject GetPrefab(int v)
        {
            return instances[v].gameObject;
        }

        internal void Init(bool isDebug)
        {
            debug = isDebug;
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
                    _facts += "%asset(Index)."+Environment.NewLine;
                    _facts += "%has_property(Index,dangerous)." + Environment.NewLine;
                    _facts += "%has_property(Index,walkable)." + Environment.NewLine;
                    _facts += "%has_property(Index,obstacle)." + Environment.NewLine;
                    _facts += "%has_property(Index,canFloat)." + Environment.NewLine;
                    _facts += "%has_property(Index,stackable)." + Environment.NewLine;
                    //_facts += "%has_property(Index,canNotBeTopped)." + Environment.NewLine;
                    //_facts += "%cannotBeTop(Index)." + Environment.NewLine;
                    //_facts += "%canBeToppedBy(BottomIndex,PossibleTopperIndex)." + Environment.NewLine;
                    //_facts += "%canBeSidedBy(Index,PossibleSiderIndex)." + Environment.NewLine;
                    //_facts += "%bestSidedBy(Index,PossibleSiderIndex)." + Environment.NewLine;
                    _facts += "%compatible(Index1,Index2,direction(D1,D2))." + Environment.NewLine;
                    _facts += "%preference(Index1,Index2,direction(D1,D2),Priority)." + Environment.NewLine;
                }
                return _facts;
            }
        }
        /*
        internal string Mapping()
        {
            List<string> maps = new List<string>();
            if (first)
            {
                maps.Add("prefabName(-1,\"Empty\")");
                maps.Add("prefabName(-2,\"NotPassable\")");
            }
            maps.Add("prefabName(" + index + ",\"" + gameObject.name + "\")");
            if (!debug)
            {
                if (first)
                {
                    maps.Add("has_property(-1,passable)");
                    maps.Add("leftright(\"Empty\",\"Empty\")");
                    maps.Add("leftright(\"NotPassable\",\"NotPassable\")");
                    maps.Add("abovevelow(\"Empty\",\"Empty\")");
                    maps.Add("abovevelow(\"NotPassable\",\"NotPassable\")");
                }
                if (isDangerous)
                {
                    maps.Add("justabove(\"" + gameObject.name + "\",\"NotPassable\")");
                }
                if (isWalkable)
                {
                    //maps.Add("has_property(" + index + ",walkable)");
                }
                if (isObstacle)
                {
                    //maps.Add("has_property(" + index + ",obstacle)");
                }
                if (canFloat)
                {
                    //maps.Add("has_property(" + index + ",canFloat)");
                    maps.Add("justabove(\"Empty\",\"" + gameObject.name + "\")");
                    maps.Add("leftright(\"" + gameObject.name + "\",\"Empty\")");
                }
                if (isStackable)
                {
                    //maps.Add("has_property(" + index + ",stackable)");
                    maps.Add("abovebelow(\"" + gameObject.name + "\",\"" + gameObject.name + "\")");
                }
                if (cannotBeTopped)
                {
                    if (!isDangerous)
                    {
                        maps.Add("justabove(\"" + gameObject.name + "\",\"Empty\")");
                    }
                    //maps.Add("compatible(" + index + ",-1"  + ",direction(0,-1))");
                }
                if (cannotBeTop)
                {
                    maps.Add("cannotBeTop(" + index + ")");
                }
                if (canBeToppedBy.Count > 0)
                {
                    foreach (ContentPrefabConfigurator prefab in canBeToppedBy)
                    {
                        //maps.Add("compatible("+index+","+prefab.index+",direction(0,-1))");
                        maps.Add("justabove(\"" + gameObject.name + "\",\"" + prefab.gameObject.name + "\")");
                    }
                }
                if (canBeSidedBy.Count > 0)
                {
                    foreach (ContentPrefabConfigurator prefab in canBeSidedBy)
                    {
                        //maps.Add("compatible(" + index + "," + prefab.index + ",direction(-1,0))");
                        maps.Add("leftright(\"" + gameObject.name + "\",\"" + prefab.gameObject.name + "\")");
                    }
                }
                if (bestSidedBy.Count > 0)
                {
                    foreach (ContentPrefabConfigurator prefab in bestSidedBy)
                    {
                        maps.Add("preference(" + index + "," + prefab.index + ",direction(-1,0),low)");
                    }
                }
            }
            first = false;
            return string.Join("."+Environment.NewLine, maps)+"."+Environment.NewLine;
        }
*/
        internal string Mapping()
        {
            List<string> maps = new List<string>();
            if (first)
            {
                maps.Add("prefabName(-1,\"Empty\")");
                maps.Add("prefabName(-2,\"NotPassable\")");
            }
            maps.Add("prefabName(" + index + ",\"" + gameObject.name + "\")");
            if (!debug)
            {
                if (first)
                {
                    maps.Add("has_property(-1,passable)");
                    maps.Add("leftright(\"Empty\",\"Empty\")");
                    maps.Add("leftright(\"NotPassable\",\"NotPassable\")");
                    maps.Add("abovebelow(\"Empty\",\"Empty\")");
                }
                foreach (FieldInfo info in GetType().GetFields())
                {
                    if (Attribute.IsDefined(info, typeof(Mappable)))
                    {
                        object value = info.GetValue(this);
                        BasicTypeMapper basicTypeMapper = BasicTypeMapper.GetMapper(value.GetType());
                        if (basicTypeMapper != null)
                        {
                            if (value is bool boolValue)
                            {
                                if (boolValue)
                                {
                                    maps.Add("has_property(" + index + "," + info.Name + ")");
                                }
                            }
                            else
                            {
                                maps.Add("has_property(" + index + "," + info.Name + "," + basicTypeMapper.BasicMap(value) + ")");
                            }
                        }
                        else if(value is List<ContentPrefabConfigurator> prefabList)
                        {
                            foreach(ContentPrefabConfigurator prefab in prefabList)
                            {
                                maps.Add(ASPMapperHelper.AspFormat(info.Name) + "(" + index + "," + prefab.index + ")");
                            }
                        }
                    }
                }
            }
            first = false;
            return string.Join("." + Environment.NewLine, maps) + "." + Environment.NewLine;
        }
            private string PropertyMap(object obj)
        {
            return BasicTypeMapper.GetMapper(obj.GetType()).BasicMap(obj);
        }

    }
}
