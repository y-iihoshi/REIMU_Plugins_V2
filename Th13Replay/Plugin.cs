//-----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed.")]

namespace ReimuPlugins.Th13Replay
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
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
        public static ErrorCode GetFileInfoText2(IntPtr src, uint size, out IntPtr dst)
        {
            return Impl.GetFileInfoText2(src, size, out dst);
        }

        [DllExport]
        public static ErrorCode EditDialog(IntPtr parent, string file)
        {
            return Impl.EditDialog(parent, file);
        }

        private sealed class PluginImpl : ReimuPluginRev1<PluginImpl.ColumnKey>
        {
            private static readonly string ValidSignature = "t13r".ToCP932();

            private static readonly string[] PluginInfoImpl =
            {
                "REIMU Plug-in For 東方神霊廟 Ver2.00 (C) IIHOSHI Yoshinori, 2015\0".ToCP932(),
                "東方神霊廟\0".ToCP932(),
                "th13_*.rpy\0".ToCP932(),
                "東方神霊廟 リプレイファイル (th13_*.rpy)\0".ToCP932(),
            };

            private static readonly Dictionary<ColumnKey, ColumnInfo> Columns =
                new Dictionary<ColumnKey, ColumnInfo>
                {
                    {
                        ColumnKey.Filename,
                        new ColumnInfo
                        {
                            Title = "ファイル名\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.Title,
                        }
                    },
                    {
                        ColumnKey.LastWriteDate,
                        new ColumnInfo
                        {
                            Title = "更新日時\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.LastWriteTime,
                        }
                    },
                    {
                        ColumnKey.Number,
                        new ColumnInfo
                        {
                            Title = "No.\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.Player,
                        new ColumnInfo
                        {
                            Title = "プレイヤー名\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.PlayTime,
                        new ColumnInfo
                        {
                            Title = "プレイ時刻\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.Character,
                        new ColumnInfo
                        {
                            Title = "使用キャラ\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.Level,
                        new ColumnInfo
                        {
                            Title = "難易度\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.Stage,
                        new ColumnInfo
                        {
                            Title = "ステージ\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.Score,
                        new ColumnInfo
                        {
                            Title = "スコア\0".ToCP932(),
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.SlowRate,
                        new ColumnInfo
                        {
                            Title = "処理落ち率\0".ToCP932(),
                            Align = TextAlign.Right,
                            Sort = SortType.Float,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.Version,
                        new ColumnInfo
                        {
                            Title = "バージョン\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.Comment,
                        new ColumnInfo
                        {
                            Title = "コメント\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String,
                        }
                    },
                    {
                        ColumnKey.FileSize,
                        new ColumnInfo
                        {
                            Title = "ファイルサイズ\0".ToCP932(),
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.FileSize,
                        }
                    },
                    {
                        ColumnKey.Directory,
                        new ColumnInfo
                        {
                            Title = "ディレクトリ\0".ToCP932(),
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

            [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:CodeMustNotContainMultipleWhitespaceInARow", Justification = "Reviewed.")]
            private static readonly Dictionary<ColumnKey, Func<ReplayData, string>> FileInfoGetters =
                new Dictionary<ColumnKey, Func<ReplayData, string>>
                {
                    { ColumnKey.Player,    (data) => data.Name     },
                    { ColumnKey.PlayTime,  (data) => data.Date     },
                    { ColumnKey.Character, (data) => data.Chara    },
                    { ColumnKey.Level,     (data) => data.Rank     },
                    { ColumnKey.Stage,     (data) => data.Stage    },
                    { ColumnKey.Score,     (data) => data.Score    },
                    { ColumnKey.SlowRate,  (data) => data.SlowRate },
                    { ColumnKey.Version,   (data) => data.Version  },
                    { ColumnKey.Comment,   (data) => data.Comment  },
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

            protected override ReadOnlyCollection<string> ManagedPluginInfo
            {
                get { return Array.AsReadOnly(PluginInfoImpl); }
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
                            var reader = new IO.BinaryReader(pair.Item2);
                            var readSize = Math.Min((int)reader.BaseStream.Length, ValidSignature.Length);
                            signature = Enc.CP932.GetString(reader.ReadBytes(readSize));
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
                    var number = string.Empty;
                    if (size == 0u)
                    {
                        var path = Marshal.PtrToStringAnsi(src);
                        number = ThReplayData.GetNumberFromPath(
                            path, @"^th13_(\d{2})\.rpy$", @"^th13_ud(.{0,4})\.rpy$");
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
                                Func<ReplayData, string> getter;
                                if (FileInfoGetters.TryGetValue(key, out getter))
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
                        var bytes = Enc.CP932.GetBytes(pair.Item2.Info);
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
                        var bytes = Enc.CP932.GetBytes(pair.Item2.Comment);
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
                    using (var dialog = new EditDialog())
                    {
                        dialog.Content = pair.Item2.Comment;
                        result = dialog.ShowDialog(new Win32Window(parent));
                        if (result == DialogResult.OK)
                        {
                            pair.Item2.Comment = dialog.Content + "\0\0";
                            pair.Item2.Write(file);
                        }
                    }
                }

                return (result == DialogResult.OK) ? ErrorCode.AllRight : ErrorCode.DialogCanceled;
            }

            public override ErrorCode ConfigDialog(IntPtr parent)
            {
                throw new NotImplementedException();
            }

            private static Tuple<ErrorCode, T> CreateReplayData<T>(IntPtr src, uint size)
                where T : ThReplayData, new()
            {
                using (var pair = ReimuPluginRev1<ColumnKey>.CreateStream(src, size))
                {
                    T replay = null;

                    if (pair.Item1 == ErrorCode.AllRight)
                    {
                        replay = new T();
                        replay.Read(pair.Item2);
                    }

                    return Tuple.Create(pair.Item1, replay);
                }
            }

            private static Tuple<ErrorCode, T> CreateReplayData<T>(string path)
                where T : ThReplayData, new()
            {
                using (var stream = new IO.FileStream(path, IO.FileMode.Open, IO.FileAccess.Read))
                {
                    var replay = new T();
                    replay.Read(stream);
                    return Tuple.Create(ErrorCode.AllRight, replay);
                }
            }
        }
    }
}
