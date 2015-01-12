using System;

namespace ReimuPlugins.Common
{
    public interface IReimuPluginRev2 : IReimuPluginRev1
    {
        #region Optional Methods

        ErrorCode GetFileInfoImage1(IntPtr src, uint size, out IntPtr dst, out IntPtr info);

        ErrorCode GetFileInfoImage2(IntPtr src, uint size, out IntPtr dst, out IntPtr info);

        #endregion
    }
}
