//-----------------------------------------------------------------------
// <copyright file="ReplayData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Th165Replay
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ReimuPlugins.Common;

    public sealed class ReplayData : ThReplayData
    {
        private readonly Dictionary<string, string> info;

        public ReplayData()
        {
            this.info = new Dictionary<string, string>
            {
                { "Version",     string.Empty },
                { "Name",        string.Empty },
                { "Date",        string.Empty },
                { "Day",         string.Empty },
                { "Scene",       string.Empty },
                { "Score",       string.Empty },
                { "Slow Rate",   string.Empty },
            };
        }

        public string Version => this.info["Version"];

        public string Name => this.info["Name"];

        public string Date => this.info["Date"];

        public string Weekday
            => Weekdays.TryGetValue(this.info["Day"], out var weekday) ? weekday : this.info["Day"];

        public string Scene => this.info["Scene"];

        public string Score => this.info["Score"];

        public string SlowRate => this.info["Slow Rate"];

        private static Dictionary<string, string> Weekdays => new Dictionary<string, string>()
        {
            { "1",  "日曜日" },
            { "2",  "月曜日" },
            { "3",  "火曜日" },
            { "4",  "水曜日" },
            { "5",  "木曜日" },
            { "6",  "金曜日" },
            { "7",  "土曜日" },
            { "8",  "裏・日曜日" },
            { "9",  "裏・月曜日" },
            { "10", "裏・火曜日" },
            { "11", "裏・水曜日" },
            { "12", "裏・木曜日" },
            { "13", "裏・金曜日" },
            { "14", "裏・土曜日" },
            { "15", "悪夢日曜" },
            { "16", "悪夢月曜" },
            { "17", "悪夢火曜" },
            { "18", "悪夢水曜" },
            { "19", "悪夢木曜" },
            { "20", "悪夢金曜" },
            { "21", "悪夢土曜" },
            { "22", "ナイトメアダイアリー" },
        };

        public override void Read(Stream input)
        {
            base.Read(input);

            foreach (var elem in this.InfoArray)
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
