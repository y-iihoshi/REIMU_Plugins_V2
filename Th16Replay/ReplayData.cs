//-----------------------------------------------------------------------
// <copyright file="ReplayData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Th16Replay;

using System;
using System.Collections.Generic;
using System.IO;
using ReimuPlugins.Common;

public sealed class ReplayData : ReplayDataBase
{
    private readonly Dictionary<string, string> info;

    public ReplayData()
    {
        this.info = new Dictionary<string, string>
        {
            { "Version",     string.Empty },
            { "Name",        string.Empty },
            { "Date",        string.Empty },
            { "Chara",       string.Empty },
            { "Rank",        string.Empty },
            { "Stage",       string.Empty },
            { "Extra Stage", string.Empty },
            { "Score",       string.Empty },
            { "Slow Rate",   string.Empty },
        };
    }

    public string Version => this.info["Version"];

    public string Name => this.info["Name"];

    public string Date => this.info["Date"];

    public string Chara => this.info["Chara"];

    public string Rank => this.info["Rank"];

    public string Stage => this.info["Rank"].StartsWith("Extra", StringComparison.Ordinal)
        ? this.info["Extra Stage"] : this.info["Stage"];

    public string Score => this.info["Score"];

    public string SlowRate => this.info["Slow Rate"];

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
