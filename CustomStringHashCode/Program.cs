namespace CustomStringHashCode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Original: {"Hello, World!".GetHashCode()}");
            Console.WriteLine($"GetHashCode64BitsRelease: {GetHashCode64BitsRelease("Hello, World!")}");
            Console.WriteLine($"GetHashCode64BitsRelease2: {GetHashCode64BitsRelease2("Hello, World!")}");
        }

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

        public static unsafe int GetHashCode64BitsRelease2(string str)
        {
            unsafe
            {
                fixed (char* ptr = str)
                {
                    int Length = str.Length;
                    int num = 352654597;
                    int num2 = num;
                    int* ptr2 = (int*)ptr;
                    int num3;
                    for (num3 = Length; num3 > 2; num3 -= 4)
                    {
                        num = ((num << 5) + num + (num >> 27)) ^ *ptr2;
                        num2 = ((num2 << 5) + num2 + (num2 >> 27)) ^ ptr2[1];
                        ptr2 += 2;
                    }

                    if (num3 > 0)
                    {
                        num = ((num << 5) + num + (num >> 27)) ^ *ptr2;
                    }

                    return num + num2 * 1566083941;
                }
            }
        }
    }
}