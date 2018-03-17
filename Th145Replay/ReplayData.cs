//-----------------------------------------------------------------------
// <copyright file="ReplayData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed.")]

namespace ReimuPlugins.Th145Replay
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

    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:CodeMustNotContainMultipleWhitespaceInARow", Justification = "Reviewed.")]
    public enum GameMode
    {
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
    }

    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:CodeMustNotContainMultipleWhitespaceInARow", Justification = "Reviewed.")]
    public enum Character
    {
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
        [EnumAltName("華扇",        LongName = "茨木 華扇")]        Kasen,
        [EnumAltName("妹紅",        LongName = "藤原 妹紅")]        Mokou,
        [EnumAltName("針妙丸",      LongName = "少名 針妙丸")]      Shinmyoumaru,
        [EnumAltName("菫子",        LongName = "宇佐見 菫子")]      Sumireko,
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

        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:CodeMustNotContainMultipleWhitespaceInARow", Justification = "Reviewed.")]
        private static readonly Dictionary<int, string> BackgroundNames =
            new Dictionary<int, string>
            {
                {  1, "博麗神社(昼)" },
                {  2, "香霖堂" },
                {  3, "雲上の宝船(昼)" },
                {  4, "命蓮寺(昼)" },
                {  5, "夢殿大祀廟(昼)" },
                {  6, "神霊廟(昼)" },
                {  7, "玄武の沢(昼)" },
                {  8, "地霊殿(昼)" },
                {  9, "妖怪狸の森(夕)" },
                { 10, "妖怪狸の森(夜)" },
                { 11, "人間の里(夜)" },
                { 12, "輝針城(夕)" },
                { 15, "人間の里(昼)" },
                { 16, "博麗神社(夕)" },
                { 17, "博麗神社(夜)" },
                { 19, "雲上の宝船(夜)" },
                { 20, "命蓮寺(夕)" },
                { 21, "夢殿大祀廟(夜)" },
                { 22, "神霊廟(夜)" },
                { 23, "玄武の沢(夜)" },
                { 24, "地霊殿(夜)" },
                { 25, "輝針城(夜)" },
                { 26, "迷いの竹林" },
                { 27, "異変の神社" },
                { 28, "外の世界" },
            };

        private static readonly Dictionary<int, string> BgmNames =
            new Dictionary<int, string>
            {
                { 501, "七玉蒐集ショウダウン" },
                { 502, "オカルトアラカルト" },
                { 503, "公正なる奪い合い" },
                { 504, "対蹠地の鐘" },
                { 505, "竹林インフレイム" },
                { 511, "ラストオカルティズム　～ 現し世の秘術師" },
                { 512, "華狭間のバトルフィールド" },
                { 801, "二色蓮花蝶　～ Red and White" },
                { 802, "恋色マスタースパーク" },
                { 803, "時代親父とハイカラ少女" },
                { 804, "感情の摩天楼　～ Cosmic Mind" },
                { 805, "大神神話伝" },
                { 806, "聖徳伝説　～ True Administrator" },
                { 807, "芥川龍之介の河童　～ Candid Friend" },
                { 808, "ハルトマンの妖怪少女" },
                { 809, "幻想郷の二ッ岩" },
                { 810, "亡失のエモーション" },
                { 811, "月まで届け、不死の煙" },
                { 812, "輝く針の小人族　～ Little Princess" },
            };

        private static readonly Dictionary<Character, Dictionary<int, string>> SpellCardNames =
            new Dictionary<Character, Dictionary<int, string>>
            {
                {
                    Character.Reimu,
                    new Dictionary<int, string>
                    {
                        { 0, "霊符「夢想封印」" },
                        { 1, "神技「八方龍殺陣」" },
                        { 2, "宝具「陰陽飛鳥井」" },
                    }
                },
                {
                    Character.Marisa,
                    new Dictionary<int, string>
                    {
                        { 0, "恋符「マスタースパーク」" },
                        { 1, "彗星「ブレイジングスター」" },
                        { 2, "星符「サテライトイリュージョン」" },
                    }
                },
                {
                    Character.IchirinUnzan,
                    new Dictionary<int, string>
                    {
                        { 0, "嵐符「仏罰の野分雲」" },
                        { 1, "積乱「見越し入道雲」" },
                        { 2, "拳固「懺悔の殺風」" },
                    }
                },
                {
                    Character.Byakuren,
                    new Dictionary<int, string>
                    {
                        { 0, "天符「三千大千世界の主」" },
                        { 1, "天符「大日如来の輝き」" },
                        { 2, "天符「釈迦牟尼の五行山」" },
                    }
                },
                {
                    Character.Futo,
                    new Dictionary<int, string>
                    {
                        { 0, "風符「三輪の皿嵐」" },
                        { 1, "炎符「太乙真火」" },
                        { 2, "運気「破局の開門」" },
                    }
                },
                {
                    Character.Miko,
                    new Dictionary<int, string>
                    {
                        { 0, "仙符「日出ずる処の道士」" },
                        { 1, "道符「掌の上の天道」" },
                        { 2, "人符「勧善懲悪は古の良き典なり」" },
                    }
                },
                {
                    Character.Nitori,
                    new Dictionary<int, string>
                    {
                        { 0, "豪雨「河底大戦争」" },
                        { 1, "泡符「撃て！バブルドラゴン」" },
                        { 2, "戦機「飛べ！三平ファイト」" },
                    }
                },
                {
                    Character.Koishi,
                    new Dictionary<int, string>
                    {
                        { 0, "抑制「スーパーエゴ」" },
                        { 1, "本能「イドの解放」" },
                        { 2, "夢符「ご先祖様が見ているぞ」" },
                    }
                },
                {
                    Character.Mamizou,
                    new Dictionary<int, string>
                    {
                        { 0, "変化「分福熱湯風呂」" },
                        { 1, "変化「百鬼妖界の門」" },
                        { 2, "変化「二ッ岩家の裁き」" },
                    }
                },
                {
                    Character.Kokoro,
                    new Dictionary<int, string>
                    {
                        { 0, "怒面「怒れる忌狼の面」" },
                        { 1, "憑依「喜怒哀楽ポゼッション」" },
                        { 2, "憂面「杞人地を憂う」" },
                    }
                },
                {
                    Character.Kasen,
                    new Dictionary<int, string>
                    {
                        { 0, "包符「義腕プロテウス」" },
                        { 1, "龍符「ドラゴンズグロウル」" },
                        { 2, "鷹符「ホークビーコン」" },
                    }
                },
                {
                    Character.Mokou,
                    new Dictionary<int, string>
                    {
                        { 0, "焔符「自滅火焔大旋風」" },
                        { 1, "不死「凱風快晴飛翔蹴」" },
                        { 2, "呪札「無差別発火の符」" },
                    }
                },
                {
                    Character.Shinmyoumaru,
                    new Dictionary<int, string>
                    {
                        { 0, "小槌「伝説の椀飯振舞」" },
                        { 1, "小人「一寸法師にも五分の魂」" },
                        { 2, "釣符「可愛い太公望」" },
                    }
                },
                {
                    Character.Sumireko,
                    new Dictionary<int, string>
                    {
                        { 0, "銃符「３Ｄプリンターガン」" },
                        { 1, "念力「サイコキネシスアプリ」" },
                        { 2, "念力「テレキネシス　電波塔」" },
                    }
                },
            };

        private Info info;

        public ReplayData()
        {
            this.info = new Info();
        }

        public int BackgroundId
        {
            get { return (int)this.info["background"]; }
        }

        public string BackgroundName
        {
            get
            {
                string name;
                return BackgroundNames.TryGetValue(this.BackgroundId, out name) ? name : string.Empty;
            }
        }

        public int BgmId
        {
            get { return (int)this.info["bgm"]; }
        }

        public string BgmName
        {
            get
            {
                string name;
                return BgmNames.TryGetValue(this.BgmId, out name) ? name : string.Empty;
            }
        }

        public GameMode GameMode
        {
            get { return this.info["game_mode"].ToValidEnum<GameMode>(); }
        }

        public Character Character1
        {
            get { return this.info["player0"].ToValidEnum<Character>(); }
        }

        public int Color1
        {
            get { return (int)this.info["color0"]; }
        }

        public int SpellCard1Id
        {
            get { return (int)this.info["spellcard0"]; }
        }

        public string SpellCard1Name
        {
            get
            {
                string name;
                return SpellCardNames[this.Character1].TryGetValue(this.SpellCard1Id, out name)
                    ? name : string.Empty;
            }
        }

        public string Profile1Name
        {
            get { return this.info["profile", 0, "name"] as string; }
        }

        public string Player1Info
        {
            get { return this.GetPlayerInfo(0); }
        }

        public Character Character2
        {
            get { return this.info["player1"].ToValidEnum<Character>(); }
        }

        public int Color2
        {
            get { return (int)this.info["color1"]; }
        }

        public int SpellCard2Id
        {
            get { return (int)this.info["spellcard1"]; }
        }

        public string SpellCard2Name
        {
            get
            {
                string name;
                return SpellCardNames[this.Character2].TryGetValue(this.SpellCard2Id, out name)
                    ? name : string.Empty;
            }
        }

        public string Profile2Name
        {
            get { return this.info["profile", 1, "name"] as string; }
        }

        public string Player2Info
        {
            get { return this.GetPlayerInfo(1); }
        }

        public int Seed
        {
            get { return (int)this.info["seed"]; }
        }

        public string Version
        {
            get { return this.info.Version; }
        }

        public DateTime DateTime
        {
            get
            {
                return new DateTime(
                    (int)this.info["year"],
                    (int)this.info["month"],
                    (int)this.info["day"],
                    (int)this.info["hour"],
                    (int)this.info["min"],
                    (int)this.info["sec"]);
            }
        }

        public void Read(Stream input)
        {
            using (var reader = new BinaryReader(input, Enc.UTF8NoBOM, true))
            {
                this.info.ReadFrom(reader);
            }
        }

        private static bool ReadObject(BinaryReader reader, out object obj)
        {
            var type = reader.ReadUInt32();

            Func<BinaryReader, object> objectReader;
            obj = ObjectReaders.TryGetValue(type, out objectReader) ? objectReader(reader) : null;

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
                    object index;
                    if (ReadObject(reader, out index))
                    {
                        object value;
                        if (ReadObject(reader, out value))
                        {
                            if ((index is int) && ((int)index < num))
                            {
                                array[(int)index] = value;
                            }
                        }
                    }
                }

                object endmark;
                if (ReadObject(reader, out endmark) && (endmark is EndMark))
                {
                    return array;
                }
            }

            return new object[] { };
        }

        private static object ReadDictionary(BinaryReader reader)
        {
            var dictionary = new Dictionary<object, object>();
            while (true)
            {
                object key;
                if (ReadObject(reader, out key))
                {
                    if (key is EndMark)
                    {
                        break;
                    }

                    object value;
                    if (ReadObject(reader, out value))
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

                using (var stream = new DeflateStream(
                    new MemoryStream(input, validHeader.Length, input.Length - validHeader.Length, false),
                    CompressionMode.Decompress))
                {
                    extractedSize = stream.Read(extracted, 0, extracted.Length);
                }

                output = new byte[extractedSize];
                Array.Copy(extracted, output, output.Length);
            }
            else
            {
                output = new byte[] { };
            }

            return output.Length > 0;
        }

        private string GetPlayerInfo(int index)
        {
            var playerInfo = string.Empty;

            if ((index == 0) || (index == 1))
            {
                var character = (index == 0) ? this.Character1 : this.Character2;
                var card = (index == 0) ? this.SpellCard1Name : this.SpellCard2Name;
                var indexStr = index.ToString(CultureInfo.CurrentCulture);

                var format = string.Join(
                    Environment.NewLine,
                    "Player {0}",
                    "Character: {1}",
                    "Color: {2}",
                    "Spell Card: {3}");

                playerInfo = string.Format(
                    CultureInfo.CurrentCulture,
                    format,
                    index + 1,
                    character.ToLongName(),
                    (int)this.info["color" + indexStr] + 1,
                    card);
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
            public ReadOnlyCollection<byte> Signature
            {
                get { return Array.AsReadOnly(this.signature); }
            }

            public string Version { get; private set; }

            public object this[string key]
            {
                get
                {
                    object value;
                    if (this.dictionary.TryGetValue(key, out value))
                    {
                        return value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("key");
                    }
                }
            }

            public object this[string key1, int key2]
            {
                get
                {
                    var array = this[key1] as object[];
                    if (array != null)
                    {
                        if ((key2 >= 0) && (key2 < array.Length))
                        {
                            return array[key2];
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("key2");
                        }
                    }
                    else
                    {
                        throw new InvalidDataException("key1");
                    }
                }
            }

            public object this[string key1, int key2, string key3]
            {
                get
                {
                    var dict = this[key1, key2] as Dictionary<object, object>;
                    if (dict != null)
                    {
                        object value;
                        if (dict.TryGetValue(key3, out value))
                        {
                            return value;
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("key3");
                        }
                    }
                    else
                    {
                        throw new InvalidDataException("key1 and/or key2");
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

                    if ((deflateSize == size - (sizeof(int) * 2)) && (version >= 1450000))
                    {
                        var extractedSize = reader.ReadInt32();
                        var deflateData = reader.ReadBytes(deflateSize);

                        version -= 1450000;
                        var remainder = version % 10;
                        this.Version = (version / 1000f).ToString("F2", CultureInfo.CurrentCulture) +
                            ((remainder > 0) ? ((char)((int)'a' + remainder)).ToString() : string.Empty);

                        byte[] extractedData;
                        Extract(deflateData, out extractedData, extractedSize);
                        if (extractedData.Length == extractedSize)
                        {
                            using (var stream = new MemoryStream(extractedData, false))
                            {
                                using (var reader2 = new BinaryReader(stream, Enc.UTF8NoBOM, true))
                                {
                                    if (ReadDictionary(reader2) is Dictionary<object, object> dict)
                                    {
                                        this.dictionary = dict
                                            .Where(pair => pair.Key is string)
                                            .ToDictionary(pair => pair.Key as string, pair => pair.Value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
