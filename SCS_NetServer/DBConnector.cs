using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SCS.Net.Server
{
    /// <summary>
    /// MySQL 데이터베이스에 대한 연결을 관리합니다.
    /// </summary>
    public static class DBConnector
    {
        /// <summary>
        /// 연결 문자열
        /// </summary>
        private const string strConn = "Server=scsproject.caye6rcn3n1a.ap-northeast-1.rds.amazonaws.com;Database=scsp;Uid=jacobs902207;Pwd=rlarlwls;";

        /// <summary>
        /// DB 연결 객체
        /// </summary>
        private static MySqlConnection conn = new MySqlConnection(strConn);

        /// <summary>
        /// DB와 연결합니다.
        /// </summary>
        public static void Open()
        {
            if (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken)
            {
                Program.m_Logger.WriteLog("[DBConnector] Connecting to DB");
                conn.Open();
                Program.m_Logger.WriteLog("[DBConnector] successfully connected to DB");
            }
        }

        public static int ExecuteNonQuery(string sql)
        {
            if (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken)
                Open();

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            int ret = cmd.ExecuteNonQuery();
            Program.m_Logger.WriteLog("[DBConnector] successfully execute nonQuery: " + sql);
            return ret;
        }

        public static int ExecuteNonQuery(MySqlCommand comm)
        {
            if (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken)
                Open();

            comm.Connection = conn;

            int ret = comm.ExecuteNonQuery();
            Program.m_Logger.WriteLog("[DBConnector] successfully execute nonQuery: " + comm.CommandText);
            return ret;
        }

        public static DataRowCollection ExecuteQuery(MySqlCommand comm)
        {
            if (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken)
                Open();

            comm.Connection = conn;

            MySqlDataAdapter adp = new MySqlDataAdapter(comm);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            Program.m_Logger.WriteLog("[DBConnector] successfully execute Query: " + comm.CommandText);
            return ds.Tables[0].Rows;
        }

        public static DataRowCollection ExecuteQuery(string sql)
        {
            if (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken)
                Open();

            MySqlDataAdapter adp = new MySqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            Program.m_Logger.WriteLog("[DBConnector] successfully execute Query: " + sql);
            return ds.Tables[0].Rows;
        }

        /// <summary>
        /// 지정된 ip주소의 유저에 대한 부서코드를 가져옵니다.
        /// </summary>
        /// <param name="ip">부서코드를 가져올 유저의 ip주소입니다.</param>
        /// <returns>부서코드입니다. 지정된 ip에 대한 정보를 찾을 수 없는 경우 -1입니다.</returns>
        public static int GetDepCode(IPAddress ip)
        {
            MySqlCommand comm = new MySqlCommand();
            comm.CommandText = "SELECT DepCode_Num From User WHERE User_IP = @userIP";
            comm.Parameters.AddWithValue("@userIP", ip.ToString());
            var rows = ExecuteQuery(comm);
            // 부서정보를 찾을 수 없는 경우엔 -1
            if (rows.Count == 0)
                return -1;

            return (int)rows[0][0];
        }

        /// <summary>
        /// 지정된 Hash값 또는 URL 주소에 대한 코드번호를 구합니다.
        /// </summary>
        /// <param name="hashOrURL">Hash값 또는 URL주소입니다.</param>
        /// <returns>코드번호입니다. 찾을 수 없는 경우 -1입니다.</returns>
        public static int GetAppCode(string hashOrURL)
        {
            MySqlCommand comm = new MySqlCommand();

            // URL인 경우
            if (hashOrURL.IndexOf('.') > -1)
            {
                hashOrURL = SCS.Common.URLTidy.UrlTidy(hashOrURL);

                comm.CommandText = "SELECT AppCode_Num FROM AppCode WHERE AppCode_Hash = @hashOrURL";
                comm.Parameters.AddWithValue("@hashOrURL", hashOrURL);
                var rows = ExecuteQuery(comm);
                if (rows.Count == 0)
                    return -1;

                return (int)rows[0][0];
            }
            // Hash값인 경우
            else
            {
                comm.CommandText = "SELECT AppCode_Num FROM AppCode WHERE AppCode_Hash = @hashOrURL";
                comm.Parameters.AddWithValue("@hashOrURL", hashOrURL);
                var rows = ExecuteQuery(comm);
                if (rows.Count == 0)
                    return -1;

                return (int)rows[0][0];
            }
        }

        /// <summary>
        /// 유저 코드 번호를 구합니다.
        /// </summary>
        /// <param name="ip">코드번호를 구할 유저의 IP 주소입니다.</param>
        /// <returns>코드번호를 찾을 수 없는 경우 -1입니다.</returns>
        public static int GetUserCode(IPAddress ip)
        {
            MySqlCommand comm = new MySqlCommand();
            comm.CommandText = "SELECT User_Num From User WHERE User_IP= @userIP";
            comm.Parameters.AddWithValue("@userIP", ip.ToString());
            var rows = ExecuteQuery(comm);
            if (rows.Count == 0)
                return -1;

            return (int)rows[0][0];
        }

        /// <summary>
        /// <see cref="DateTime"/>을 DB에 넣을 수 있는 <see cref="string"/>형태로 바꿉니다.
        /// </summary>
        public static string DateTimeToDB(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// DB와의 연결을 닫습니다.
        /// </summary>
        public static void Close()
        {
            conn.Close();
        }
    }
}
