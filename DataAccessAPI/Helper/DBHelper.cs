using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;
using DataAccessAPI.Helper.Interfaces.DBHelper;

namespace DataAccessAPI.Helper
{
    /// <summary>
    /// A base class for ADO.Net based data access layer, 
    /// that encapsulates connection, command, and data adapters.
    /// Enables quick and easy execution of major database operation:
    /// ExecuteNonQuery, ExecuteScalar, 
    /// Load data using a DataReader, DataSet or DataTable.
    /// Also provides a shorten way to create and initialize parameters.
    /// </summary>
    public abstract class DBHelper<TConnection, TCommand, TParameter, TAdapter> : IDBHelper
        where TConnection : IDbConnection, new()
        where TCommand : IDbCommand, new()
        where TParameter : IDbDataParameter, new()
        where TAdapter : IDbDataAdapter, IDisposable, new()
    {
        #region private memberes

        private string _ConnectionString;

        #endregion  private memberes

        #region ctor

        /// <summary>
        /// Initializes the connection string needed to connect to the database.
        /// Inheritors must call this constractor with the proper connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        public DBHelper(string connectionString)
        {
            _ConnectionString = connectionString;
        }

        #endregion ctor

        #region public methods

        /// <summary>
        /// Executes an SQL non-query statement (INSERT/UPDATE/DELETE).
        /// </summary>
        /// <param name="sql">SQL statement to execute.</param>
        /// <param name="commandType">One of the Sql.Data.CommandType values. The default is Text.</param>
        /// <param name="parameters">Parameters of the SQL statement.</param>
        /// <returns>An integer value indication the number of rows effected by the SQL statement.</returns>
        public int ExecuteNonQuery(string sql, CommandType commandType, params NpgsqlParameter[] parameters)
        {
            return Execute<int>(sql, commandType, c => c.ExecuteNonQuery(), parameters);
        }

        /// <summary>
        /// Execute an SQL select statement that returns a single scalar value.
        /// </summary>
        /// <typeparam name="T">Data type of the value to return.</typeparam>
        /// <param name="sql">SQL statement to execute.</param>
        /// <param name="commandType">One of the Sql.Data.CommandType values. The default is Text.</param>
        /// <param name="parameters">Parameters of the SQL statement.</param>
        /// <returns>An instance of T, or it's default.</returns>
        public T ExecuteScalar<T>(string sql, CommandType commandType, params NpgsqlParameter[] parameters)
        {
            return Execute<T>(sql, commandType, c =>
            {
                var returnValue = c.ExecuteScalar();
                return (returnValue != null && returnValue != DBNull.Value && returnValue is T)
                 ? (T)returnValue
                 : default(T);
            }, parameters);
        }

        /// <summary>
        /// Executes an SQL Select statement using an instance of a class that's implementing IDataReader.
        /// Recommended use: Populating data objects.
        /// </summary>
        /// <param name="sql">SQL statement to execute.</param>
        /// <param name="commandType">One of the Sql.Data.CommandType values. The default is Text.</param>
        /// <param name="populate">A function to run that accepts an IDataReader and returns a boolean, to do the actuall population of the data object.</param>
        /// <param name="parameters">Parameters of the SQL statement.</param>
        /// <returns>The boolean value returned from the populate argument.</returns>
        public bool ExecuteReader(string sql, CommandType commandType, Func<IDataReader, bool> populate, params NpgsqlParameter[] parameters)
        {
            return Execute<bool>(sql, commandType, c => populate(c.ExecuteReader()), parameters);
        }

        /// <summary>
        /// Executes an SQL Select statement and returns it's results using a DataSet.
        /// </summary>
        /// <param name="sql">SQL statement to execute.</param>
        /// <param name="commandType">One of the Sql.Data.CommandType values. The default is Text.</param>
        /// <param name="parameters">Parameters of the SQL statement.</param>
        /// <returns>An instance of the DataSet class with the results of the SQL query.</returns>
        public DataSet FillDataSet(string sql, CommandType commandType, params NpgsqlParameter[] parameters)
        {
            return Execute<DataSet>(sql, commandType, c => FillDataSet(c), parameters);
        }

        /// <summary>
        /// Executes an SQL Select statement and returns it's results using a DataTable.
        /// </summary>
        /// <param name="sql">SQL statement to execute.</param>
        /// <param name="commandType">One of the Sql.Data.CommandType values. The default is Text.</param>
        /// <param name="parameters">Parameters of the SQL statement.</param>
        /// <returns>An instance of the DataTable class with the results of the SQL query.</returns>
        /// <remarks>
        /// Note to inhreritors: Concrete DataAdapter might have an overload of the Fill method 
        /// that works directly with a data table. You might want to use it instead of this method.
        /// </remarks>
        public virtual DataTable FillDataTable(string sql, CommandType commandType, params NpgsqlParameter[] parameters)
        {
            var dataset = FillDataSet(sql, commandType, parameters);
            return (dataset.Tables.Count > 0) ? dataset.Tables[0] : null;
        }

        /// <summary>
        /// Executes an SQL statement.
        /// </summary>
        /// <typeparam name="T">Data type of the value to return.</typeparam>
        /// <param name="sql">SQL statement to execute.</param>
        /// <param name="commandType">One of the Sql.Data.CommandType values. The default is Text.</param>
        /// <param name="function">A function to execute with the IDbCommand (i.e. Filling a DataTable).</param>
        /// <param name="parameters">Parameters of the SQL statement.</param>
        /// <returns>The value returned from the function argument.</returns>
        public T Execute<T>(string sql, CommandType commandType, Func<IDbCommand, T> function, params NpgsqlParameter[] parameters)
        {
            using (var con = new NpgsqlConnection())
            {
                con.ConnectionString = _ConnectionString;
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Connection = con;
                    cmd.CommandType = commandType;
                    if (parameters != null)
                    {
                        if (parameters.Length > 0)
                        {
                            foreach (NpgsqlParameter dbDataParameter in parameters)
                                if (parameters != null)
                                    cmd.Parameters.Add(dbDataParameter);
                        }
                    }
                    con.Open();
                    return function(cmd);
                }
            }
        }

        /// <summary>
        /// Creates an output parameter with the specified name, type and size.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        /// <param name="size">The size of the parameter.</param>
        /// <returns>An output parameter with the specified name, type and size.</returns>
        public NpgsqlParameter CreateOutputParameter(string name, NpgsqlDbType type, int size)
        {
            var param = CreateOutputParameter(name, type);
            param.Size = size;
            return param;
        }

        /// <summary>
        /// Creates an output parameter with the specified name and type.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        /// <returns>An output parameter with the specified name and type.</returns>
        public NpgsqlParameter CreateOutputParameter(string name, NpgsqlDbType type)
        {
            var param = CreateParameter(name, type);
            param.Direction = ParameterDirection.Output;
            return param;
        }

        /// <summary>
        /// Creates an input parameter with the specified name, type and value.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>An input parameter with the specified name, type and value</returns>
        public NpgsqlParameter CreateParameter(string name, NpgsqlDbType type, object value)
        {
            var param = CreateParameter(name, type);
            param.Value = value ?? DBNull.Value;
            return param;
        }

        /// <summary>
        /// Creates an input parameter with the specified name, type, size and value.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        /// <param name="size">The size of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns></returns>
        public NpgsqlParameter CreateParameter(string name, NpgsqlDbType type, int size, object value)
        {
            var param = CreateParameter(name, type, value);
            param.Size = size;
            return param;
        }

        //public void Dispose()
        //{
        //}

        #endregion public methods

        #region private methods

        private DataSet FillDataSet(IDbCommand command)
        {
            var dataSet = new DataSet();
            using (var adapter = new TAdapter())
            {
                adapter.SelectCommand = command;
                adapter.Fill(dataSet);
            }
            return dataSet;
        }

        #endregion private methods

        #region protected methods

        /// <summary>
        /// Creates an instance of TParameter with the specified name and type.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        /// <returns>An instance of TParameter with the specified name and type.</returns>
        protected virtual NpgsqlParameter CreateParameter(string name, NpgsqlDbType type)
        {
            return new NpgsqlParameter()
            {
                ParameterName = name,
                DbType =  (DbType)type
            };
        }
        #endregion protected methods

    }


    /// <summary>
    /// An implementation of IDBHelper to be used with SQL Server database.
    /// </summary>
    public class PGDBHelper : DBHelper<NpgsqlConnection, NpgsqlCommand, NpgsqlParameter, NpgsqlDataAdapter>,IDisposable
    {

        #region ctor

        /// <summary>
        /// Initializes the connection string needed to connect to the database.
        /// Inheritors must call this constractor with the proper connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        internal PGDBHelper(string connectionString)
            : base(connectionString)
        {
        }

        #endregion ctor

        //#region protected methods
        public void Dispose()
        {
        }

        ///// <summary>
        ///// Fills a DataTable with the results of an SQL query.
        ///// </summary>
        ///// <param name="sql">SQL query to execute.</param>
        ///// <param name="commandType">One of the Sql.Data.CommandType values. The default is Text.</param>
        ///// <param name="parameters">Parameters of the SQL statement.</param>
        ///// <returns></returns>
        //protected DataTable FillDataTable(string sql, CommandType commandType, params NpgsqlParameter[] parameters)
        //{
        //    return Execute<DataTable>(sql, commandType, command =>
        //    {
        //        var dataTable = new DataTable();
        //        using (var adapter = new NpgsqlDataAdapter((NpgsqlCommand)command))
        //        {
        //            adapter.Fill(dataTable);
        //        }
        //        return dataTable;
        //    }, parameters);
        //}

        ///// <summary>
        ///// Creates a new instance of the SqlParameter class.
        ///// </summary>
        ///// <param name="name">The name of the paramenter.</param>
        ///// <param name="type">The type of the parameter.</param>
        ///// <returns>A new instance of SqlParameter with the specified name and type.</returns>
        //protected override IDbDataParameter CreateParameter(string name, NpgsqlDbType type)
        //{
        //    return new NpgsqlParameter(name, type);
        //}

        //#endregion protected methods

    }
}
