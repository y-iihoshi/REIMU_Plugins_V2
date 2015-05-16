//-----------------------------------------------------------------------
// <copyright file="Th135ReplayPlugin.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed.")]

namespace ReimuPlugins.Th135Replay
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
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

        private sealed class PluginImpl : ReimuPluginRev1<PluginImpl.ColumnKey>
        {
            private static readonly string ValidSignature = "TFRP\0".ToCP932();

            private static readonly string[] PluginInfoImpl =
            {
                "REIMU Plug-in For 東方心綺楼 Ver2.00 (C) IIHOSHI Yoshinori, 2015\0".ToCP932(),
                "東方心綺楼\0".ToCP932(),
                "*.rep\0".ToCP932(),
                "東方心綺楼 リプレイファイル (*.rep)\0".ToCP932(),
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
                            System = SystemInfoType.Title
                        }
                    },
                    {
                        ColumnKey.LastWriteDate,
                        new ColumnInfo
                        {
                            Title = "更新日時\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.LastWriteTime
                        }
                    },
                    {
                        ColumnKey.Version,
                        new ColumnInfo
                        {
                            Title = "バージョン\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.Number,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnKey.GameMode,
                        new ColumnInfo
                        {
                            Title = "モード\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.Number,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnKey.Player0,
                        new ColumnInfo
                        {
                            Title = "プレイヤー 1\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.Number,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnKey.Color0,
                        new ColumnInfo
                        {
                            Title = "色 1\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.Number,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnKey.Player1,
                        new ColumnInfo
                        {
                            Title = "プレイヤー 2\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.Number,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnKey.Color1,
                        new ColumnInfo
                        {
                            Title = "色 2\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.Number,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnKey.Background,
                        new ColumnInfo
                        {
                            Title = "ステージ\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.Number,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnKey.Bgm,
                        new ColumnInfo
                        {
                            Title = "BGM\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.Number,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnKey.Profile0,
                        new ColumnInfo
                        {
                            Title = "Profile 1\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnKey.Profile1,
                        new ColumnInfo
                        {
                            Title = "Profile 2\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnKey.DateTime,
                        new ColumnInfo
                        {
                            Title = "リプレイ保存日時\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnKey.FileSize,
                        new ColumnInfo
                        {
                            Title = "ファイルサイズ\0".ToCP932(),
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.FileSize
                        }
                    },
                    {
                        ColumnKey.Directory,
                        new ColumnInfo
                        {
                            Title = "ディレクトリ\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.Directory
                        }
                    },
                    {
                        ColumnKey.Sentinel,
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
            private static readonly Dictionary<ColumnKey, Func<ReplayData, string>> FileInfoGetters =
                new Dictionary<ColumnKey, Func<ReplayData, string>>
                {
                    {
                        ColumnKey.Version,
                        (data) => data.Version.ToCP932()
                    },
                    {
                        ColumnKey.GameMode,
                        (data) => data.GameMode.ToShortName().ToCP932()
                    },
                    {
                        ColumnKey.Player0,
                        (data) => data.Character0.ToLongName().ToCP932()
                    },
                    {
                        ColumnKey.Color0,
                        (data) => data.Color0.ToString(CultureInfo.CurrentCulture).ToCP932()
                    },
                    {
                        ColumnKey.Profile0,
                        (data) => data.Profile0Name
                    },
                    {
                        ColumnKey.Player1,
                        (data) => ((data.GameMode == GameMode.Story)
                            ? "-" : data.Character1.ToLongName()).ToCP932()
                    },
                    {
                        ColumnKey.Color1,
                        (data) => ((data.GameMode == GameMode.Story)
                            ? "-" : data.Color1.ToString(CultureInfo.CurrentCulture)).ToCP932()
                    },
                    {
                        ColumnKey.Profile1,
                        (data) => ((data.GameMode == GameMode.Story)
                            ? "-".ToCP932() : data.Profile1Name)
                    },
                    {
                        ColumnKey.Background,
                        (data) => ((data.GameMode == GameMode.Story) ? "-" : data.BackgroundName).ToCP932()
                    },
                    {
                        ColumnKey.Bgm,
                        (data) => ((data.GameMode == GameMode.Story) ? "-" : data.BgmName).ToCP932()
                    },
                    {
                        ColumnKey.DateTime,
                        (data) => data.DateTime.ToString(CultureInfo.CurrentCulture).ToCP932()
                    },
                };

            internal enum ColumnKey
            {
                Filename = 0,
                LastWriteDate,
                Version,
                GameMode,
                Player0,
                Color0,
                Profile0,
                Player1,
                Color1,
                Profile1,
                Background,
                Bgm,
                DateTime,
                FileSize,
                Directory,
                Sentinel
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
                            Func<ReplayData, string> getter;
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
                    var pair = CreateReplayData(src, size);
                    if (pair.Item1 == ErrorCode.AllRight)
                    {
                        var bytes = Enc.CP932.GetBytes(pair.Item2.Player0Info.ToCStr().ToCP932());
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
                            ? string.Empty : pair.Item2.Player1Info;
                        var bytes = Enc.CP932.GetBytes(text.ToCStr().ToCP932());
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

            private static Tuple<ErrorCode, ReplayData> CreateReplayData(IntPtr src, uint size)
            {
                using (var pair = ReimuPluginRev1<ColumnKey>.CreateStream(src, size))
                {
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
    }
}
