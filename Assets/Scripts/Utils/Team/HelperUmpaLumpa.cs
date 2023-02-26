using System;
using UnityEngine;

public static class HelperUmpaLumpa
{
    public static class VectorUtils
    {
        public static float Vector2Angle(Vector2 vector)
        {
            return Mathf.Rad2Deg * Mathf.Atan2(vector.y, vector.x);
        }

        public static Vector3 Vector2To3XY(Vector2 vector)
        {
            return new Vector3(vector.x, vector.y, 0);
        }
    }

    public static class LevenshteinDistance
    {
        private static char[] splitters = {' ', '\n', '.'};

        // Thanks Marty Neal: https://stackoverflow.com/a/6944095
        public static int Compute(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (int i = 0; i <= n; d[i, 0] = i++);
            for (int j = 1; j <= m; d[0, j] = j++);

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }

        public static float ComputeRelative(string s, string t)
        {
            float max = Math.Max(s.Length, t.Length);
            float val = Compute(s, t);

            return (max - val) / max;
        }

        public static float MatchWords(string source, string search)
        {
            float sum = 0;
            int count = 0;
            foreach (string searchWord in search.Split(splitters))
            {
                if (string.IsNullOrEmpty(searchWord)) continue;
                float similarity = 0;
                foreach (string sourceWord in source.Split(splitters))
                {
                    similarity = Math.Max(ComputeRelative(searchWord, sourceWord), similarity);
                }
                sum += similarity;
                count++;
            }
            if (count == 0) return 1;
            return sum / count;
        }
    }
}