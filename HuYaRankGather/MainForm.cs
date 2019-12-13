using FluentScheduler;
using MySql.Data.MySqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace HuYaRankGather
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        /*
         全部在线主播     https://www.huya.com/l              https://www.huya.com/cache.php?m=LiveList&do=getLiveListByPage&tagAll=0&page=1
         星秀             https://www.huya.com/g/xingxiu      https://www.huya.com/cache.php?m=LiveList&do=getLiveListByPage&gameId=1663&tagAll=0&page=1
         颜值             https://www.huya.com/g/2168         https://www.huya.com/cache.php?m=LiveList&do=getLiveListByPage&gameId=2168&tagAll=0&page=1
         二次元           https://www.huya.com/g/2633         https://www.huya.com/cache.php?m=LiveList&do=getLiveListByPage&gameId=2633&tagAll=0&page=1
        */

        private void MainForm_Load(object sender, EventArgs e)
        {
            //RankTask();

            //FansTask("星秀", 100000);

            //FansTask("颜值", 100000);

            //FansTask("二次元", 100000);

            //PercentTask("星秀", 100000, 100000);

            //PercentTask("颜值", 100000, 100000);

            //PercentTask("二次元", 100000, 100000);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;

            btnStop.Enabled = true;

            var registry = new Registry();

            //每隔5分钟,在线热门榜
            registry.Schedule(() => RankTask()).NonReentrant().ToRunEvery(5).Minutes();

            //每天1:00点执行
            registry.Schedule(() => FansTask("星秀", 100000)).NonReentrant().ToRunEvery(1).Days().At(1, 0);

            //每天2:00点执行
            registry.Schedule(() => FansTask("颜值", 100000)).NonReentrant().ToRunEvery(1).Days().At(2, 0);

            //每天3:00点执行
            registry.Schedule(() => FansTask("二次元", 100000)).NonReentrant().ToRunEvery(1).Days().At(3, 0);

            //每天4:00点执行
            registry.Schedule(() => PercentTask("颜值", 100000, 100000)).NonReentrant().ToRunEvery(1).Days().At(4, 0);

            //每天4:30点执行
            registry.Schedule(() => PercentTask("舞蹈", 100000, 100000)).NonReentrant().ToRunEvery(1).Days().At(4, 30);

            //每天5:00点执行
            registry.Schedule(() => PercentTask("二次元", 100000, 100000)).NonReentrant().ToRunEvery(1).Days().At(5, 0);

            JobManager.Initialize(registry);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;

            btnStart.Enabled = true;

            JobManager.StopAndBlock();
        }

        private static void RankTask()
        {
            //所有 3 页
            LiveList liveList = getAll(1);

            if (liveList.data.totalPage >= 2)
            {
                getAll(2);
            }

            if (liveList.data.totalPage >= 3)
            {
                getAll(3);
            }

            //星秀 3 页
            liveList = getXX(1);

            if (liveList.data.totalPage >= 2)
            {
                getXX(2);
            }

            if (liveList.data.totalPage >= 3)
            {
                getXX(3);
            }

            //颜值 2 页
            liveList = getYZ(1);

            if (liveList.data.totalPage >= 2)
            {
                getYZ(2);
            }

            //二次元 2 页
            liveList = getECY(1);

            if (liveList.data.totalPage >= 2)
            {
                liveList = getECY(2);
            }
        }

        //所有
        public static LiveList getAll(int page)
        {
            string text = getWeb($"https://www.huya.com/cache.php?m=LiveList&do=getLiveListByPage&tagAll=0&page={page}");

            LiveList liveList = JsonHelper.DeSerialize<LiveList>(text);

            InsertData(liveList);

            return liveList;
        }

        //星秀
        public static LiveList getXX(int page)
        {
            string text = getWeb($"https://www.huya.com/cache.php?m=LiveList&do=getLiveListByPage&gameId=1663&tagAll=0&page={page}");

            LiveList liveList = JsonHelper.DeSerialize<LiveList>(text);

            InsertData(liveList);

            return liveList;
        }

        //颜值
        public static LiveList getYZ(int page)
        {
            string text = getWeb($"https://www.huya.com/cache.php?m=LiveList&do=getLiveListByPage&gameId=2168&tagAll=0&page={page}");

            LiveList liveList = JsonHelper.DeSerialize<LiveList>(text);

            InsertData(liveList);

            return liveList;
        }

        //二次元
        public static LiveList getECY(int page)
        {
            string text = getWeb($"https://www.huya.com/cache.php?m=LiveList&do=getLiveListByPage&gameId=2633&tagAll=0&page={page}");

            LiveList liveList = JsonHelper.DeSerialize<LiveList>(text);

            InsertData(liveList);

            return liveList;
        }

        private static void InsertData(LiveList liveList)
        {
            var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

            var db = new QueryFactory(connection, new MySqlCompiler());

            foreach (var item in liveList.data.datas)
            {
                var query = db.Query("RankInfo").Insert(new
                {
                    UserId = item.uid,
                    UserNick = item.nick,
                    RoomId = item.profileRoom,
                    RoomOnline = item.totalCount,
                    RoomTitle = item.introduction,
                    AppName = item.gameFullName
                });
            }

            connection.Close();
        }

        /// <summary>
        /// 主播粉丝数采集 星秀   颜值  二次元分类 且 房间人数>10万 
        /// </summary>
        private static void FansTask(string appName, int online)
        {
            var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

            var db = new QueryFactory(connection, new MySqlCompiler());

            db.Logger = compiled =>
            {
                Console.WriteLine(compiled.ToString());
            };

            var rooms = db.Query("RankInfo")
                .Select("RoomId")
                .SelectRaw("MAX(`RoomOnline`) as RoomOnline")
                .Where("AppName", "=", appName)
                .GroupBy("RoomId")
                .HavingRaw($"MAX(`RoomOnline`) >= {online}")
                .OrderByRaw("MAX(`RoomOnline`) DESC")
                .Get<Room>();

            foreach (var room in rooms)
            {
                try
                {
                    //查询粉丝数
                    int fans = GetFans(room.RoomId);

                    //更新粉丝数
                    UpdateFans(room.RoomId, fans);
                }
                catch
                {


                }
            }

            connection.Close();
        }

        private static int GetFans(int roomId)
        {
            string text = getWeb($"https://www.huya.com/{roomId}");

            string activityCount = text.Substring("<div class=\"subscribe-count\" id=\"activityCount\">", "<");

            if (string.IsNullOrEmpty(activityCount))
            {
                return 0;
            }

            return int.Parse(activityCount);
        }

        private static void UpdateFans(int roomId, int fans)
        {
            var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

            var db = new QueryFactory(connection, new MySqlCompiler());

            db.Logger = compiled =>
            {
                Console.WriteLine(compiled.ToString());
            };

            int affected = db.Query("RankInfo")
                .Where("RoomId", "=", roomId)
                .Where("UserFans", "=", 0)
                .Update(new { UserFans = fans });

            connection.Close();
        }

        /// <summary>
        /// 计算当前分类所占比例
        /// </summary>
        private static void PercentTask(string appName, int online, int fans)
        {
            var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

            var db = new QueryFactory(connection, new MySqlCompiler());

            db.Logger = compiled =>
            {
                Console.WriteLine(compiled.ToString());
            };

            var rooms = db.Query("RankInfo")
                .Select("RoomId")
                .SelectRaw("MAX(`RoomOnline`) as RoomOnline")
                .SelectRaw("MAX(`UserFans`)")
                .Where("AppName", "=", appName)
                .GroupBy("RoomId")
                .HavingRaw($"MAX(`RoomOnline`) >= {online} and MAX(`UserFans`) >= {fans}")
                .OrderByRaw("MAX(`RoomOnline`) DESC")
                .Get<Room>();

            foreach (var room in rooms)
            {
                try
                {
                    //查询分类占百分比
                    int percent = GetPercent(room.RoomId, appName);

                    //更新百分比
                    UpdatePercent(room.RoomId, appName, percent);
                }
                catch
                {


                }
            }

            connection.Close();
        }

        private static int GetPercent(int roomId, string appName)
        {
            int count = 0;

            int total = 0;

            var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

            var db = new QueryFactory(connection, new MySqlCompiler());

            db.Logger = compiled =>
            {
                Console.WriteLine(compiled.ToString());
            };

            var roomCount = db.Query("RankInfo")
                .Select("AppName")
                .SelectRaw("count(*) as Count")
                .Where("RoomId", "=", roomId)
                .GroupBy("AppName")
                .Get<RoomCount>();

            foreach (var room in roomCount)
            {
                total += room.Count;

                if (room.AppName == appName)
                {
                    count = room.Count;
                }
            }

            connection.Close();

            int percent = (int)(((double)count / total) * 100);

            return percent;
        }

        private static void UpdatePercent(int roomId, string appName, int percent)
        {
            var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

            var db = new QueryFactory(connection, new MySqlCompiler());

            db.Logger = compiled =>
            {
                Console.WriteLine(compiled.ToString());
            };

            int affected = db.Query("RankInfo")
                .Where("RoomId", "=", roomId)
                .Where("AppName", "=", appName)
                .Where("AppPercent", "=", 0)
                .Update(new { AppPercent = percent });

            connection.Close();
        }

        private static string getWeb(string url)
        {
            GZipWebClient webClient = new GZipWebClient();

            webClient.Encoding = System.Text.Encoding.UTF8;

            return webClient.DownloadString(url);
        }
    }
}
