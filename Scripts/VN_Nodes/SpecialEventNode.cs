using System;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class SpecialEventNode : VNNode
    {
        public string SpecialEvent;

        public SpecialEventNode() => NodeType = VNNodeType.SpecialEvent;
    }
}