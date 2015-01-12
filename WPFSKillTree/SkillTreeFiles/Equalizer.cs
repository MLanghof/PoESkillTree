using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POESKillTree.SkillTreeFiles
{
    class Equalizer
    {
        private SkillTree _treeInst;

        private List<ClusterFloat> _clusters;

        private Random _rand;

        private int count = 0;

        public Equalizer(SkillTree treeInst, List<SkillNodeGroup> groups)
        {
            _treeInst = treeInst;

            _clusters = new List<ClusterFloat>();

            foreach (SkillNodeGroup group in groups)
            {
                _clusters.Add(new ClusterFloat(group, treeInst));
            }

            _rand = new Random();
        }

        DateTime lastDrawSignalTime = DateTime.Now;
        public void EqualizeTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            Equalize();

            /*if (e.SignalTime.Subtract(lastDrawSignalTime).TotalSeconds > 1)
            {
                lastDrawSignalTime = e.SignalTime;
                _treeInst.RedrawTree();
            }*/
        }

        public void Equalize()
        {
            for (int i = 0; i < 1; i++)
                foreach (ClusterFloat cluster in _clusters)
                {
                    cluster.Equalize(_rand);
                }
            Console.WriteLine(count++);

        }


        class ClusterFloat
        {
            private SkillTree _treeInst;
            private SkillNodeGroup _group;

            private ushort[] _nodeKeys;

            private double _mass;

            private Vector2D _originalPosition;
            private Vector2D _position;

            private Vector2D _v;

            private List<Spring> attachedSprings = new List<Spring>();

            public ClusterFloat(SkillNodeGroup group, SkillTree treeInst)
            {
                _treeInst = treeInst;
                _group = group;
                _originalPosition = group.Position;
                _position = group.Position;
                _mass = group.Nodes.Count;

                _nodeKeys = _treeInst.Skillnodes.Keys.ToArray();

                _v = new Vector2D(0, 0);

                foreach (SkillNode node in _group.Nodes)
                {
                    foreach (SkillNode neighbor in node.Neighbor)
                    {
                        if (neighbor.SkillNodeGroup != group)
                        {
                            attachedSprings.Add(new Spring(node, neighbor, 400, 1/5d));
                        }
                    }
                }
            }

            public void Equalize(Random r)
            {
                Vector2D fSum = new Vector2D();

                foreach (SkillNode node in _group.Nodes)
                {
                    int index = r.Next(_nodeKeys.Length);
                    SkillNode target = _treeInst.Skillnodes[_nodeKeys[index]];
                    var pathDist = _treeInst.GetShortestPathTo(target.Id, new HashSet<ushort>{ node.Id }).Count;
                    // If no path was found, move on.
                    if (pathDist == 0) continue;
                    //if (pathDist > 10) continue;

                    Spring spring = new Spring(node, target, pathDist * 400, 1 / (20d * pathDist));
                    attachedSprings.Add(spring);
                    /*foreach (SkillNode neighbor in node.Neighbor)
                    {
                        
                    Vector2D d = neighbor.Position - node.Position;
                    // Skill tree currently has ~400 average distance between random nodes
                    double off = d.Length - 400;
                    fSum += d.Normalized() * off / 2;
                    }*/
                }

                foreach (Spring spring in attachedSprings)
                {
                    fSum = spring.GetForce(_group);
                }

                Vector2D acc = fSum / _mass;
                _v += acc;
                _v *= 0.9; // Dampen to prevent oscillation
                //_v = fSum;

                _position += _v;
                _group.Position = _position;
            }

            public void Reset()
            {
                _group.Position = _originalPosition;
            }
        }

        class Spring
        {
            private double _equilLength = 400;
            private double _stiffness = 1 / 20d;

            private SkillNode _n1, _n2;

            public Spring(SkillNode n1, SkillNode n2, double equilLength = 400, double stiffness = 1/20d)
            {
                _equilLength = equilLength;
                _stiffness = stiffness;
                _n1 = n1;
                _n2 = n2;
            }

            public Vector2D GetForce(SkillNodeGroup group)
            {
                Vector2D d = _n2.Position - _n1.Position;
                double off = d.Length - _equilLength;
                Vector2D f = d.Normalized() * off * _stiffness;
                return (group == _n1.SkillNodeGroup ? f : f.Rotate(180));
                //f2 += f;
            }
        }


    }
}
