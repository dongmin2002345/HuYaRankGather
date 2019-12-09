using System.Collections.Generic;

public class LiveList
{
    public int status { get; set; }

    public string message { get; set; }

    public Data data { get; set; }

    public class DatasItem
    {
        /// <summary>
        /// 在线观看人数
        /// </summary>
        public string totalCount { get; set; }

        /// <summary>
        /// 房间名称
        /// </summary>
        public string roomName { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string nick { get; set; }

        /// <summary>
        /// 房间标题/介绍
        /// </summary>
        public string introduction { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public string uid { get; set; }

        /// <summary>
        /// 房间Id
        /// </summary>
        public string profileRoom { get; set; }

        /// <summary>
        /// 直播分类
        /// </summary>
        public string gameFullName { get; set; }
    }

    public class Data
    {
        public int page { get; set; }

        public int pageSize { get; set; }

        public int totalPage { get; set; }

        public int totalCount { get; set; }

        public List<DatasItem> datas { get; set; }

        public int time { get; set; }
    }
}
