//-----------------------------------------------------------------------
// <copyright file="ReplayData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Th105Replay;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ReimuPlugins.Common;
using ReimuPlugins.Common.Extensions;

public enum GameMode
{
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
#pragma warning disable SA1134 // Attributes should not share line
    [EnumAltName("Story")]                         Story,
    [EnumAltName("Arcade")]                        Arcade,
    [EnumAltName("vs COM")]                        VersusCom,
    [EnumAltName("vs PLAYER")]                     VersusPlayer,
    [EnumAltName("vs NETWORK （ホスト側）")]       VersusNetworkHost,
    [EnumAltName("vs NETWORK （クライアント側）")] VersusNetworkClient,
    [EnumAltName("vs NETWORK （観戦）")]           VersusNetworkWatch,
#pragma warning restore SA1134 // Attributes should not share line
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
}

public enum Character
{
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
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
#pragma warning restore SA1134 // Attributes should not share line
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
}

public sealed class ReplayData
{
    private static readonly IReadOnlyDictionary<short, string> Versions =
        new Dictionary<short, string>
        {
            { 100, "1.00" },
            { 101, "1.01" },
            { 102, "1.02" },
            { 103, "1.03" },
            { 104, "1.04 or 1.05" },
            { 105, "1.05?" },
            { 106, "1.06" },
        };

    private static readonly IReadOnlyDictionary<byte, string> StageNames =
        new Dictionary<byte, string>
        {
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
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
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
        };

    private static readonly IReadOnlyDictionary<byte, string> BgmNames =
        new Dictionary<byte, string>
        {
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
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
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
        };

    private static readonly IReadOnlyDictionary<short, string> SystemCardNames =
        new Dictionary<short, string>
        {
            { 0, "「気質発現」" },
            { 1, "「霊撃」" },
            { 2, "「ガード反撃」" },
            { 3, "「スペル増幅」" },
            { 4, "「体力回復」" },
            { 5, "「霊力回復」" },
        };

    private static readonly IReadOnlyDictionary<Character, IReadOnlyDictionary<short, string>> CardNames =
        new Dictionary<Character, IReadOnlyDictionary<short, string>>
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
                    { 200, "霊符「夢想妙珠」" },
                    { 201, "神霊「夢想封印」" },
                    { 206, "神技「八方鬼縛陣」" },
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
                    { 111, "グリーンスプレッド" },
                    { 200, "恋符「マスタースパーク」" },
                    { 202, "魔砲「ファイナルスパーク」" },
                    { 203, "星符「ドラゴンメテオ」" },
                    { 205, "魔符「スターダストレヴァリエ」" },
                    { 206, "星符「エスケープベロシティ」" },
                    { 207, "彗星「ブレイジングスター」" },
                    { 208, "星符「メテオニックシャワー」" },
                    { 211, "光符「ルミネスストライク」" },
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
                    { 109, "大江戸爆薬からくり人形" },
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
                    { 200, "火金符「セントエルモピラー」" },
                    { 201, "土水符「ノエキアンデリュージュ」" },
                    { 202, "金木符「エレメンタルハーベスター」" },
                    { 203, "日符「ロイヤルフレア」" },
                    { 204, "月符「サイレントセレナ」" },
                    { 205, "火水木金土符「賢者の石」" },
                    { 206, "水符「ジェリーフィッシュプリンセス」" },
                    { 207, "月木符「サテライトヒマワリ」" },
                    { 210, "日木符「フォトシンセシス」" },
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
                    { 109, "炯眼剣" },
                    { 111, "奇び半身" },
                    { 200, "人符「現世斬」" },
                    { 201, "断命剣「冥想斬」" },
                    { 202, "魂符「幽明の苦輪」" },
                    { 203, "人鬼「未来永劫斬」" },
                    { 204, "断迷剣「迷津慈航斬」" },
                    { 205, "魂魄「幽明求聞持聡明の法」" },
                    { 206, "剣伎「桜花閃々」" },
                    { 207, "断霊剣「成仏得脱斬」" },
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
                    { 109, "デモンズディナーフォーク" },
                    { 200, "紅符「不夜城レッド」" },
                    { 201, "必殺「ハートブレイク」" },
                    { 202, "夜符「デーモンキングクレイドル」" },
                    { 203, "紅魔「スカーレットデビル」" },
                    { 204, "神槍「スピア・ザ・グングニル」" },
                    { 205, "夜王「ドラキュラクレイドル」" },
                    { 206, "夜符「バッドレディスクランブル」" },
                    { 207, "運命「ミゼラブルフェイト」" },
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
                    { 200, "死符「ギャストリドリーム」" },
                    { 201, "冥符「黄泉平坂行路」" },
                    { 202, "霊符「无寿の夢」" },
                    { 203, "死蝶「華胥の永眠」" },
                    { 204, "再迷「幻想郷の黄泉還り」" },
                    { 205, "寿命「无寿国への約束手形」" },
                    { 206, "霊蝶「蝶の羽風生に暫く」" },
                    { 207, "蝶符「鳳蝶紋の死槍」" },
                    { 208, "幽雅「死出の誘蛾灯」" },
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
                    { 200, "境符「四重結界」" },
                    { 201, "式神「八雲藍」" },
                    { 202, "境符「二次元と三次元の境界」" },
                    { 203, "結界「魅力的な四重結界」" },
                    { 204, "式神「橙」" },
                    { 205, "結界「客観結界」" },
                    { 206, "幻巣「飛光虫ネスト」" },
                    { 207, "空餌「中毒性のあるエサ」" },
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
                    { 109, "火鬼" },
                    { 200, "萃符「戸隠山投げ」" },
                    { 201, "酔神「鬼縛りの術」" },
                    { 202, "鬼符「ミッシングパワー」" },
                    { 203, "萃鬼「天手力男投げ」" },
                    { 204, "酔夢「施餓鬼縛りの術」" },
                    { 205, "鬼神「ミッシングパープルパワー」" },
                    { 206, "霧符「雲集霧散」" },
                    { 207, "鬼火「超高密度燐禍術」" },
                    { 208, "鬼符「大江山悉皆殺し」" },
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
                    { 200, "惑視「離円花冠(カローラヴィジョン)」" },
                    { 202, "幻爆「近眼花火(マインドスターマイン)」" },
                    { 203, "幻惑「花冠視線(クラウンヴィジョン)」" },
                    { 205, "「幻朧月睨(ルナティックレッドアイズ)」" },
                    { 206, "弱心「喪心喪意(ディモチヴェイション)」" },
                    { 207, "喪心「喪心創痍(ディスカーダー)」" },
                    { 208, "毒煙幕「瓦斯織物の玉」" },
                    { 209, "生薬「国士無双の薬」" },
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
                    { 109, "天狗ナメシ" },
                    { 110, "天狗の太鼓" },
                    { 200, "旋符「紅葉扇風」" },
                    { 201, "竜巻「天孫降臨の道しるべ」" },
                    { 202, "逆風「人間禁制の道」" },
                    { 203, "突符「天狗のマクロバースト」" },
                    { 205, "風符「天狗道の開風」" },
                    { 206, "「幻想風靡」" },
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
                    { 110, "お迎え体験版" },
                    { 200, "舟符「河の流れのように」" },
                    { 201, "薄命「余命幾許も無し」" },
                    { 202, "霊符「何処にでもいる浮遊霊」" },
                    { 203, "死歌「八重霧の渡し」" },
                    { 204, "換命「不惜身命、可惜身命」" },
                    { 205, "恨符「未練がましい緊縛霊」" },
                    { 206, "死符「死者選別の鎌」" },
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
                    { 111, "龍の眼" },
                    { 200, "電符「雷鼓弾」" },
                    { 201, "魚符「龍魚ドリル」" },
                    { 202, "雷符「エレキテルの龍宮」" },
                    { 203, "光星「光龍の吐息」" },
                    { 206, "雷魚「雷雲魚遊泳弾」" },
                    { 207, "羽衣「羽衣は空の如く」" },
                    { 209, "棘符「雷雲棘魚」" },
                    { 210, "龍魚「龍宮の使い遊泳弾」" },
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
                    { 200, "地符「不譲土壌の剣」" },
                    { 201, "非想「非想非非想の剣」" },
                    { 202, "天符「天道是非の剣」" },
                    { 203, "地震「先憂後楽の剣」" },
                    { 204, "気符「天啓気象の剣」" },
                    { 205, "要石「天地開闢プレス」" },
                    { 206, "気符「無念無想の境地」" },
                    { 207, "「全人類の緋想天」" },
                }
            },
        };

    private readonly Info info;

    public ReplayData()
    {
        this.info = new Info();
    }

    public int StageId => this.info.Stage;

    public string StageName => StageNames.TryGetValue(this.info.Stage, out var name) ? name : string.Empty;

    public int BgmId => this.info.Bgm;

    public string BgmName => BgmNames.TryGetValue(this.info.Bgm, out var name) ? name : string.Empty;

    public GameMode GameMode => this.info.GameMode;

    public Character Character1 => this.info.Deck1.Character;

    public int Color1 => this.info.Deck1.Color;

    public string Player1Info => this.GetPlayerInfo(0);

    public Character Character2 => this.info.Deck2.Character;

    public int Color2 => this.info.Deck2.Color;

    public string Player2Info => this.GetPlayerInfo(1);

    public string Version => Versions.TryGetValue(this.info.Version, out var version) ? version : string.Empty;

    public static bool IsValidVersion(short versionId)
    {
        return Versions.ContainsKey(versionId);
    }

    public void Read(Stream input)
    {
        using var reader = new BinaryReader(input, Encoding.UTF8NoBOM, true);
        this.info.ReadFrom(reader);
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
                    return cardNames.TryGetValue(pair.Key, out var name)
                        ? string.Format(CultureInfo.CurrentCulture, "{0} * {1}", name, pair.Value) : string.Empty;
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
        private readonly Dictionary<short, int> cards;

        public Deck()
        {
            this.cards = new Dictionary<short, int>();
        }

        public Character Character { get; private set; }

        public byte Color { get; private set; }

        public int NumCards { get; private set; }

        public IReadOnlyDictionary<short, int> Cards => this.cards;

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

        public int Frames { get; private set; }

        public Deck Deck1 { get; private set; }

        public Deck Deck2 { get; private set; }

        public void ReadFrom(BinaryReader reader)
        {
            if (reader != null)
            {
                this.Version = reader.ReadInt16();
                _ = reader.ReadBytes(6);

                this.GameMode = ((int)reader.ReadByte()).ToValidEnum<GameMode>();
                _ = reader.ReadBytes(5);

                this.Deck1 = new Deck();
                this.Deck1.ReadFrom(reader);
                _ = reader.ReadBytes(3);

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
                    _ = reader.ReadBytes(4);

                    this.Stage = reader.ReadByte();
                    this.Bgm = reader.ReadByte();
                    _ = reader.ReadBytes(7);

                    var remainSize = reader.BaseStream.Length - reader.BaseStream.Position;
                    this.Frames = (int)(remainSize / ((this.GameMode == GameMode.VersusCom) ? 4 : 2));
                }
            }
        }
    }
}
