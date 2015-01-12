using System;

namespace ReimuPlugins.Common
{
    public interface IReimuPluginRev1
    {
        #region Required Methods

        Revision GetPluginRevision();

        int GetPluginInfo(int index, IntPtr info, uint size);

        ErrorCode GetColumnInfo(out IntPtr info);

        uint IsSupported(IntPtr src, uint size);

        ErrorCode GetFileInfoList(IntPtr src, uint size, out IntPtr info);

        #endregion

        #region Optional Methods

        ErrorCode GetFileInfoText1(IntPtr src, uint size, out IntPtr dst);

        ErrorCode GetFileInfoText2(IntPtr src, uint size, out IntPtr dst);

        ErrorCode EditDialog(IntPtr hWnd, string file);

        ErrorCode ConfigDialog(IntPtr hWnd);

        #endregion
    }
}
