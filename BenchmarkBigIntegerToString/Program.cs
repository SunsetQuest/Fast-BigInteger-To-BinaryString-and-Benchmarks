// Created by Ryan Scott White (sunsetquest) on 7/16/2022
// Sharing under the MIT License 

using System;
using System.Collections;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;


//#if RELEASE
        var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
//#endif

var bic = new BigIntegerToStringBenchmarks();
var BigInteger = new BigInteger[] { -10000000000000000, -100, -1, 0, 1, 100, 10000000000000000, long.MaxValue };
foreach (var bi in BigInteger)
{
    Console.WriteLine("mjs3339: " + BigIntegerToStringBenchmarks.ToBinaryString_mjs3339(bi));
    Console.WriteLine("Kevin:   " + BigIntegerToStringBenchmarks.ToBinaryString_Kevin_Rice(bi));
    Console.WriteLine("gkovacs: " + BigIntegerToStringBenchmarks.ToNBase_gkovacs90(bi));
    Console.WriteLine("Ghasan:  " + BigIntegerToStringBenchmarks.ToBinaryString_Ghasan(bi));
    Console.WriteLine("ryan:    " + BigIntegerToStringBenchmarks.BigIntegerToBinaryString(bi));
    Console.WriteLine();
}

Console.ReadKey();


public class BigIntegerToStringBenchmarks
{
    private const int N = 100;
    private readonly BigInteger bi, bi2, bi3;

    public BigIntegerToStringBenchmarks()
    {
        byte[] data = new byte[N];
        new Random(4).NextBytes(data);
        bi = new BigInteger(data, true);
        Console.WriteLine("using: " + bi);

        data = new byte[N / 2];
        new Random(5).NextBytes(data);
        bi2 = new BigInteger(data, true);
        Console.WriteLine("using: " + bi2);

        data = new byte[N / 4];
        new Random(6).NextBytes(data);
        bi3 = new BigInteger(data, true);
        Console.WriteLine("using: " + bi3);
    }



    [Benchmark] public string ToBinaryString_mjs3339_LenOf100() => ToBinaryString_mjs3339(bi);
    [Benchmark] public string ToBinaryString_mjs3339__LenOf50() => ToBinaryString_mjs3339(bi2);
    [Benchmark] public string ToBinaryString_mjs3339__LenOf25() => ToBinaryString_mjs3339(bi3);

    // https://gist.github.com/mjs3339/73042bc0e717f98796ee9fa131e458d4 mjs3339 Jan 2, 2018
    public static string ToBinaryString_mjs3339(BigInteger bigint)
    {
        var bytes = bigint.ToByteArray();
        var index = bytes.Length - 1;
        var base2 = new StringBuilder(bytes.Length * 8);
        var binary = Convert.ToString(bytes[index], 2);
        //if (binary[0] != '0' && bigint.Sign == 1) base2.Append('0'); // for leading zero
        base2.Append(binary);
        for (index--; index >= 0; index--)
            base2.Append(Convert.ToString(bytes[index], 2).PadLeft(8, '0'));
        return base2.ToString();
    }


    [Benchmark] public string ToBinaryString_Kevin_Rice_LenOf100() => ToBinaryString_Kevin_Rice(bi);
    [Benchmark] public string ToBinaryString_Kevin_Rice__LenOf50() => ToBinaryString_Kevin_Rice(bi2);
    [Benchmark] public string ToBinaryString_Kevin_Rice__LenOf25() => ToBinaryString_Kevin_Rice(bi3);

    // https://stackoverflow.com/a/15447131/2352507
    public static string ToBinaryString_Kevin_Rice(BigInteger bigint)
    {
        var bytes = bigint.ToByteArray();
        var idx = bytes.Length - 1;

        // Create a StringBuilder having appropriate capacity.
        var base2 = new StringBuilder(bytes.Length * 8);

        // Convert first byte to binary.
        var binary = Convert.ToString(bytes[idx], 2);

        // Commented out for fairness in benchmarking.
        //// Ensure leading zero exists if value is positive.
        //if (binary[0] != '0' && bigint.Sign == 1)
        //{
        //    base2.Append('0');
        //}

        // Append binary string to StringBuilder.
        base2.Append(binary);

        // Convert remaining bytes adding leading zeros.
        for (idx--; idx >= 0; idx--)
        {
            base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
        }
        
        return base2.ToString();
    }


    [Benchmark] public string ToNBase_gkovacs90_LenOf100() => ToNBase_gkovacs90(bi);
    [Benchmark] public string ToNBase_gkovacs90__LenOf50() => ToNBase_gkovacs90(bi2);
    [Benchmark] public string ToNBase_gkovacs90__LenOf25() => ToNBase_gkovacs90(bi3);

    //https://stackoverflow.com/a/23447432/2352507
    public static string ToNBase_gkovacs90(BigInteger a) //, int n
    {
        if (a.Sign == 0) { return "0"; }  // added for fairness in benchmarking
        const int n = 2;
        StringBuilder sb = new StringBuilder();
        while (a > 0)
        {
            sb.Insert(0, a % n);
            a /= n;
        }
        return sb.ToString();
    }

    [Benchmark] public string ToNBase_gkovacs90_AdjustableBase_LenOf100() => ToNBase_gkovacs90_AdjustableBase(bi,2);
    [Benchmark] public string ToNBase_gkovacs90_AdjustableBase__LenOf50() => ToNBase_gkovacs90_AdjustableBase(bi2,2);
    [Benchmark] public string ToNBase_gkovacs90_AdjustableBase__LenOf25() => ToNBase_gkovacs90_AdjustableBase(bi3,2);

    //https://stackoverflow.com/a/23447432/2352507
    public static string ToNBase_gkovacs90_AdjustableBase(BigInteger a, int n) //, int n
    {
        if (a.Sign == 0) { return "0"; }  // added for fairness in benchmarking
        StringBuilder sb = new StringBuilder();
        while (a > 0)
        {
            sb.Insert(0, a % n);
            a /= n;
        }
        return sb.ToString();
    }

    [Benchmark] public string ToBinaryString_Ghasan_LenOf100() => ToBinaryString_Ghasan(bi);
    [Benchmark] public string ToBinaryString_Ghasan__LenOf50() => ToBinaryString_Ghasan(bi2);
    [Benchmark] public string ToBinaryString_Ghasan__LenOf25() => ToBinaryString_Ghasan(bi3);

    // https://stackoverflow.com/a/15447131/2352507
    public static string ToBinaryString_Ghasan(BigInteger x)
    {
        if (x.Sign == 0) { return "0"; } // added for fairness in benchmarking

        var biBytes = x.ToByteArray();

        var bits = new bool[8 * biBytes.Length];

        new BitArray(x.ToByteArray()).CopyTo(bits, 0);

        bits = bits.Reverse().ToArray(); // BigInteger uses little endian when extracting bytes (thus bits), so we inverse them.

        var builder = new StringBuilder();

        foreach (var bit in bits)
        {
            builder.Append(bit ? '1' : '0');
        }

        return Regex.Replace(builder.ToString(), @"^0+", ""); // Because bytes consume full 8 bits, we might occasionally get leading zeros.
    }
    
    
    [Benchmark] public string BigIntegerToBinaryString_Ryan_LenOf100() => BigIntegerToBinaryString(bi);
    [Benchmark] public string BigIntegerToBinaryString_Ryan__LenOf50() => BigIntegerToBinaryString(bi2);
    [Benchmark] public string BigIntegerToBinaryString_Ryan__LenOf25() => BigIntegerToBinaryString(bi3);

    /// <summary>
    /// A high performance BigInteger to binary string converter
    /// that supports 0 and negative numbers.
    /// License: MIT  / Created by Ryan Scott White, 7/16/2022;
    /// </summary>
    public static string BigIntegerToBinaryString(BigInteger x)
    {
        // Setup source
        ReadOnlySpan<byte> srcBytes = x.ToByteArray();
        int srcLoc = srcBytes.Length - 1;

        // Find the first bit set in the first byte so we don't print extra zeros.
        int msb = BitOperations.Log2(srcBytes[srcLoc]);
        
        // Setup Target
        Span<char> dstBytes = stackalloc char[srcLoc * 8 + msb + 1];
        int dstLoc = 0;

        // Commented out by for fairness in benchmarking
        // Add leading '-' sign if negative.
        //if (x.Sign < 0)
        //{
        //    dstBytes[dstLoc++] = '-';
        //}
        //else if (!x.IsZero) dstBytes[dstLoc++] = '0'; // add adding leading '0' (optional)

        // The first byte is special because we don't want to print leading zeros.
        byte b = srcBytes[srcLoc--];
        for (int j = msb; j >= 0; j--)
        {
            dstBytes[dstLoc++] = (char)('0' + ((b >> j) & 1));
        }
        
        // Add the remaining bits.
        for (; srcLoc >= 0; srcLoc--)
        {
            byte b2 = srcBytes[srcLoc];
            for (int j = 7; j >= 0; j--)
            {
                dstBytes[dstLoc++] = (char)('0' + ((b2 >> j) & 1));
            }
        }

        return dstBytes.ToString();
    }
}


