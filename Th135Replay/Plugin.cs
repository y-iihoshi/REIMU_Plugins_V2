﻿//-----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Th135Replay;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using NXPorts.Attributes;
using ReimuPlugins.Common;
using ReimuPlugins.Common.Extensions;
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
    public static ErrorCode GetFileInfoText1(IntPtr src, uint size, out IntPtr dst)
    {
        return Impl.GetFileInfoText1(src, size, out dst);
    }

    [DllExport(callingConvention: CallingConvention.StdCall)]
    public static ErrorCode GetFileInfoText2(IntPtr src, uint size, out IntPtr dst)
    {
        return Impl.GetFileInfoText2(src, size, out dst);
    }

    private sealed class PluginImpl : ReimuPluginRev1<PluginImpl.ColumnKey>
    {
        private const string ValidSignature = "TFRP\0";

        private static readonly string[] PluginInfoImpl =
        {
            "REIMU Plug-in For 東方心綺楼 Ver2.2.0 (C) 2015 IIHOSHI Yoshinori\0",
            "東方心綺楼\0",
            "*.rep\0",
            "東方心綺楼 リプレイファイル (*.rep)\0",
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
                    ColumnKey.Version,
                    new ColumnInfo
                    {
                        Title = "バージョン\0",
                        Align = TextAlign.Left,
                        Sort = SortType.String,
                        System = SystemInfoType.None,
                    }
                },
                {
                    ColumnKey.GameMode,
                    new ColumnInfo
                    {
                        Title = "モード\0",
                        Align = TextAlign.Left,
                        Sort = SortType.String,
                        System = SystemInfoType.None,
                    }
                },
                {
                    ColumnKey.Player1,
                    new ColumnInfo
                    {
                        Title = "プレイヤー 1\0",
                        Align = TextAlign.Left,
                        Sort = SortType.String,
                        System = SystemInfoType.None,
                    }
                },
                {
                    ColumnKey.Color1,
                    new ColumnInfo
                    {
                        Title = "色 1\0",
                        Align = TextAlign.Left,
                        Sort = SortType.String,
                        System = SystemInfoType.None,
                    }
                },
                {
                    ColumnKey.Profile1,
                    new ColumnInfo
                    {
                        Title = "プロファイル 1\0",
                        Align = TextAlign.Left,
                        Sort = SortType.String,
                        System = SystemInfoType.None,
                    }
                },
                {
                    ColumnKey.Player2,
                    new ColumnInfo
                    {
                        Title = "プレイヤー 2\0",
                        Align = TextAlign.Left,
                        Sort = SortType.String,
                        System = SystemInfoType.None,
                    }
                },
                {
                    ColumnKey.Color2,
                    new ColumnInfo
                    {
                        Title = "色 2\0",
                        Align = TextAlign.Left,
                        Sort = SortType.String,
                        System = SystemInfoType.None,
                    }
                },
                {
                    ColumnKey.Profile2,
                    new ColumnInfo
                    {
                        Title = "プロファイル 2\0",
                        Align = TextAlign.Left,
                        Sort = SortType.String,
                        System = SystemInfoType.None,
                    }
                },
                {
                    ColumnKey.Background,
                    new ColumnInfo
                    {
                        Title = "ステージ\0",
                        Align = TextAlign.Left,
                        Sort = SortType.String,
                        System = SystemInfoType.None,
                    }
                },
                {
                    ColumnKey.Bgm,
                    new ColumnInfo
                    {
                        Title = "BGM\0",
                        Align = TextAlign.Left,
                        Sort = SortType.String,
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

        private static readonly IReadOnlyDictionary<ColumnKey, Func<ReplayData, string>> FileInfoGetters =
            InitializeFileInfoGetters();

        internal enum ColumnKey
        {
#pragma warning disable SA1413 // Use trailing comma in multi-line initializers
            Filename = 0,
            LastWriteDate,
            Version,
            GameMode,
            Player1,
            Color1,
            Profile1,
            Player2,
            Color2,
            Profile2,
            Background,
            Bgm,
            DateTime,
            FileSize,
            Directory,
            Sentinel
#pragma warning restore SA1413 // Use trailing comma in multi-line initializers
        }

        protected override IReadOnlyList<string> ManagedPluginInfo { get; } = PluginInfoImpl;

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
                var pair = CreateReplayData(src, size);
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
                var pair = CreateReplayData(src, size);
                if (pair.Item1 == ErrorCode.AllRight)
                {
                    var bytes = Encoding.CP932.GetBytes(pair.Item2.Player1Info.ToCStr());
                    dst = Marshal.AllocHGlobal(bytes.Length);
                    Marshal.Copy(bytes, 0, dst, bytes.Length);
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
            var errorCode = ErrorCode.UnknownError;

            dst = IntPtr.Zero;

            try
            {
                var pair = CreateReplayData(src, size);
                if (pair.Item1 == ErrorCode.AllRight)
                {
                    var text = (pair.Item2.GameMode == GameMode.Story)
                        ? string.Empty : pair.Item2.Player2Info;
                    var bytes = Encoding.CP932.GetBytes(text.ToCStr());
                    dst = Marshal.AllocHGlobal(bytes.Length);
                    Marshal.Copy(bytes, 0, dst, bytes.Length);
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

        public override ErrorCode EditDialog(IntPtr parent, string file)
        {
            throw new NotImplementedException();
        }

        public override ErrorCode ConfigDialog(IntPtr parent)
        {
            throw new NotImplementedException();
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed.")]
        private static IReadOnlyDictionary<ColumnKey, Func<ReplayData, string>> InitializeFileInfoGetters()
        {
            return new Dictionary<ColumnKey, Func<ReplayData, string>>
            {
                {
                    ColumnKey.Version,
                    (data) => data.Version
                },
                {
                    ColumnKey.GameMode,
                    (data) => data.GameMode.ToShortName()
                },
                {
                    ColumnKey.Player1,
                    (data) => data.Character1.ToLongName()
                },
                {
                    ColumnKey.Color1,
                    (data) => (data.Color1 + 1).ToString(CultureInfo.CurrentCulture)
                },
                {
                    ColumnKey.Profile1,
                    (data) => data.Profile1Name
                },
                {
                    ColumnKey.Player2,
                    (data) => (data.GameMode == GameMode.Story) ? "-" : data.Character2.ToLongName()
                },
                {
                    ColumnKey.Color2,
                    (data) => (data.GameMode == GameMode.Story)
                        ? "-" : (data.Color2 + 1).ToString(CultureInfo.CurrentCulture)
                },
                {
                    ColumnKey.Profile2,
                    (data) => (data.GameMode == GameMode.Story) ? "-" : data.Profile2Name
                },
                {
                    ColumnKey.Background,
                    (data) => (data.GameMode == GameMode.Story) ? "-" : data.BackgroundName
                },
                {
                    ColumnKey.Bgm,
                    (data) => (data.GameMode == GameMode.Story) ? "-" : data.BgmName
                },
                {
                    ColumnKey.DateTime,
                    (data) => data.DateTime.ToString(CultureInfo.CurrentCulture)
                },
            };
        }

        private static Tuple<ErrorCode, ReplayData> CreateReplayData(IntPtr src, uint size)
        {
            using var pair = CreateStream(src, size);
            ReplayData replay = null;

            if (pair.Item1 == ErrorCode.AllRight)
            {
                replay = new ReplayData();
                replay.Read(pair.Item2);
            }

            return Tuple.Create(pair.Item1, replay);
        }
    }
}
