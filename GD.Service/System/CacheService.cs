﻿using GD.Infrastructure.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD.Service.System
{
    public class CacheService
    {
        private readonly static string CK_verifyScan = "verifyScan_";
        #region 用户权限 缓存
        public static List<string> GetUserPerms(string key)
        {
            return (List<string>)CacheHelper.GetCache(key);
            //return RedisServer.Cache.Get<List<string>>(key).ToList();
        }

        public static void SetUserPerms(string key, object data)
        {
            CacheHelper.SetCache(key, data);
            //RedisServer.Cache.Set(key, data);
        }
        public static void RemoveUserPerms(string key)
        {
            CacheHelper.Remove(key);
            //RedisServer.Cache.Del(key);
        }
        #endregion

        public static object SetScanLogin(string key, Dictionary<string, object> val)
        {
            var ck = CK_verifyScan + key;

            return CacheHelper.SetCache(ck, val, 1);
        }
        public static object GetScanLogin(string key)
        {
            var ck = CK_verifyScan + key;
            return CacheHelper.Get(ck);
        }
        public static void RemoveScanLogin(string key)
        {
            var ck = CK_verifyScan + key;
            CacheHelper.Remove(ck);
        }

        public static void SetLockUser(string key, long val, int time)
        {
            var CK = "lock_user_" + key;

            CacheHelper.SetCache(CK, val, time);
        }

        public static long GetLockUser(string key)
        {
            var CK = "lock_user_" + key;

            if (CacheHelper.Get(CK) is long t)
            {
                return t;
            }
            return 0;
        }
    }
}
