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
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;

            btnStop.Enabled = true;

            var registry = new Registry();

            //每隔5分钟,在线热门榜
            registry.Schedule(() => RankTask()).NonReentrant().ToRunEvery(5).Minutes();

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
        }

        private static string getWeb(string url)
        {
            GZipWebClient webClient = new GZipWebClient();

            webClient.Encoding = System.Text.Encoding.UTF8;

            return webClient.DownloadString(url);
        }
    }
}
