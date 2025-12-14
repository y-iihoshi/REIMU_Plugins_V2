//-----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Th143Screenshot;

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using ReimuPlugins.Common;
using ReimuPlugins.SourceGenerator;
using Windows.Win32.Graphics.Gdi;
using IO = System.IO;

[ReimuPlugin]
internal sealed class Plugin : ReimuPluginRev2<Plugin.ColumnKey>
{
    private const string ValidSignature = "BST3";

    private static readonly string[] PluginInfo =
    {
        "REIMU Plug-in For 弾幕アマノジャク スクリーンショット Ver2.2.0 (C) 2015 IIHOSHI Yoshinori\0",
        "弾幕アマノジャク スクリーンショット\0",
        "sc*.dat\0",
        "弾幕アマノジャク スクリーンショットファイル (sc*.dat)\0",
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
                ColumnKey.Day,
                new ColumnInfo
                {
                    Title = "日付\0",
                    Align = TextAlign.Left,
                    Sort = SortType.String,
                    System = SystemInfoType.None,
                }
            },
            {
                ColumnKey.Scene,
                new ColumnInfo
                {
                    Title = "シーン\0",
                    Align = TextAlign.Left,
                    Sort = SortType.Integer,
                    System = SystemInfoType.None,
                }
            },
            {
                ColumnKey.DayScene,
                new ColumnInfo
                {
                    Title = "日付-シーン\0",
                    Align = TextAlign.Left,
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

    private static readonly string[] DayStrings =
    {
        "一日目", "二日目", "三日目", "四日目", "五日目",
        "六日目", "七日目", "八日目", "九日目", "最終日",
    };

    private static readonly IReadOnlyDictionary<ColumnKey, Func<ScreenshotData, string>> FileInfoGetters =
        InitializeFileInfoGetters();

    internal enum ColumnKey
    {
#pragma warning disable SA1413 // Use trailing comma in multi-line initializers
        Filename = 0,
        LastWriteDate,
        Day,
        Scene,
        DayScene,
        Width,
        Height,
        DateTime,
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
            var pair = CreateBestshotData<ScreenshotData>(src, size, false);
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

    public override ErrorCode GetFileInfoImage1(IntPtr src, uint size, out IntPtr dst, out IntPtr info)
    {
        var errorCode = ErrorCode.UnknownError;

        dst = IntPtr.Zero;
        info = IntPtr.Zero;

        try
        {
            var pair = CreateBestshotData<ScreenshotData>(src, size, true);
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

    private static IReadOnlyDictionary<ColumnKey, Func<ScreenshotData, string>> InitializeFileInfoGetters()
    {
        return new Dictionary<ColumnKey, Func<ScreenshotData, string>>
        {
            {
                ColumnKey.Day,
                (data) => DayStrings[data.Day]
            },
            {
                ColumnKey.Scene,
                (data) => (data.Scene + 1).ToString(CultureInfo.CurrentCulture)
            },
            {
                ColumnKey.DayScene,
                (data) => string.Format(
                    CultureInfo.CurrentCulture, "{0}-{1}", data.Day + 1, data.Scene + 1)
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
        };
    }
}
