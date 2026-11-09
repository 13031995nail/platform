namespace Tmobis.Utils.Lzw.Converters;
using System.Collections.Generic;
public interface ILzwConverter
{
    List<int> CompressLzw(string uncompressed);

    string Decompress(List<int> compressed);
}