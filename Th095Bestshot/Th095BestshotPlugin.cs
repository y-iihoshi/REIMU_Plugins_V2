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

        private sealed class PluginImpl : IReimuPluginRev2
        {
            private static readonly string ValidSignature = "BSTS".ToCP932();

            private static readonly string[] PluginInfo =
            {
                "REIMU Plug-in For 東方文花帖 ベストショット Ver2.00 (C) IIHOSHI Yoshinori, 2015\0".ToCP932(),
                "東方文花帖 ベストショット\0".ToCP932(),
                "bs_*.dat\0".ToCP932(),
                "東方文花帖 ベストショットファイル (bs_*.dat)\0".ToCP932(),
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
                        ColumnIndex.Scene,
                        new ColumnInfo
                        {
                            Title = "シーン\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnIndex.CardName,
                        new ColumnInfo
                        {
                            Title = "スペルカード名\0".ToCP932(),
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnIndex.Width,
                        new ColumnInfo
                        {
                            Title = "幅\0".ToCP932(),
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnIndex.Height,
                        new ColumnInfo
                        {
                            Title = "高さ\0".ToCP932(),
                            Align = TextAlign.Right,
                            Sort = SortType.Number,
                            System = SystemInfoType.String
                        }
                    },
                    {
                        ColumnIndex.Score,
                        new ColumnInfo
                        {
                            Title = "評価点\0".ToCP932(),
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
            private static readonly Dictionary<ColumnIndex, Func<Th095BestshotData, string>> FileInfoGetters =
                new Dictionary<ColumnIndex, Func<Th095BestshotData, string>>
                {
                    {
                        ColumnIndex.Scene,
                        (data) => string.Format(
                            CultureInfo.CurrentCulture, "{0}-{1}", data.Level, data.Scene)
                    },
                    {
                        ColumnIndex.CardName,
                        (data) => data.CardName
                    },
                    {
                        ColumnIndex.Width,
                        (data) => data.Width.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnIndex.Height,
                        (data) => data.Height.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnIndex.Score,
                        (data) => data.Score.ToString(CultureInfo.CurrentCulture)
                    },
                    {
                        ColumnIndex.SlowRate,
                        (data) => data.SlowRate.ToString("F6", CultureInfo.CurrentCulture)
                    },
                };

            private enum ColumnIndex
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

            public Revision GetPluginRevision()
            {
                return Revision.Rev2;
            }

            public int GetPluginInfo(int index, IntPtr info, uint size)
            {
                try
                {
                    var byteCount = Enc.CP932.GetByteCount(PluginInfo[index]);
                    if (info == IntPtr.Zero)
                    {
                        return byteCount - 1;   // except a null terminator
                    }
                    else
                    {
                        if (size >= byteCount)
                        {
                            Marshal.Copy(Enc.CP932.GetBytes(PluginInfo[index]), 0, info, byteCount);
                            return byteCount - 1;   // except a null terminator
                        }
                    }
                }
                catch (IndexOutOfRangeException)
                {
                }
                catch (ArgumentNullException)
                {
                }
                catch (EncoderFallbackException)
                {
                }
                catch (ArgumentOutOfRangeException)
                {
                }

                return 0;
            }

            public ErrorCode GetColumnInfo(out IntPtr info)
            {
                var errorCode = ErrorCode.UnknownError;

                info = IntPtr.Zero;

                try
                {
                    var size = Marshal.SizeOf(typeof(ColumnInfo));

                    info = Marshal.AllocHGlobal(size * Columns.Count);

                    var address = info.ToInt64();
                    foreach (var index in Utils.GetEnumerator<ColumnIndex>())
                    {
                        var pointer = new IntPtr(address);
                        Marshal.StructureToPtr(Columns[index], pointer, false);
                        address += size;
                    }

                    errorCode = ErrorCode.AllRight;
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

            public uint IsSupported(IntPtr src, uint size)
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

            public ErrorCode GetFileInfoList(IntPtr src, uint size, out IntPtr info)
            {
                var errorCode = ErrorCode.UnknownError;

                info = IntPtr.Zero;

                try
                {
                    var replay = CreateTh095BestshotData(src, size, ref errorCode);
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
                                Func<Th095BestshotData, string> getter;
                                if (FileInfoGetters.TryGetValue(index, out getter))
                                {
                                    fileInfo.Text = getter(replay);
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

            public ErrorCode GetFileInfoText1(IntPtr src, uint size, out IntPtr dst)
            {
                throw new NotImplementedException();
            }

            public ErrorCode GetFileInfoText2(IntPtr src, uint size, out IntPtr dst)
            {
                throw new NotImplementedException();
            }

            public ErrorCode GetFileInfoImage1(IntPtr src, uint size, out IntPtr dst, out IntPtr info)
            {
                dst = IntPtr.Zero;
                info = IntPtr.Zero;

                return ErrorCode.AllRight;
            }

            public ErrorCode GetFileInfoImage2(IntPtr src, uint size, out IntPtr dst, out IntPtr info)
            {
                throw new NotImplementedException();
            }

            public ErrorCode EditDialog(IntPtr parent, string file)
            {
                throw new NotImplementedException();
            }

            public ErrorCode ConfigDialog(IntPtr parent)
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
