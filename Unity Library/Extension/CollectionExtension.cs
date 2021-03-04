using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-03-05 AM 3:48:12
// 작성자 : Rito

/*
    [목록]

    GetRandomElement() : 리스트, 배열 내에서 무작위 요소 뽑아 리턴
    GetRandomElements(int n) : 리스트, 배열 내에서 중복되지 않게 무작위 요소 n개 뽑아 리턴
*/

namespace Rito.UnityLibrary.Extension
{
    public static class CollectionExtension
    {
        /***********************************************************************
        *                           Random, Shuffle
        ***********************************************************************/
        #region .
        /// <summary> 리스트 내에서 무작위 요소 뽑아 리턴 </summary>
        public static T Ex_GetRandomElement<T>(this List<T> @this)
        {
            if(@this.Count == 0) return default;
            if(@this.Count == 1) return @this[0];

            int ranNum = UnityEngine.Random.Range(0, @this.Count);
            return @this[ranNum];
        }

        /// <summary> 배열 내에서 무작위 요소 뽑아 리턴 </summary>
        public static T Ex_GetRandomElement<T>(this T[] @this)
        {
            if(@this.Length == 0) return default;
            if(@this.Length == 1) return @this[0];

            int ranNum = UnityEngine.Random.Range(0, @this.Length);
            return @this[ranNum];
        }

        /// <summary> 리스트 내에서 무작위 요소들을 지정 개수만큼 중복되지 않게 뽑아 리스트로 리턴 </summary>
        public static List<T> Ex_GetRandomElements<T>(this List<T> @this, int count)
        {
            int len = @this.Count;
            if (len == 0) return null;
            if (count == 0) return null;

            // 리스트 개수 범위 초과하여 지정하지 않도록 제한
            if(count > len)
                count = len;

            List<T> ranList = new List<T>(count);
            HashSet<int> indexSet = new HashSet<int>();

            // 1. 랜덤 인덱스 뽑기
            for (int i = 0; i < count; i++)
            {
                int ranNum = UnityEngine.Random.Range(0, len);

                InfiniteLoopChecker.Reset();
                while (indexSet.Contains(ranNum))
                {
                    ranNum = UnityEngine.Random.Range(0, len);

                    InfiniteLoopChecker.Check();
                }
                indexSet.Add(ranNum);
            }

            // 2. 결과 리스트 채우기
            foreach (var index in indexSet)
            {
                ranList.Add(@this[index]);
            }

            return ranList;
        }

        // TODO : Ex_GetRandomElements - Array 버전

        // TODO : Shuffle
        /*
            https://stackoverflow.com/questions/273313/randomize-a-listt

            private static Random rng = new Random();  

            public static void Shuffle<T>(this IList<T> list)  
            {  
                int n = list.Count;  
                while (n > 1) {  
                    n--;  
                    int k = rng.Next(n + 1);  
                    T value = list[k];  
                    list[k] = list[n];  
                    list[n] = value;  
                }  
            }
        */

        #endregion
    }
}