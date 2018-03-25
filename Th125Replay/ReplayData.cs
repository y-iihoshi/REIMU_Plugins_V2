//-----------------------------------------------------------------------
// <copyright file="ReplayData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Th125Replay
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
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

        public string Scene { get; private set; }

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
                if (Regex.IsMatch(elem, @"^.{2}\-\d$"))
                {
                    this.Scene = elem;
                }
                else
                {
                    foreach (var key in this.info.Keys)
                    {
                        if (string.IsNullOrEmpty(this.info[key]))
                        {
                            var keyWithSpace = key + " ";
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
}
