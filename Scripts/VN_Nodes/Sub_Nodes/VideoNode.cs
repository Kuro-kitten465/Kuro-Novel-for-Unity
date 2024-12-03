using System;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class VideoNode : VNNode
    {
        public string Video;

        public VideoNode() => NodeType = VNNodeType.Video;
    }
}