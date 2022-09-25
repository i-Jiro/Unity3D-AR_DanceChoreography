#if SHARED_AR_V2

using System;

namespace Niantic.Experimental.ARDK.SharedAR
{
  public interface IPeerID: IEquatable<IPeerID>
  {
    public string ToString();
    public Guid ToGUID();
  };

} // namespace Niantic.ARDK.SharedAR

#endif // SHARED_AR_V2