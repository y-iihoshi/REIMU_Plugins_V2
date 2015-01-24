//-----------------------------------------------------------------------
// <copyright file="Th11ReplayPlugin.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ReimuPlugins.Common;
using RGiesecke.DllExport;
using IO = System.IO;

[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed.")]

namespace ReimuPlugins.Th11Replay
{
    public sealed class Th11ReplayPlugin
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
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Reviewed.")]
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
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Reviewed.")]
        public static ErrorCode GetFileInfoList(IntPtr src, uint size, out IntPtr info)
        {
            return Impl.GetFileInfoList(src, size, out info);
        }

        private sealed class PluginImpl : IReimuPluginRev1
        {
            private static readonly string ValidSignature = "t11r".ToCP932();

            private static readonly string[] PluginInfo =
            {
                "REIMU Plug-in For 東方地霊殿 Ver2.00 (C) IIHOSHI Yoshinori, 2015\0".ToCP932(),
                "東方地霊殿\0".ToCP932(),
                "th11_*.rpy\0".ToCP932(),
                "東方地霊殿 リプレイファイル (th11_*.rpy)\0".ToCP932(),
            };

            private static readonly ColumnInfo[] Columns =
            {
                new ColumnInfo
                {
                    Title = "ファイル名\0".ToCP932(),
                    Align = TextAlign.Left,
                    Sort = SortType.String,
                    System = SystemInfoType.Title
                },
                new ColumnInfo
                {
                    Title = "更新日時\0".ToCP932(),
                    Align = TextAlign.Left,
                    Sort = SortType.String,
                    System = SystemInfoType.LastWriteTime
                },
                new ColumnInfo
                {
                    Title = "No.\0".ToCP932(),
                    Align = TextAlign.Left,
                    Sort = SortType.String,
                    System = SystemInfoType.String
                },
                new ColumnInfo
                {
                    Title = "プレイヤー名\0".ToCP932(),
                    Align = TextAlign.Left,
                    Sort = SortType.String,
                    System = SystemInfoType.String
                },
                new ColumnInfo
                {
                    Title = "プレイ時刻\0".ToCP932(),
                    Align = TextAlign.Left,
                    Sort = SortType.String,
                    System = SystemInfoType.String
                },
                new ColumnInfo
                {
                    Title = "使用キャラ\0".ToCP932(),
                    Align = TextAlign.Left,
                    Sort = SortType.String,
                    System = SystemInfoType.String
                },
                new ColumnInfo
                {
                    Title = "難易度\0".ToCP932(),
                    Align = TextAlign.Left,
                    Sort = SortType.String,
                    System = SystemInfoType.String
                },
                new ColumnInfo
                {
                    Title = "ステージ\0".ToCP932(),
                    Align = TextAlign.Left,
                    Sort = SortType.String,
                    System = SystemInfoType.String
                },
                new ColumnInfo
                {
                    Title = "スコア\0".ToCP932(),
                    Align = TextAlign.Right,
                    Sort = SortType.Number,
                    System = SystemInfoType.String
                },
                new ColumnInfo
                {
                    Title = "処理落ち率\0".ToCP932(),
                    Align = TextAlign.Right,
                    Sort = SortType.Float,
                    System = SystemInfoType.String
                },
                new ColumnInfo
                {
                    Title = "バージョン\0".ToCP932(),
                    Align = TextAlign.Left,
                    Sort = SortType.String,
                    System = SystemInfoType.String
                },
                new ColumnInfo
                {
                    Title = "コメント\0".ToCP932(),
                    Align = TextAlign.Left,
                    Sort = SortType.String,
                    System = SystemInfoType.String
                },
                new ColumnInfo
                {
                    Title = "ファイルサイズ\0".ToCP932(),
                    Align = TextAlign.Right,
                    Sort = SortType.Number,
                    System = SystemInfoType.FileSize
                },
                new ColumnInfo
                {
                    Title = "ディレクトリ\0".ToCP932(),
                    Align = TextAlign.Left,
                    Sort = SortType.String,
                    System = SystemInfoType.Directory
                },
                new ColumnInfo  // sentinel
                {
                    Title = "\0",
                    Align = TextAlign.Left,
                    Sort = SortType.String,
                    System = SystemInfoType.String
                }
            };

            public Revision GetPluginRevision()
            {
                return Revision.Rev1;
            }

            public int GetPluginInfo(int index, IntPtr info, uint size)
            {
                if ((0 <= index) && (index < PluginInfo.Length))
                {
                    var byteCount = Enc.CP932.GetByteCount(PluginInfo[index]);
                    if (info == IntPtr.Zero)
                    {
                        return byteCount - 1;       // except null terminator
                    }
                    else
                    {
                        if (size >= byteCount)
                        {
                            Marshal.Copy(Enc.CP932.GetBytes(PluginInfo[index]), 0, info, byteCount);
                            return byteCount - 1;   // except null terminator
                        }
                    }
                }

                return 0;
            }

            public ErrorCode GetColumnInfo(out IntPtr info)
            {
                try
                {
                    var size = Marshal.SizeOf(typeof(ColumnInfo));

                    info = Marshal.AllocHGlobal(size * Columns.Length);

                    var address = info.ToInt64();
                    foreach (var column in Columns)
                    {
                        var pointer = new IntPtr(address);
                        Marshal.StructureToPtr(column, pointer, false);
                        address += size;
                    }
                }
                catch (OutOfMemoryException)
                {
                    info = IntPtr.Zero;
                    return ErrorCode.NoMemory;
                }

                return ErrorCode.AllRight;
            }

            public uint IsSupported(IntPtr src, uint size)
            {
                if (src == IntPtr.Zero)
                {
                    return (uint)ValidSignature.Length;
                }

                var signature = string.Empty;

                try
                {
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
                }
                catch (Exception)
                {
                    return 0u;
                }

                return (signature == ValidSignature) ? 1u : 0u;
            }

            public ErrorCode GetFileInfoList(IntPtr src, uint size, out IntPtr info)
            {
                try
                {
                    info = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FileInfo)) * (Columns.Length - 1));
                }
                catch (OutOfMemoryException)
                {
                    info = IntPtr.Zero;
                    return ErrorCode.NoMemory;
                }

                return ErrorCode.AllRight;
            }

            public ErrorCode GetFileInfoText1(IntPtr src, uint size, out IntPtr dst)
            {
                throw new NotImplementedException();
            }

            public ErrorCode GetFileInfoText2(IntPtr src, uint size, out IntPtr dst)
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
        }
    }
}
