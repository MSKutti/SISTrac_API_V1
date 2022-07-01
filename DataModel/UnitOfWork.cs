using Oracle.ManagedDataAccess.Client;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessEntity;
using Microsoft.Extensions.Configuration;
using Oracle.DataAccess.Client;
using OracleConnection = Oracle.ManagedDataAccess.Client.OracleConnection;
using OracleTransaction = Oracle.ManagedDataAccess.Client.OracleTransaction;
using System.Data.Entity.Infrastructure;

namespace DataModel
{
    public class UnitOfWork : IDisposable
    {
        private OracleConnection _Connection;
        #region DB
        protected OracleTransaction _Transaction { get; private set; }
        private bool isTransactionActive = false;

        private readonly string _connectionString;
        public UnitOfWork(IConfiguration _configuratio)
        {
            _connectionString = _configuratio.GetConnectionString("DBConnection");

        }

        public void Open()
        {
            if (_Connection.State == ConnectionState.Open)
            {
                return;
            }

            if (_Connection.State == ConnectionState.Closed && isTransactionActive == false)
            {
                _Connection.Open();
            }
            else
            {
                if (isTransactionActive == true)
                {
                    throw new Exception("Transaction is not committed");
                }
            }
        }

        public void Close()
        {
            if (_Connection.State == ConnectionState.Closed)
            {
                return;
            }

            if (_Connection.State == ConnectionState.Open && isTransactionActive == false)
            {
                _Connection.Close();
            }
            else
            {
                if (isTransactionActive == true)
                {
                    throw new Exception("Transaction is not committed");
                }

                if (_Connection.State != ConnectionState.Open)
                {
                    throw new Exception("Connection is not opened");
                }

            }
        }

        public void BeginTransaction()
        {
            if (_Connection.State == ConnectionState.Open && isTransactionActive == false)
            {
                _Transaction = _Connection.BeginTransaction();
                isTransactionActive = true;
            }
            else
            {
                if (_Connection.State != ConnectionState.Open)
                {
                    throw new Exception("Connection is not opened");
                }
                if (isTransactionActive == true)
                {
                    throw new Exception("Transaction is already opened");
                }
            }
        }

        public void CommitTransaction()
        {
            if (_Connection.State == ConnectionState.Open && isTransactionActive == true)
            {
                _Transaction.Commit();
                isTransactionActive = false;
            }
            else
            {
                if (_Connection.State != ConnectionState.Open)
                {
                    throw new Exception("Connection is not opened");
                }
                if (isTransactionActive == false)
                {
                    throw new Exception("Transaction is not begin");
                }
            }
        }

        public void RollbackTransaction()
        {
            if (_Connection.State == ConnectionState.Open && isTransactionActive == true)
            {
                _Transaction.Rollback();
                isTransactionActive = false;
            }
            else
            {
                if (_Connection.State != ConnectionState.Open)
                {
                    throw new Exception("Connection is not opened");
                }
                if (isTransactionActive == false)
                {
                    throw new Exception("Transaction is not begin");
                }
            }
        }

        public T QuerySingle<T>(string spName, object Params)
        {
            T EntityToBeReturn;

            try
            {
                if (_Connection.State == ConnectionState.Closed && isTransactionActive == false)
                    _Connection.Open();
                if (isTransactionActive)
                {
                    var res = _Connection.QueryFirstOrDefaultAsync<T>(spName, Params, _Transaction, commandType: CommandType.StoredProcedure);
                    res.Wait();
                    EntityToBeReturn = res.Result;
                }
                else
                {
                    var res = _Connection.QueryFirstOrDefaultAsync<T>(spName, Params, commandType: CommandType.StoredProcedure);
                    res.Wait();
                    EntityToBeReturn = res.Result;
                }
            }
            //catch (Exception ex)
            //{
            //    throw new Exception("Exception is raised " + ex.Message);
            //}
            finally
            {
                if (_Connection.State == ConnectionState.Open && isTransactionActive == false)
                    _Connection.Close();
            }
            return EntityToBeReturn;
        }

        public List<T> QueryMultiple<T>(string spName, object Params)
        {
            List<T> EntityList;
            try
            {
                if (_Connection.State == ConnectionState.Closed && isTransactionActive == false)
                    _Connection.Open();
                var res = _Connection.QueryAsync<T>(spName, Params, commandType: CommandType.StoredProcedure);
                res.Wait();
                EntityList = res.Result.AsList();
            }
            finally
            {
                if (_Connection.State == ConnectionState.Open && isTransactionActive == false)
                    _Connection.Close();
            }
            return EntityList;
        }

        public List<IEnumerable<object>> QueryMultipleResultSets(string spName, object Params, List<object> objectsList)
        {
            List<IEnumerable<object>> LstReturn = new List<IEnumerable<object>>();
            try
            {
                if (_Connection.State == ConnectionState.Closed && isTransactionActive == false)
                    _Connection.Open();

                var res = _Connection.QueryMultiple(spName, Params, null, 300, commandType: CommandType.StoredProcedure);
                for (int itr = 0; itr < objectsList.Count; itr++)
                {
                    if (!res.IsConsumed)
                    {
                        LstReturn.Add(res.Read(objectsList[itr].GetType()));
                    }
                }
            }
            finally
            {
                if (_Connection.State == ConnectionState.Open && isTransactionActive == false)
                    _Connection.Close();
            }
            return LstReturn;
        }



        public DbRawSqlQuery<T> SQLQuery<T>(string sql, params object[] parameters)
        {
            return _Connection.QuerySingle(sql, parameters);
        }

        #endregion



        public UnitOfWork()
        {
           // _Connection = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString);
            _connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=172.22.151.203)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=syap)));User Id=sis;Password=sis100;";
            _Connection = new OracleConnection(_connectionString);

           // _Connection = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["USER ID=SYSTEM;DATA SOURCE=localhost:1521/Sys;Password=User@123"].ConnectionString);
        }

        #region Implementing IDiosposable...

        #region private dispose variable declaration...
        private bool disposed = false;

        #endregion

        /// <summary>
        /// Protected Virtual Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Debug.WriteLine("UnitOfWork is being disposed");
                    _Connection.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion   
    }
}
