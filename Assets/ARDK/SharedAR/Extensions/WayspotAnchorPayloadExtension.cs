using Niantic.ARDK.AR.WayspotAnchors;

namespace Niantic.Experimental.ARDK.SharedAR.Extensions
{
  public static class WayspotAnchorPayloadExtension
  {
    internal static byte[] _CopyBlob(this WayspotAnchorPayload wayspotAnchorPayload)
    {
      return (byte[])wayspotAnchorPayload._Blob.Clone();
    }
  }
}
