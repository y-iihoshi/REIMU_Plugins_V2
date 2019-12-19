//-----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Th165Bestshot
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using CommonWin32.Bitmaps;
    using ReimuPlugins.Common;
    using DllExportAttribute = NXPorts.Attributes.ExportAttribute;
    using IO = System.IO;

    public static class Plugin
    {
        private static readonly PluginImpl Impl = new PluginImpl();

        [DllExport(callingConvention: CallingConvention.StdCall)]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "To comply with the REIMU plugin spec.")]
        public static Revision GetPluginRevision()
        {
            return Impl.GetPluginRevision();
        }

        [DllExport(callingConvention: CallingConvention.StdCall)]
        public static int GetPluginInfo(int index, IntPtr info, uint size)
        {
            return Impl.GetPluginInfo(index, info, size);
        }

        [DllExport(callingConvention: CallingConvention.StdCall)]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "To comply with the REIMU plugin spec.")]
        public static ErrorCode GetColumnInfo(out IntPtr info)
        {
            return Impl.GetColumnInfo(out info);
        }

        [DllExport(callingConvention: CallingConvention.StdCall)]
        public static uint IsSupported(IntPtr src, uint size)
        {
            return Impl.IsSupported(src, size);
        }

        [DllExport(callingConvention: CallingConvention.StdCall)]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "To comply with the REIMU plugin spec.")]
        public static ErrorCode GetFileInfoList(IntPtr src, uint size, out IntPtr info)
        {
            return Impl.GetFileInfoList(src, size, out info);
        }

        [DllExport(callingConvention: CallingConvention.StdCall)]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "To comply with the REIMU plugin spec.")]
        public static ErrorCode GetFileInfoText1(IntPtr src, uint size, out IntPtr dst)
        {
            return Impl.GetFileInfoText1(src, size, out dst);
        }

        [DllExport(callingConvention: CallingConvention.StdCall)]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "To comply with the REIMU plugin spec.")]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "To comply with the REIMU plugin spec.")]
        public static ErrorCode GetFileInfoImage2(IntPtr src, uint size, out IntPtr dst, out IntPtr info)
        {
            return Impl.GetFileInfoImage2(src, size, out dst, out info);
        }

        private sealed class PluginImpl : ReimuPluginRev2<PluginImpl.ColumnKey>
        {
            private const string ValidSignature = "BST4";

            private static readonly string[] PluginInfo =
            {
                "REIMU Plug-in For 秘封ナイトメアダイアリー ベストショット Ver2.00 (C) IIHOSHI Yoshinori, 2019\0",
                "秘封ナイトメアダイアリー ベストショット\0",
                "bs*.dat\0",
                "秘封ナイトメアダイアリー ベストショット (bs*.dat)\0",
            };

            private static readonly Dictionary<ColumnKey, ColumnInfo> Columns =
                new Dictionary<ColumnKey, ColumnInfo>
                {
                    {
                        ColumnKey.Filename,
                        new ColumnInfo
                        {
                            Title = "ファイル名\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.Title,
                        }
                    },
                    {
                        ColumnKey.LastWriteDate,
                        new ColumnInfo
                        {
                            Title = "更新日時\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.LastWriteTime,
                        }
                    },
                    {
                        ColumnKey.Weekday,
                        new ColumnInfo
                        {
                            Title = "曜日\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Dream,
                        new ColumnInfo
                        {
                            Title = "弾幕夢\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Width,
                        new ColumnInfo
                        {
                            Title = "幅\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Height,
                        new ColumnInfo
                        {
                            Title = "高さ\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.SlowRate,
                        new ColumnInfo
                        {
                            Title = "処理落ち率\0",
                            Align = TextAlign.Right,
                            Sort = SortType.FloatingPoint,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.DateTime,
                        new ColumnInfo
                        {
                            Title = "日時\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Angle,
                        new ColumnInfo
                        {
                            Title = "Angle\0",
                            Align = TextAlign.Right,
                            Sort = SortType.FloatingPoint,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Score,
                        new ColumnInfo
                        {
                            Title = "総合評価点\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
#if DEBUG
                    {
                        ColumnKey.Hashtags1,
                        new ColumnInfo
                        {
                            Title = "Hashtags1\0",
                            Align = TextAlign.Right,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Hashtags2,
                        new ColumnInfo
                        {
                            Title = "Hashtags2\0",
                            Align = TextAlign.Right,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Hashtags3,
                        new ColumnInfo
                        {
                            Title = "Hashtags3\0",
                            Align = TextAlign.Right,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
#endif
                    {
                        ColumnKey.BasePoint,
                        new ColumnInfo
                        {
                            Title = "Base Point\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.NumViewed,
                        new ColumnInfo
                        {
                            Title = "この写真を見た回数\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.NumLikes,
                        new ColumnInfo
                        {
                            Title = "イイッすね！\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.NumFavs,
                        new ColumnInfo
                        {
                            Title = "お気に入り！\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.NumBullets,
                        new ColumnInfo
                        {
                            Title = "敵弾数\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.NumBulletsNearby,
                        new ColumnInfo
                        {
                            Title = "近くの敵弾数\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.RiskBonus,
                        new ColumnInfo
                        {
                            Title = "Risk Bonus\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.BossShot,
                        new ColumnInfo
                        {
                            Title = "Boss Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.FloatingPoint,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.AngleBonus,
                        new ColumnInfo
                        {
                            Title = "Angle Bonus\0",
                            Align = TextAlign.Right,
                            Sort = SortType.FloatingPoint,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.MacroBonus,
                        new ColumnInfo
                        {
                            Title = "Macro Bonus\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.LikesPerView,
                        new ColumnInfo
                        {
                            Title = "イイッすね率\0",
                            Align = TextAlign.Right,
                            Sort = SortType.FloatingPoint,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.FavsPerView,
                        new ColumnInfo
                        {
                            Title = "お気に入り率\0",
                            Align = TextAlign.Right,
                            Sort = SortType.FloatingPoint,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.NumHashtags,
                        new ColumnInfo
                        {
                            Title = "ハッシュタグ数\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.NumRedBullets,
                        new ColumnInfo
                        {
                            Title = "赤弾数\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.NumPurpleBullets,
                        new ColumnInfo
                        {
                            Title = "紫弾数\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.NumBlueBullets,
                        new ColumnInfo
                        {
                            Title = "青弾数\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.NumCyanBullets,
                        new ColumnInfo
                        {
                            Title = "水弾数\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.NumGreenBullets,
                        new ColumnInfo
                        {
                            Title = "緑弾数\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.NumYellowBullets,
                        new ColumnInfo
                        {
                            Title = "黄弾数\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.NumOrangeBullets,
                        new ColumnInfo
                        {
                            Title = "橙弾数\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.NumLightBullets,
                        new ColumnInfo
                        {
                            Title = "光弾数\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.FileSize,
                        new ColumnInfo
                        {
                            Title = "ファイルサイズ\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.FileSize,
                        }
                    },
                    {
                        ColumnKey.Directory,
                        new ColumnInfo
                        {
                            Title = "ディレクトリ\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.Directory,
                        }
                    },
                    {
                        ColumnKey.Sentinel,
                        new ColumnInfo
                        {
                            Title = "\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                };

            private static readonly string[] WeekdayStrings =
            {
                "日曜日", "月曜日", "火曜日", "水曜日", "木曜日", "金曜日", "土曜日",
                "裏・日曜日", "裏・月曜日", "裏・火曜日", "裏・水曜日", "裏・木曜日", "裏・金曜日", "裏・土曜日",
                "悪夢日曜", "悪夢月曜", "悪夢火曜", "悪夢水曜", "悪夢木曜", "悪夢金曜", "悪夢土曜",
                "ナイトメアダイアリー",
            };

            private static readonly Dictionary<ColumnKey, Func<BestshotData, string>> FileInfoGetters =
                InitializeFileInfoGetters();

            internal enum ColumnKey
            {
#pragma warning disable SA1413 // Use trailing comma in multi-line initializers
                Filename = 0,
                LastWriteDate,
                Weekday,
                Dream,
                Width,
                Height,
                SlowRate,
                DateTime,
                Angle,
                Score,
#if DEBUG
                Hashtags1,
                Hashtags2,
                Hashtags3,
#endif
                BasePoint,
                NumViewed,
                NumLikes,
                NumFavs,
                NumBullets,
                NumBulletsNearby,
                RiskBonus,
                BossShot,
                AngleBonus,
                MacroBonus,
                LikesPerView,
                FavsPerView,
                NumHashtags,
                NumRedBullets,
                NumPurpleBullets,
                NumBlueBullets,
                NumCyanBullets,
                NumGreenBullets,
                NumYellowBullets,
                NumOrangeBullets,
                NumLightBullets,
                FileSize,
                Directory,
                Sentinel
#pragma warning restore SA1413 // Use trailing comma in multi-line initializers
            }

            protected override ReadOnlyCollection<string> ManagedPluginInfo => Array.AsReadOnly(PluginInfo);

            protected override IDictionary<PluginImpl.ColumnKey, ColumnInfo> ManagedColumnInfo => Columns;

            public override uint IsSupported(IntPtr src, uint size)
            {
                if (src == IntPtr.Zero)
                {
                    return (uint)ValidSignature.Length;
                }

                var signature = string.Empty;

                try
                {
                    using var pair = ReimuPluginRev1<ColumnKey>.CreateStream(src, size);
                    if (pair.Item1 == ErrorCode.AllRight)
                    {
                        using var reader = new IO.BinaryReader(pair.Item2, Enc.UTF8NoBOM, true);
                        var readSize = Math.Min((int)reader.BaseStream.Length, ValidSignature.Length);
                        signature = Enc.CP932.GetString(reader.ReadBytes(readSize));
                    }
                }
                catch (OutOfMemoryException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (IO.IOException)
                {
                }
                catch (NotSupportedException)
                {
                }
                catch (ObjectDisposedException)
                {
                }

                return (signature == ValidSignature) ? 1u : 0u;
            }

            public override ErrorCode GetFileInfoList(IntPtr src, uint size, out IntPtr info)
            {
                var errorCode = ErrorCode.UnknownError;

                info = IntPtr.Zero;

                try
                {
                    var pair = CreateBestshotData(src, size, false);
                    if (pair.Item1 == ErrorCode.AllRight)
                    {
                        var fileInfoSize = Marshal.SizeOf(typeof(FileInfo));
                        var keys = Utils.GetEnumerator<ColumnKey>().Where(key => key != ColumnKey.Sentinel);

                        info = Marshal.AllocHGlobal(fileInfoSize * keys.Count());

                        var address = info.ToInt64();
                        foreach (var key in keys)
                        {
                            var fileInfo = new FileInfo { Text = string.Empty };
                            if (FileInfoGetters.TryGetValue(key, out var getter))
                            {
                                fileInfo.Text = getter(pair.Item2);
                            }

                            var pointer = new IntPtr(address);
                            Marshal.StructureToPtr(fileInfo, pointer, false);
                            address += fileInfoSize;
                        }
                    }

                    errorCode = pair.Item1;
                }
                catch (OutOfMemoryException)
                {
                    errorCode = ErrorCode.NoMemory;
                }
                catch (ArgumentException)
                {
                }
                catch (OverflowException)
                {
                }
                finally
                {
                    if (errorCode != ErrorCode.AllRight)
                    {
                        Marshal.FreeHGlobal(info);
                        info = IntPtr.Zero;
                    }
                }

                return errorCode;
            }

            public override ErrorCode GetFileInfoText1(IntPtr src, uint size, out IntPtr dst)
            {
                var errorCode = ErrorCode.UnknownError;

                dst = IntPtr.Zero;

                try
                {
                    var pair = CreateBestshotData(src, size, false);
                    if (pair.Item1 == ErrorCode.AllRight)
                    {
                        var data = pair.Item2;

                        IO.MemoryStream stream = null;
                        try
                        {
#pragma warning disable IDISP001 // Dispose created.
                            stream = new IO.MemoryStream();
#pragma warning restore IDISP001 // Dispose created.
                            using var writer = new IO.StreamWriter(stream, Enc.CP932);
#pragma warning disable IDISP003 // Dispose previous before re-assigning.
                            stream = null;
#pragma warning restore IDISP003 // Dispose previous before re-assigning.

                            writer.NewLine = "\r\n";
                            writer.CondWriteLine(data.IsSelfie, "＃自撮り！");
                            writer.CondWriteLine(data.IsTwoShot, "＃ツーショット！");
                            writer.CondWriteLine(data.IsThreeShot, "＃スリーショット！");
                            writer.CondWriteLine(data.TwoEnemiesTogether, "＃二人まとめて撮影した！");
                            writer.CondWriteLine(data.EnemyIsPartlyInFrame, "＃敵が見切れてる");
                            writer.CondWriteLine(data.WholeEnemyIsInFrame, "＃敵を収めたよ");
                            writer.CondWriteLine(data.EnemyIsInMiddle, "＃敵がど真ん中");
                            writer.CondWriteLine(data.PeaceSignAlongside, "＃並んでピース");
                            writer.CondWriteLine(data.EnemiesAreTooClose, "＃二人が近すぎるｗ");
                            writer.CondWriteLine(data.EnemiesAreOverlapping, "＃二人が重なってるｗｗ");
                            writer.CondWriteLine(data.Closeup, "＃接写！");
                            writer.CondWriteLine(data.QuiteCloseup, "＃かなりの接写！");
                            writer.CondWriteLine(data.TooClose, "＃近すぎてぶつかるー！");
                            writer.CondWriteLine(data.TooManyBullets, "＃弾多すぎｗ");
                            writer.CondWriteLine(data.TooPlayfulBarrage, "＃弾幕ふざけすぎｗｗ");
                            writer.CondWriteLine(data.TooDense, "＃ちょっ、密度濃すぎｗｗｗ");
                            writer.CondWriteLine(data.BitDangerous, "＃ちょっと危なかった");
                            writer.CondWriteLine(data.SeriouslyDangerous, "＃マジで危なかった");
                            writer.CondWriteLine(data.ThoughtGonnaDie, "＃死ぬかと思った");
                            writer.CondWriteLine(data.EnemyIsInFullView, "＃敵が丸見えｗ");
                            writer.CondWriteLine(data.ManyReds, "＃赤色多いな");
                            writer.CondWriteLine(data.ManyPurples, "＃紫色多いね");
                            writer.CondWriteLine(data.ManyBlues, "＃青色多いよ");
                            writer.CondWriteLine(data.ManyCyans, "＃水色多いし");
                            writer.CondWriteLine(data.ManyGreens, "＃緑色多いねぇ");
                            writer.CondWriteLine(data.ManyYellows, "＃黄色多いなぁ");
                            writer.CondWriteLine(data.ManyOranges, "＃橙色多いお");
                            writer.CondWriteLine(data.TooColorful, "＃カラフル過ぎｗ");
                            writer.CondWriteLine(data.SevenColors, "＃七色全部揃った！");
                            writer.CondWriteLine(data.Dazzling, "＃うおっ、まぶし！");
                            writer.CondWriteLine(data.MoreDazzling, "＃ぐあ、眩しすぎるー！");
                            writer.CondWriteLine(data.MostDazzling, "＃うあー、目が、目がー！");
                            writer.CondWriteLine(data.EnemyIsUndamaged, "＃敵は無傷だ");
                            writer.CondWriteLine(data.EnemyCanAfford, "＃敵はまだ余裕がある");
                            writer.CondWriteLine(data.EnemyIsWeakened, "＃敵がだいぶ弱ってる");
                            writer.CondWriteLine(data.EnemyIsDying, "＃敵が瀕死だ");
                            writer.CondWriteLine(data.Finished, "＃トドメをさしたよ！");
                            writer.CondWriteLine(data.FinishedTogether, "＃二人まとめてトドメ！");
                            writer.CondWriteLine(data.Chased, "＃追い打ちしたよ！");
                            writer.CondWriteLine(data.IsSuppository, "＃座薬ｗｗｗ");
                            writer.CondWriteLine(data.IsButterflyLikeMoth, "＃蛾みたいな蝶だ！");
                            writer.CondWriteLine(data.Scorching, "＃アチチ、焦げちゃうよ");
                            writer.CondWriteLine(data.TooBigBullet, "＃弾、大きすぎでしょｗ");
                            writer.CondWriteLine(data.ThrowingEdgedTools, "＃刃物投げんな (و｀ω´)6");
                            writer.CondWriteLine(data.IsThunder, "＃ぎゃー、雷はスマホがー");
                            writer.CondWriteLine(data.Snaky, "＃うねうねだー！");
                            writer.CondWriteLine(data.LightLooksStopped, "＃光が止まって見える！");
                            writer.CondWriteLine(data.IsSuperMoon, "＃スーパームーン！");
                            writer.CondWriteLine(data.IsRockyBarrage, "＃岩の弾幕とかｗｗ");
                            writer.CondWriteLine(data.IsStickDestroyingBarrage, "＃弾幕を破壊する棒……？");
                            writer.CondWriteLine(data.IsLovelyHeart, "＃ラブリーハート！");
                            writer.CondWriteLine(data.IsDrum, "＃ドンドコドンドコ");
                            writer.CondWriteLine(data.Fluffy, "＃もふもふもふー");
                            writer.CondWriteLine(data.IsDoggiePhoto, "＃わんわん写真");
                            writer.CondWriteLine(data.IsAnimalPhoto, "＃アニマルフォト");
                            writer.CondWriteLine(data.IsZoo, "＃動物園！");
                            writer.CondWriteLine(data.IsMisty, "＃身体が霧状に！？");
                            writer.CondWriteLine(data.WasScolded, "＃怒られちゃった……");
                            writer.CondWriteLine(data.IsLandscapePhoto, "＃風景写真");
                            writer.CondWriteLine(data.IsBoringPhoto, "＃何ともつまらない写真");
                            writer.CondWriteLine(data.IsSumireko, "＃私こそが宇佐見菫子だ！");
                            writer.WriteLine();
                            writer.WriteLine("この写真を見た回数  {0}", data.NumViewed);
                            writer.WriteLine("イイッすね！        {0}", data.NumLikes);
                            writer.WriteLine("お気に入り！        {0}", data.NumFavs);
                            writer.WriteLine("総合評価点          {0}", data.Score);
                            writer.Write("\0");
                            writer.Flush();

                            writer.BaseStream.Seek(0, IO.SeekOrigin.Begin);
                            var source = ((IO.MemoryStream)writer.BaseStream).ToArray();
                            dst = Marshal.AllocHGlobal(source.Length);
                            Marshal.Copy(source, 0, dst, source.Length);
                        }
                        finally
                        {
                            stream?.Dispose();
                        }
                    }

                    errorCode = pair.Item1;
                }
                catch (OutOfMemoryException)
                {
                    errorCode = ErrorCode.NoMemory;
                }
                catch (ArgumentException)
                {
                }
                finally
                {
                    if (errorCode != ErrorCode.AllRight)
                    {
                        Marshal.FreeHGlobal(dst);
                        dst = IntPtr.Zero;
                    }
                }

                return errorCode;
            }

            public override ErrorCode GetFileInfoText2(IntPtr src, uint size, out IntPtr dst)
            {
                throw new NotImplementedException();
            }

            public override ErrorCode GetFileInfoImage1(IntPtr src, uint size, out IntPtr dst, out IntPtr info)
            {
                throw new NotImplementedException();
            }

            public override ErrorCode GetFileInfoImage2(IntPtr src, uint size, out IntPtr dst, out IntPtr info)
            {
                var errorCode = ErrorCode.UnknownError;

                dst = IntPtr.Zero;
                info = IntPtr.Zero;

                try
                {
                    var pair = CreateBestshotData(src, size, true);
                    if (pair.Item1 == ErrorCode.AllRight)
                    {
                        var copySize = 4 * pair.Item2.Width * pair.Item2.Height;

                        dst = Marshal.AllocHGlobal(copySize);

                        using (var locked = new BitmapLock(pair.Item2.Bitmap, ImageLockMode.ReadOnly))
                        {
                            var copy = new byte[copySize];
                            Marshal.Copy(locked.Scan0, copy, 0, copy.Length);
                            Marshal.Copy(copy, 0, dst, copy.Length);
                        }

                        var bitmapInfo = new BITMAPINFO
                        {
                            bmiHeader = new BITMAPINFOHEADER
                            {
                                biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER)),
                                biWidth = pair.Item2.Bitmap.Width,
                                biHeight = -pair.Item2.Bitmap.Height,
                                biPlanes = 1,
                                biBitCount = 32,
                                biCompression = BITMAPINFOHEADER.CompressionType.BI_RGB,
                                biSizeImage = 0,
                                biXPelsPerMeter = 0,
                                biYPelsPerMeter = 0,
                                biClrUsed = 0,
                                biClrImportant = 0,
                            },
                            bmiColors = IntPtr.Zero,
                        };

                        info = Marshal.AllocHGlobal(Marshal.SizeOf(bitmapInfo));
                        Marshal.StructureToPtr(bitmapInfo, info, true);
                    }

                    errorCode = pair.Item1;
                }
                catch (OutOfMemoryException)
                {
                    errorCode = ErrorCode.NoMemory;
                }
                catch (ArgumentException)
                {
                }
                finally
                {
                    if (errorCode != ErrorCode.AllRight)
                    {
                        Marshal.FreeHGlobal(dst);
                        dst = IntPtr.Zero;
                        Marshal.FreeHGlobal(info);
                        info = IntPtr.Zero;
                    }
                }

                return errorCode;
            }

            public override ErrorCode EditDialog(IntPtr parent, string file)
            {
                throw new NotImplementedException();
            }

            public override ErrorCode ConfigDialog(IntPtr parent)
            {
                throw new NotImplementedException();
            }

            [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed.")]
            private static Dictionary<ColumnKey, Func<BestshotData, string>> InitializeFileInfoGetters()
            {
                return new Dictionary<ColumnKey, Func<BestshotData, string>>
                {
                    {
                        ColumnKey.Weekday,
                        (data) => WeekdayStrings[data.Weekday]
                    },
                    {
                        ColumnKey.Dream,
                        (data) => (data.Dream + 1).ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.Width,
                        (data) => data.Width.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.Height,
                        (data) => data.Height.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.SlowRate,
                        (data) => data.SlowRate.ToString("F6", CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.DateTime,
                        (data) => new DateTime(1970, 1, 1).AddSeconds(data.DateTime).ToLocalTime()
                            .ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.Angle,
                        (data) => data.Angle.ToString("F6", CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.Score,
                        (data) => data.Score.ToString(CultureInfo.CurrentCulture)
                    },
#if DEBUG
                    {
                        ColumnKey.Hashtags1,
                        (data) => Convert.ToString(data.Hashtags1, 2)
                    },
                    {
                        ColumnKey.Hashtags2,
                        (data) => Convert.ToString(data.Hashtags2, 2)
                    },
                    {
                        ColumnKey.Hashtags3,
                        (data) => Convert.ToString(data.Hashtags3, 2)
                    },
#endif
                    {
                        ColumnKey.BasePoint,
                        (data) => data.BasePoint.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.NumViewed,
                        (data) => data.NumViewed.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.NumLikes,
                        (data) => data.NumLikes.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.NumFavs,
                        (data) => data.NumFavs.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.NumBullets,
                        (data) => data.NumBullets.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.NumBulletsNearby,
                        (data) => data.NumBulletsNearby.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.RiskBonus,
                        (data) => data.RiskBonus.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.BossShot,
                        (data) => data.BossShot.ToString("F6", CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.AngleBonus,
                        (data) => data.AngleBonus.ToString("F6", CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.MacroBonus,
                        (data) => data.MacroBonus.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.LikesPerView,
                        (data) => data.LikesPerView.ToString("F6", CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.FavsPerView,
                        (data) => data.FavsPerView.ToString("F6", CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.NumHashtags,
                        (data) => data.NumHashtags.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.NumRedBullets,
                        (data) => data.NumRedBullets.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.NumPurpleBullets,
                        (data) => data.NumPurpleBullets.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.NumBlueBullets,
                        (data) => data.NumBlueBullets.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.NumCyanBullets,
                        (data) => data.NumCyanBullets.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.NumGreenBullets,
                        (data) => data.NumGreenBullets.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.NumYellowBullets,
                        (data) => data.NumYellowBullets.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.NumOrangeBullets,
                        (data) => data.NumOrangeBullets.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.NumLightBullets,
                        (data) => data.NumLightBullets.ToString(CultureInfo.CurrentCulture)
                    },
                };
            }

            private static Tuple<ErrorCode, BestshotData> CreateBestshotData(
                IntPtr src, uint size, bool withBitmap)
            {
                using var pair = ReimuPluginRev1<ColumnKey>.CreateStream(src, size);
                BestshotData bestshot = null;

                if (pair.Item1 == ErrorCode.AllRight)
                {
                    bestshot = new BestshotData();
                    bestshot.Read(pair.Item2, withBitmap);
                }

                return Tuple.Create(pair.Item1, bestshot);
            }
        }
    }
}
