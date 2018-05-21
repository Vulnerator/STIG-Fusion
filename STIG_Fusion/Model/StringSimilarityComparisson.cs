using System;
using System.Collections.Generic;

namespace STIG_Fusion.Model
{
    public static class StringSimilarityComparisson
    {
        public static bool CompareTitles(this string oldtitle, string newTitle)
        {
            int distance = DamerauLevenshteinDistance(oldtitle, newTitle);
            int length = oldtitle.Length > newTitle.Length ? oldtitle.Length : newTitle.Length;
            double score = 1.0 - (double)distance / length;

            return score >= 0.85;
        }
        
        public static int DamerauLevenshteinDistance(string oldTitle, string newTitle)
        {
            if (oldTitle == newTitle)
            { return 0; }

            if (String.IsNullOrWhiteSpace(oldTitle) || String.IsNullOrWhiteSpace(newTitle))
            {
                return (oldTitle ?? String.Empty).Length + (newTitle ?? String.Empty).Length;
            }

            if (oldTitle.Length > newTitle.Length)
            {
                var tmp = oldTitle;
                oldTitle = newTitle;
                newTitle = tmp;
            }

            if (newTitle.Contains(oldTitle))
            { return newTitle.Length - oldTitle.Length; }

            int m = oldTitle.Length;
            int n = newTitle.Length;
            int[,] H = new int[m + 2, n + 2];
            int inf = m + n;
            H[0, 0] = inf;

            for (int i = 0; i <= m; i++)
            {
                H[i + 1, 1] = i;
                H[i + 1, 0] = inf;
            }

            for (int j = 0; j <= m; j++)
            {
                H[1, j + 1] = j;
                H[0, j + 1] = inf;
            }

            SortedDictionary<char, int> sd = new SortedDictionary<char, int>();
            foreach (char l in (oldTitle + newTitle))
            {
                if (!sd.ContainsKey(l))
                { sd.Add(l, 0); }
            }

            for (int i = 1; i <= m; i++)
            {
                int db = 0;
                for (int j = 1; j <= n; j++)
                {
                    int i1 = sd[newTitle[j - 1]];
                    int j1 = db;

                    if (oldTitle[i - 1] == newTitle[j-1])
                    {
                        H[i + 1, j + 1] = H[i, j];
                        db = j;
                    }
                    else
                    {
                        H[i + 1, j + 1] = Math.Min(H[i, j], Math.Min(H[i + 1, j], H[i, j + 1])) + 1;
                    }
                    H[i + 1, j + 1] = Math.Min(H[i + 1, j + 1], H[i1, j1] + (i - i1 - 1) + 1 + (j - j1 - 1));
                }

                sd[oldTitle[i - 1]] = i;
            }

            return H[m + 1, n + 1];
        }
    }
}
