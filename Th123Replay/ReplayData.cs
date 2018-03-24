//-----------------------------------------------------------------------
// <copyright file="ReplayData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed.")]

namespace ReimuPlugins.Th123Replay
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
        [EnumAltName("Story")]                         Story,
        [EnumAltName("Arcade")]                        Arcade,
        [EnumAltName("vs COM")]                        VersusCom,
        [EnumAltName("vs PLAYER")]                     VersusPlayer,
        [EnumAltName("vs NETWORK （ホスト側）")]       VersusNetworkHost,
        [EnumAltName("vs NETWORK （クライアント側）")] VersusNetworkClient,
        [EnumAltName("vs NETWORK （観戦）")]           VersusNetworkWatch,
#pragma warning restore SA1134 // Attributes should not share line
    }

    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:CodeMustNotContainMultipleWhitespaceInARow", Justification = "Reviewed.")]
    public enum Character
    {
#pragma warning disable SA1134 // Attributes should not share line
        [EnumAltName("霊夢",       LongName = "博麗 霊夢")]              Reimu,
        [EnumAltName("魔理沙",     LongName = "霧雨 魔理沙")]            Marisa,
        [EnumAltName("咲夜",       LongName = "十六夜 咲夜")]            Sakuya,
        [EnumAltName("アリス",     LongName = "アリス・マーガトロイド")] Alice,
        [EnumAltName("パチュリー", LongName = "パチュリー・ノーレッジ")] Patchouli,
        [EnumAltName("妖夢",       LongName = "魂魄 妖夢")]              Youmu,
        [EnumAltName("レミリア",   LongName = "レミリア・スカーレット")] Remilia,
        [EnumAltName("幽々子",     LongName = "西行寺 幽々子")]          Yuyuko,
        [EnumAltName("紫",         LongName = "八雲 紫")]                Yukari,
        [EnumAltName("萃香",       LongName = "伊吹 萃香")]              Suika,
        [EnumAltName("鈴仙",       LongName = "鈴仙・優曇華院・イナバ")] Reisen,
        [EnumAltName("文",         LongName = "射命丸 文")]              Aya,
        [EnumAltName("小町",       LongName = "小野塚 小町")]            Komachi,
        [EnumAltName("衣玖",       LongName = "永江 衣玖")]              Iku,
        [EnumAltName("天子",       LongName = "比那名居 天子")]          Tenshi,
        [EnumAltName("早苗",       LongName = "東風谷 早苗")]            Sanae,
        [EnumAltName("チルノ",     LongName = "チルノ")]                 Cirno,
        [EnumAltName("美鈴",       LongName = "紅 美鈴")]                Meiling,
        [EnumAltName("空",         LongName = "霊烏路 空")]              Utsuho,
        [EnumAltName("諏訪子",     LongName = "洩矢 諏訪子")]            Suwako,
#pragma warning restore SA1134 // Attributes should not share line
    }

    public sealed class ReplayData
    {
        private static readonly Dictionary<short, string> Versions =
            new Dictionary<short, string>
            {
                { 200, "1.00" },
                { 201, "1.01" },
                { 202, "1.02" },
                { 203, "1.03" },
                { 210, "1.10" },
            };

        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:CodeMustNotContainMultipleWhitespaceInARow", Justification = "Reviewed.")]
        private static readonly Dictionary<byte, string> StageNames =
            new Dictionary<byte, string>
            {
                {  0, "博麗神社(倒壊)" },
                {  1, "魔法の森" },
                {  2, "玄武の沢" },
                {  3, "妖怪の山" },
                {  4, "玄武海" },
                {  5, "有頂天" },
                {  6, "緋想天" },
                { 10, "博麗神社" },
                { 11, "霧雨魔法店" },
                { 12, "紅魔館時計台" },
                { 13, "人形の森" },
                { 14, "紅魔館大図書館" },
                { 15, "冥界" },
                { 16, "紅魔館ロビー" },
                { 17, "白玉楼の雪庭" },
                { 18, "迷いの竹林" },
                { 30, "霧の湖の辺" },
                { 31, "守矢神社" },
                { 32, "間欠泉地下センター入口" },
                { 33, "間欠泉地下センター通路" },
                { 34, "核融合炉心部" },
                { 35, "真っ暗なステージ" },
                { 36, "時計台(美鈴ストーリー)" },
                { 37, "時計台２(美鈴ストーリー)" },
                { 38, "美鈴ストーリーラスト" },
            };

        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:CodeMustNotContainMultipleWhitespaceInARow", Justification = "Reviewed.")]
        private static readonly Dictionary<byte, string> BgmNames =
            new Dictionary<byte, string>
            {
                {  0, "地の色は黄色" },
                {  1, "香る樹葉花" },
                {  2, "踊る水飛沫" },
                {  3, "嘲りの遊戯" },
                {  4, "黒い海に紅く　～ Legendary Fish" },
                {  5, "有頂天変　～ Wonderful Heaven" },
                {  6, "幼心地の有頂天" },
                { 10, "東方妖恋談" },
                { 11, "星の器　～ Casket of Star" },
                { 12, "フラワリングナイト" },
                { 13, "ブクレシュティの人形師" },
                { 14, "広有射怪鳥事　～ Till When？" },
                { 15, "ラクトガール　～ 少女密室" },
                { 16, "幽雅に咲かせ、墨染の桜　～ Border of Life" },
                { 17, "亡き王女の為のセプテット" },
                { 18, "夜が降りてくる" },
                { 19, "砕月" },
                { 20, "狂気の瞳　～ Invisible Full Moon" },
                { 21, "風神少女" },
                { 22, "彼岸帰航　～ Riverside View" },
                { 30, "信仰は儚き人間の為に" },
                { 31, "おてんば恋娘" },
                { 32, "上海紅茶館　～ Chinese Tea" },
                { 33, "霊知の太陽信仰　～ Nuclear Fusion" },
                { 34, "明日ハレの日、ケの昨日" },
                { 35, "アンノウンＸ　～ Unfound Adventure" },
                { 36, "空に浮かぶ物体Ｘ" },
                { 40, "二色蓮花蝶　～ Ancients" },
                { 41, "恋色マジック" },
                { 42, "the Grimoire of Alice" },
                { 43, "ヴワル魔法図書館" },
            };

        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:CodeMustNotContainMultipleWhitespaceInARow", Justification = "Reviewed.")]
        private static readonly Dictionary<short, string> SystemCardNames =
            new Dictionary<short, string>
            {
                {  0, "「霊撃札」" },
                {  1, "「マジックポーション」" },
                {  2, "「ストップウォッチ」" },
                {  3, "「白楼剣」" },
                {  4, "「身代わり人形」" },
                {  5, "「グリモワール」" },
                {  6, "「特注の日傘」" },
                {  7, "「人魂灯」" },
                {  8, "「左扇」" },
                {  9, "「伊吹瓢」" },
                { 10, "「天狗団扇」" },
                { 11, "「符蝕薬」" },
                { 12, "「宵越しの銭」" },
                { 13, "「龍魚の羽衣」" },
                { 14, "「緋想の剣」" },
                { 15, "「病気平癒守」" },
                { 16, "「冷凍カエル」" },
                { 17, "「龍星」" },
                { 18, "「制御棒」" },
                { 19, "「三粒の天滴」" },
                { 20, "「ナマズの大地震」" },
            };

        private static readonly Dictionary<Character, Dictionary<short, string>> CardNames =
            new Dictionary<Character, Dictionary<short, string>>
            {
                {
                    Character.Reimu,
                    new Dictionary<short, string>
                    {
                        { 100, "博麗アミュレット" },
                        { 101, "警醒陣" },
                        { 102, "亜空穴" },
                        { 103, "昇天脚" },
                        { 104, "妖怪バスター" },
                        { 105, "繋縛陣" },
                        { 106, "封魔亜空穴" },
                        { 107, "抄地昇天脚" },
                        { 108, "拡散アミュレット" },
                        { 109, "常置陣" },
                        { 110, "刹那亜空穴" },
                        { 111, "雨乞祈り" },
                        { 200, "霊符「夢想妙珠」" },
                        { 201, "神霊「夢想封印」" },
                        { 204, "夢符「封魔陣」" },
                        { 206, "神技「八方鬼縛陣」" },
                        { 207, "結界「拡散結界」" },
                        { 208, "珠符「明珠暗投」" },
                        { 209, "宝符「陰陽宝玉」" },
                        { 210, "宝具「陰陽鬼神玉」" },
                        { 214, "神技「天覇風神脚」" },
                        { 219, "「夢想天生」" },
                    }
                },
                {
                    Character.Marisa,
                    new Dictionary<short, string>
                    {
                        { 100, "ウィッチレイライン" },
                        { 101, "ミアズマスウィープ" },
                        { 102, "グラウンドスターダスト" },
                        { 103, "メテオニックデブリ" },
                        { 104, "ラジアルストライク" },
                        { 105, "バスキースウィーパー" },
                        { 106, "デビルダムトーチ" },
                        { 107, "ナロースパーク" },
                        { 108, "アップスウィープ" },
                        { 109, "ステラミサイル" },
                        { 110, "マジカル産廃再利用ボム" },
                        { 111, "グリーンスプレッド" },
                        { 200, "恋符「マスタースパーク」" },
                        { 202, "魔砲「ファイナルスパーク」" },
                        { 203, "星符「ドラゴンメテオ」" },
                        { 204, "恋符「ノンディレクショナルレーザー」" },
                        { 205, "魔符「スターダストレヴァリエ」" },
                        { 206, "星符「エスケープベロシティ」" },
                        { 207, "彗星「ブレイジングスター」" },
                        { 208, "星符「メテオニックシャワー」" },
                        { 209, "星符「グラビティビート」" },
                        { 211, "光符「ルミネスストライク」" },
                        { 212, "光符「アースライトレイ」" },
                        { 214, "魔廃「ディープエコロジカルボム」" },
                        { 215, "儀符「オーレリーズサン」" },
                        { 219, "邪恋「実りやすいマスタースパーク」" },
                    }
                },
                {
                    Character.Sakuya,
                    new Dictionary<short, string>
                    {
                        { 100, "クロースアップマジック" },
                        { 101, "バウンスノーバウンス" },
                        { 102, "マジックスターソード" },
                        { 103, "バニシングエブリシング" },
                        { 104, "プロペリングシルバー" },
                        { 105, "スクウェアリコシェ" },
                        { 106, "離剣の見" },
                        { 107, "パーフェクトメイド" },
                        { 108, "ダンシングスターソード" },
                        { 109, "ミスディレクション" },
                        { 110, "パラレルブレーン" },
                        { 111, "タイムパラドックス" },
                        { 200, "幻符「殺人ドール」" },
                        { 201, "時符「プライベートスクウェア」" },
                        { 202, "傷符「インスクライブレッドソウル」" },
                        { 203, "幻葬「夜霧の幻影殺人鬼」" },
                        { 204, "「咲夜の世界」" },
                        { 205, "傷魂「ソウルスカルプチュア」" },
                        { 206, "銀符「シルバーバウンド」" },
                        { 207, "奇術「エターナルミーク」" },
                        { 208, "速符「ルミネスリコシェ」" },
                        { 209, "時符「咲夜特製ストップウォッチ」" },
                        { 210, "光速「Ｃ．リコシェ」" },
                        { 211, "時符「イマジナリバーチカルタイム」" },
                        { 212, "時計「ルナ・ダイアル」" },
                    }
                },
                {
                    Character.Alice,
                    new Dictionary<short, string>
                    {
                        { 100, "人形操創" },
                        { 101, "人形無操" },
                        { 102, "人形置操" },
                        { 103, "人形振起" },
                        { 104, "人形帰巣" },
                        { 105, "人形火葬" },
                        { 106, "人形千槍" },
                        { 107, "人形ＳＰ" },
                        { 108, "人形伏兵" },
                        { 109, "大江戸爆薬からくり人形" },
                        { 110, "人形弓兵" },
                        { 111, "シーカーワイヤー" },
                        { 200, "魔符「アーティフルサクリファイス」" },
                        { 201, "戦符「リトルレギオン」" },
                        { 202, "咒符「上海人形」" },
                        { 203, "魔操「リターンイナニメトネス」" },
                        { 204, "戦操「ドールズウォー」" },
                        { 205, "咒詛「蓬莱人形」" },
                        { 206, "偵符「シーカードールズ」" },
                        { 207, "紅符「和蘭人形」" },
                        { 208, "人形「未来文楽」" },
                        { 209, "注力「トリップワイヤー」" },
                        { 210, "槍符「キューティ大千槍」" },
                        { 211, "人形「レミングスパレード」" },
                    }
                },
                {
                    Character.Patchouli,
                    new Dictionary<short, string>
                    {
                        { 100, "サマーレッド" },
                        { 101, "ウィンターエレメント" },
                        { 102, "スプリングウィンド" },
                        { 103, "オータムエッジ" },
                        { 104, "ドヨースピア" },
                        { 105, "サマーフレイム" },
                        { 106, "コンデンスドバブル" },
                        { 107, "フラッシュオブスプリング" },
                        { 108, "オータムブレード" },
                        { 109, "エメラルドシティ" },
                        { 110, "ワイプモイスチャー" },
                        { 111, "スティッキーバブル" },
                        { 112, "スタティックグリーン" },
                        { 113, "フォールスラッシャー" },
                        { 114, "ダイアモンドハードネス" },
                        { 200, "火金符「セントエルモピラー」" },
                        { 201, "土水符「ノエキアンデリュージュ」" },
                        { 202, "金木符「エレメンタルハーベスター」" },
                        { 203, "日符「ロイヤルフレア」" },
                        { 204, "月符「サイレントセレナ」" },
                        { 205, "火水木金土符「賢者の石」" },
                        { 206, "水符「ジェリーフィッシュプリンセス」" },
                        { 207, "月木符「サテライトヒマワリ」" },
                        { 210, "日木符「フォトシンセシス」" },
                        { 211, "火水符「フロギスティックピラー」" },
                        { 212, "土金符「エメラルドメガロポリス」" },
                        { 213, "日月符「ロイヤルダイアモンドリング」" },
                    }
                },
                {
                    Character.Youmu,
                    new Dictionary<short, string>
                    {
                        { 100, "反射下界斬" },
                        { 101, "弦月斬" },
                        { 102, "生死流転斬" },
                        { 103, "憑坐の縛" },
                        { 104, "結跏趺斬" },
                        { 105, "折伏無間" },
                        { 106, "心抄斬" },
                        { 107, "悪し魂" },
                        { 108, "燐気斬" },
                        { 109, "炯眼剣" },
                        { 110, "頭上花剪斬" },
                        { 111, "奇び半身" },
                        { 200, "人符「現世斬」" },
                        { 201, "断命剣「冥想斬」" },
                        { 202, "魂符「幽明の苦輪」" },
                        { 203, "人鬼「未来永劫斬」" },
                        { 204, "断迷剣「迷津慈航斬」" },
                        { 205, "魂魄「幽明求聞持聡明の法」" },
                        { 206, "剣伎「桜花閃々」" },
                        { 207, "断霊剣「成仏得脱斬」" },
                        { 208, "空観剣「六根清浄斬」" },
                        { 212, "転生剣「円心流転斬」" },
                    }
                },
                {
                    Character.Remilia,
                    new Dictionary<short, string>
                    {
                        { 100, "デーモンロードウォーク" },
                        { 101, "サーヴァントフライヤー" },
                        { 102, "デーモンロードクレイドル" },
                        { 103, "デーモンロードアロー" },
                        { 104, "ヴァンパイアクロウ" },
                        { 105, "チェーンギャング" },
                        { 106, "ロケットキックアップ" },
                        { 107, "シーリングフィア" },
                        { 108, "トリックスターデビル" },
                        { 109, "デモンズディナーフォーク" },
                        { 110, "バンパイアキス" },
                        { 111, "スティグマナイザー" },
                        { 200, "紅符「不夜城レッド」" },
                        { 201, "必殺「ハートブレイク」" },
                        { 202, "夜符「デーモンキングクレイドル」" },
                        { 203, "紅魔「スカーレットデビル」" },
                        { 204, "神槍「スピア・ザ・グングニル」" },
                        { 205, "夜王「ドラキュラクレイドル」" },
                        { 206, "夜符「バッドレディスクランブル」" },
                        { 207, "運命「ミゼラブルフェイト」" },
                        { 208, "「ミレニアムの吸血鬼」" },
                        { 209, "悪魔「レミリアストレッチ」" },
                    }
                },
                {
                    Character.Yuyuko,
                    new Dictionary<short, string>
                    {
                        { 100, "幽胡蝶" },
                        { 101, "未生の光" },
                        { 102, "悉皆彷徨" },
                        { 103, "胡蝶夢の舞" },
                        { 104, "好死の霊" },
                        { 105, "鳳蝶紋の槍" },
                        { 106, "誘霊の甘蜜" },
                        { 107, "逆さ屏風" },
                        { 108, "スフィアブルーム" },
                        { 109, "死還の大地" },
                        { 110, "故人のお届け物" },
                        { 111, "センスオブエレガンス" },
                        { 200, "死符「ギャストリドリーム」" },
                        { 201, "冥符「黄泉平坂行路」" },
                        { 202, "霊符「无寿の夢」" },
                        { 203, "死蝶「華胥の永眠」" },
                        { 204, "再迷「幻想郷の黄泉還り」" },
                        { 205, "寿命「无寿国への約束手形」" },
                        { 206, "霊蝶「蝶の羽風生に暫く」" },
                        { 207, "蝶符「鳳蝶紋の死槍」" },
                        { 208, "幽雅「死出の誘蛾灯」" },
                        { 209, "桜符「センスオブチェリーブロッサム」" },
                        { 219, "「反魂蝶」" },
                    }
                },
                {
                    Character.Yukari,
                    new Dictionary<short, string>
                    {
                        { 100, "開けて悔しき玉手箱" },
                        { 101, "禅寺に潜む妖蝶" },
                        { 102, "枕石漱流" },
                        { 103, "幻想狂想穴" },
                        { 104, "至る処に青山あり" },
                        { 105, "幻想卍傘" },
                        { 106, "物質と反物質の宇宙" },
                        { 107, "肉体分解機" },
                        { 108, "魅惑のエサ" },
                        { 109, "知能と脚の境界" },
                        { 110, "変容を見る眼" },
                        { 111, "キマイラの翼" },
                        { 200, "境符「四重結界」" },
                        { 201, "式神「八雲藍」" },
                        { 202, "境符「二次元と三次元の境界」" },
                        { 203, "結界「魅力的な四重結界」" },
                        { 204, "式神「橙」" },
                        { 205, "結界「客観結界」" },
                        { 206, "幻巣「飛光虫ネスト」" },
                        { 207, "空餌「中毒性のあるエサ」" },
                        { 208, "魔眼「ラプラスの魔」" },
                        { 215, "廃線「ぶらり廃駅下車の旅」" },
                    }
                },
                {
                    Character.Suika,
                    new Dictionary<short, string>
                    {
                        { 100, "妖鬼-密-" },
                        { 101, "地霊-密-" },
                        { 102, "妖鬼-疎-" },
                        { 103, "萃鬼" },
                        { 104, "元鬼玉" },
                        { 105, "地霊-疎-" },
                        { 106, "厭霧" },
                        { 107, "疎鬼" },
                        { 108, "踏鞴" },
                        { 109, "火鬼" },
                        { 110, "鬼神燐火術" },
                        { 111, "攫鬼" },
                        { 200, "萃符「戸隠山投げ」" },
                        { 201, "酔神「鬼縛りの術」" },
                        { 202, "鬼符「ミッシングパワー」" },
                        { 203, "萃鬼「天手力男投げ」" },
                        { 204, "酔夢「施餓鬼縛りの術」" },
                        { 205, "鬼神「ミッシングパープルパワー」" },
                        { 206, "霧符「雲集霧散」" },
                        { 207, "鬼火「超高密度燐禍術」" },
                        { 208, "鬼符「大江山悉皆殺し」" },
                        { 212, "四天王奥義「三歩壊廃」" },
                    }
                },
                {
                    Character.Reisen,
                    new Dictionary<short, string>
                    {
                        { 100, "マインドエクスプロージョン" },
                        { 101, "イリュージョナリィブラスト" },
                        { 102, "フィールドウルトラレッド" },
                        { 103, "ディスビリーフアスペクト" },
                        { 104, "マインドベンディング" },
                        { 105, "アイサイトクリーニング" },
                        { 106, "フィールドウルトラバイオレット" },
                        { 107, "ディスオーダーアイ" },
                        { 108, "マインドドロッピング" },
                        { 109, "リップルヴィジョン" },
                        { 110, "アンダーセンスブレイク" },
                        { 111, "アキュラースペクトル" },
                        { 200, "惑視「離円花冠(カローラヴィジョン)」" },
                        { 202, "幻爆「近眼花火(マインドスターマイン)」" },
                        { 203, "幻惑「花冠視線(クラウンヴィジョン)」" },
                        { 204, "赤眼「望遠円月(ルナティックブラスト)」" },
                        { 205, "「幻朧月睨(ルナティックレッドアイズ)」" },
                        { 206, "弱心「喪心喪意(ディモチヴェイション)」" },
                        { 207, "喪心「喪心創痍(ディスカーダー)」" },
                        { 208, "毒煙幕「瓦斯織物の玉」" },
                        { 209, "生薬「国士無双の薬」" },
                        { 210, "短視「超短脳波(エックスウェイブ)」" },
                        { 211, "長視「赤月下(インフレアドムーン)」" },
                    }
                },
                {
                    Character.Aya,
                    new Dictionary<short, string>
                    {
                        { 100, "疾風扇" },
                        { 101, "疾走風靡" },
                        { 102, "天狗の立風露" },
                        { 103, "暗夜の礫" },
                        { 104, "烈風扇" },
                        { 105, "疾走優美" },
                        { 106, "天狗のダウンバースト" },
                        { 107, "鎌風ベーリング" },
                        { 108, "楓扇風" },
                        { 109, "天狗ナメシ" },
                        { 110, "天狗の太鼓" },
                        { 111, "天狗礫" },
                        { 200, "旋符「紅葉扇風」" },
                        { 201, "竜巻「天孫降臨の道しるべ」" },
                        { 202, "逆風「人間禁制の道」" },
                        { 203, "突符「天狗のマクロバースト」" },
                        { 205, "風符「天狗道の開風」" },
                        { 206, "「幻想風靡」" },
                        { 207, "風符「天狗報即日限」" },
                        { 208, "鴉符「暗夜のデイメア」" },
                        { 211, "魔獣「鎌鼬ベーリング」" },
                        { 212, "突風「猿田彦の先導」" },
                    }
                },
                {
                    Character.Komachi,
                    new Dictionary<short, string>
                    {
                        { 100, "迷わず生きた人霊" },
                        { 101, "浮かばれない地縛霊" },
                        { 102, "脱魂の儀" },
                        { 103, "怠惰に生きた浮遊霊" },
                        { 104, "死神の大鎌" },
                        { 105, "死出の風" },
                        { 106, "無間の道" },
                        { 107, "寂しがり屋の緊縛霊" },
                        { 108, "通りすがりの人霊" },
                        { 109, "三途の舟" },
                        { 110, "お迎え体験版" },
                        { 111, "離魂の鎌" },
                        { 200, "舟符「河の流れのように」" },
                        { 201, "薄命「余命幾許も無し」" },
                        { 202, "霊符「何処にでもいる浮遊霊」" },
                        { 203, "死歌「八重霧の渡し」" },
                        { 204, "換命「不惜身命、可惜身命」" },
                        { 205, "恨符「未練がましい緊縛霊」" },
                        { 206, "死符「死者選別の鎌」" },
                        { 207, "魂符「生魂流離の鎌」" },
                        { 211, "地獄「無間の狭間」" },
                    }
                },
                {
                    Character.Iku,
                    new Dictionary<short, string>
                    {
                        { 100, "龍魚の一撃" },
                        { 101, "羽衣は水の如く" },
                        { 102, "龍魚の怒り" },
                        { 103, "静電誘導弾" },
                        { 104, "龍神の一撃" },
                        { 105, "羽衣は風の如く" },
                        { 106, "龍神の怒り" },
                        { 107, "龍神の稲光り" },
                        { 108, "水得の龍魚" },
                        { 109, "天女の一撃" },
                        { 110, "龍神の髭" },
                        { 111, "龍の眼" },
                        { 200, "電符「雷鼓弾」" },
                        { 201, "魚符「龍魚ドリル」" },
                        { 202, "雷符「エレキテルの龍宮」" },
                        { 203, "光星「光龍の吐息」" },
                        { 206, "雷魚「雷雲魚遊泳弾」" },
                        { 207, "羽衣「羽衣は空の如く」" },
                        { 208, "羽衣「羽衣は時の如く」" },
                        { 209, "棘符「雷雲棘魚」" },
                        { 210, "龍魚「龍宮の使い遊泳弾」" },
                        { 211, "珠符「五爪龍の珠」" },
                    }
                },
                {
                    Character.Tenshi,
                    new Dictionary<short, string>
                    {
                        { 100, "坤儀の剣" },
                        { 101, "天罰の石柱" },
                        { 102, "非想の威光" },
                        { 103, "非想の剣" },
                        { 104, "六震-相-" },
                        { 105, "守りの要" },
                        { 106, "天地プレス" },
                        { 107, "緋想の剣" },
                        { 108, "因果の剣" },
                        { 109, "地精の起床" },
                        { 110, "緋想の剣気" },
                        { 111, "昇天突" },
                        { 200, "地符「不譲土壌の剣」" },
                        { 201, "非想「非想非非想の剣」" },
                        { 202, "天符「天道是非の剣」" },
                        { 203, "地震「先憂後楽の剣」" },
                        { 204, "気符「天啓気象の剣」" },
                        { 205, "要石「天地開闢プレス」" },
                        { 206, "気符「無念無想の境地」" },
                        { 207, "「全人類の緋想天」" },
                        { 208, "剣技「気炎万丈の剣」" },
                        { 209, "天気「緋想天促」" },
                    }
                },
                {
                    Character.Sanae,
                    new Dictionary<short, string>
                    {
                        { 100, "風起こし" },
                        { 101, "おみくじ爆弾" },
                        { 102, "乾神招来　突" },
                        { 103, "坤神招来　盾" },
                        { 104, "波起こし" },
                        { 105, "スカイサーペント" },
                        { 106, "乾神招来　風" },
                        { 107, "坤神招来　鉄輪" },
                        { 108, "星落とし" },
                        { 109, "コバルトスプレッド" },
                        { 110, "乾神招来　御柱" },
                        { 111, "坤神招来　罠" },
                        { 200, "祈願「商売繁盛守り」" },
                        { 201, "秘術「グレイソーマタージ」" },
                        { 202, "秘術「忘却の祭儀」" },
                        { 203, "神籤「乱れおみくじ連続引き」" },
                        { 204, "開海「海が割れる日」" },
                        { 205, "開海「モーゼの奇跡」" },
                        { 206, "奇跡「白昼の客星」" },
                        { 207, "奇跡「客星の明るすぎる夜」" },
                        { 210, "秘法「九字刺し」" },
                    }
                },
                {
                    Character.Cirno,
                    new Dictionary<short, string>
                    {
                        { 100, "アイシクルシュート" },
                        { 101, "真夏のスノーマン" },
                        { 102, "リトルアイスバーグ" },
                        { 103, "フリーズタッチミー" },
                        { 104, "フロストピラーズ" },
                        { 105, "アイスチャージ" },
                        { 106, "アイシクルボム" },
                        { 107, "アイシクルライズ" },
                        { 108, "冷凍光線" },
                        { 109, "アイスキック" },
                        { 110, "フローズン冷凍法" },
                        { 111, "アイシクルソード" },
                        { 200, "氷符「アイシクルマシンガン」" },
                        { 201, "霜符「フロストコラムス」" },
                        { 202, "氷塊「コールドスプリンクラー」" },
                        { 203, "冷体「スーパーアイスキック」" },
                        { 204, "凍符「パーフェクトフリーズ」" },
                        { 205, "氷符「フェアリースピン」" },
                        { 206, "吹氷「アイストルネード」" },
                        { 207, "氷符「ソードフリーザー」" },
                        { 208, "氷塊「グレートクラッシャー」" },
                        { 210, "凍符「フリーズアトモスフェア」" },
                        { 213, "冷符「瞬間冷凍ビーム」" },
                    }
                },
                {
                    Character.Meiling,
                    new Dictionary<short, string>
                    {
                        { 100, "螺光歩" },
                        { 101, "紅砲" },
                        { 102, "黄震脚" },
                        { 103, "芳波" },
                        { 104, "烈虹拳" },
                        { 105, "紅寸剄" },
                        { 106, "地龍波" },
                        { 107, "水形太極拳" },
                        { 108, "降華蹴" },
                        { 109, "虎剄" },
                        { 110, "天龍脚" },
                        { 111, "彩雨" },
                        { 200, "彩符「彩光風鈴」" },
                        { 201, "極彩「彩光乱舞」" },
                        { 202, "気符「星脈弾」" },
                        { 203, "星気「星脈地転弾」" },
                        { 204, "撃符「大鵬拳」" },
                        { 205, "熾撃「大鵬墜撃拳」" },
                        { 206, "虹符「烈虹真拳」" },
                        { 207, "気符「地龍天龍脚」" },
                        { 208, "彩華「虹色太極拳」" },
                        { 209, "華符「彩光蓮華掌」" },
                        { 211, "気符「猛虎内剄」" },
                    }
                },
                {
                    Character.Utsuho,
                    new Dictionary<short, string>
                    {
                        { 100, "フレアアップ" },
                        { 101, "グラウンドメルト" },
                        { 102, "ブレイクサン" },
                        { 103, "シューティングスター" },
                        { 104, "ロケットダイブ" },
                        { 105, "ヘルゲイザー" },
                        { 106, "シューティングサン" },
                        { 107, "レトロ原子核モデル" },
                        { 108, "メルティング浴びせ蹴り" },
                        { 109, "レイディアントブレード" },
                        { 110, "地獄波動砲" },
                        { 111, "核熱の怨霊" },
                        { 200, "爆符「メガフレア」" },
                        { 201, "爆符「ギガフレア」" },
                        { 203, "焔星「フィクストスター」" },
                        { 204, "焔星「十凶星」" },
                        { 205, "核符「クリーピングサン」" },
                        { 206, "「地獄の人工太陽」" },
                        { 207, "地熱「核ブレイズゲイザー」" },
                        { 208, "光熱「ハイテンションブレード」" },
                        { 209, "鴉符「八咫烏ダイブ」" },
                        { 210, "核熱「核反応制御不能ダイブ」" },
                        { 211, "制御「セルフトカマク」" },
                        { 212, "「サブタレイニアンサン」" },
                        { 213, "遮光「核熱バイザー」" },
                        { 214, "「アビスノヴァ」" },
                    }
                },
                {
                    Character.Suwako,
                    new Dictionary<short, string>
                    {
                        { 100, "古の間欠泉" },
                        { 101, "大蝦蟇神" },
                        { 102, "大地の湖" },
                        { 103, "土着神の祟り" },
                        { 104, "水蛙神" },
                        { 105, "古の鉄輪" },
                        { 106, "古代翡翠" },
                        { 107, "祟られた大地" },
                        { 108, "雨を呼ぶ雨蛙" },
                        { 109, "手長足長さん" },
                        { 110, "蛙石神" },
                        { 111, "ミシャグジさまの祟り" },
                        { 200, "土着神「洩矢神」" },
                        { 201, "源符「諏訪清水」" },
                        { 202, "開宴「二拝二拍一拝」" },
                        { 203, "土着神「ケロちゃん風雨に負けず」" },
                        { 204, "神具「洩矢の鉄の輪」" },
                        { 205, "源符「厭い川の翡翠」" },
                        { 206, "蛙狩「蛙は口ゆえ蛇に呑まるる」" },
                        { 207, "土着神「手長足長さま」" },
                        { 208, "祟り神「赤口（ミシャグチ）さま」" },
                        { 209, "土着神「宝永四年の赤蛙」" },
                        { 212, "蛙休「オールウェイズ冬眠できます」" },
                    }
                },
            };

        private Info info;

        public ReplayData()
        {
            this.info = new Info();
        }

        public int StageId
        {
            get { return (int)this.info.Stage; }
        }

        public string StageName
        {
            get
            {
                return StageNames.TryGetValue(this.info.Stage, out string name) ? name : string.Empty;
            }
        }

        public int BgmId
        {
            get { return (int)this.info.Bgm; }
        }

        public string BgmName
        {
            get
            {
                return BgmNames.TryGetValue(this.info.Bgm, out string name) ? name : string.Empty;
            }
        }

        public GameMode GameMode
        {
            get { return this.info.GameMode; }
        }

        public Character Character1
        {
            get { return this.info.Deck1.Character; }
        }

        public int Color1
        {
            get { return (int)this.info.Deck1.Color; }
        }

        public string Player1Info
        {
            get { return this.GetPlayerInfo(0); }
        }

        public Character Character2
        {
            get { return this.info.Deck2.Character; }
        }

        public int Color2
        {
            get { return (int)this.info.Deck2.Color; }
        }

        public string Player2Info
        {
            get { return this.GetPlayerInfo(1); }
        }

        public string Version
        {
            get
            {
                return Versions.TryGetValue(this.info.Version, out string version) ? version : string.Empty;
            }
        }

        public static bool IsValidVersion(short versionId)
        {
            return Versions.ContainsKey(versionId);
        }

        public void Read(Stream input)
        {
            using (var reader = new BinaryReader(input, Enc.UTF8NoBOM, true))
            {
                this.info.ReadFrom(reader);
            }
        }

        private string GetPlayerInfo(int index)
        {
            var playerInfo = string.Empty;

            if ((index == 0) || (index == 1))
            {
                var deck = (index == 0) ? this.info.Deck1 : this.info.Deck2;

                var cards = string.Join(
                    Environment.NewLine,
                    deck.Cards.OrderBy(pair => pair.Key).Select(pair =>
                    {
                        var cardNames = SystemCardNames.ContainsKey(pair.Key)
                            ? SystemCardNames : CardNames[deck.Character];
                        return cardNames.TryGetValue(pair.Key, out string name)
                            ? string.Format(CultureInfo.CurrentCulture, "{0} * {1}", name, pair.Value)
                            : string.Empty;
                    }));

                var format = string.Join(
                    Environment.NewLine,
                    "Player {0}",
                    "Character: {1}",
                    "Color: {2}",
                    cards);

                playerInfo = string.Format(
                    CultureInfo.CurrentCulture,
                    format,
                    index + 1,
                    deck.Character.ToLongName(),
                    deck.Color + 1);
            }

            return playerInfo;
        }

        private sealed class Deck
        {
            private Dictionary<short, int> cards;

            public Deck()
            {
                this.cards = new Dictionary<short, int>();
            }

            public Character Character { get; private set; }

            public byte Color { get; private set; }

            public int NumCards { get; private set; }

            public ReadOnlyCollection<KeyValuePair<short, int>> Cards
            {
                get { return Array.AsReadOnly(this.cards.ToArray()); }
            }

            public void ReadFrom(BinaryReader reader)
            {
                if (reader != null)
                {
                    this.Character = ((int)reader.ReadByte()).ToValidEnum<Character>();
                    this.Color = reader.ReadByte();
                    this.NumCards = reader.ReadInt32();

                    this.cards.Clear();
                    for (var count = 0; count < this.NumCards; count++)
                    {
                        var card = reader.ReadInt16();
                        if (this.cards.ContainsKey(card))
                        {
                            this.cards[card]++;
                        }
                        else
                        {
                            this.cards.Add(card, 1);
                        }
                    }
                }
            }
        }

        private sealed class Info
        {
            public short Version { get; private set; }

            public GameMode GameMode { get; private set; }

            public byte Stage { get; private set; }

            public byte Bgm { get; private set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
            public int Frames { get; private set; }

            public Deck Deck1 { get; private set; }

            public Deck Deck2 { get; private set; }

            public void ReadFrom(BinaryReader reader)
            {
                if (reader != null)
                {
                    this.Version = reader.ReadInt16();
                    reader.ReadBytes(6);

                    this.GameMode = ((int)reader.ReadByte()).ToValidEnum<GameMode>();
                    reader.ReadBytes(5);

                    this.Deck1 = new Deck();
                    this.Deck1.ReadFrom(reader);
                    reader.ReadBytes(3);

                    if (this.GameMode == GameMode.Story)
                    {
                        this.Deck2 = null;
                        this.Stage = byte.MaxValue;
                        this.Bgm = byte.MaxValue;
                        this.Frames = -1;
                    }
                    else
                    {
                        this.Deck2 = new Deck();
                        this.Deck2.ReadFrom(reader);
                        reader.ReadBytes(4);

                        this.Stage = reader.ReadByte();
                        this.Bgm = reader.ReadByte();
                        reader.ReadBytes(7);

                        var remainSize = reader.BaseStream.Length - reader.BaseStream.Position;
                        this.Frames = (int)(remainSize / ((this.GameMode == GameMode.VersusCom) ? 4 : 2));
                    }
                }
            }
        }
    }
}
