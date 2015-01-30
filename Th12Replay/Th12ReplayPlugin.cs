//-----------------------------------------------------------------------
// <copyright file="Th12ReplayPlugin.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed.")]

namespace ReimuPlugins.Th12Replay
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Windows.Forms;
    using ReimuPlugins.Common;
    using RGiesecke.DllExport;
    using IO = System.IO;

    public static class Th12ReplayPlugin
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
        public static ErrorCode GetFileInfoText2(IntPtr src, uint size, out IntPtr dst)
        {
            return Impl.GetFileInfoText2(src, size, out dst);
        }

        [DllExport]
        public static ErrorCode EditDialog(IntPtr parent, string file)
        {
            return Impl.EditDialog(parent, file);
        }

        private sealed class PluginImpl : ReimuPluginRev1<PluginImpl.ColumnIndex>
        {
            private static readonly string ValidSignature = "t12r".ToCP932();

            private static readonly string[] PluginInfoImpl =
            {
                "REIMU Plug-in For 東方星蓮船 Ver2.00 (C) IIHOSHI Yoshinori, 2015\0".ToCP932(),
                "東方星蓮船\0".ToCP932(),
                "th12_*.rpy\0".ToCP932(),
                "東方星蓮船 リプレイファイル (th12_*.rpy)\0".ToCP932(),
            };

            private static readonly Dictionary<ColumnIndex, ColumnInfo> Columns =
                new Dictionary<ColumnIndex, ColumnInfo>
                {
                    {
                        ColumnIndex.Filename,
                        new ColumnInfo
                        {
                            Title = "ファイル名\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.Title
                        }
                    },
                    {
                        ColumnIndex.LastWriteDate,
                        new ColumnInfo
                        {
                            Title = "更新日時\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.LastWriteTime
                        }
                    },
                    {
                        ColumnIndex.Number,
                        new ColumnInfo
                        {
                            Title = "No.\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnIndex.Player,
                        new ColumnInfo
                        {
                            Title = "プレイヤー名\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnIndex.PlayTime,
                        new ColumnInfo
                        {
                            Title = "プレイ時刻\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnIndex.Character,
                        new ColumnInfo
                        {
                            Title = "使用キャラ\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnIndex.Level,
                        new ColumnInfo
                        {
                            Title = "難易度\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnIndex.Stage,
                        new ColumnInfo
                        {
                            Title = "ステージ\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnIndex.Score,
                        new ColumnInfo
                        {
                            Title = "スコア\0".ToCP932(),
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnIndex.SlowRate,
                        new ColumnInfo
                        {
                            Title = "処理落ち率\0".ToCP932(),
                            Align = TextAlign.Right,
                            Sort = SortType.Float,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnIndex.Version,
                        new ColumnInfo
                        {
                            Title = "バージョン\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnIndex.Comment,
                        new ColumnInfo
                        {
                            Title = "コメント\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnIndex.FileSize,
                        new ColumnInfo
                        {
                            Title = "ファイルサイズ\0".ToCP932(),
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.FileSize
                        }
                    },
                    {
                        ColumnIndex.Directory,
                        new ColumnInfo
                        {
                            Title = "ディレクトリ\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.Directory
                        }
                    },
                    {
                        ColumnIndex.Sentinel,
                        new ColumnInfo
                        {
                            Title = "\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String
                        }
                    }
                };

            [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:CodeMustNotContainMultipleWhitespaceInARow", Justification = "Reviewed.")]
            private static readonly Dictionary<ColumnIndex, Func<Th12ReplayData, string>> FileInfoGetters =
                new Dictionary<ColumnIndex, Func<Th12ReplayData, string>>
                {
                    { ColumnIndex.Player,    (data) => data.Name     },
                    { ColumnIndex.PlayTime,  (data) => data.Date     },
                    { ColumnIndex.Character, (data) => data.Chara    },
                    { ColumnIndex.Level,     (data) => data.Rank     },
                    { ColumnIndex.Stage,     (data) => data.Stage    },
                    { ColumnIndex.Score,     (data) => data.Score    },
                    { ColumnIndex.SlowRate,  (data) => data.SlowRate },
                    { ColumnIndex.Version,   (data) => data.Version  },
                    { ColumnIndex.Comment,   (data) => data.Comment  },
                };

            internal enum ColumnIndex
            {
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
            }

            protected override ReadOnlyCollection<string> ManagedPluginInfo
            {
                get { return Array.AsReadOnly(PluginInfoImpl); }
            }

            protected override IDictionary<PluginImpl.ColumnIndex, ColumnInfo> ManagedColumnInfo
            {
                get { return Columns; }
            }

            public override uint IsSupported(IntPtr src, uint size)
            {
                if (src == IntPtr.Zero)
                {
                    return (uint)ValidSignature.Length;
                }

                try
                {
                    var signature = string.Empty;

                    if (size > 0u)
                    {
                        var content = new byte[Math.Min(size, ValidSignature.Length)];
                        Marshal.Copy(src, content, 0, content.Length);
                        signature = Enc.CP932.GetString(content);
                    }
                    else
                    {
                        var path = Marshal.PtrToStringAnsi(src);
                        using (var stream = new IO.FileStream(path, IO.FileMode.Open, IO.FileAccess.Read))
                        {
                            var reader = new IO.BinaryReader(stream);
                            var readSize = Math.Min((int)reader.BaseStream.Length, ValidSignature.Length);
                            signature = Enc.CP932.GetString(reader.ReadBytes(readSize));
                        }
                    }

                    return (signature == ValidSignature) ? 1u : 0u;
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
                catch (SecurityException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (ObjectDisposedException)
                {
                }

                return 0u;
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
                        number = ThReplayData.GetNumberFromPath(
                            path, @"^th12_(\d{2})\.rpy$", @"^th12_ud(.{0,4})\.rpy$");
                    }

                    var replay = CreateTh12ReplayData(src, size, ref errorCode);
                    if (errorCode != ErrorCode.FileReadError)
                    {
                        var fileInfoSize = Marshal.SizeOf(typeof(FileInfo));
                        info = Marshal.AllocHGlobal(
                            fileInfoSize * Columns.Keys.Count(key => key != ColumnIndex.Sentinel));

                        var address = info.ToInt64();
                        foreach (var index in Utils.GetEnumerator<ColumnIndex>())
                        {
                            if (index != ColumnIndex.Sentinel)
                            {
                                var fileInfo = new FileInfo { Text = string.Empty };
                                if (index == ColumnIndex.Number)
                                {
                                    fileInfo.Text = number;
                                }
                                else
                                {
                                    Func<Th12ReplayData, string> getter;
                                    if (FileInfoGetters.TryGetValue(index, out getter))
                                    {
                                        fileInfo.Text = getter(replay);
                                    }
                                }
    
                                var pointer = new IntPtr(address);
                                Marshal.StructureToPtr(fileInfo, pointer, false);
                                address += fileInfoSize;
                            }
                        }

                        errorCode = ErrorCode.AllRight;
                    }
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

                if (errorCode != ErrorCode.AllRight)
                {
                    Marshal.FreeHGlobal(info);
                    info = IntPtr.Zero;
                }

                return errorCode;
            }

            public override ErrorCode GetFileInfoText1(IntPtr src, uint size, out IntPtr dst)
            {
                var errorCode = ErrorCode.UnknownError;

                dst = IntPtr.Zero;

                try
                {
                    var replay = CreateTh12ReplayData(src, size, ref errorCode);
                    if (errorCode != ErrorCode.FileReadError)
                    {
                        var bytes = Enc.CP932.GetBytes(replay.Info);
                        dst = Marshal.AllocHGlobal(bytes.Length);
                        Marshal.Copy(bytes, 0, dst, bytes.Length);

                        errorCode = ErrorCode.AllRight;
                    }
                }
                catch (OutOfMemoryException)
                {
                    errorCode = ErrorCode.NoMemory;
                }
                catch (ArgumentException)
                {
                }

                if (errorCode != ErrorCode.AllRight)
                {
                    Marshal.FreeHGlobal(dst);
                    dst = IntPtr.Zero;
                }

                return errorCode;
            }

            public override ErrorCode GetFileInfoText2(IntPtr src, uint size, out IntPtr dst)
            {
                var errorCode = ErrorCode.UnknownError;

                dst = IntPtr.Zero;

                try
                {
                    var replay = CreateTh12ReplayData(src, size, ref errorCode);
                    if (errorCode != ErrorCode.FileReadError)
                    {
                        var bytes = Enc.CP932.GetBytes(replay.Comment);
                        dst = Marshal.AllocHGlobal(bytes.Length);
                        Marshal.Copy(bytes, 0, dst, bytes.Length);

                        errorCode = ErrorCode.AllRight;
                    }
                }
                catch (OutOfMemoryException)
                {
                    errorCode = ErrorCode.NoMemory;
                }
                catch (ArgumentException)
                {
                }

                if (errorCode != ErrorCode.AllRight)
                {
                    Marshal.FreeHGlobal(dst);
                    dst = IntPtr.Zero;
                }

                return errorCode;
            }

            public override ErrorCode EditDialog(IntPtr parent, string file)
            {
                var result = DialogResult.None;

                var errorCode = ErrorCode.UnknownError;
                var replay = CreateTh12ReplayData(file, ref errorCode);
                if (errorCode != ErrorCode.FileReadError)
                {
                    using (var dialog = new EditDialog())
                    {
                        dialog.Content = replay.Comment;
                        result = dialog.ShowDialog(new Win32Window(parent));
                        if (result == DialogResult.OK)
                        {
                            replay.Comment = dialog.Content + "\0\0";
                            replay.Write(file);
                        }
                    }
                }

                return (result == DialogResult.OK) ? ErrorCode.AllRight : ErrorCode.DialogCanceled;
            }

            public override ErrorCode ConfigDialog(IntPtr parent)
            {
                throw new NotImplementedException();
            }

            private static Th12ReplayData CreateTh12ReplayData(IntPtr src, uint size, ref ErrorCode errorCode)
            {
                if (size > 0u)
                {
                    var replay = new Th12ReplayData();
                    var content = new byte[size];

                    Marshal.Copy(src, content, 0, content.Length);
                    using (var stream = new IO.MemoryStream(content, false))
                    {
                        replay.Read(stream);
                    }

                    return replay;
                }
                else
                {
                    var path = Marshal.PtrToStringAnsi(src);
                    return CreateTh12ReplayData(path, ref errorCode);
                }
            }

            private static Th12ReplayData CreateTh12ReplayData(string path, ref ErrorCode errorCode)
            {
                var replay = new Th12ReplayData();

                try
                {
                    using (var stream = new IO.FileStream(path, IO.FileMode.Open, IO.FileAccess.Read))
                    {
                        replay.Read(stream);
                    }
                }
                catch (ArgumentException)
                {
                    errorCode = ErrorCode.FileReadError;
                }
                catch (IO.IOException)
                {
                    errorCode = ErrorCode.FileReadError;
                }
                catch (NotSupportedException)
                {
                    errorCode = ErrorCode.FileReadError;
                }
                catch (SecurityException)
                {
                    errorCode = ErrorCode.FileReadError;
                }
                catch (UnauthorizedAccessException)
                {
                    errorCode = ErrorCode.FileReadError;
                }

                return replay;
            }
        }
    }
}
