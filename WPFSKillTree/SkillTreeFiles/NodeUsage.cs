using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POESKillTree.SkillTreeFiles
{
    public class NodeUsage
    {
        public Dictionary<ushort, double> nodeFrequencies;

        public double maxUsage = 0.00000001;

        public NodeUsage(String filename)
        {
            String text = "";
            if (File.Exists(filename))
            {
                text = File.ReadAllText(filename);
            }

            nodeFrequencies = new Dictionary<ushort, double>();

            String[] lines = text.Split('\n');
            foreach (String line in lines.Skip(1))
            {
                String[] values = line.Split(',');
                if (values.Length < 4)
                    continue;

                ushort nodeId = 0;
                ushort.TryParse(values[1], out nodeId);
                double nodePercent = 0;
                String percentValue = values[3].Replace("%", "");
                double.TryParse(percentValue, out nodePercent);

                nodeFrequencies.Add(nodeId, nodePercent / 100);
                maxUsage = Math.Max(maxUsage, nodePercent / 100);
            }
        }
    }
}
