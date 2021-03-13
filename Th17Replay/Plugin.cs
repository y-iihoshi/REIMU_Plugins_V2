//-----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Th17Replay
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using NXPorts.Attributes;
    using ReimuPlugins.Common;
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

        [DllExport(callingConvention: CallingConvention.StdCall)]
        public static ErrorCode EditDialog(IntPtr parent, string file)
        {
            return Impl.EditDialog(parent, file);
        }

        private sealed class PluginImpl : ReimuPluginRev1<PluginImpl.ColumnKey>
        {
            private const string ValidSignature = "t17r";

            private static readonly string[] PluginInfoImpl =
            {
                "REIMU Plug-in For 東方鬼形獣 Ver2.1.0 (C) IIHOSHI Yoshinori, 2019-2021\0",
                "東方鬼形獣\0",
                "th17_*.rpy\0",
                "東方鬼形獣 リプレイファイル (th17_*.rpy)\0",
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
                        ColumnKey.Number,
                        new ColumnInfo
                        {
                            Title = "No.\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Player,
                        new ColumnInfo
                        {
                            Title = "プレイヤー名\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.PlayTime,
                        new ColumnInfo
                        {
                            Title = "プレイ時刻\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Character,
                        new ColumnInfo
                        {
                            Title = "使用キャラ\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Level,
                        new ColumnInfo
                        {
                            Title = "難易度\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Stage,
                        new ColumnInfo
                        {
                            Title = "ステージ\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Score,
                        new ColumnInfo
                        {
                            Title = "スコア\0",
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
                        ColumnKey.Comment,
                        new ColumnInfo
                        {
                            Title = "コメント\0",
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
                new Dictionary<ColumnKey, Func<ReplayData, string>>
                {
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
                    { ColumnKey.Player,    (data) => data.Name     },
                    { ColumnKey.PlayTime,  (data) => data.Date     },
                    { ColumnKey.Character, (data) => data.Chara    },
                    { ColumnKey.Level,     (data) => data.Rank     },
                    { ColumnKey.Stage,     (data) => data.Stage    },
                    { ColumnKey.Score,     (data) => data.Score    },
                    { ColumnKey.SlowRate,  (data) => data.SlowRate },
                    { ColumnKey.Version,   (data) => data.Version  },
                    { ColumnKey.Comment,   (data) => data.Comment  },
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
                };

            internal enum ColumnKey
            {
#pragma warning disable SA1413 // Use trailing comma in multi-line initializers
                Filename = 0,
                LastWriteDate,
                Number,
                Player,
                PlayTime,
                Character,
                Level,
                Stage,
                Score,
                SlowRate,
                Version,
                Comment,
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
                    var number = string.Empty;
                    if (size == 0u)
                    {
                        var path = Marshal.PtrToStringAnsi(src);
                        number = ReplayDataBase.GetNumberFromPath(
                            path, @"^th16_(\d{2})\.rpy$", @"^th16_ud(.{0,4})\.rpy$");
                    }

                    var pair = CreateReplayData<ReplayData>(src, size);
                    if (pair.Item1 == ErrorCode.AllRight)
                    {
                        var fileInfoSize = Marshal.SizeOf(typeof(FileInfo));
                        var keys = Utils.GetEnumerator<ColumnKey>().Where(key => key != ColumnKey.Sentinel);

                        info = Marshal.AllocHGlobal(fileInfoSize * keys.Count());

                        var address = info.ToInt64();
                        foreach (var key in keys)
                        {
                            var fileInfo = new FileInfo { Text = string.Empty };
                            if (key == ColumnKey.Number)
                            {
                                fileInfo.Text = number;
                            }
                            else
                            {
                                if (FileInfoGetters.TryGetValue(key, out var getter))
                                {
                                    fileInfo.Text = getter(pair.Item2);
                                }
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
                    var pair = CreateReplayData<ReplayData>(src, size);
                    if (pair.Item1 == ErrorCode.AllRight)
                    {
                        var bytes = Encoding.CP932.GetBytes(pair.Item2.Info);
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
                    var pair = CreateReplayData<ReplayData>(src, size);
                    if (pair.Item1 == ErrorCode.AllRight)
                    {
                        var bytes = Encoding.CP932.GetBytes(pair.Item2.Comment);
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
                var result = DialogResult.None;

                var pair = CreateReplayData<ReplayData>(file);
                if (pair.Item1 == ErrorCode.AllRight)
                {
                    using var dialog = new EditDialog
                    {
                        Content = pair.Item2.Comment,
                    };

                    result = dialog.ShowDialog(new Win32Window(parent));
                    if (result == DialogResult.OK)
                    {
                        pair.Item2.Comment = dialog.Content + "\0\0";
                        pair.Item2.Write(file);
                    }
                }

                return (result == DialogResult.OK) ? ErrorCode.AllRight : ErrorCode.DialogCanceled;
            }

            public override ErrorCode ConfigDialog(IntPtr parent)
            {
                throw new NotImplementedException();
            }
        }
    }
}
