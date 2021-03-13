//-----------------------------------------------------------------------
// <copyright file="ReplayData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Th155Replay
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
    using ReimuPlugins.Th155Replay.Properties;

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

    public enum Difficulty
    {
#pragma warning disable SA1134 // Attributes should not share line
        [EnumAltName("E")] Easy,
        [EnumAltName("N")] Normal,
        [EnumAltName("H")] Hard,
        [EnumAltName("L")] Lunatic,
        [EnumAltName("D")] OverDrive,
#pragma warning restore SA1134 // Attributes should not share line
    }

    public enum Character
    {
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
#pragma warning disable SA1134 // Attributes should not share line
        [EnumAltName("霊夢",        LongName = "博麗 霊夢")]          Reimu,
        [EnumAltName("魔理沙",      LongName = "霧雨 魔理沙")]        Marisa,
        [EnumAltName("一輪 & 雲山", LongName = "雲居 一輪 & 雲山")]   IchirinUnzan,
        [EnumAltName("白蓮",        LongName = "聖 白蓮")]            Byakuren,
        [EnumAltName("布都",        LongName = "物部 布都")]          Futo,
        [EnumAltName("神子",        LongName = "豊聡耳 神子")]        Miko,
        [EnumAltName("にとり",      LongName = "河城 にとり")]        Nitori,
        [EnumAltName("こいし",      LongName = "古明地 こいし")]      Koishi,
        [EnumAltName("マミゾウ",    LongName = "二ッ岩 マミゾウ")]    Mamizou,
        [EnumAltName("こころ",      LongName = "秦 こころ")]          Kokoro,
        [EnumAltName("華扇",        LongName = "茨木 華扇")]          Kasen,
        [EnumAltName("妹紅",        LongName = "藤原 妹紅")]          Mokou,
        [EnumAltName("針妙丸",      LongName = "少名 針妙丸")]        Shinmyoumaru,
        [EnumAltName("菫子",        LongName = "宇佐見 菫子")]        Sumireko,
        [EnumAltName("鈴仙",        LongName = "鈴仙・Ｕ・イナバ")]   Reisen,
        [EnumAltName("ドレミー",    LongName = "ドレミー・スイート")] Doremy,
        [EnumAltName("天子",        LongName = "比那名居 天子")]      Tenshi,
        [EnumAltName("紫",          LongName = "八雲 紫")]            Yukari,
        [EnumAltName("女苑 & 紫苑", LongName = "依神 女苑 & 紫苑")]   JoonShion,
#pragma warning restore SA1134 // Attributes should not share line
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
    }

    public enum StoryCharacter
    {
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
#pragma warning disable SA1134 // Attributes should not share line
        [EnumAltName("霊夢",        LongName = "博麗 霊夢")]          Reimu,
        [EnumAltName("魔理沙",      LongName = "霧雨 魔理沙")]        Marisa,
        [EnumAltName("一輪 & 雲山", LongName = "雲居 一輪 & 雲山")]   IchirinUnzan,
        [EnumAltName("白蓮",        LongName = "聖 白蓮")]            Byakuren,
        [EnumAltName("布都",        LongName = "物部 布都")]          Futo,
        [EnumAltName("神子",        LongName = "豊聡耳 神子")]        Miko,
        [EnumAltName("にとり",      LongName = "河城 にとり")]        Nitori,
        [EnumAltName("こいし",      LongName = "古明地 こいし")]      Koishi,
        [EnumAltName("マミゾウ",    LongName = "二ッ岩 マミゾウ")]    Mamizou,
        [EnumAltName("こころ",      LongName = "秦 こころ")]          Kokoro,
        [EnumAltName("華扇",        LongName = "茨木 華扇")]          Kasen,
        [EnumAltName("妹紅",        LongName = "藤原 妹紅")]          Mokou,
        [EnumAltName("針妙丸",      LongName = "少名 針妙丸")]        Shinmyoumaru,
        [EnumAltName("菫子",        LongName = "宇佐見 菫子")]        Sumireko,
        [EnumAltName("鈴仙",        LongName = "鈴仙・Ｕ・イナバ")]   Reisen,
        [EnumAltName("ドレミー",    LongName = "ドレミー・スイート")] Doremy,
        [EnumAltName("天子",        LongName = "比那名居 天子")]      Tenshi,
        [EnumAltName("紫",          LongName = "八雲 紫")]            Yukari,
        [EnumAltName("女苑",        LongName = "依神 女苑")]          Joon,
        [EnumAltName("紫苑",        LongName = "依神 紫苑")]          Shion,
#pragma warning restore SA1134 // Attributes should not share line
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
    }

    public sealed class ReplayData
    {
        private static readonly IReadOnlyDictionary<string, Character> Characters =
            new Dictionary<string, Character>
            {
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
                { "reimu",       Character.Reimu        },
                { "marisa",      Character.Marisa       },
                { "ichirin",     Character.IchirinUnzan },
                { "hijiri",      Character.Byakuren     },
                { "futo",        Character.Futo         },
                { "miko",        Character.Miko         },
                { "mamizou",     Character.Mamizou      },
                { "koishi",      Character.Koishi       },
                { "nitori",      Character.Nitori       },
                { "kokoro",      Character.Kokoro       },
                { "kasen",       Character.Kasen        },
                { "mokou",       Character.Mokou        },
                { "sinmyoumaru", Character.Shinmyoumaru },
                { "usami",       Character.Sumireko     },
                { "udonge",      Character.Reisen       },
                { "doremy",      Character.Doremy       },
                { "tenshi",      Character.Tenshi       },
                { "yukari",      Character.Yukari       },
                { "jyoon",       Character.JoonShion    },
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
            };

        private static readonly IReadOnlyDictionary<string, StoryCharacter> StoryCharacters =
            new Dictionary<string, StoryCharacter>
            {
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
                { "reimu",       StoryCharacter.Reimu        },
                { "marisa",      StoryCharacter.Marisa       },
                { "ichirin",     StoryCharacter.IchirinUnzan },
                { "hijiri",      StoryCharacter.Byakuren     },
                { "futo",        StoryCharacter.Futo         },
                { "miko",        StoryCharacter.Miko         },
                { "mamizou",     StoryCharacter.Mamizou      },
                { "koishi",      StoryCharacter.Koishi       },
                { "nitori",      StoryCharacter.Nitori       },
                { "kokoro",      StoryCharacter.Kokoro       },
                { "kasen",       StoryCharacter.Kasen        },
                { "mokou",       StoryCharacter.Mokou        },
                { "sinmyoumaru", StoryCharacter.Shinmyoumaru },
                { "usami",       StoryCharacter.Sumireko     },
                { "udonge",      StoryCharacter.Reisen       },
                { "doremy",      StoryCharacter.Doremy       },
                { "tenshi",      StoryCharacter.Tenshi       },
                { "yukari",      StoryCharacter.Yukari       },
                { "jyoon",       StoryCharacter.Joon         },
                { "shion",       StoryCharacter.Shion        },
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
            };

        private static readonly IReadOnlyDictionary<int, string> BackgroundNames =
            new Dictionary<int, string>
            {
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
                {  1, "博麗神社(昼)" },
                {  2, "香霖堂(夕)" },
                {  3, "雲上の宝船(昼)" },
                {  4, "命蓮寺(昼)" },
                {  5, "神霊廟(昼)" },
                {  6, "夢殿大祀廟(夕)" },
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
                { 30, "永遠亭" },
                { 31, "博麗神社(紅)" },
                { 40, "夢の世界" },
                { 41, "天界" },
                { 42, "太陽のライブステージ(夕)" },
                { 43, "太陽のライブステージ(夜)" },
                { 44, "華扇の仙界" },
                { 45, "香霖堂(夜)" },
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
            };

        private static readonly IReadOnlyDictionary<int, string> BgmNames =
            new Dictionary<int, string>
            {
                { 101, "二色蓮花蝶　～ Red and White" },
                { 102, "恋色マスタースパーク" },
                { 103, "時代親父とハイカラ少女" },
                { 104, "感情の摩天楼　～ Cosmic Mind" },
                { 105, "大神神話伝" },
                { 106, "聖徳伝説　～ True Administrator" },
                { 107, "芥川龍之介の河童　～ Candid Friend" },
                { 108, "ハルトマンの妖怪少女" },
                { 109, "幻想郷の二ッ岩" },
                { 110, "亡失のエモーション" },
                { 111, "月まで届け、不死の煙" },
                { 112, "輝く針の小人族　～ Little Princess" },
                { 113, "華狭間のバトルフィールド" },
                { 114, "ラストオカルティズム　～ 現し世の秘術師" },
                { 115, "狂気の瞳　～ Invisible Full Moon" },
                { 116, "永遠の春夢" },
                { 117, "有頂天変　～ Wonderful Heaven" },
                { 118, "夜が降りてくる　～ Evening Star" },
                { 301, "地の色は黄色　～ Primrose" },
                { 302, "マッシュルーム・ワルツ" },
                { 303, "聖輦船空を往く" },
                { 304, "法力の下の平等" },
                { 305, "恒常不変の参廟祀" },
                { 306, "光輝く天球儀" },
                { 307, "沢の河童の技術力" },
                { 308, "地底に咲く薔薇" },
                { 309, "新緑の狸森にて" },
                { 310, "心綺楼演舞" },
                { 311, "不滅のレッドソウル" },
                { 312, "落日に映える逆さ城" },
                { 313, "千の試練を超えて" },
                { 314, "夢世界フォークロア" },
                { 315, "永遠に続く回廊" },
                { 316, "スリープシープ・パレード" },
                { 317, "至る有頂天" },
                { 318, "憑坐は夢と現の間に　～ Necro-Fantasia" },
                { 319, "今宵は飄逸なエゴイスト(Live ver)　～ Egoistic Flowers." },
                { 351, "オカルトアトラクト" },
                { 352, "ネオ竹林インフレイム" },
                { 353, "億万劫の鐘" },
                { 354, "アンノウンX　～ Occultly Madness" },
            };

        private static readonly IReadOnlyDictionary<Character, IReadOnlyDictionary<int, string>> SpellCardNames =
            new Dictionary<Character, IReadOnlyDictionary<int, string>>
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
                {
                    Character.Reisen,
                    new Dictionary<int, string>
                    {
                        { 0, "「月面跳弾（ルナティックダブル）」" },
                        { 1, "「幻朧月睨（ルナティックレッドアイズ）」" },
                        { 2, "「地上跳弾（ルナティックエコー）」" },
                    }
                },
                {
                    Character.Doremy,
                    new Dictionary<int, string>
                    {
                        { 0, "夢符「留紺色の逃走夢」" },
                        { 1, "夢符「漆黒の宇宙夢」" },
                        { 2, "羊符「ナイトメア・オブ・キメラ」" },
                    }
                },
                {
                    Character.Tenshi,
                    new Dictionary<int, string>
                    {
                        { 0, "要石「カナメファンネル」" },
                        { 1, "地符「一撃震乾坤」" },
                        { 2, "桃符「堅牢堅固の仙桃」" },
                    }
                },
                {
                    Character.Yukari,
                    new Dictionary<int, string>
                    {
                        { 0, "「無人廃線車両爆弾」" },
                        { 1, "式神「八雲藍＆橙」" },
                        { 2, "境界「溢れ出る漂流物」" },
                    }
                },
                {
                    Character.JoonShion,
                    new Dictionary<int, string>
                    {
                        { 0, "憑依剥奪「スレイブロバー」" },
                        { 1, "貧符「超貧乏玉」" },
                        { 2, "「クイーンオブバブル」" },
                    }
                },
            };

        private readonly Info info;

        // NOTE: If we define this as an expression-bodied constructor, CA1812 is reported for the Info class.
        public ReplayData()
        {
            this.info = new Info();
        }

        public string Version
            => this.info.Version;

        public DateTime DateTime => new(
            this.info["year"] as SQInteger,
            this.info["month"] as SQInteger,
            this.info["day"] as SQInteger,
            this.info["hour"] as SQInteger,
            this.info["min"] as SQInteger,
            this.info["sec"] as SQInteger);

        public int GetBackgroundId()
        {
            return (this.info["background_id"] is SQInteger value)
                ? value : throw NewInvalidPropertyException(nameof(this.GetBackgroundId));
        }

        public string GetBackgroundName()
        {
            return BackgroundNames.TryGetValue(this.GetBackgroundId(), out var name) ? name : string.Empty;
        }

        public int GetBgmId()
        {
            return (this.info["bgm_id"] is SQInteger value)
                ? value : throw NewInvalidPropertyException(nameof(this.GetBgmId));
        }

        public string GetBgmName()
        {
            return BgmNames.TryGetValue(this.GetBgmId(), out var name) ? name : string.Empty;
        }

        public GameMode GetGameMode()
        {
            return (this.info["game_mode"] as SQInteger).ToInt32().ToValidEnum<GameMode>();
        }

        public Difficulty GetDifficulty()
        {
            return (this.info["difficulty"] as SQInteger).ToInt32().ToValidEnum<Difficulty>();
        }

        public Character GetMasterName1()
        {
            return Characters.TryGetValue(this.info["master_name", 0] as SQString, out var chara)
                ? chara : throw NewInvalidPropertyException(nameof(this.GetMasterName1));
        }

        public int GetMasterColor1()
        {
            return (this.info["master_color", 0] is SQInteger value)
                ? value : throw NewInvalidPropertyException(nameof(this.GetMasterColor1));
        }

        public Character GetSlaveName1()
        {
            return Characters.TryGetValue(this.info["slave_name", 0] as SQString, out var chara)
                ? chara : throw NewInvalidPropertyException(nameof(this.GetSlaveName1));
        }

        public int GetSlaveColor1()
        {
            return (this.info["slave_color", 0] is SQInteger value)
                ? value : throw NewInvalidPropertyException(nameof(this.GetSlaveColor1));
        }

        public int GetSpellCard1Id()
        {
            return (this.info["spell", 0] is SQInteger value)
                ? value : throw NewInvalidPropertyException(nameof(this.GetSpellCard1Id));
        }

        public string GetSpellCard1Name()
        {
            return SpellCardNames[this.GetMasterName1()].TryGetValue(this.GetSpellCard1Id(), out var name)
                ? name : string.Empty;
        }

        public string GetPlayer1Name()
        {
            return (this.info["player_name", 0] is SQString value)
                ? value : throw NewInvalidPropertyException(nameof(this.GetPlayer1Name));
        }

        public Character GetMasterName2()
        {
            return Characters.TryGetValue(this.info["master_name", 1] as SQString, out var chara)
                ? chara : throw NewInvalidPropertyException(nameof(this.GetMasterName2));
        }

        public int GetMasterColor2()
        {
            return (this.info["master_color", 1] is SQInteger value)
                ? value : throw NewInvalidPropertyException(nameof(this.GetMasterColor2));
        }

        public Character GetSlaveName2()
        {
            return Characters.TryGetValue(this.info["slave_name", 1] as SQString, out var chara)
                ? chara : throw NewInvalidPropertyException(nameof(this.GetSlaveName2));
        }

        public int GetSlaveColor2()
        {
            return (this.info["slave_color", 1] is SQInteger value)
                ? value : throw NewInvalidPropertyException(nameof(this.GetSlaveColor2));
        }

        public int GetSpellCard2Id()
        {
            return (this.info["spell", 1] is SQInteger value)
                ? value : throw NewInvalidPropertyException(nameof(this.GetSpellCard2Id));
        }

        public string GetSpellCard2Name()
        {
            return SpellCardNames[this.GetMasterName2()].TryGetValue(this.GetSpellCard2Id(), out var name)
                ? name : string.Empty;
        }

        public string GetPlayer2Name()
        {
            return (this.info["player_name", 1] is SQString value)
                ? value : throw NewInvalidPropertyException(nameof(this.GetPlayer2Name));
        }

        public StoryCharacter GetStoryMaster()
        {
            return StoryCharacters.TryGetValue(this.info["scenario_name"] as SQString, out var chara)
                ? chara : throw NewInvalidPropertyException(nameof(this.GetStoryMaster));
        }

        public StoryCharacter GetStorySlave()
        {
            return StoryCharacters.TryGetValue(this.info["slave_name"] as SQString, out var chara)
                ? chara : throw NewInvalidPropertyException(nameof(this.GetStorySlave));
        }

        public int GetStorySpellCardId()
        {
            return (this.info["spell"] is SQInteger value)
                ? value : throw NewInvalidPropertyException(nameof(this.GetStorySpellCardId));
        }

        public string GetStorySpellCardName()
        {
            return SpellCardNames[(Character)this.GetStoryMaster()].TryGetValue(this.GetStorySpellCardId(), out var name)
                ? name : string.Empty;
        }

        public void Read(Stream input)
        {
            using var reader = new BinaryReader(input, Encoding.UTF8, leaveOpen: true);
            this.info.ReadFrom(reader);
        }

        private static InvalidDataException NewInvalidPropertyException(string propertyName)
        {
            return new InvalidDataException(string.Format(
                CultureInfo.CurrentCulture, Resources.InvalidDataExceptionPropertyIsInvalid, propertyName));
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
                => (this.table != null)
                    ? this.table.Value[new SQString(key)]
                    : throw new InvalidOperationException(string.Format(
                        CultureInfo.InvariantCulture, $"Call {nameof(this.ReadFrom)}() first."));

            public SQObject this[string key1, int key2]
                => (this[key1] is SQArray array)
                    ? array.Value.ElementAt(key2)
                    : throw new ArgumentException(string.Format(
                        CultureInfo.InvariantCulture, $"Item[{key1}] is not an array."));

            public void ReadFrom(BinaryReader reader)
            {
                if (reader == null)
                {
                    throw new ArgumentNullException(nameof(reader));
                }

                this.signature = reader.ReadBytes(5);
                this.Version = ParseVersion(reader.ReadInt32());

                var size = reader.ReadInt32();
                var deflateSize = reader.ReadInt32();
                var extractedSize = reader.ReadInt32();
                if (deflateSize != size - (sizeof(int) * 2))
                {
                    throw new InvalidDataException(string.Format(
                        CultureInfo.InvariantCulture,
                        "Invalid {0} ({1}) or {2} ({3})",
                        nameof(size),
                        size,
                        nameof(deflateSize),
                        deflateSize));
                }

                var deflateData = reader.ReadBytes(deflateSize);

                var extractedData = Extract(deflateData, extractedSize);
                if (extractedData.Length != extractedSize)
                {
                    throw new InvalidDataException(string.Format(
                        CultureInfo.InvariantCulture,
                        "Invalid {0} ({1}) or {2}",
                        nameof(extractedSize),
                        extractedSize,
                        nameof(deflateData)));
                }

                using var stream = new MemoryStream(extractedData, false);
                using var reader2 = new BinaryReader(stream);

                this.table = SQTable.Create(reader2, true);
            }

            private static string ParseVersion(int number)
            {
                const int ValidMinVersion = 1550000;
                const int ValidMaxVersion = 1559999;

                if ((number < ValidMinVersion) || (number > ValidMaxVersion))
                {
                    throw new ArgumentOutOfRangeException(nameof(number));
                }

                number -= ValidMinVersion;
                var remainder = number % 10;
                return (number / 1000f).ToString("F2", CultureInfo.InvariantCulture) +
                    ((remainder > 0)
                        ? ((char)('a' + remainder)).ToString(CultureInfo.InvariantCulture) : string.Empty);
            }

            private static byte[] Extract(byte[] input, int expectedSize)
            {
                if (input == null)
                {
                    throw new ArgumentNullException(nameof(input));
                }

                if (expectedSize <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(expectedSize));
                }

                // See section 2.2 of RFC 1950
                var validHeader = new byte[] { 0x78, 0x9C };

                if (!input.Take(validHeader.Length).SequenceEqual(validHeader))
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.InvariantCulture, "Invalid header."),
                        nameof(input));
                }

                var extracted = new byte[expectedSize];
                var extractedSize = 0;
                {
                    using var baseStream = new MemoryStream(
                        input, validHeader.Length, input.Length - validHeader.Length, false);
                    using var stream = new DeflateStream(baseStream, CompressionMode.Decompress);

                    extractedSize = stream.Read(extracted, 0, extracted.Length);
                }

                var output = new byte[extractedSize];
                Array.Copy(extracted, output, output.Length);

                return output;
            }
        }
    }
}
