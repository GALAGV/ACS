using System;
using System.Collections.Generic;

namespace AGVSystem.APP.agv_Map
{
    /// <summary>
    /// 缓存处理
    /// </summary>
    public class CachePlant
    {
        private static Dictionary<string, object> pairs = new Dictionary<string, object>();

        /// <summary>
        /// 判断缓存是否存在
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        private static bool ExistsCache(string Key)
        {
            return pairs.ContainsKey(Key);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valus"></param>
        public static void Add(string key,object valus)
        {
            pairs.Add(key, valus);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="Key"></param>
        public static void Remove(string Key)
        {
            List<string> Keys = new List<string>();
            foreach (var item in pairs.Keys)
            {
                if (item.Contains(Key))
                {
                    Keys.Add(item);
                }
            }
            Keys.ForEach(p => pairs.Remove(p));
        }

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <returns></returns>
        private static T GetValue<T>(string Key)
        {
            return (T)pairs[Key];
        }

        /// <summary>
        /// 缓存判断
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T GetResult<T>(string Key, Func<T> func)
        {
            T obj = default(T);
            if (ExistsCache(Key))
            {
                obj = GetValue<T>(Key);
            }
            else
            {
                obj = func.Invoke();
                Add(Key, obj);
            }
            return obj;
        }


    }
}
