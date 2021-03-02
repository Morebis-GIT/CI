using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace xggameplan.common.Databases
{
    public class OleDbDatabase : Database
    {
        protected OleDbConnection _connection = null;

        public OleDbDatabase(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Connection string not set");
            }
            _connectionString = connectionString;
        }

        public override void Open()
        {
            if (_connection == null)
            {
                _connection = new OleDbConnection(_connectionString);
                _connection.Open();
            }
        }

        public OleDbTransaction BeginTransaction()
        {
            return _connection.BeginTransaction();
        }

        /// <summary>
        /// Gets the parameter values to return for the command and sets them in 'parameters'
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        private void GetOutputParameters(OleDbCommand command, Dictionary<string, OleDbParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (OleDbParameter currentParameter in parameters.Values)
                {
                    OleDbParameter parameter = command.Parameters[currentParameter.ParameterName];
                    if (parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Output || parameter.Direction == ParameterDirection.ReturnValue)
                    {
                        currentParameter.Value = parameter.Value;
                    }
                }
            }
        }

        private OleDbCommand CreateCommand(CommandType commandType, string commandText, Dictionary<string, OleDbParameter> parameters, OleDbTransaction transaction)
        {
            OleDbCommand command = _connection.CreateCommand();
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            command.CommandText = commandText;
            command.CommandType = commandType;
            command.CommandTimeout = _commandTimeout;

            if (commandType == CommandType.StoredProcedure)
            {
                OleDbCommandBuilder.DeriveParameters(command);
            }

            if (parameters != null)
            {
                if (commandType == CommandType.StoredProcedure)
                {
                    // Set command parameter values as per input values
                    foreach (OleDbParameter currentParameter in command.Parameters)
                    {
                        if (parameters.ContainsKey(currentParameter.ParameterName))
                        {
                            currentParameter.Value = parameters[currentParameter.ParameterName].Value;
                        }
                        else
                        {
                            currentParameter.Value = null;
                        }
                    }
                }
                else
                {
                    foreach (OleDbParameter parameter in parameters.Values)
                    {
                        _ = command.Parameters.Add(parameter);
                    }
                }
            }
            return command;
        }

        public override void Close()
        {
            _connection?.Close();
        }

        public override bool IsOpen =>
            _connection != null && _connection.State != ConnectionState.Closed;

        public OleDbConnection Connection => _connection;

        protected void CleanUpResources()
        {
            if (_connection == null)
            {
                return;
            }

            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }

            _connection = null;
        }

        public DataTable ExecuteDataTable(CommandType commandType, string strCommand, OleDbTransaction transaction = null)
        {
            return ExecuteDataTable(commandType, strCommand, new Dictionary<string, OleDbParameter>(), transaction);
        }

        public DataTable ExecuteDataTable(CommandType commandType, string strCommand, Dictionary<string, OleDbParameter> parameters, OleDbTransaction transaction = null)
        {
            if (!IsOpen)
            {
                throw new Exception("Attempt to execute a query against invalid connection");
            }

            using (OleDbCommand command = CreateCommand(commandType, strCommand, parameters, transaction))
            {
                using (var da = new OleDbDataAdapter(command))
                {
                    var dt = new DataTable();
                    _ = da.Fill(dt);

                    GetOutputParameters(command, parameters);

                    return dt;
                }
            }
        }

        public object ExecuteScalar(CommandType commandType, string strCommand, OleDbTransaction transaction = null)
        {
            return ExecuteScalar(commandType, strCommand, new Dictionary<string, OleDbParameter>(), transaction);
        }

        public object ExecuteScalar(CommandType commandType, string strCommand, Dictionary<string, OleDbParameter> parameters, OleDbTransaction transaction = null)
        {
            if (!IsOpen)
            {
                throw new Exception("Attempt to execute a query against invalid connection");
            }

            OleDbCommand command = CreateCommand(commandType, strCommand, parameters, transaction);
            object value = command.ExecuteScalar();
            GetOutputParameters(command, parameters);

            return value;
        }

        public int ExecuteNonQuery(CommandType commandType, string strCommand, OleDbTransaction transaction = null)
        {
            return ExecuteNonQuery(commandType, strCommand, new Dictionary<string, OleDbParameter>(), transaction);
        }

        public int ExecuteNonQuery(CommandType commandType, string strCommand, Dictionary<string, OleDbParameter> parameters, OleDbTransaction transaction = null)
        {
            if (!IsOpen)
            {
                throw new Exception("Attempt to execute a query against invalid connection");
            }

            int result = 0;

            using (OleDbCommand command = CreateCommand(commandType, strCommand, parameters, transaction))
            {
                result = command.ExecuteNonQuery();
                GetOutputParameters(command, parameters);
            }

            return result;
        }

        public OleDbDataReader ExecuteReader(CommandType commandType, string strCommand, CommandBehavior commandBehaviour, OleDbTransaction transaction = null)
        {
            return ExecuteReader(commandType, strCommand, new Dictionary<string, OleDbParameter>(), commandBehaviour, transaction);
        }

        public OleDbDataReader ExecuteReader(CommandType commandType, string strCommand, Dictionary<string, OleDbParameter> parameters, CommandBehavior commandBehaviour, OleDbTransaction transaction = null)
        {
            if (!IsOpen)
            {
                throw new Exception("Attempt to execute a query against invalid connection");
            }

            OleDbCommand com = CreateCommand(commandType, strCommand, parameters, transaction);
            OleDbDataReader reader = com.ExecuteReader(commandBehaviour);
            GetOutputParameters(com, parameters);

            return reader;
        }

        public Dictionary<string, OleDbParameter> GetStoredProcedureParameters(string storedProcedureName)
        {
            return GetProgramabilityParameters(storedProcedureName, null);
        }

        public Dictionary<string, OleDbParameter> GetStoredProcedureParameters(string storedProcedureName, OleDbTransaction transaction)
        {
            return GetProgramabilityParameters(storedProcedureName, transaction);
        }

        public Dictionary<string, OleDbParameter> GetUserDefineFunctionParameters(string userDefinedFunctionName)
        {
            return GetProgramabilityParameters(userDefinedFunctionName, null);
        }

        public Dictionary<string, OleDbParameter> GetUserDefineFunctionParameters(string userDefinedFunctionName, OleDbTransaction transaction)
        {
            return GetProgramabilityParameters(userDefinedFunctionName, transaction);
        }

        /// <summary>
        /// Returns parameters for programability object (E.g. Stored procedure,
        /// UDF etc)
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private Dictionary<string, OleDbParameter> GetProgramabilityParameters(string objectName, OleDbTransaction transaction)
        {
            var parameters = new Dictionary<string, OleDbParameter>();
            using (var command = new OleDbCommand(objectName, _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaction;
                OleDbCommandBuilder.DeriveParameters(command);

                foreach (OleDbParameter parameter in command.Parameters)
                {
                    OleDbParameter parameterClone = CloneParameter(parameter);
                    if (parameterClone.Value == null)
                    {
                        parameterClone.Value = DBNull.Value;
                    }
                    parameters.Add(parameter.ParameterName, parameterClone);
                }
            }

            return parameters;
        }

        private static OleDbParameter CloneParameter(OleDbParameter input)
        {
            var parameter = new OleDbParameter();
            parameter.ParameterName = input.ParameterName;
            parameter.DbType = input.DbType;
            parameter.Direction = input.Direction;
            parameter.IsNullable = input.IsNullable;
            parameter.OleDbType = input.OleDbType;
            parameter.Precision = input.Precision;
            parameter.Scale = input.Scale;
            parameter.Size = input.Size;
            parameter.SourceColumn = input.SourceColumn;
            parameter.SourceColumnNullMapping = input.SourceColumnNullMapping;
            parameter.SourceVersion = input.SourceVersion;
            parameter.Value = input.Value;
            return parameter;
        }

        public static List<DataTable> GetDataTables(OleDbDataReader reader)
        {
            var dataTableList = new List<DataTable>();
            do
            {
                var dataTable = new DataTable();
                dataTable.Load(reader);   // Calls NextResult()
                dataTableList.Add(dataTable);
            } while (!reader.IsClosed);
            return dataTableList;
        }

        public override void Dispose()
        {
            CleanUpResources();
        }
    }
}
