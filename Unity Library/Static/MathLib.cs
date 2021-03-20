using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-03-20 PM 9:37:37
// 작성자 : Rito

namespace Rito.UnityLibrary
{
    public static class MathLib
    {
        public static int Factorial(int n)
        {
            if (n == 0 || n == 1) return 1;
            if (n == 2) return 2;

            int result = n;
            for (int i = n - 1; i > 1; i--)
            {
                result *= i;
            }
            return result;
        }

        public static int Permutation(int n, int r)
        {
            if (r == 0) return 1;
            if (r == 1) return n;

            int result = n;
            int end = n - r + 1;
            for (int i = n - 1; i >= end; i--)
            {
                result *= i;
            }
            return result;
        }

        public static int Combination(int n, int r)
        {
            if (n == r) return 1;
            if (r == 0) return 1;

            // C(n, r) == C(n, n - r)
            if(n - r < r) 
                r = n - r;

            return Permutation(n, r) / Factorial(r);
        }
    }
}