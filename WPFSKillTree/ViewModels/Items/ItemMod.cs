﻿using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;
using POESKillTree.ViewModels.Items;
using Newtonsoft.Json.Linq;
using System;

namespace POESKillTree.ViewModels.Items
{
    public class ItemMod
    {
        public Stat Parent { get; set; }
        public enum ValueColoring
        {
            White = 0,
            LocallyAffected = 1,

            Fire = 4,
            Cold = 5,
            Lightning = 6
        }

        private string _Attribute;

        public string Attribute
        {
            get { return _Attribute; }
            set { _Attribute = value; }
        }

        public List<float> Value;
        public List<ValueColoring> ValueColor = new List<ValueColoring>();

        public bool isLocal = false;
        private ItemClass itemclass;

        public static ItemMod CreateMod(Item item, JObject obj, Regex numberfilter)
        {
            ItemClass ic = item.Class;
            var mod = new ItemMod();

            int dmode = (obj["displayMode"] != null) ? obj["displayMode"].Value<int>() : 0;
            string at = obj["name"].Value<string>();
            at = numberfilter.Replace(at, "#");

            var parsed = ((JArray)obj["values"]).Select(a =>
            {
                var str = ((JArray)a)[0].Value<string>();
                var floats = new List<float>();
                var parts = str.Split('-');

                if (dmode != 3)
                    if (parts.Length > 1)
                        at += ": ";
                    else
                        at += " ";

                for (int i = 0; i < parts.Length; i++)
                {
                    string v = parts[i];
                    float val = 0;
                    if (float.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out val))
                    {
                        floats.Add(val);
                        if (dmode != 3)
                            at += "#";
                    }
                    else
                    {
                        foreach (Match m in numberfilter.Matches(v))
                            floats.Add(float.Parse(m.Value, CultureInfo.InvariantCulture));

                        at += " " + numberfilter.Replace(v, "#");
                    }

                    if (i < parts.Length - 1)
                    {
                        if (dmode != 3)
                            at += "-";
                    }
                }

                var cols = floats.Select(f => (ItemMod.ValueColoring)((JArray)a)[1].Value<int>()).ToList();
                return new { floats, cols };
            }).ToList();


            mod = new ItemMod
            {
                itemclass = ic,
                Value = parsed.Select(p => p.floats).SelectMany(v => v).ToList(),
                ValueColor = parsed.Select(p => p.cols).SelectMany(v => v).ToList(),
                _Attribute = at,
                isLocal = DetermineLocal(item, at)
            };

            return mod;
        }

        public static ItemMod CreateMod(Item item, string attribute, Regex numberfilter)
        {
            ItemClass ic = item.Class;
            var mod = new ItemMod();
            var values = new List<float>();
            foreach (Match match in numberfilter.Matches(attribute))
            {
                values.Add(float.Parse(match.Value, CultureInfo.InvariantCulture));
            }
            string at = numberfilter.Replace(attribute, "#");

            mod = new ItemMod
            {
                itemclass = ic,
                Value = values,
                _Attribute = at,
                isLocal = DetermineLocal(item, at)
            };

            return mod;
        }

        public static List<ItemMod> CreateMods(Item item, string attribute, Regex numberfilter)
        {
            ItemClass ic = item.Class;
            var mods = new List<ItemMod>();
            var values = new List<float>();

            foreach (Match match in numberfilter.Matches(attribute))
            {
                values.Add(float.Parse(match.Value, CultureInfo.InvariantCulture));
            }
            string at = numberfilter.Replace(attribute, "#");
            if (at == "+# to all Attributes")
            {
                mods.Add(new ItemMod
                {
                    itemclass = ic,
                    Value = values,
                    _Attribute = "+# to Strength"
                });
                mods.Add(new ItemMod
                {
                    itemclass = ic,
                    Value = values,
                    _Attribute = "+# to Dexterity"
                });
                mods.Add(new ItemMod
                {
                    itemclass = ic,
                    Value = values,
                    _Attribute = "+# to Intelligence"
                });
            }
            else
            {
                mods.Add(new ItemMod
                {
                    itemclass = ic,
                    Value = values,
                    _Attribute = at,
                    isLocal = DetermineLocal(item, at)
                });
            }
            return mods;
        }

        public bool DetermineLocalFor(Item itm)
        {
            return ItemMod.DetermineLocal(itm, this._Attribute);
        }

        // Returns true if property/mod is local, false otherwise.
        public static bool DetermineLocal(Item item, string attr)
        {
            return (item.Class != ItemClass.Amulet && item.Class != ItemClass.Ring &&
                    item.Class != ItemClass.Belt)
                   && ((attr.Contains("Armour") && !attr.EndsWith("Armour against Projectiles"))
                       || attr.Contains("Evasion")
                       || (attr.Contains("Energy Shield") && !attr.EndsWith("Energy Shield Recharge"))
                       || attr.Contains("Weapon Class")
                       || attr.Contains("Critical Strike Chance with this Weapon")
                       || attr.Contains("Critical Strike Damage Multiplier with this Weapon"))
                   || (item.Class == ItemClass.MainHand || item.Class == ItemClass.OffHand || item.Class == ItemClass.TwoHand)
                      && item.Keywords != null // Only weapons have keyword.
                      && (attr == "#% increased Attack Speed"
                          || attr == "#% increased Accuracy Rating"
                          || attr == "+# to Accuracy Rating"
                          || attr.StartsWith("Adds ") && (attr.EndsWith(" Damage") || attr.EndsWith(" Damage in Main Hand") || attr.EndsWith(" Damage in Off Hand"))
                          || attr == "#% increased Physical Damage"
                          || attr == "#% increased Critical Strike Chance");
        }

        private enum ValueType
        {
            Flat,
            Percentage,
            FlatMinMax
        }

        public ItemMod Sum(ItemMod m)
        {
            var mod = new ItemMod()
            {
                Attribute = this.Attribute,
                itemclass = this.itemclass,
                isLocal = this.isLocal,
                Parent = this.Parent,
                ValueColor = this.ValueColor.ToList()
            };

            mod.Value = this.Value.Zip(m.Value, (f1, f2) => f1 + f2).ToList();

            return mod;
        }

        public void IncreaseBy(double p)
        {
            throw new System.NotImplementedException();
        }

        public JToken ToJobject(bool asMod = false)
        {
            string defaultFormat = "###0.##";
            if (asMod)
            {
                int index = 0;
                return new JValue(ItemAttributes.Attribute.Backreplace.Replace(this.Attribute, m => Value[index++].ToString(defaultFormat)));
            }
            else
            {
                var j = new JObject();

                if (Value != null && Value.Count > 2)
                    j.Add("displayMode", 3);
                else
                    j.Add("displayMode", 0);



                if (Value == null || Value.Count == 0)
                {
                    j.Add("name", _Attribute);
                    j.Add("values", new JArray());
                }
                else if (Value.Count == 1)
                {
                    if (_Attribute.EndsWith(": #"))
                    {
                        j.Add("name", _Attribute.Substring(0, _Attribute.Length - 3));
                        j.Add("values", new JArray((object)new JArray(Value[0], ValueColor[0])));
                    }
                    else if (_Attribute.EndsWith(" #%"))
                    {
                        j.Add("name", _Attribute.Substring(0, _Attribute.Length - 3));
                        j.Add("values", new JArray((object)new JArray(Value[0] + "%", ValueColor[0])));
                    }
                    else if (_Attribute.StartsWith("# "))
                    {
                        j.Add("name", _Attribute.Substring(2));
                        j.Add("values", new JArray((object)new JArray(Value[0].ToString(defaultFormat), ValueColor[0])));
                    }
                    else
                        throw new NotImplementedException();

                }
                else if (Value.Count == 2)
                {
                    if (_Attribute.EndsWith(": #-#") && ValueColor.All(v => v == ValueColor[0]))
                    {
                        j.Add("name", _Attribute.Substring(0, _Attribute.Length - 5));
                        j.Add("values", new JArray((object)new JArray(string.Join("-", Value), ValueColor[0])));
                    }
                    else
                        throw new NotImplementedException();
                }
                else
                {
                    var str = _Attribute;
                    while (str.EndsWith(" #-#"))
                    {
                        str = str.Substring(0, str.Length - 4);
                        str = str.Trim(',', ':', ' ');
                    }
                    j.Add("name", str);

                    JArray vals = new JArray();
                    for (int i = 0; i < Value.Count; i += 2)
                    {
                        var val = string.Format("{0}-{1}", Value[i], Value[i + 1]);

                        if (ValueColor[i] != ValueColor[i + 1])
                            throw new NotImplementedException();
                        vals.Add(new JArray(val, ValueColor[i]));
                    }

                    j.Add("values", vals);

                }

                return j;
            }
        }

    }
}