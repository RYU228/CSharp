using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

/// <summary>
/// Database 관련 클래스
/// </summary>
namespace DSLibrary
{
    #region 열거형 - Log 구분
    /// <summary>
    /// Application/Form/Button에 대한 사용 로그 구분
    /// /// </summary>
    public enum ActionLogType
    {
        /// <summary>입력(Insert)</summary> 
        Insert,
        /// <summary>조회(Select)</summary>
        Select,
        /// <summary>수정(Update)</summary>
        Update,
        /// <summary>삭제(Delete)</summary>
        Delete,
        /// <summary>폼 열기/닫기</summary>
        Form,
    };
    #endregion

    public class SqlHelper
    {
        #region 클래스 변수
        // Connection String 변수
        private static string connectionString;

        // Output Parameter 이름 변수 : 반환 코드, 반환 메세지, 반환 값
        private static readonly string RETURN_CODE = "@o_ReturnCode";
        private static readonly string RETURN_MSG = "@o_ReturnMsg";
        private static readonly string RETURN_VALUE = "@o_ReturnValue";
        #endregion

        #region 속성
        /// <summary>
        /// 데이터베이스 연결 문자열을 반환한다.
        /// </summary>
        public static string ConnectionString { get => connectionString; }

        #endregion

        #region 메서드 - 연결문자 설정
        /// <summary>
        /// 데이터베이스 연결문자를 생성한다.
        /// </summary>
        /// <param name="server">서버 명/IP</param>
        /// <param name="database">데이터베이스명</param>
        /// <param name="uid">사용자 ID</param>
        /// <param name="password">암호</param>
        public static void SetConnetionString(string server, string database, string uid, string password)
        {
            connectionString = $@"Data Source={server};Initial Catalog={database};User ID={uid};Password={password};Connection Timeout=5"; //;Min Pool Size=20
        }

        #endregion

        #region GetDataSet 반환
        /// <summary>
        /// T-SQL문을 실행하고 DataSet을 반환한다.
        /// </summary>
        /// <param name="procName">저장프로시저 명</param>
        /// <param name="paramList">저장프로시저 파라메터</param>
        /// <param name="returnCode">처리 결과 코드(정상:'00')</param>
        /// <param name="returnMsg">처리 결과 메세지(정상:'OK')</param>
        /// <returns></returns>
        public static DataSet GetDataSet(string procName, DbParamList paramList, out string returnCode, out string returnMsg)
        {
            SqlConnection connection = null;
            DataSet dataset = null;

            try
            {
                connection = new SqlConnection(connectionString);

                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = procName;

                CreateParameter(command, paramList);
                command.Parameters.Add(RETURN_CODE, SqlDbType.Char, 2).Direction = ParameterDirection.Output;
                command.Parameters.Add(RETURN_MSG, SqlDbType.Char, 200).Direction = ParameterDirection.Output;

                var adapter = new SqlDataAdapter(command);

                dataset = new DataSet();
                adapter.Fill(dataset);

                returnCode = command.Parameters[RETURN_CODE].Value.CString();
                returnMsg = command.Parameters[RETURN_MSG].Value.CString();

                adapter = null;
            }
            catch (Exception ex)
            {
                dataset = null;
                returnCode = "XX";
                returnMsg = ex.Message;

                //throw;
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }

            return dataset;
        }

        /// <summary>
        /// T-SQL문을 실행하고 DataSet을 반환한다.
        /// </summary>
        /// <param name="sql">Sql 문장</param>
        /// <param name="returnCode">처리 결과 코드(정상:'00')</param>
        /// <param name="returnMsg">처리 결과 메세지(정상:'OK')</param>
        /// <returns></returns>
        public static DataSet GetDataSet(string sql, out string returnCode, out string returnMsg)
        {
            SqlConnection connection = null;
            DataSet dataset = null;

            returnCode = "00";
            returnMsg = "OK";

            try
            {
                connection = new SqlConnection(connectionString);

                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = sql;

                var adapter = new SqlDataAdapter(command);

                dataset = new DataSet();
                adapter.Fill(dataset);

                adapter = null;
            }
            catch (Exception ex)
            {
                dataset = null;
                returnCode = "XX";
                returnMsg = ex.Message;
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }

            return dataset;
        }
        #endregion

        #region GetDataTable 반환
        /// <summary>
        /// T-SQL문을 실행하고 DataTable을 반환한다.
        /// </summary>
        /// <param name="procName">저장프로시저 명</param>
        /// <param name="paramList">저장프로시저 파라메터</param>
        /// <param name="returnCode">처리 결과 코드(정상:'00')</param>
        /// <param name="returnMsg">처리 결과 메세지(정상:'OK')</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string procName, DbParamList paramList, out string returnCode, out string returnMsg)
        {
            SqlConnection connection = null;
            DataTable table = null;

            try
            {
                connection = new SqlConnection(connectionString);

                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = procName;

                CreateParameter(command, paramList);
                command.Parameters.Add(RETURN_CODE, SqlDbType.Char, 2).Direction = ParameterDirection.Output;
                command.Parameters.Add(RETURN_MSG, SqlDbType.Char, 200).Direction = ParameterDirection.Output;

                var adapter = new SqlDataAdapter(command);

                table = new DataTable();
                adapter.Fill(table);

                returnCode = command.Parameters[RETURN_CODE].Value.CString();
                returnMsg = command.Parameters[RETURN_MSG].Value.CString();

                adapter = null;
            }
            catch (Exception ex)
            {
                table = null;
                returnCode = "XX";
                returnMsg = ex.Message;
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }

            return table;
        }

        /// <summary>
        /// T-SQL문을 실행하고 DataTable을 반환한다.
        /// </summary>
        /// <param name="sql">Sql 문장</param>
        /// <param name="returnCode">처리 결과 코드(정상:'00')</param>
        /// <param name="returnMsg">처리 결과 메세지(정상:'OK')</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, out string returnCode, out string returnMsg)
        {
            SqlConnection connection = null;
            DataTable table = null;

            returnCode = "00";
            returnMsg = "OK";

            try
            {
                connection = new SqlConnection(connectionString);

                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = sql;

                var adapter = new SqlDataAdapter(command);

                table = new DataTable();
                adapter.Fill(table);

                adapter = null;
            }
            catch (Exception ex)
            {
                table = null;
                returnCode = "XX";
                returnMsg = ex.Message;
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }

            return table;
        }
        #endregion

        #region GetDataRow 반환
        /// <summary>
        /// T-SQL문을 실행하고 DataRow를 반환한다.
        /// </summary>
        /// <param name="procName">저장프로시저 명</param>
        /// <param name="paramList">저장프로시저 파라메터</param>
        /// <param name="returnCode">처리 결과 코드(정상:'00')</param>
        /// <param name="returnMsg">처리 결과 메세지(정상:'OK')</param>
        /// <returns></returns>
        public static DataRow GetDataRow(string procName, DbParamList paramList, out string returnCode, out string returnMsg)
        {
            DataRow row = null;

            try
            {
                var table = GetDataTable(procName, paramList, out returnCode, out returnMsg);
                if (table != null && table.Rows.Count > 0)
                    row = table.Rows[0];
            }
            catch (Exception ex)
            {
                row = null;
                returnCode = "XX";
                returnMsg = ex.Message;
            }

            return row;
        }

        /// <summary>
        /// T-SQL문을 실행하고 DataRow를 반환한다.
        /// </summary>
        /// <param name="sql">Sql 문장</param>
        /// <param name="returnCode">처리 결과 코드(정상:'00')</param>
        /// <param name="returnMsg">처리 결과 메세지(정상:'OK')</param>
        /// <returns></returns>
        public static DataRow GetDataRow(string sql, out string returnCode, out string returnMsg)
        {
            DataRow row = null;

            try
            {
                var table = GetDataTable(sql, out returnCode, out returnMsg);
                if (table != null && table.Rows.Count > 0)
                    row = table.Rows[0];
            }
            catch (Exception ex)
            {
                row = null;
                returnCode = "XX";
                returnMsg = ex.Message;
            }

            return row;
        }

        #endregion

        #region Execute - 데이터 처리
        /// <summary>
        /// T-SQL문을 실행하고 영향받은 행수를 반환한다. 
        /// </summary>
        /// <param name="procName">저장프로시저 명</param>
        /// <param name="paramList">저장프로시저 파라메터</param>
        /// <param name="returnCode">처리 결과 코드(정상:'00')</param>
        /// <param name="returnMsg">처리 결과 메세지(정상:'OK')</param>
        /// <returns></returns>
        public static int Execute(string procName, DbParamList paramList, out string returnCode, out string returnMsg)
        {
            SqlConnection connection = null;
            int effectRows = -1;

            try
            {
                connection = new SqlConnection(connectionString);

                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = procName;

                CreateParameter(command, paramList);
                command.Parameters.Add(RETURN_CODE, SqlDbType.Char, 2).Direction = ParameterDirection.Output;
                command.Parameters.Add(RETURN_MSG, SqlDbType.Char, 200).Direction = ParameterDirection.Output;

                connection.Open();
                effectRows = command.ExecuteNonQuery();

                returnCode = command.Parameters[RETURN_CODE].Value.CString();
                returnMsg = command.Parameters[RETURN_MSG].Value.CString();
            }
            catch (Exception ex)
            {
                returnCode = "XX";
                returnMsg = ex.Message;
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }

            return effectRows;
        }

        /// <summary>
        /// T-SQL문을 실행하고 영향받은 행수를 반환한다. 
        /// </summary>
        /// <param name="procName">저장프로시저 명</param>
        /// <param name="paramList">저장프로시저 파라메터</param>
        /// <param name="returnValue">저장프로시저 내부에서 생성된 값 반환('^^'로 다중값 구분)</param>
        /// <param name="returnCode">처리 결과 코드(정상:'00')</param>
        /// <param name="returnMsg">처리 결과 메세지(정상:'OK')</param>
        /// <returns></returns>
        public static int Execute(string procName, DbParamList paramList, out string[] returnValue, out string returnCode, out string returnMsg)
        {
            SqlConnection connection = null;
            int effectRows = -1;

            try
            {
                connection = new SqlConnection(connectionString);
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = procName;

                CreateParameter(command, paramList);
                command.Parameters.Add(RETURN_VALUE, SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                command.Parameters.Add(RETURN_CODE, SqlDbType.Char, 2).Direction = ParameterDirection.Output;
                command.Parameters.Add(RETURN_MSG, SqlDbType.Char, 200).Direction = ParameterDirection.Output;

                connection.Open();
                effectRows = command.ExecuteNonQuery();

                returnValue = command.Parameters[RETURN_VALUE].Value.CString().Split(new string[] { "^^" }, StringSplitOptions.RemoveEmptyEntries);
                returnCode = command.Parameters[RETURN_CODE].Value.CString();
                returnMsg = command.Parameters[RETURN_MSG].Value.CString();
            }
            catch (Exception ex)
            {
                returnValue = null;
                returnCode = "XX";
                returnMsg = ex.Message;
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }

            return effectRows;
        }

        /// <summary>
        /// T-SQL문을 실행하고 영향받은 행수를 반환한다. 
        /// </summary>
        /// <param name="sql">Sql 문장</param>
        /// <param name="returnCode">처리 결과 코드(정상:'00')</param>
        /// <param name="returnMsg">처리 결과 메세지(정상:'OK')</param>
        /// <returns></returns>
        public static int Execute(string sql, out string returnCode, out string returnMsg)
        {
            SqlConnection connection = null;
            int effectRows = -1;

            try
            {
                connection = new SqlConnection(connectionString);

                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = sql;

                connection.Open();
                effectRows = command.ExecuteNonQuery();

                returnCode = "00";
                returnMsg = string.Empty;
            }
            catch (Exception ex)
            {
                effectRows = -1;
                returnCode = "XX";
                returnMsg = ex.Message;
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }

            return effectRows;
        }

        #endregion

        #region Exist - 데이터 존재 여부 체크
        /// <summary>
        /// T-SQL문을 실행하고 레코드가 존재하는지 확인한다.
        /// </summary>
        /// <param name="procName">저장프로시저 명</param>
        /// <param name="paramList">저장프로시저 명</param>
        /// <param name="returnCode">처리 결과 코드(정상:'00')</param>
        /// <param name="returnMsg">처리 결과 메세지(정상:'OK')</param>
        /// <returns></returns>
        public static bool Exist(string procName, DbParamList paramList, out string returnCode, out string returnMsg)
        {
            bool exist;

            try
            {
                var table = GetDataTable(procName, paramList, out returnCode, out returnMsg);
                exist = (table != null && table.Rows.Count > 0);
            }
            catch (Exception ex)
            {
                exist = false;
                returnCode = "XX";
                returnMsg = ex.Message;
            }

            return exist;
        }

        /// <summary>
        /// T-SQL문을 실행하고 레코드가 존재하는지 확인한다.
        /// </summary>
        /// <param name="sql">Sql 문장</param>
        /// <param name="returnCode">처리 결과 코드(정상:'00')</param>
        /// <param name="returnMsg">처리 결과 메세지(정상:'OK')</param>
        /// <returns></returns>
        public static bool Exist(string sql, out string returnCode, out string returnMsg)
        {
            bool exist;

            try
            {
                var table = GetDataTable(sql, out returnCode, out returnMsg);
                exist = (table != null && table.Rows.Count > 0);
            }
            catch (Exception ex)
            {
                exist = false;
                returnCode = "XX";
                returnMsg = ex.Message;
            }

            return exist;
        }

        #endregion

        #region CreateParameter(DbParameterList To SqlParameter)
        /// <summary>
        /// DbParameterList를 SqlParameter 개체로 변환한다.
        /// </summary>
        /// <param name="command">Command 개체</param>
        /// <param name="paramList">DbParameterList를 개체</param>
        private static void CreateParameter(SqlCommand command, DbParamList paramList)
        {
            if (paramList == null)
                return;

            paramList.GetList.ToList().ForEach(p =>
            {
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = p.Name,
                    Value = p.Value,
                    SqlDbType = p.Type
                });
            });
        }
        #endregion

        #region 데이터베이스에 로그를 기록
        /// <summary>
        /// Action(프로그램/폼/버튼) Log를 기록한다.
        /// </summary>
        /// <param name="activeForm">활성화된 폼(Tag값 : MenuID, StartTime)</param>
        /// <param name="logType">로그 유형</param>
        /// <param name="startTime">시작 시각</param>
        /// <param name="logCount">Action 발생 횟수</param>
        public static void WriteLog(Form activeForm, ActionLogType logType, DateTime startTime, int logCount = 1)
        {
            try
            {
                var menuID = (activeForm.Tag as FormLog).MenuID;
                var endTime = DateTime.Now;
                var type = new string[] { "Ins", "Sel", "Upd", "Del", "Frm" }[(int)logType];

                var inParams = new DbParamList();
                inParams.Add("@i_MenuID", menuID, SqlDbType.Char);
                inParams.Add("@i_ComputerName", AppConfig.Machine.ComputerName);
                inParams.Add("@i_LocalIP", AppConfig.Machine.LocalIP);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID);
                inParams.Add("@i_LogType", type, SqlDbType.Char);
                inParams.Add("@i_LogCount", logCount, SqlDbType.Int);
                inParams.Add("@i_StartTime", startTime, SqlDbType.DateTime);
                inParams.Add("@i_EndTime", endTime, SqlDbType.DateTime);

                SqlHelper.Execute("xp_LogWriter", inParams, out string retCode, out string retMsg);

                // DB에 저장 실패한 경우 File로 Log를 남긴다.
                if (retCode != "00")
                {
                    var log = new LogWriter(Application.StartupPath + @"\Log", FileLogType.Daily, "", "");
                    log.WriteLine($"MenuID:{menuID} / ComputerName:{AppConfig.Machine.ComputerName} / IP : {AppConfig.Machine.LocalIP} " +
                                  $"PersonID:{ AppConfig.Login.PersonID} / LogType:{type} / Count:{logCount} / " +
                                  $"StartTime:{startTime:yyyy-MM-dd HH:mm:ss.fff} / EndTime:{endTime:yyyy-MM-dd HH:mm:ss.fff}");
                    log.WriteLine($"Message:{retMsg}");
                }
            }
            catch
            {
            }
        }

        #endregion

        #region 데이터베이스 접속 체크
        /// <summary>
        /// 데이터베이스 접속 체크
        /// </summary>
        public static bool DBConnectCheck()
        {
            SqlConnection connection = null;
            bool connected = false;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                connected = true;
            }
            catch 
            {
                connected = false;
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }

            return connected;
        }
        #endregion
    }

    #region DbParameterList 클래스
    /// <summary>
    /// DbParameterList 클래스
    /// </summary>
    public class DbParamList
    {
        IList<DbParameter> paramList = new List<DbParameter>();

        public DbParamList()
        {
        }

        /// <summary>
        /// DbParameter List를 반환한다.
        /// </summary>
        public IList<DbParameter> GetList
        {
            get { return paramList; }
        }

        /// <summary>
        /// DbParameter의 Count를 반환한다.
        /// </summary>
        public int GetCount
        {
            get { return paramList.Count; }
        }

        /// <summary>
        /// DbParameter List 개체를 초기화한다.
        /// </summary>
        public void Clear()
        {
            paramList.Clear();
        }

        /// <summary>
        /// DbParameter List에 개체에 DbParameter를 추가한다.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public void Add(string name, object value, SqlDbType type)
        {
            var param = new DbParameter()
            {
                Name = name,
                Value = value,
                Type = type,
            };

            paramList.Add(param);
        }

        /// <summary>
        /// DbParameter List에 개체에 SqlDbType.VarChar Parameter를 추가한다.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(string name, object value)
        {
            Add(name, value, SqlDbType.VarChar);
        }
    }
    #endregion

    #region DbParameter 클래스
    /// <summary>
    /// DbParameter 클래스
    /// </summary>
    public class DbParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public SqlDbType Type { get; set; }

        public DbParameter()
        {
        }

        public void Dispose()
        {
        }
    }
    #endregion
}
