using System;

namespace NetCoreWebAPI.Business
{
    // 支持私有的无参构造函数的单例泛型
    public class BaseInstance<T> where T : class
    {
        private static T _instance;
        private static readonly object SyncObject = new Object();
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncObject)
                    {
                        if (_instance == null)
                        {
                            _instance = (T)Activator.CreateInstance(typeof(T), true);
                            // 第二个参数防止异常：没有为该对象定义无参数的构造函数。
                        }
                    }
                }
                return _instance;
            }
        }

        public static void SetInstance(T value)
        {
            _instance = value;
        }
    }
}
