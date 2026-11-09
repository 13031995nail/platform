namespace Tmobis.Utils.Lzw.Converters;
using System.Collections.Generic;
using System.Text;

public class LzwConverter : ILzwConverter
{
    public virtual List<int> CompressLzw(string uncompressed)
    {
        Dictionary<string, int> dictionary = [];
        for (int i = 0; i < 256; i++)
            dictionary.Add(((char)i).ToString(), i);

        string w = string.Empty;
        List<int> compressed = [];

        foreach (char c in uncompressed)
        {
            string wc = w + c;
            if (dictionary.ContainsKey(wc))
            {
                w = wc;
            }
            else
            {
                compressed.Add(dictionary[w]);
                dictionary.Add(wc, dictionary.Count);
                w = c.ToString();
            }
        }

        if (!string.IsNullOrEmpty(w))
            compressed.Add(dictionary[w]);

        return compressed;
    }

    public virtual string Decompress(List<int> compressed)
    {
        Dictionary<int, string> dictionary = [];
        for (int i = 0; i < 256; i++)
            dictionary.Add(i, ((char)i).ToString());

        string w = dictionary[compressed[0]];
        compressed.RemoveAt(0);
        var decompressed = new StringBuilder(w);

        foreach (int k in compressed)
        {
            string entry = null;
            if (dictionary.TryGetValue(k, out string? value))
                entry = value;
            else if (k == dictionary.Count)
                entry = w + w[0];

            decompressed.Append(entry);

            dictionary.Add(dictionary.Count, w + entry[0]);

            w = entry;
        }

        return decompressed.ToString();
    }
}