﻿//-----------------------------------------------------------------------
// <copyright file="ReplayData.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed.")]

namespace ReimuPlugins.Th143Replay
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ReimuPlugins.Common;

    public sealed class ReplayData : ThReplayData
    {
        private Dictionary<string, string> info;

        public ReplayData()
        {
            this.info = new Dictionary<string, string>
            {
                { "Version",     string.Empty },
                { "Name",        string.Empty },
                { "Date",        string.Empty },
                { "Chara",       string.Empty },
                { "Day",         string.Empty },
                { "Scene",       string.Empty },
                { "Score",       string.Empty },
                { "Slow Rate",   string.Empty },
            };
        }

        public string Version
        {
            get { return this.info["Version"]; }
        }

        public string Name
        {
            get { return this.info["Name"]; }
        }

        public string Date
        {
            get { return this.info["Date"]; }
        }

        public string Chara
        {
            get { return this.info["Chara"]; }
        }

        public string Day
        {
            get { return this.info["Day"]; }
        }

        public string Scene
        {
            get { return this.info["Scene"]; }
        }

        public string Score
        {
            get { return this.info["Score"]; }
        }

        public string SlowRate
        {
            get { return this.info["Slow Rate"]; }
        }

        public override void Read(Stream input)
        {
            base.Read(input);

            foreach (var elem in this.InfoArray)
            {
                foreach (var key in this.info.Keys)
                {
                    if (string.IsNullOrEmpty(this.info[key]))
                    {
                        var keyWithSpace = (key + " ").ToCP932();
                        if (elem.StartsWith(keyWithSpace, StringComparison.Ordinal))
                        {
                            this.info[key] = elem.Substring(keyWithSpace.Length);
                            break;
                        }
                    }
                }
            }
        }
    }
}