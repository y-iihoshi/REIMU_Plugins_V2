//-----------------------------------------------------------------------
// <copyright file="ReplayData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Th135Replay
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using ReimuPlugins.Common;
    using ReimuPlugins.Common.Extensions;
    using ReimuPlugins.Common.Squirrel;
    using ReimuPlugins.Th135Replay.Properties;

#if DEBUG
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
#endif

    public enum GameMode
    {
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
#pragma warning disable SA1134 // Attributes should not share line
        [EnumAltName("Vs Com")]                        VersusCom,
        [EnumAltName("Vs Player")]                     VersusPlayer,
#if false
        [EnumAltName("Vs Network （ホスト側）")]       VersusNetworkHost,
        [EnumAltName("Vs Network （クライアント側）")] VersusNetworkClient,
        [EnumAltName("Vs Network （観戦）")]           VersusNetworkWatch,
#endif
        [EnumAltName("Story Mode")]                    Story = 10,
#pragma warning restore SA1134 // Attributes should not share line
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
    }

    public enum Character
    {
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
#pragma warning disable SA1134 // Attributes should not share line
        [EnumAltName("霊夢",        LongName = "博麗 霊夢")]        Reimu,
        [EnumAltName("魔理沙",      LongName = "霧雨 魔理沙")]      Marisa,
        [EnumAltName("一輪 & 雲山", LongName = "雲居 一輪 & 雲山")] IchirinUnzan,
        [EnumAltName("白蓮",        LongName = "聖 白蓮")]          Byakuren,
        [EnumAltName("布都",        LongName = "物部 布都")]        Futo,
        [EnumAltName("神子",        LongName = "豊聡耳 神子")]      Miko,
        [EnumAltName("にとり",      LongName = "河城 にとり")]      Nitori,
        [EnumAltName("こいし",      LongName = "古明地 こいし")]    Koishi,
        [EnumAltName("マミゾウ",    LongName = "二ッ岩 マミゾウ")]  Mamizou,
        [EnumAltName("こころ",      LongName = "秦 こころ")]        Kokoro,
#pragma warning restore SA1134 // Attributes should not share line
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
    }

    public enum Direction
    {
#pragma warning disable SA1134 // Attributes should not share line
        [EnumAltName("打撃 ←")] HitLeft,
        [EnumAltName("打撃 ↓")] HitDown,
        [EnumAltName("打撃 ↑")] HitUp,
        [EnumAltName("打撃 →")] HitRight,
        [EnumAltName("射撃 ←")] ShotLeft,
        [EnumAltName("射撃 ↓")] ShotDown,
        [EnumAltName("射撃 ↑")] ShotUp,
        [EnumAltName("射撃 →")] ShotRight,
#pragma warning restore SA1134 // Attributes should not share line
    }

    public sealed class ReplayData
    {
        private static readonly IReadOnlyDictionary<int, string> BackgroundNames =
            new Dictionary<int, string>
            {
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
                {  0, "博麗神社" },
                {  1, "人間の里" },
                {  2, "雲上の宝船" },
                {  3, "命蓮寺" },
                {  4, "夢殿大祀廟" },
                {  5, "神霊廟" },
                {  6, "玄武の沢" },
                {  7, "地霊殿" },
                {  8, "妖怪狸の森" },
                {  9, "妖怪狸の森(夜)" },
                { 10, "人間の里(夜)" },
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
            };

        private static readonly IReadOnlyDictionary<int, string> BgmNames =
            new Dictionary<int, string>
            {
                { 101, "春色小径　～ Colorful Path" },
                { 102, "メイガスナイト" },
                { 103, "時代親父とハイカラ少女" },
                { 104, "感情の摩天楼　～ Cosmic Mind" },
                { 105, "大神神話伝" },
                { 106, "聖徳伝説　～ True Administrator" },
                { 107, "芥川龍之介の河童　～ Candid Friend" },
                { 108, "ハルトマンの妖怪少女" },
                { 109, "佐渡の二ッ岩" },
                { 110, "幻想郷の二ッ岩" },
                { 111, "亡失のエモーション" },
            };

        private static readonly IReadOnlyDictionary<int, string> EquipNames =
            new Dictionary<int, string>
            {
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
                {    -1, "（なし）" },
                {     0, "霊撃札" },
                {     1, "マジックキノコ" },
                {     2, "否徳の法輪" },
                {     3, "魔女の箒" },
                {     4, "毘沙門天の宝塔" },
                {     5, "仙風大皿" },
                {     6, "厳つがましい笏" },
                {     7, "フォースシールド" },
                {     8, "よくあるお守り" },
                {     9, "仙人の飾り剣" },
                {    10, "懐に忍ばす数珠" },
                {    11, "無邪気な帽子" },
                {  1000, "妖怪バスター" },
                {  1001, "即妙神域札" },
                {  1002, "空中昇天脚" },
                {  1003, "亜空点穴" },
                {  1004, "陰陽弾" },
                {  1005, "前方安全札" },
                {  1006, "滞空三角飛び" },
                {  1011, "霊符「夢想封印」" },
                {  1012, "宝具「陰陽飛鳥井」" },
                {  1015, "神技「八方龍殺陣」" },
                {  2000, "ウィッチレイライン" },
                {  2001, "ライジングスウィープ" },
                {  2002, "ウィッチングブラスト" },
                {  2003, "ストラトフラクション" },
                {  2004, "スウィープアサイド" },
                {  2005, "パワフルドラッグ" },
                {  2006, "ルミナリーショット" },
                {  2010, "恋符「マスタースパーク」" },
                {  2013, "彗星「ブレイジングスター」" },
                {  2014, "星符「サテライトイリュージョン」" },
                {  3000, "垂雲の鉄槌" },
                {  3001, "一握りの浮雲" },
                {  3002, "小さな神立雲" },
                {  3003, "怒りの走雲" },
                {  3004, "嶺雲のご来光" },
                {  3005, "殴殺の流雲" },
                {  3006, "慈悲の北颪" },
                {  3011, "嵐符「仏罰の野分雲」" },
                {  3014, "積乱「見越し入道雲」" },
                {  3015, "拳固「懺悔の殺風」" },
                {  4000, "ハヌマーンの舞" },
                {  4001, "ヴィルーパークシャの目" },
                {  4002, "インドラの雷" },
                {  4003, "スカンダの脚" },
                {  4004, "ヴィルーダカの剣" },
                {  4005, "ガルーダの爪" },
                {  4006, "ドゥルガーの魂" },
                {  4010, "天符「三千大千世界の主」" },
                {  4011, "天符「大日如来の輝き」" },
                {  4012, "天符「釈迦牟尼の五行山」" },
                {  5000, "風の凶穴" },
                {  5001, "貴竜の矢" },
                {  5002, "抱水皿" },
                {  5003, "天の磐舟" },
                {  5004, "六壬神火" },
                {  5005, "立向坐山" },
                {  5006, "合局風" },
                {  5010, "風符「三輪の皿嵐」" },
                {  5011, "炎符「太乙真火」" },
                {  5012, "運気「破局の開門」" },
                {  6000, "縮地のマント" },
                {  6001, "十七条のレーザー" },
                {  6002, "デザイアの求魂" },
                {  6003, "黄金の剣ジパング" },
                {  6004, "東方のインフルーエンス" },
                {  6005, "テンフォールドの垂聴" },
                {  6006, "輝く者の慈雨" },
                {  6011, "仙符「日出ずる処の道士」" },
                {  6012, "道符「掌の上の天道」" },
                {  6013, "人符「勧善懲悪は古の良き典なり」" },
                {  7000, "空中ブラスター" },
                {  7001, "光子トゥーピド" },
                {  7002, "さよならラバーリング" },
                {  7003, "クリミナルギア" },
                {  7004, "ミズバク大回転" },
                {  7005, "菊一文字コンプレッサー" },
                {  7006, "キューリサウンドシステム" },
                {  7010, "豪雨「河底大戦争」" },
                {  7011, "泡符「撃て！バブルドラゴン」" },
                {  7012, "戦機「飛べ！三平ファイト」" },
                {  8000, "コンディションドテレポート" },
                {  8001, "フィゲッティスナッチャー" },
                {  8002, "グローイングペイン" },
                {  8003, "キャッチアンドローズ" },
                {  8004, "リフレクスレーダー" },
                {  8005, "スティンギングマインド" },
                {  8006, "アンアンサードラブ" },
                {  8010, "抑制「スーパーエゴ」" },
                {  8011, "本能「イドの解放」" },
                {  8012, "夢符「ご先祖様が見ているぞ」" },
                {  9000, "うつせみ地蔵変化" },
                {  9001, "怪奇送り提灯" },
                {  9002, "鳥獣琵琶法師" },
                {  9003, "妖怪アミキリ変化" },
                {  9004, "妖怪カラカッサ変化" },
                {  9005, "妖怪つるべぇ変化" },
                {  9006, "妖怪オモカゲ変化" },
                {  9010, "変化「分福熱湯風呂」" },
                {  9011, "変化「百鬼妖界の門」" },
                {  9012, "変化「二ッ岩家の裁き」" },
                { 10000, "吼怒の妖狐面" },
                { 10001, "怒声の大蜘蛛面" },
                { 10002, "憂嘆の長壁面" },
                { 10003, "憂心の鬼婆面" },
                { 10004, "歓喜の獅子面" },
                { 10005, "狂喜の火男面" },
                { 10006, "こころのルーレット" },
                { 10010, "怒面「怒れる忌狼の面」" },
                { 10011, "憑依「喜怒哀楽ポゼッション」" },
                { 10012, "憂面「杞人地を憂う」" },
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
            };

        private readonly Info info;

        public ReplayData()
        {
            this.info = new Info();
        }

        public int BackgroundId => this.info["background"] as SQInteger;

        public string BackgroundName => BackgroundNames.TryGetValue(this.BackgroundId, out var name)
            ? name : string.Empty;

        public int BgmId => this.info["bgm"] as SQInteger;

        public string BgmName => BgmNames.TryGetValue(this.BgmId, out var name) ? name : string.Empty;

        public GameMode GameMode => (this.info["game_mode"] as SQInteger).ToInt32().ToValidEnum<GameMode>();

        public Character Character1 => (this.info["player0"] as SQInteger).ToInt32().ToValidEnum<Character>();

        public int Color1 => this.info["color0"] as SQInteger;

        public string Profile1Name => this.info["profile", 0, "name"] as SQString;

        public string Player1Info => this.GetPlayerInfo(0);

        public Character Character2 => (this.info["player1"] as SQInteger).ToInt32().ToValidEnum<Character>();

        public int Color2 => this.info["color1"] as SQInteger;

        public string Profile2Name => this.info["profile", 1, "name"] as SQString;

        public string Player2Info => this.GetPlayerInfo(1);

        public int Seed => this.info["seed"] as SQInteger;

        public string Version => this.info.Version;

        public DateTime DateTime => new(
            this.info["year"] as SQInteger,
            this.info["month"] as SQInteger,
            this.info["day"] as SQInteger,
            this.info["hour"] as SQInteger,
            this.info["min"] as SQInteger,
            this.info["sec"] as SQInteger);

        public void Read(Stream input)
        {
            using var reader = new BinaryReader(input, Encoding.UTF8NoBOM, true);
            this.info.ReadFrom(reader);
        }

        private static bool Extract(byte[] input, out byte[] output, int expectedSize)
        {
            // See section 2.2 of RFC 1950
            var validHeader = new byte[] { 0x78, 0x9C };

            if (input.Take(validHeader.Length).SequenceEqual(validHeader))
            {
                var extracted = new byte[expectedSize];
                var extractedSize = 0;
                {
                    using var memory = new MemoryStream(
                        input, validHeader.Length, input.Length - validHeader.Length, false);
                    using var deflate = new DeflateStream(memory, CompressionMode.Decompress);

                    extractedSize = deflate.Read(extracted, 0, extracted.Length);
                }

                output = new byte[extractedSize];
                Array.Copy(extracted, output, output.Length);
            }
            else
            {
                output = Array.Empty<byte>();
            }

            return output.Length > 0;
        }

        private string GetPlayerInfo(int index)
        {
            var playerInfo = string.Empty;

            if ((index == 0) || (index == 1))
            {
                var indexStr = index.ToString(CultureInfo.CurrentCulture);

                var equips = string.Join(
                    Environment.NewLine,
                    Utils.GetEnumerator<Direction>().Select(dir =>
                    {
                        var dirStr = ((int)dir).ToString(CultureInfo.CurrentCulture);
                        var equipId = this.info["equip_" + indexStr + dirStr] as SQInteger;
                        return dir.ToShortName() + ": " +
                            (EquipNames.TryGetValue(equipId, out var equipName) ? equipName : string.Empty);
                    }));

                var format = string.Join(
                    Environment.NewLine,
                    "Player {0}",
                    "Character: {1}",
                    "Color: {2}",
                    equips);

                playerInfo = string.Format(
                    CultureInfo.CurrentCulture,
                    format,
                    index + 1,
                    (this.info["player" + indexStr] as SQInteger).ToInt32().ToValidEnum<Character>().ToLongName(),
                    (this.info["color" + indexStr] as SQInteger) + 1);
            }

            return playerInfo;
        }

        private class Info
        {
            private byte[] signature;

            private SQTable table;

            public Info()
            {
            }

            public IEnumerable<byte> Signature => this.signature;

            public string Version { get; private set; }

            public SQObject this[string key]
                => this.table.Value.TryGetValue(new SQString(key), out var value)
                    ? value : throw new ArgumentOutOfRangeException(nameof(key));

            public SQObject this[string key1, int key2]
                => this[key1] is SQArray array
                    ? (key2 >= 0) && (key2 < array.Value.Count())
                        ? array.Value.ElementAt(key2)
                        : throw new ArgumentOutOfRangeException(nameof(key2))
                    : throw new InvalidDataException(string.Format(
                        CultureInfo.CurrentCulture, Resources.InvalidDataExceptionMustBeAnArray, key1));

            public SQObject this[string key1, int key2, string key3]
                => this[key1, key2] is SQTable table
                    ? table.Value.TryGetValue(new SQString(key3), out var value)
                        ? value
                        : throw new ArgumentOutOfRangeException(nameof(key3))
                    : throw new InvalidDataException(string.Format(
                        CultureInfo.CurrentCulture, Resources.InvalidDataExceptionMustBeADictionary, key1, key2));

            public void ReadFrom(BinaryReader reader)
            {
                if (reader != null)
                {
                    this.signature = reader.ReadBytes(5);
                    var version = reader.ReadInt32();
                    var size = reader.ReadInt32();
                    var deflateSize = reader.ReadInt32();

                    if (deflateSize == size - (sizeof(int) * 2))
                    {
                        var extractedSize = reader.ReadInt32();
                        var deflateData = reader.ReadBytes(deflateSize);

                        var remainder = version % 10;
                        this.Version = (version / 1000f).ToString("F2", CultureInfo.InvariantCulture) +
                            ((remainder > 0)
                                ? ((char)('a' + remainder)).ToString(CultureInfo.InvariantCulture) : string.Empty);

                        Extract(deflateData, out var extractedData, extractedSize);
                        if (extractedData.Length == extractedSize)
                        {
                            {
                                using var stream = new MemoryStream(extractedData, false);
                                using var reader2 = new BinaryReader(stream, Encoding.UTF8NoBOM);

                                this.table = SQTable.Create(reader2, true);
                            }

#if DEBUG
                            var profiles = this.table.Value[new SQString("profile")] as SQArray;
                            var prof0_dict = (profiles.Value.ElementAt(0) as SQTable).Value
                                .Where(pair => pair.Key is SQString)
                                .ToDictionary(pair => pair.Key as SQString, pair => pair.Value);
                            var icon_dump_str = prof0_dict[new SQString("icon_dump")] as SQString;
                            var icon_dump_bytes = Encoding.CP932.GetBytes(icon_dump_str);
                            var icon_dump = icon_dump_bytes.Select(elem => (char)elem).ToArray();
                            var icon = Convert.FromBase64CharArray(
                                icon_dump, "32,32,21,".Length, (icon_dump.Length - 16) & ~0x3);
                            using var bitmap = new Bitmap(32, 32, PixelFormat.Format32bppArgb);

                            using (var locked = new BitmapLock(bitmap, ImageLockMode.WriteOnly))
                            {
                                Marshal.Copy(icon, 0, locked.Scan0, icon.Length);
                            }

                            bitmap.Save("icon.bmp", ImageFormat.Bmp);
#endif
                        }
                    }
                }
            }
        }
    }
}
