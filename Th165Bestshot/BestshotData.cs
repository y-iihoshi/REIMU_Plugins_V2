//-----------------------------------------------------------------------
// <copyright file="BestshotData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Th165Bestshot
{
    using System.Collections.Specialized;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using ReimuPlugins.Common;

    public sealed class BestshotData : BestshotDataBase
    {
        private static readonly int[] Masks;

        private readonly BitVector32[] hashtagFields;

        static BestshotData()
        {
            Masks = new int[32];
            Masks[0] = BitVector32.CreateMask();
            for (var i = 1; i < Masks.Length; i++)
            {
                Masks[i] = BitVector32.CreateMask(Masks[i - 1]);
            }
        }

        public BestshotData()
        {
            this.Signature = string.Empty;
            this.Weekday = 0;
            this.Dream = 0;
            this.Width = 0;
            this.Height = 0;
            this.Width2 = 0;
            this.Height2 = 0;
            this.HalfWidth = 0;
            this.HalfHeight = 0;
            this.SlowRate = 0;
            this.DateTime = 0;
            this.Score = 0;
            this.hashtagFields = new BitVector32[3];
            this.Score2 = 0;
            this.NumViewed = 0;
            this.NumLikes = 0;
            this.NumFavs = 0;
            this.Bitmap = null;
        }

        public string Signature { get; private set; }

        public short Weekday { get; private set; }

        public short Dream { get; private set; }

        public short Width { get; private set; }

        public short Height { get; private set; }

        public short Width2 { get; private set; }

        public short Height2 { get; private set; }

        public short HalfWidth { get; private set; }

        public short HalfHeight { get; private set; }

        public float SlowRate { get; private set; }

        public uint DateTime { get; private set; }

        public float Angle { get; private set; } // -PI .. +PI [rad]

        public int Score { get; private set; }

        // 0x0000_0001: (logical OR of bit 1, 2, 3?)
        // 0x0000_0002: 敵が見切れてる
        // 0x0000_0004: 敵を収めたよ
        // 0x0000_0008: 敵がど真ん中
        // 0x0000_0010: 自撮り！
        // 0x0000_0020: ツーショット！
        // 0x0000_0040: (always 0b0?)
        // 0x0000_0080: ちょっと危なかった
        // 0x0000_0100: マジで危なかった
        // 0x0000_0200: 死ぬかと思った
        // 0x0000_0400: 赤色多いな
        // 0x0000_0800: 紫色多いね
        // 0x0000_1000: 青色多いよ
        // 0x0000_2000: 水色多いし
        // 0x0000_4000: 緑色多いねぇ
        // 0x0000_8000: 黄色多いなぁ
        // 0x0001_0000: 橙色多いお
        // 0x0002_0000: カラフル過ぎｗ
        // 0x0004_0000: 七色全部揃った！
        // 0x0008_0000: (NumBullets == 0)
        // 0x0010_0000: (always 0b0?)
        // 0x0020_0000: 風景写真
        // 0x03C0_0000: (always 0b1111?)
        // 0x0400_0000: 接写！
        // 0x0800_0000: かなりの接写！
        // 0x1000_0000: 近すぎてぶつかるー！
        // 0x2000_0000: (always equal to 0x0000_0001?)
        // 0xC000_0000: (always 0b00?)
        public int Hashtags1 => this.hashtagFields[0].Data;

        public bool EnemyIsInFrame => this.hashtagFields[0][Masks[0]];  // Not used

        public bool EnemyIsPartlyInFrame => this.hashtagFields[0][Masks[1]];

        public bool WholeEnemyIsInFrame => this.hashtagFields[0][Masks[2]];

        public bool EnemyIsInMiddle => this.hashtagFields[0][Masks[3]];

        public bool IsSelfie => this.hashtagFields[0][Masks[4]];

        public bool IsTwoShot => this.hashtagFields[0][Masks[5]];

        public bool BitDangerous => this.hashtagFields[0][Masks[7]];

        public bool SeriouslyDangerous => this.hashtagFields[0][Masks[8]];

        public bool ThoughtGonnaDie => this.hashtagFields[0][Masks[9]];

        public bool ManyReds => this.hashtagFields[0][Masks[10]];

        public bool ManyPurples => this.hashtagFields[0][Masks[11]];

        public bool ManyBlues => this.hashtagFields[0][Masks[12]];

        public bool ManyCyans => this.hashtagFields[0][Masks[13]];

        public bool ManyGreens => this.hashtagFields[0][Masks[14]];

        public bool ManyYellows => this.hashtagFields[0][Masks[15]];

        public bool ManyOranges => this.hashtagFields[0][Masks[16]];

        public bool TooColorful => this.hashtagFields[0][Masks[17]];

        public bool SevenColors => this.hashtagFields[0][Masks[18]];

        public bool NoBullet => this.hashtagFields[0][Masks[19]];  // Not used

        public bool IsLandscapePhoto => this.hashtagFields[0][Masks[21]];

        public bool Closeup => this.hashtagFields[0][Masks[26]];

        public bool QuiteCloseup => this.hashtagFields[0][Masks[27]];

        public bool TooClose => this.hashtagFields[0][Masks[28]];

        // 0x0000_0001: (always 0b0?)
        // 0x0000_0002: 敵が丸見えｗ
        // 0x0000_000C: (always 0b00?)
        // 0x0000_0010: 弾多すぎｗ
        // 0x0000_0020: 弾幕ふざけすぎｗｗ
        // 0x0000_0040: (ちょっ、密度濃すぎｗｗｗ)
        // 0x0000_0080: 追い打ちしたよ！
        // 0x0000_0100: 座薬ｗｗｗ
        // 0x0000_0200: 蛾みたいな蝶だ！
        // 0x0000_0400: 敵は無傷だ
        // 0x0000_0800: 敵はまだ余裕がある
        // 0x0000_1000: 敵がだいぶ弱ってる
        // 0x0000_2000: 敵が瀕死だ
        // 0x0000_4000: トドメをさしたよ！
        // 0x0000_8000: スリーショット！
        // 0x0001_0000: 二人まとめて撮影した！
        // 0x0002_0000: 二人が重なってるｗｗ
        // 0x0004_0000: 並んでピース
        // 0x0008_0000: (二人が近すぎるｗ)
        // 0x0010_0000: アチチ、焦げちゃうよ
        // 0x0020_0000: 弾、大きすぎでしょｗ
        // 0x0040_0000: 刃物投げんな
        // 0x0080_0000: うねうねだー！
        // 0x0100_0000: 光が止まって見える！
        // 0x0200_0000: スーパームーン！
        // 0x0400_0000: うぉっ、まぶし！
        // 0x0800_0000: ぐあ、眩しすぎるー！
        // 0x1000_0000: うあー、目が、目がー！
        // 0x2000_0000: 二人まとめてトドメ！
        // 0x4000_0000: (はっ！夢だったか)
        // 0x8000_0000: 岩の弾幕とかｗｗ
        public int Hashtags2 => this.hashtagFields[1].Data;

        public bool EnemyIsInFullView => this.hashtagFields[1][Masks[1]];

        public bool TooManyBullets => this.hashtagFields[1][Masks[4]];

        public bool TooPlayfulBarrage => this.hashtagFields[1][Masks[5]];

        public bool TooDense => this.hashtagFields[1][Masks[6]]; // FIXME

        public bool Chased => this.hashtagFields[1][Masks[7]];

        public bool IsSuppository => this.hashtagFields[1][Masks[8]];

        public bool IsButterflyLikeMoth => this.hashtagFields[1][Masks[9]];

        public bool EnemyIsUndamaged => this.hashtagFields[1][Masks[10]];

        public bool EnemyCanAfford => this.hashtagFields[1][Masks[11]];

        public bool EnemyIsWeakened => this.hashtagFields[1][Masks[12]];

        public bool EnemyIsDying => this.hashtagFields[1][Masks[13]];

        public bool Finished => this.hashtagFields[1][Masks[14]];

        public bool IsThreeShot => this.hashtagFields[1][Masks[15]];

        public bool TwoEnemiesTogether => this.hashtagFields[1][Masks[16]];

        public bool EnemiesAreOverlapping => this.hashtagFields[1][Masks[17]];

        public bool PeaceSignAlongside => this.hashtagFields[1][Masks[18]];

        public bool EnemiesAreTooClose => this.hashtagFields[1][Masks[19]]; // FIXME

        public bool Scorching => this.hashtagFields[1][Masks[20]];

        public bool TooBigBullet => this.hashtagFields[1][Masks[21]];

        public bool ThrowingEdgedTools => this.hashtagFields[1][Masks[22]];

        public bool Snaky => this.hashtagFields[1][Masks[23]];

        public bool LightLooksStopped => this.hashtagFields[1][Masks[24]];

        public bool IsSuperMoon => this.hashtagFields[1][Masks[25]];

        public bool Dazzling => this.hashtagFields[1][Masks[26]];

        public bool MoreDazzling => this.hashtagFields[1][Masks[27]];

        public bool MostDazzling => this.hashtagFields[1][Masks[28]];

        public bool FinishedTogether => this.hashtagFields[1][Masks[29]];

        public bool WasDream => this.hashtagFields[1][Masks[30]]; // FIXME; Not used

        public bool IsRockyBarrage => this.hashtagFields[1][Masks[31]];

        // 0x0000_0001: 弾幕を破壊する棒……？
        // 0x0000_0002: もふもふもふー
        // 0x0000_0004: わんわん写真
        // 0x0000_0008: アニマルフォト
        // 0x0000_0010: 動物園！
        // 0x0000_0020: (ラブリーハート！)
        // 0x0000_0040: ぎゃー、雷はスマホがー
        // 0x0000_0080: ドンドコドンドコ
        // 0x0000_0100: (身体が霧状に！？)
        // 0x0000_0200: 何ともつまらない写真
        // 0x0000_0400: (怒られちゃった……)
        // 0x0000_0800: 私こそが宇佐見菫子だ！
        // 0xFFFF_F000: (always all 0?)
        public int Hashtags3 => this.hashtagFields[2].Data;

        public bool IsStickDestroyingBarrage => this.hashtagFields[2][Masks[0]];

        public bool Fluffy => this.hashtagFields[2][Masks[1]];

        public bool IsDoggiePhoto => this.hashtagFields[2][Masks[2]];

        public bool IsAnimalPhoto => this.hashtagFields[2][Masks[3]];

        public bool IsZoo => this.hashtagFields[2][Masks[4]];

        public bool IsLovelyHeart => this.hashtagFields[2][Masks[5]]; // FIXME

        public bool IsThunder => this.hashtagFields[2][Masks[6]];

        public bool IsDrum => this.hashtagFields[2][Masks[7]];

        public bool IsMisty => this.hashtagFields[2][Masks[8]]; // FIXME

        public bool IsBoringPhoto => this.hashtagFields[2][Masks[9]];

        public bool WasScolded => this.hashtagFields[2][Masks[10]]; // FIXME

        public bool IsSumireko => this.hashtagFields[2][Masks[11]];

        public int Score2 { get; private set; }

        public int BasePoint { get; private set; } // FIXME

        public int NumViewed { get; private set; }

        public int NumLikes { get; private set; }

        public int NumFavs { get; private set; }

        public int NumBullets { get; private set; }

        public int NumBulletsNearby { get; private set; }

        public int RiskBonus { get; private set; } // max(NumBulletsNearby, 2) * 40 .. min(NumBulletsNearby, 25) * 40

        public float BossShot { get; private set; } // 1.20? .. 2.00

        public float AngleBonus { get; private set; } // 1.00? .. 1.30

        public int MacroBonus { get; private set; } // 0 .. 60?

        public float LikesPerView { get; private set; }

        public float FavsPerView { get; private set; }

        public int NumHashtags { get; private set; }

        public int NumRedBullets { get; private set; }

        public int NumPurpleBullets { get; private set; }

        public int NumBlueBullets { get; private set; }

        public int NumCyanBullets { get; private set; }

        public int NumGreenBullets { get; private set; }

        public int NumYellowBullets { get; private set; }

        public int NumOrangeBullets { get; private set; }

        public int NumLightBullets { get; private set; }

        public Bitmap Bitmap { get; private set; }

        public override void Read(Stream input, bool withBitmap)
        {
            using var reader = new BinaryReader(input);

            this.Signature = Encoding.CP932.GetString(reader.ReadBytes(4));
            _ = reader.ReadInt16(); // always 0x0401?
            this.Weekday = reader.ReadInt16();
            this.Dream = reader.ReadInt16();
            _ = reader.ReadInt16(); // always 0x0100?
            this.Width = reader.ReadInt16();
            this.Height = reader.ReadInt16();
            _ = reader.ReadInt32(); // always 0?
            this.Width2 = reader.ReadInt16();
            this.Height2 = reader.ReadInt16();
            this.HalfWidth = reader.ReadInt16();
            this.HalfHeight = reader.ReadInt16();
            _ = reader.ReadInt32(); // always 0?
            this.SlowRate = reader.ReadSingle();
            this.DateTime = reader.ReadUInt32();
            _ = reader.ReadInt32(); // always 0?
            this.Angle = reader.ReadSingle();
            this.Score = reader.ReadInt32();
            _ = reader.ReadInt32(); // always 0?
            this.hashtagFields[0] = new BitVector32(reader.ReadInt32());
            this.hashtagFields[1] = new BitVector32(reader.ReadInt32());
            this.hashtagFields[2] = new BitVector32(reader.ReadInt32());
            _ = reader.ReadBytes(0x28); // always all 0?
            this.Score2 = reader.ReadInt32();
            this.BasePoint = reader.ReadInt32();
            this.NumViewed = reader.ReadInt32();
            this.NumLikes = reader.ReadInt32();
            this.NumFavs = reader.ReadInt32();
            this.NumBullets = reader.ReadInt32();
            this.NumBulletsNearby = reader.ReadInt32();
            this.RiskBonus = reader.ReadInt32();
            this.BossShot = reader.ReadSingle();
            _ = reader.ReadInt32(); // always 0? (Nice Shot?)
            this.AngleBonus = reader.ReadSingle();
            this.MacroBonus = reader.ReadInt32();
            _ = reader.ReadInt32(); // always 0? (Front/Side/Back Shot?)
            _ = reader.ReadInt32(); // always 0? (Clear Shot?)
            this.LikesPerView = reader.ReadSingle();
            this.FavsPerView = reader.ReadSingle();
            this.NumHashtags = reader.ReadInt32();
            this.NumRedBullets = reader.ReadInt32();
            this.NumPurpleBullets = reader.ReadInt32();
            this.NumBlueBullets = reader.ReadInt32();
            this.NumCyanBullets = reader.ReadInt32();
            this.NumGreenBullets = reader.ReadInt32();
            this.NumYellowBullets = reader.ReadInt32();
            this.NumOrangeBullets = reader.ReadInt32();
            this.NumLightBullets = reader.ReadInt32();
            _ = reader.ReadBytes(0x44); // always all 0?
            _ = reader.ReadBytes(0x34);

            if (withBitmap)
            {
                this.Bitmap = ReadBitmap(input, this.Width, this.Height);
            }
        }

        private static Bitmap ReadBitmap(Stream input, int width, int height)
        {
            using var extracted = new MemoryStream();
            Lzss.Extract(input, extracted);
            _ = extracted.Seek(0, SeekOrigin.Begin);

            using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            using (var locked = new BitmapLock(bitmap, ImageLockMode.WriteOnly))
            {
                var source = extracted.ToArray();
                var destination = locked.Scan0;
                Marshal.Copy(source, 0, destination, source.Length);
            }

            return bitmap.Clone() as Bitmap;
        }
    }
}
