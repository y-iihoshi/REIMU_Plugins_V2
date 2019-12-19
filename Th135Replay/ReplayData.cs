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
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using ReimuPlugins.Common;
    using ReimuPlugins.Th135Replay.Properties;

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
        private static readonly Dictionary<uint, Func<BinaryReader, object>> ObjectReaders =
            new Dictionary<uint, Func<BinaryReader, object>>
            {
                { 0x01000001, reader => new EndMark() },
                { 0x01000008, reader => reader.ReadByte() == 0x01 },
                { 0x05000002, reader => reader.ReadInt32() },
                { 0x05000004, reader => reader.ReadSingle() },
                { 0x08000010, reader => ReadString(reader) },
                { 0x08000040, reader => ReadArray(reader) },
                { 0x0A000020, reader => ReadDictionary(reader) },
            };

        private static readonly Dictionary<int, string> BackgroundNames =
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

        private static readonly Dictionary<int, string> BgmNames =
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

        private static readonly Dictionary<int, string> EquipNames =
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

        public int BackgroundId => (int)this.info["background"];

        public string BackgroundName => BackgroundNames.TryGetValue(this.BackgroundId, out var name)
            ? name : string.Empty;

        public int BgmId => (int)this.info["bgm"];

        public string BgmName => BgmNames.TryGetValue(this.BgmId, out var name) ? name : string.Empty;

        public GameMode GameMode => this.info["game_mode"].ToValidEnum<GameMode>();

        public Character Character1 => this.info["player0"].ToValidEnum<Character>();

        public int Color1 => (int)this.info["color0"];

        public string Profile1Name => this.info["profile", 0, "name"] as string;

        public string Player1Info => this.GetPlayerInfo(0);

        public Character Character2 => this.info["player1"].ToValidEnum<Character>();

        public int Color2 => (int)this.info["color1"];

        public string Profile2Name => this.info["profile", 1, "name"] as string;

        public string Player2Info => this.GetPlayerInfo(1);

        public int Seed => (int)this.info["seed"];

        public string Version => this.info.Version;

        public DateTime DateTime => new DateTime(
            (int)this.info["year"],
            (int)this.info["month"],
            (int)this.info["day"],
            (int)this.info["hour"],
            (int)this.info["min"],
            (int)this.info["sec"]);

        public void Read(Stream input)
        {
            using var reader = new BinaryReader(input, Enc.UTF8NoBOM, true);
            this.info.ReadFrom(reader);
        }

        private static bool ReadObject(BinaryReader reader, out object obj)
        {
            var type = reader.ReadUInt32();
            obj = ObjectReaders.TryGetValue(type, out var objectReader) ? objectReader(reader) : null;
            return obj != null;
        }

        private static object ReadString(BinaryReader reader)
        {
            var size = reader.ReadInt32();
            return (size > 0) ? Enc.CP932.GetString(reader.ReadBytes(size)) : string.Empty;
        }

        private static object ReadArray(BinaryReader reader)
        {
            var num = reader.ReadInt32();
            if (num > 0)
            {
                var array = new object[num];
                for (var count = 0; count < num; count++)
                {
                    if (ReadObject(reader, out var index))
                    {
                        if (ReadObject(reader, out var value))
                        {
                            if ((index is int) && ((int)index < num))
                            {
                                array[(int)index] = value;
                            }
                        }
                    }
                }

                if (ReadObject(reader, out var endmark) && (endmark is EndMark))
                {
                    return array;
                }
            }

            return Array.Empty<object>();
        }

        private static object ReadDictionary(BinaryReader reader)
        {
            var dictionary = new Dictionary<object, object>();
            while (true)
            {
                if (ReadObject(reader, out var key))
                {
                    if (key is EndMark)
                    {
                        break;
                    }

                    if (ReadObject(reader, out var value))
                    {
                        dictionary.Add(key, value);
                    }
                }
                else
                {
                    break;
                }
            }

            return dictionary;
        }

        private static bool Extract(byte[] input, out byte[] output, int expectedSize)
        {
            // See section 2.2 of RFC 1950
            var validHeader = new byte[] { 0x78, 0x9C };

            if (input.Take(validHeader.Length).SequenceEqual(validHeader))
            {
                var extracted = new byte[expectedSize];
                var extractedSize = 0;

                MemoryStream memory = null;
                try
                {
#pragma warning disable IDISP001 // Dispose created.
                    memory = new MemoryStream(input, validHeader.Length, input.Length - validHeader.Length, false);
#pragma warning restore IDISP001 // Dispose created.
                    using var deflate = new DeflateStream(memory, CompressionMode.Decompress);
#pragma warning disable IDISP003 // Dispose previous before re-assigning.
                    memory = null;
#pragma warning restore IDISP003 // Dispose previous before re-assigning.

                    extractedSize = deflate.Read(extracted, 0, extracted.Length);
                }
                finally
                {
                    memory?.Dispose();
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
                        var equipId = (int)this.info["equip_" + indexStr + dirStr];
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
                    this.info["player" + indexStr].ToValidEnum<Character>().ToLongName(),
                    (int)this.info["color" + indexStr] + 1);
            }

            return playerInfo;
        }

        private class EndMark
        {
        }

        private class Info
        {
            private byte[] signature;

            private Dictionary<string, object> dictionary;

            public Info()
            {
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
            public ReadOnlyCollection<byte> Signature => Array.AsReadOnly(this.signature);

            public string Version { get; private set; }

            public object this[string key]
            {
                get
                {
                    if (this.dictionary.TryGetValue(key, out var value))
                    {
                        return value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(key));
                    }
                }
            }

            public object this[string key1, int key2]
            {
                get
                {
                    if (this[key1] is object[] array)
                    {
                        if ((key2 >= 0) && (key2 < array.Length))
                        {
                            return array[key2];
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException(nameof(key2));
                        }
                    }
                    else
                    {
                        throw new InvalidDataException(string.Format(
                            CultureInfo.CurrentCulture, Resources.InvalidDataExceptionMustBeAnArray, key1));
                    }
                }
            }

            public object this[string key1, int key2, string key3]
            {
                get
                {
                    if (this[key1, key2] is Dictionary<object, object> dict)
                    {
                        if (dict.TryGetValue(key3, out var value))
                        {
                            return value;
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException(nameof(key3));
                        }
                    }
                    else
                    {
                        throw new InvalidDataException(string.Format(
                            CultureInfo.CurrentCulture, Resources.InvalidDataExceptionMustBeADictionary, key1, key2));
                    }
                }
            }

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
                                ? ((char)((int)'a' + remainder)).ToString(CultureInfo.InvariantCulture)
                                : string.Empty);

                        Extract(deflateData, out var extractedData, extractedSize);
                        if (extractedData.Length == extractedSize)
                        {
                            MemoryStream stream = null;
                            try
                            {
#pragma warning disable IDISP001 // Dispose created.
                                stream = new MemoryStream(extractedData, false);
#pragma warning restore IDISP001 // Dispose created.
                                using var reader2 = new BinaryReader(stream, Enc.UTF8NoBOM);
#pragma warning disable IDISP003 // Dispose previous before re-assigning.
                                stream = null;
#pragma warning restore IDISP003 // Dispose previous before re-assigning.

                                if (ReadDictionary(reader2) is Dictionary<object, object> dict)
                                {
                                    this.dictionary = dict
                                        .Where(pair => pair.Key is string)
                                        .ToDictionary(pair => pair.Key as string, pair => pair.Value);
                                }
                            }
                            finally
                            {
                                stream?.Dispose();
                            }

#if false
                            var profiles = this.dictionary["profile"] as object[];
                            var prof0_dict = (profiles[0] as Dictionary<object, object>)
                                .Where(pair => pair.Key is string)
                                .ToDictionary(pair => pair.Key as string, pair => pair.Value);
                            var icon_dump_str = prof0_dict["icon_dump"] as string;
                            var icon_dump_bytes = Enc.CP932.GetBytes(icon_dump_str);
                            var icon_dump = icon_dump_bytes.Select(elem => (char)elem).ToArray();
                            var icon = Convert.FromBase64CharArray(
                                icon_dump, "32,32,21,".Length, (icon_dump.Length - 16) & ~0x3);
                            var bitmap = new Bitmap(32, 32, PixelFormat.Format32bppArgb);

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
