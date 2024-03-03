using System.Diagnostics.Contracts;

namespace CustomStringHashCode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Original: {"Hello, World!".GetHashCode()}");
            Console.WriteLine($"GetHashCode64BitsRelease: {GetHashCode64BitsRelease("Hello, World!")}");
            Console.WriteLine($"GetHashCode32BitsRelease: {GetHashCode32BitsRelease("Hello, World!")}");
            Console.WriteLine($"GetHashCodeFromSourceCode: {GetHashCodeFromSourceCode("Hello, World!")}");
        }

        // Source Code .net framework 481 https://referencesource.microsoft.com/#mscorlib/system/string.cs,0a17bbac4851d0d4,references
        public static unsafe int GetHashCode64BitsRelease(string str)
        {
            unsafe
            {
                fixed (char* src = str)
                {
                    int hash1 = 5381;
                    int hash2 = hash1;

                    int c;
                    char* s = src;
                    while ((c = s[0]) != 0)
                    {
                        hash1 = (hash1 << 5) + hash1 ^ c;
                        c = s[1];
                        if (c == 0)
                            break;
                        hash2 = (hash2 << 5) + hash2 ^ c;
                        s += 2;
                    }

                    return hash1 + hash2 * 1566083941;
                }
            }
        }

        public static unsafe int GetHashCode32BitsRelease(string str)
        {
            unsafe
            {
                fixed (char* src = str)
                {
                    int hash1 = (5381 << 16) + 5381;
                    int hash2 = hash1;
                    int* pint = (int*)src;
                    int len = str.Length;
                    while (len > 2)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                        hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];
                        pint += 2;
                        len -= 4;
                    }

                    if (len > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                    }

                    return hash1 + (hash2 * 1566083941);
                }
            }
        }

        public static int GetHashCodeFromSourceCode(string str)
        {
            unsafe
            {
                fixed (char* src = str)
                {
                    Contract.Assert(src[str.Length] == '\0', "src[this.Length] == '\\0'");
                    Contract.Assert(((int)src) % 4 == 0, "Managed string should start at 4 bytes boundary");

#if WIN32
                    int hash1 = (5381<<16) + 5381;
#else
                    int hash1 = 5381;
#endif
                    int hash2 = hash1;

#if WIN32
                    // 32 bit machines.
                    int* pint = (int *)src;
                    int len = str.Length;
                    while (len > 2)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                        hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];
                        pint += 2;
                        len  -= 4;
                    }
 
                    if (len > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                    }
#else
                    int c;
                    char* s = src;
                    while ((c = s[0]) != 0)
                    {
                        hash1 = ((hash1 << 5) + hash1) ^ c;
                        c = s[1];
                        if (c == 0)
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ c;
                        s += 2;
                    }
#endif
#if DEBUG
                    // We want to ensure we can change our hash function daily.
                    // This is perfectly fine as long as you don't persist the
                    // value from GetHashCode to disk or count on String A 
                    // hashing before string B.  Those are bugs in your code.
                    // hash1 ^= ThisAssembly.DailyBuildNumber;
#endif
                    return hash1 + (hash2 * 1566083941);
                }
            }
        }
    }
}