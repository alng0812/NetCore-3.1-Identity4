namespace NetCoreWebAPI.Business
{
    /// <summary>
    /// table的返回数据
    /// </summary>
    public class TableData<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code;
        /// <summary>
        /// 操作消息
        /// </summary>
        public string Message;

        /// <summary>
        /// 总记录条数
        /// </summary>
        public int Count;

        /// <summary>
        /// 数据内容
        /// </summary>
        public T Data;

        public TableData()
        {
            Code = 200;
            Message = "加载成功";
        }
    }
    /// <summary>
    /// table的返回数据
    /// </summary>
    public class TableData : TableData<dynamic>
    {

    }
}
