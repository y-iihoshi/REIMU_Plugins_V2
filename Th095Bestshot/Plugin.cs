//-----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Th095Bestshot;

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using NXPorts.Attributes;
using ReimuPlugins.Common;
using Windows.Win32.Graphics.Gdi;
using IO = System.IO;

public static class Plugin
{
    private static readonly PluginImpl Impl = new();

    [DllExport(callingConvention: CallingConvention.StdCall)]
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
    public static ErrorCode GetFileInfoList(IntPtr src, uint size, out IntPtr info)
    {
        return Impl.GetFileInfoList(src, size, out info);
    }

    [DllExport(callingConvention: CallingConvention.StdCall)]
    public static ErrorCode GetFileInfoImage1(IntPtr src, uint size, out IntPtr dst, out IntPtr info)
    {
        return Impl.GetFileInfoImage1(src, size, out dst, out info);
    }

    private sealed class PluginImpl : ReimuPluginRev2<PluginImpl.ColumnKey>
    {
        private const string ValidSignature = "BSTS";

        private static readonly string[] PluginInfo =
        {
            "REIMU Plug-in For 東方文花帖 ベストショット Ver2.2.0 (C) 2015 IIHOSHI Yoshinori\0",
            "東方文花帖 ベストショット\0",
            "bs_*.dat\0",
            "東方文花帖 ベストショットファイル (bs_*.dat)\0",
        };

        private static readonly IReadOnlyDictionary<ColumnKey, ColumnInfo> Columns =
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
                        System = SystemInfoType.None,
                    }
                },
                {
                    ColumnKey.CardName,
                    new ColumnInfo
                    {
                        Title = "スペルカード名\0",
                        Align = TextAlign.Left,
                        Sort = SortType.String,
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
                    ColumnKey.Score,
                    new ColumnInfo
                    {
                        Title = "評価点\0",
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

        private static readonly IReadOnlyDictionary<ColumnKey, Func<BestshotData, string>> FileInfoGetters =
            new Dictionary<ColumnKey, Func<BestshotData, string>>
            {
                {
                    ColumnKey.Scene,
                    (data) => string.Format(
                        CultureInfo.CurrentCulture, "{0}-{1}", data.Level, data.Scene)
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
                    ColumnKey.Score,
                    (data) => data.Score.ToString(CultureInfo.CurrentCulture)
                },
                {
                    ColumnKey.SlowRate,
                    (data) => data.SlowRate.ToString("F6", CultureInfo.CurrentCulture)
                },
            };

        internal enum ColumnKey
        {
#pragma warning disable SA1413 // Use trailing comma in multi-line initializers
            Filename = 0,
            LastWriteDate,
            Scene,
            CardName,
            Width,
            Height,
            Score,
            SlowRate,
            FileSize,
            Directory,
            Sentinel
#pragma warning restore SA1413 // Use trailing comma in multi-line initializers
        }

        protected override IReadOnlyList<string> ManagedPluginInfo { get; } = PluginInfo;

        protected override IReadOnlyDictionary<ColumnKey, ColumnInfo> ManagedColumnInfo { get; } = Columns;

        public override uint IsSupported(IntPtr src, uint size)
        {
            if (src == IntPtr.Zero)
            {
                return (uint)ValidSignature.Length;
            }

            var signature = string.Empty;

            try
            {
                using var pair = CreateStream(src, size);
                if (pair.Item1 == ErrorCode.AllRight)
                {
                    using var reader = new IO.BinaryReader(pair.Item2, Encoding.UTF8NoBOM, true);
                    var readSize = Math.Min((int)reader.BaseStream.Length, ValidSignature.Length);
                    signature = Encoding.CP932.GetString(reader.ReadBytes(readSize));
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
                var pair = CreateBestshotData<BestshotData>(src, size, false);
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
            throw new NotImplementedException();
        }

        public override ErrorCode GetFileInfoText2(IntPtr src, uint size, out IntPtr dst)
        {
            throw new NotImplementedException();
        }

        public override ErrorCode GetFileInfoImage1(IntPtr src, uint size, out IntPtr dst, out IntPtr info)
        {
            var errorCode = ErrorCode.UnknownError;

            dst = IntPtr.Zero;
            info = IntPtr.Zero;

            try
            {
                var pair = CreateBestshotData<BestshotData>(src, size, true);
                if (pair.Item1 == ErrorCode.AllRight)
                {
                    var dstStride = 3 * pair.Item2.Width;
                    var remain = dstStride % 4;
                    if (remain != 0)
                    {
                        dstStride += 4 - remain;
                    }

                    dst = Marshal.AllocHGlobal(dstStride * pair.Item2.Height);

                    using (var locked = new BitmapLock(pair.Item2.Bitmap, ImageLockMode.ReadOnly))
                    {
                        var srcScanline = new byte[3 * pair.Item2.Width];
                        var s = locked.Scan0;
                        var d = dst;
                        for (var h = 0; h < pair.Item2.Height; h++)
                        {
                            Marshal.Copy(s, srcScanline, 0, srcScanline.Length);
                            Marshal.Copy(srcScanline, 0, d, srcScanline.Length);
                            s = new IntPtr(s.ToInt32() + locked.Stride);
                            d = new IntPtr(d.ToInt32() + dstStride);
                        }
                    }

                    var bitmapInfo = new BITMAPINFO
                    {
                        bmiHeader = new BITMAPINFOHEADER
                        {
                            biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER)),
                            biWidth = pair.Item2.Bitmap.Width,
                            biHeight = -pair.Item2.Bitmap.Height,
                            biPlanes = 1,
                            biBitCount = 24,
                            biCompression = (uint)BI_COMPRESSION.BI_RGB,
                            biSizeImage = 0,
                            biXPelsPerMeter = 0,
                            biYPelsPerMeter = 0,
                            biClrUsed = 0,
                            biClrImportant = 0,
                        },
                        bmiColors = default,
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

        public override ErrorCode GetFileInfoImage2(IntPtr src, uint size, out IntPtr dst, out IntPtr info)
        {
            throw new NotImplementedException();
        }

        public override ErrorCode EditDialog(IntPtr parent, string file)
        {
            throw new NotImplementedException();
        }

        public override ErrorCode ConfigDialog(IntPtr parent)
        {
            throw new NotImplementedException();
        }
    }
}
