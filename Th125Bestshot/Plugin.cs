//-----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed.")]

namespace ReimuPlugins.Th125Bestshot
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
    using RGiesecke.DllExport;
    using IO = System.IO;

    public static class Plugin
    {
        private static readonly PluginImpl Impl = new PluginImpl();

        [DllExport]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "To comply with the REIMU plugin spec.")]
        public static Revision GetPluginRevision()
        {
            return Impl.GetPluginRevision();
        }

        [DllExport]
        public static int GetPluginInfo(int index, IntPtr info, uint size)
        {
            return Impl.GetPluginInfo(index, info, size);
        }

        [DllExport]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "To comply with the REIMU plugin spec.")]
        public static ErrorCode GetColumnInfo(out IntPtr info)
        {
            return Impl.GetColumnInfo(out info);
        }

        [DllExport]
        public static uint IsSupported(IntPtr src, uint size)
        {
            return Impl.IsSupported(src, size);
        }

        [DllExport]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "To comply with the REIMU plugin spec.")]
        public static ErrorCode GetFileInfoList(IntPtr src, uint size, out IntPtr info)
        {
            return Impl.GetFileInfoList(src, size, out info);
        }

        [DllExport]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "To comply with the REIMU plugin spec.")]
        public static ErrorCode GetFileInfoText1(IntPtr src, uint size, out IntPtr dst)
        {
            return Impl.GetFileInfoText1(src, size, out dst);
        }

        [DllExport]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "To comply with the REIMU plugin spec.")]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "To comply with the REIMU plugin spec.")]
        public static ErrorCode GetFileInfoImage2(IntPtr src, uint size, out IntPtr dst, out IntPtr info)
        {
            return Impl.GetFileInfoImage2(src, size, out dst, out info);
        }

        private sealed class PluginImpl : ReimuPluginRev2<PluginImpl.ColumnKey>
        {
            private const string ValidSignature = "BST2";

            private static readonly string[] PluginInfo =
            {
                "REIMU Plug-in For ダブルスポイラー ベストショット Ver2.00 (C) IIHOSHI Yoshinori, 2015\0",
                "ダブルスポイラー ベストショット\0",
                "bs*.dat\0",
                "ダブルスポイラー ベストショットファイル (bs*.dat)\0",
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
                        ColumnKey.Scene,
                        new ColumnInfo
                        {
                            Title = "シーン\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.CardName,
                        new ColumnInfo
                        {
                            Title = "スペルカード名\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.Width,
                        new ColumnInfo
                        {
                            Title = "幅\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.Height,
                        new ColumnInfo
                        {
                            Title = "高さ\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.DateTime,
                        new ColumnInfo
                        {
                            Title = "日時\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.SlowRate,
                        new ColumnInfo
                        {
                            Title = "処理落ち率\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Float,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.Score,
                        new ColumnInfo
                        {
                            Title = "Result Score\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.BasePoint,
                        new ColumnInfo
                        {
                            Title = "Base Point\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.ClearShot,
                        new ColumnInfo
                        {
                            Title = "Clear Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.SoloShot,
                        new ColumnInfo
                        {
                            Title = "Solo Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.RedShot,
                        new ColumnInfo
                        {
                            Title = "Red Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.PurpleShot,
                        new ColumnInfo
                        {
                            Title = "Purple Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.BlueShot,
                        new ColumnInfo
                        {
                            Title = "Blue Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.CyanShot,
                        new ColumnInfo
                        {
                            Title = "Cyan Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.GreenShot,
                        new ColumnInfo
                        {
                            Title = "Green Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.YellowShot,
                        new ColumnInfo
                        {
                            Title = "Yellow Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.OrangeShot,
                        new ColumnInfo
                        {
                            Title = "Orange Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.ColorfulShot,
                        new ColumnInfo
                        {
                            Title = "Colorful Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.RainbowShot,
                        new ColumnInfo
                        {
                            Title = "Rainbow Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.RiskBonus,
                        new ColumnInfo
                        {
                            Title = "Risk Bonus\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.MacroBonus,
                        new ColumnInfo
                        {
                            Title = "Macro Bonus\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.FrontShot,
                        new ColumnInfo
                        {
                            Title = "Front Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.SideShot,
                        new ColumnInfo
                        {
                            Title = "Side Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.BackShot,
                        new ColumnInfo
                        {
                            Title = "Back Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.CatBonus,
                        new ColumnInfo
                        {
                            Title = "Cat Bonus\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.BossShot,
                        new ColumnInfo
                        {
                            Title = "Boss Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Float,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.TwoShot,
                        new ColumnInfo
                        {
                            Title = "Two Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Float,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.NiceShot,
                        new ColumnInfo
                        {
                            Title = "Nice Shot\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Float,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.AngleBonus,
                        new ColumnInfo
                        {
                            Title = "Angle Bonus\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Float,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.Angle,
                        new ColumnInfo
                        {
                            Title = "Angle\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Float,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.FileSize,
                        new ColumnInfo
                        {
                            Title = "ファイルサイズ\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
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
                            System = SystemInfoType.String,
                        }
                    },
                };

            private static readonly string[] LevelStrings =
            {
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "EX", "SP",
            };

            private static readonly Dictionary<ColumnKey, Func<BestshotData, string>> FileInfoGetters =
                InitializeFileInfoGetters();

            internal enum ColumnKey
            {
#pragma warning disable SA1413 // Use trailing comma in multi-line initializers
                Filename = 0,
                LastWriteDate,
                Scene,
                CardName,
                Width,
                Height,
                DateTime,
                SlowRate,
                Score,
                BasePoint,
                ClearShot,
                SoloShot,
                RedShot,
                PurpleShot,
                BlueShot,
                CyanShot,
                GreenShot,
                YellowShot,
                OrangeShot,
                ColorfulShot,
                RainbowShot,
                RiskBonus,
                MacroBonus,
                FrontShot,
                SideShot,
                BackShot,
                CatBonus,
                BossShot,
                TwoShot,
                NiceShot,
                AngleBonus,
                Angle,
                FileSize,
                Directory,
                Sentinel
#pragma warning restore SA1413 // Use trailing comma in multi-line initializers
            }

            protected override ReadOnlyCollection<string> ManagedPluginInfo
            {
                get { return Array.AsReadOnly(PluginInfo); }
            }

            protected override IDictionary<PluginImpl.ColumnKey, ColumnInfo> ManagedColumnInfo
            {
                get { return Columns; }
            }

            public override uint IsSupported(IntPtr src, uint size)
            {
                if (src == IntPtr.Zero)
                {
                    return (uint)ValidSignature.Length;
                }

                var signature = string.Empty;

                try
                {
                    using (var pair = ReimuPluginRev1<ColumnKey>.CreateStream(src, size))
                    {
                        if (pair.Item1 == ErrorCode.AllRight)
                        {
                            using (var reader = new IO.BinaryReader(pair.Item2, Enc.UTF8NoBOM, true))
                            {
                                var readSize = Math.Min((int)reader.BaseStream.Length, ValidSignature.Length);
                                signature = Enc.CP932.GetString(reader.ReadBytes(readSize));
                            }
                        }
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
                            Func<BestshotData, string> getter;
                            if (FileInfoGetters.TryGetValue(key, out getter))
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
                        using (var stream = new IO.MemoryStream())
                        {
                            using (var writer = new IO.StreamWriter(stream, Enc.UTF8NoBOM, 1024, true))
                            {
                                writer.NewLine = "\r\n";
                                writer.WriteLine("Base Point  {0}", data.BasePoint);
                                writer.CondWriteLine(data.ClearShotBit, "Clear Shot!  + {0}", data.ClearShot);
                                writer.CondWriteLine(data.SoloShotBit, "Solo Shot!  + 100");
                                writer.CondWriteLine(data.RedShotBit, "Red Shot  + 300");
                                writer.CondWriteLine(data.PurpleShotBit, "Purple Shot  + 300");
                                writer.CondWriteLine(data.BlueShotBit, "Blue Shot  + 300");
                                writer.CondWriteLine(data.CyanShotBit, "Cyan Shot  + 300");
                                writer.CondWriteLine(data.GreenShotBit, "Green Shot  + 300");
                                writer.CondWriteLine(data.YellowShotBit, "Yellow Shot  + 300");
                                writer.CondWriteLine(data.OrangeShotBit, "Orange Shot  + 300");
                                writer.CondWriteLine(data.ColorfulShotBit, "Colorful Shot  + 900");
                                writer.CondWriteLine(data.RainbowShotBit, "Rainbow Shot  + 2100");
                                writer.CondWriteLine(data.RiskBonusBit, "Risk Bonus  + {0}", data.RiskBonus);
                                writer.CondWriteLine(data.MacroBonusBit, "Macro Bonus  + {0}", data.MacroBonus);
                                writer.CondWriteLine(data.FrontShotBit, "Front Shot  + {0}", data.FrontSideBackShot);
                                writer.CondWriteLine(data.SideShotBit, "Side Shot  + {0}", data.FrontSideBackShot);
                                writer.CondWriteLine(data.BackShotBit, "Back Shot  + {0}", data.FrontSideBackShot);
                                writer.CondWriteLine(data.CatBonusBit, "Cat Bonus  + 666");
                                writer.WriteLine();
                                writer.WriteLine("Boss Shot!  * {0:F2}", data.BossShot);
                                writer.CondWriteLine(data.TwoShotBit, "Two Shot!  * 1.50");
                                writer.CondWriteLine(data.NiceShotBit, "Nice Shot!  * {0:F2}", data.NiceShot);
                                writer.WriteLine("Angle Bonus  * {0:F2}", data.AngleBonus);
                                writer.WriteLine();
                                writer.WriteLine("Result Score  {0}", data.ResultScore);
                                writer.Write("\0");
                            }

                            stream.Seek(0, IO.SeekOrigin.Begin);
                            var source = stream.ToArray();
                            dst = Marshal.AllocHGlobal(source.Length);
                            Marshal.Copy(source, 0, dst, source.Length);
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
                        ColumnKey.Scene,
                        (data) => string.Format(
                            CultureInfo.CurrentCulture, "{0}-{1}", LevelStrings[data.Level - 1], data.Scene)
                    },
                    {
                        ColumnKey.CardName,
                        (data) => data.CardName
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
                        ColumnKey.DateTime,
                        (data) => new DateTime(1970, 1, 1).AddSeconds(data.DateTime).ToLocalTime()
                            .ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.SlowRate,
                        (data) => data.SlowRate.ToString("F6", CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.Score,
                        (data) => data.ResultScore.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.BasePoint,
                        (data) => data.BasePoint.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.ClearShot,
                        (data) => (data.ClearShotBit ? data.ClearShot : 0)
                            .ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.SoloShot,
                        (data) => (data.SoloShotBit ? 100 : 0).ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.RedShot,
                        (data) => (data.RedShotBit ? 300 : 0).ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.PurpleShot,
                        (data) => (data.PurpleShotBit ? 300 : 0).ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.BlueShot,
                        (data) => (data.BlueShotBit ? 300 : 0).ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.CyanShot,
                        (data) => (data.CyanShotBit ? 300 : 0).ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.GreenShot,
                        (data) => (data.GreenShotBit ? 300 : 0).ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.YellowShot,
                        (data) => (data.YellowShotBit ? 300 : 0).ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.OrangeShot,
                        (data) => (data.OrangeShotBit ? 300 : 0).ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.ColorfulShot,
                        (data) => (data.ColorfulShotBit ? 900 : 0).ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.RainbowShot,
                        (data) => (data.RainbowShotBit ? 2100 : 0).ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.RiskBonus,
                        (data) => (data.RiskBonusBit ? data.RiskBonus : 0)
                            .ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.MacroBonus,
                        (data) => (data.MacroBonusBit ? data.MacroBonus : 0)
                            .ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.FrontShot,
                        (data) => (data.FrontShotBit ? data.FrontSideBackShot : 0)
                            .ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.SideShot,
                        (data) => (data.SideShotBit ? data.FrontSideBackShot : 0)
                            .ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.BackShot,
                        (data) => (data.BackShotBit ? data.FrontSideBackShot : 0)
                            .ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.CatBonus,
                        (data) => (data.CatBonusBit ? 666 : 0).ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.BossShot,
                        (data) => data.BossShot.ToString("F2", CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.TwoShot,
                        (data) => (data.TwoShotBit ? 1.5f : 1f).ToString("F2", CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.NiceShot,
                        (data) => (data.NiceShotBit ? data.NiceShot : 1f)
                            .ToString("F2", CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.AngleBonus,
                        (data) => data.AngleBonus.ToString("F2", CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnKey.Angle,
                        (data) => data.Angle.ToString("F6", CultureInfo.CurrentCulture)
                    },
                };
            }

            private static Tuple<ErrorCode, BestshotData> CreateBestshotData(
                IntPtr src, uint size, bool withBitmap)
            {
                using (var pair = ReimuPluginRev1<ColumnKey>.CreateStream(src, size))
                {
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
}
