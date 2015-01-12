using System.Collections.Generic;

namespace POESKillTree.SkillTreeFiles
{
    public class SkillNodeGroup
    {
        public List<SkillNode> Nodes = new List<SkillNode>();
        public Dictionary<int, bool> OcpOrb = new Dictionary<int, bool>(); //  "oo": {"1": true},
        public Vector2D Position; // "x": 1105.14,"y": -5295.31,
    }
}