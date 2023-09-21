using System.Collections.Generic;
using UnityEngine;


public class Unum
{
    public const int FRACT_SIZE = 1000;

    private List<uint> fract;
    private float zeroFract;

    public Unum()
    {
        fract = new List<uint>();
        zeroFract = 0;
    }
    public Unum(float value) : this()
    {
        zeroFract = value;
        Validate();
    }

    public static Unum operator+(Unum n1, Unum n2)
    {
        Unum result = new Unum();
        result.Add(n1);
        result.Add(n2);
        return result;
    }

    public override string ToString()
    {
        float vn = 0;
        if (fract.Count > 1)
        {
            float mf = fract[fract.Count - 1];
            float mfp;
            if (fract.Count > 2) mfp = fract[fract.Count - 2];
            else mfp = zeroFract;

            vn = mfp / (float)FRACT_SIZE;
        }
        else
        {
            vn = zeroFract;
        }
        
        int tail = 2;
        if (vn >= 10) tail = 1;
        else if (vn >= 100) tail = 0;
        return string.Format("{0:F" + tail + "}", zeroFract);
    }

    private void Add(Unum n)
    {
        zeroFract += n.zeroFract;
        if (Mathf.Abs(zeroFract) > FRACT_SIZE)
        {
            if (fract.Count < 1) fract.Add(0);
            fract[0] += (uint)Mathf.FloorToInt(zeroFract) / FRACT_SIZE;
            
            zeroFract = zeroFract % FRACT_SIZE;
        }
        
        int i = 0;
        while (true)
        {
            if (!(n.fract.Count < i + 1))
            {
                if (fract.Count < i + 1) fract.Add(0);
                fract[i] += n.fract[i];
            }
            else if (fract.Count < i + 1)
            {
                break;
            }

            if (fract[i] > FRACT_SIZE)
            {
                if (fract.Count < i + 2) fract.Add(0);
                fract[i + 1] += (uint)Mathf.FloorToInt(fract[i]) / FRACT_SIZE;
                fract[i] = fract[i] % FRACT_SIZE;
            }

            i++;
        }
    }

    private void Validate()
    {
        int i = 0;
        while (true)
        {
            if (fract.Count < i + 1)
            {
                break;
            }

            if (fract[i] > FRACT_SIZE)
            {
                if (fract.Count < i + 2) fract.Add(0);
                fract[i + 1] += (uint)Mathf.FloorToInt(fract[i]) / FRACT_SIZE;
                fract[i] = fract[i] % FRACT_SIZE;
            }
            
            i++;
        }
    }
}