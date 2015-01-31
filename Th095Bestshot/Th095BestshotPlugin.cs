//-----------------------------------------------------------------------
// <copyright file="Th095BestshotPlugin.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed.")]

namespace ReimuPlugins.Th095Bestshot
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using ReimuPlugins.Common;
    using RGiesecke.DllExport;
    using IO = System.IO;

    public static class Th095BestshotPlugin
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
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "To comply with the REIMU plugin spec.")]
        public static ErrorCode GetFileInfoImage1(IntPtr src, uint size, out IntPtr dst, out IntPtr info)
        {
            return Impl.GetFileInfoImage1(src, size, out dst, out info);
        }

        private sealed class PluginImpl : ReimuPluginRev2<PluginImpl.ColumnKey>
        {
            private static readonly string ValidSignature = "BSTS".ToCP932();

            private static readonly string[] PluginInfo =
            {
                "REIMU Plug-in For 東方文花帖 ベストショット Ver2.00 (C) IIHOSHI Yoshinori, 2015\0".ToCP932(),
                "東方文花帖 ベストショット\0".ToCP932(),
                "bs_*.dat\0".ToCP932(),
                "東方文花帖 ベストショットファイル (bs_*.dat)\0".ToCP932(),
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
                        ColumnKey.Scene,
                        new ColumnInfo
                        {
                            Title = "シーン\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnKey.CardName,
                        new ColumnInfo
                        {
                            Title = "スペルカード名\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnKey.Width,
                        new ColumnInfo
                        {
                            Title = "幅\0".ToCP932(),
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnKey.Height,
                        new ColumnInfo
                        {
                            Title = "高さ\0".ToCP932(),
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnKey.Score,
                        new ColumnInfo
                        {
                            Title = "評価点\0".ToCP932(),
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnKey.SlowRate,
                        new ColumnInfo
                        {
                            Title = "処理落ち率\0".ToCP932(),
                            Align = TextAlign.Right,
                            Sort = SortType.Float,
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
            private static readonly Dictionary<ColumnKey, Func<Th095BestshotData, string>> FileInfoGetters =
                new Dictionary<ColumnKey, Func<Th095BestshotData, string>>
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
                    var replay = CreateTh095BestshotData(src, size, ref errorCode);
                    if (errorCode != ErrorCode.FileReadError)
                    {
                        var fileInfoSize = Marshal.SizeOf(typeof(FileInfo));
                        var keys = Utils.GetEnumerator<ColumnKey>().Where(key => key != ColumnKey.Sentinel);

                        info = Marshal.AllocHGlobal(fileInfoSize * keys.Count());

                        var address = info.ToInt64();
                        foreach (var key in keys)
                        {
                            var fileInfo = new FileInfo { Text = string.Empty };
                            Func<Th095BestshotData, string> getter;
                            if (FileInfoGetters.TryGetValue(key, out getter))
                            {
                                fileInfo.Text = getter(replay);
                            }

                            var pointer = new IntPtr(address);
                            Marshal.StructureToPtr(fileInfo, pointer, false);
                            address += fileInfoSize;
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
                throw new NotImplementedException();
            }

            public override ErrorCode GetFileInfoText2(IntPtr src, uint size, out IntPtr dst)
            {
                throw new NotImplementedException();
            }

            public override ErrorCode GetFileInfoImage1(IntPtr src, uint size, out IntPtr dst, out IntPtr info)
            {
                dst = IntPtr.Zero;
                info = IntPtr.Zero;

                return ErrorCode.AllRight;
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

            private static Th095BestshotData CreateTh095BestshotData(IntPtr src, uint size, ref ErrorCode errorCode)
            {
                if (size > 0u)
                {
                    var replay = new Th095BestshotData();
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
                    return CreateTh095BestshotData(path, ref errorCode);
                }
            }

            private static Th095BestshotData CreateTh095BestshotData(string path, ref ErrorCode errorCode)
            {
                var replay = new Th095BestshotData();

                try
                {
                    replay.Read(path);
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
