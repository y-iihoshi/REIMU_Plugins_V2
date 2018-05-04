// <copyright file="ReplayDataAdapter.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ReimuPlugins.Th155Replay
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using ReimuPlugins.Common;

    public sealed class ReplayDataAdapter
    {
        private const string NotAvailable = "-";

        private ReplayData data;

        private GameMode gameMode;  // cache

        public ReplayDataAdapter(Stream input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            this.data = new ReplayData();
            this.data.Read(input);
            this.gameMode = this.data.GetGameMode();
        }

        public string Version
            => this.data.Version;

        public string GetBackgroundName()
            => EnsureString(() => (this.gameMode == GameMode.Story) ? NotAvailable : this.data.GetBackgroundName());

        public string GetBgmName()
            => EnsureString(() => (this.gameMode == GameMode.Story) ? NotAvailable : this.data.GetBgmName());

        public string GetGameMode()
            => EnsureString(() => this.gameMode.ToShortName());

        public string GetDifficulty()
            => EnsureString(() => (this.gameMode == GameMode.VersusPlayer)
                ? NotAvailable : this.data.GetDifficulty().ToString());

        public string GetMasterName1()
            => EnsureString(() => (this.gameMode == GameMode.Story)
                ? this.data.GetStoryMaster().ToLongName() : this.data.GetMasterName1().ToLongName());

        public string GetMasterColor1()
            => EnsureString(() => (this.gameMode == GameMode.Story)
                ? NotAvailable : (this.data.GetMasterColor1() + 1).ToString(CultureInfo.CurrentCulture));

        public string GetSlaveName1()
            => EnsureString(() => (this.gameMode == GameMode.Story)
                ? this.data.GetStorySlave().ToLongName() : this.data.GetSlaveName1().ToLongName());

        public string GetSlaveColor1()
            => EnsureString(() => (this.gameMode == GameMode.Story)
                ? NotAvailable : (this.data.GetSlaveColor1() + 1).ToString(CultureInfo.CurrentCulture));

        public string GetSpellCard1Name()
            => EnsureString(() => (this.gameMode == GameMode.Story)
                ? this.data.GetStorySpellCardName() : this.data.GetSpellCard1Name());

        public string GetPlayer1Name()
            => EnsureString(() => this.PlayerNameIsAvailable() ? this.data.GetPlayer1Name() : NotAvailable);

        public string GetMasterName2()
            => EnsureString(() => (this.gameMode == GameMode.Story)
                ? NotAvailable : this.data.GetMasterName2().ToLongName());

        public string GetMasterColor2()
            => EnsureString(() => (this.gameMode == GameMode.Story)
                ? NotAvailable : (this.data.GetMasterColor2() + 1).ToString(CultureInfo.CurrentCulture));

        public string GetSlaveName2()
            => EnsureString(() => (this.gameMode == GameMode.Story)
                ? NotAvailable : this.data.GetSlaveName2().ToLongName());

        public string GetSlaveColor2()
            => EnsureString(() => (this.gameMode == GameMode.Story)
                ? NotAvailable : (this.data.GetSlaveColor2() + 1).ToString(CultureInfo.CurrentCulture));

        public string GetSpellCard2Name()
            => EnsureString(() => (this.gameMode == GameMode.Story) ? NotAvailable : this.data.GetSpellCard2Name());

        public string GetPlayer2Name()
            => EnsureString(() => this.PlayerNameIsAvailable() ? this.data.GetPlayer2Name() : NotAvailable);

        public string GetDateTime()
            => EnsureString(() => this.data.GetDateTime().ToString(CultureInfo.CurrentCulture));

        public string GetPlayer1Info()
            => EnsureString(() => this.GetPlayerInfo(1));

        public string GetPlayer2Info()
            => EnsureString(() => this.GetPlayerInfo(2));

        private static string EnsureString(Func<string> func)
        {
            try
            {
                return func?.Invoke() ?? string.Empty;
            }
            catch (Exception ex)
                when ((ex is InvalidDataException) || (ex is ArgumentException) || (ex is KeyNotFoundException))
            {
#if DEBUG
                return ex?.Message ?? string.Empty;
#else
                return string.Empty;
#endif
            }
        }

        private bool PlayerNameIsAvailable()
            => (string.CompareOrdinal(this.data.Version, "1.04") >= 0) && (this.gameMode == GameMode.VersusPlayer);

        private string GetPlayerInfo(int num)
        {
            var format = string.Join(
                Environment.NewLine,
                "Player {0}",
                "Master: {1}",
                "Master Color: {2}",
                "Slave: {3}",
                "Slave Color: {4}",
                "Spell Card: {5}");

            if (num == 1)
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    format,
                    num,
                    this.GetMasterName1(),
                    this.GetMasterColor1(),
                    this.GetSlaveName1(),
                    this.GetSlaveColor1(),
                    this.GetSpellCard1Name());
            }
            else if (num == 2)
            {
                return (this.gameMode == GameMode.Story)
                    ? string.Empty
                    : string.Format(
                        CultureInfo.CurrentCulture,
                        format,
                        num,
                        this.GetMasterName2(),
                        this.GetMasterColor2(),
                        this.GetSlaveName2(),
                        this.GetSlaveColor2(),
                        this.GetSpellCard2Name());
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(num));
            }
        }
    }
}
