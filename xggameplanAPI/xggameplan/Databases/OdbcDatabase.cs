using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

namespace xggameplan.common.Databases
{
    public class OdbcDatabase : Database
    {
        protected OdbcConnection _connection = null;

        public OdbcDatabase(string connectionString)
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
                _connection = new OdbcConnection(_connectionString);
                _connection.Open();
            }
        }

        public OdbcTransaction BeginTransaction()
        {
            return _connection.BeginTransaction();
        }

        /// <summary>
        /// Gets the parameter values to return for the command and sets them in 'parameters'
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        private void GetOutputParameters(OdbcCommand command, Dictionary<string, OdbcParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (OdbcParameter currentParameter in parameters.Values)
                {
                    OdbcParameter parameter = command.Parameters[currentParameter.ParameterName];
                    if (parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Output || parameter.Direction == ParameterDirection.ReturnValue)
                    {
                        currentParameter.Value = parameter.Value;
                    }
                }
            }
        }

        private OdbcCommand CreateCommand(CommandType commandType, string commandText, Dictionary<string, OdbcParameter> parameters, OdbcTransaction transaction)
        {
            OdbcCommand command = _connection.CreateCommand();
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            command.CommandText = commandText;
            command.CommandType = commandType;
            command.CommandTimeout = _commandTimeout;
            if (commandType == CommandType.StoredProcedure)
            {
                OdbcCommandBuilder.DeriveParameters(command);
            }
            if (parameters != null)
            {
                if (commandType == CommandType.StoredProcedure)
                {
                    // Set command parameter values as per input values
                    foreach (OdbcParameter currentParameter in command.Parameters)
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
                    foreach (OdbcParameter parameter in parameters.Values)
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

        public OdbcConnection Connection => _connection;

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

        public DataTable ExecuteDataTable(CommandType commandType, string strCommand, OdbcTransaction transaction = null)
        {
            return ExecuteDataTable(commandType, strCommand, new Dictionary<string, OdbcParameter>(), transaction);
        }

        public DataTable ExecuteDataTable(CommandType commandType, string strCommand, Dictionary<string, OdbcParameter> parameters, OdbcTransaction transaction = null)
        {
            if (!IsOpen)
            {
                throw new Exception("Attempt to execute a query against invalid connection");
            }

            using (OdbcCommand command = CreateCommand(commandType, strCommand, parameters, transaction))
            using (var da = new OdbcDataAdapter(command))
            {
                var dt = new DataTable();
                _ = da.Fill(dt);
                GetOutputParameters(command, parameters);

                return dt;
            }
        }

        public object ExecuteScalar(CommandType commandType, string strCommand, OdbcTransaction transaction = null)
        {
            return ExecuteScalar(commandType, strCommand, new Dictionary<string, OdbcParameter>(), transaction);
        }

        public object ExecuteScalar(CommandType commandType, string strCommand, Dictionary<string, OdbcParameter> parameters, OdbcTransaction transaction = null)
        {
            if (!IsOpen)
            {
                throw new Exception("Attempt to execute a query against invalid connection");
            }

            OdbcCommand command = CreateCommand(commandType, strCommand, parameters, transaction);
            object value = command.ExecuteScalar();
            GetOutputParameters(command, parameters);

            return value;
        }

        public int ExecuteNonQuery(CommandType commandType, string strCommand, OdbcTransaction transaction = null)
        {
            return ExecuteNonQuery(commandType, strCommand, new Dictionary<string, OdbcParameter>(), transaction);
        }

        public int ExecuteNonQuery(CommandType commandType, string strCommand, Dictionary<string, OdbcParameter> parameters, OdbcTransaction transaction = null)
        {
            if (!IsOpen)
            {
                throw new Exception("Attempt to execute a query against invalid connection");
            }

            int result = 0;
            using (OdbcCommand command = CreateCommand(commandType, strCommand, parameters, transaction))
            {
                result = command.ExecuteNonQuery();
                GetOutputParameters(command, parameters);
            }

            return result;
        }

        public OdbcDataReader ExecuteReader(CommandType commandType, string strCommand, CommandBehavior commandBehaviour, OdbcTransaction transaction = null)
        {
            return ExecuteReader(commandType, strCommand, new Dictionary<string, OdbcParameter>(), commandBehaviour, transaction);
        }

        public OdbcDataReader ExecuteReader(CommandType commandType, string strCommand, Dictionary<string, OdbcParameter> parameters, CommandBehavior commandBehaviour, OdbcTransaction transaction = null)
        {
            if (!IsOpen)
            {
                throw new Exception("Attempt to execute a query against invalid connection");
            }

            OdbcCommand com = CreateCommand(commandType, strCommand, parameters, transaction);
            OdbcDataReader reader = com.ExecuteReader(commandBehaviour);
            GetOutputParameters(com, parameters);

            return reader;
        }

        public Dictionary<string, OdbcParameter> GetStoredProcedureParameters(string storedProcedureName)
        {
            return GetProgramabilityParameters(storedProcedureName, null);
        }

        public Dictionary<string, OdbcParameter> GetStoredProcedureParameters(string storedProcedureName, OdbcTransaction transaction)
        {
            return GetProgramabilityParameters(storedProcedureName, transaction);
        }

        public Dictionary<string, OdbcParameter> GetUserDefineFunctionParameters(string userDefinedFunctionName)
        {
            return GetProgramabilityParameters(userDefinedFunctionName, null);
        }

        public Dictionary<string, OdbcParameter> GetUserDefineFunctionParameters(string userDefinedFunctionName, OdbcTransaction transaction)
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
        private Dictionary<string, OdbcParameter> GetProgramabilityParameters(string objectName, OdbcTransaction transaction)
        {
            var parameters = new Dictionary<string, OdbcParameter>();
            using (var command = new OdbcCommand(objectName, _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaction;
                OdbcCommandBuilder.DeriveParameters(command);
                foreach (OdbcParameter parameter in command.Parameters)
                {
                    OdbcParameter parameterClone = CloneParameter(parameter);
                    if (parameterClone.Value == null)
                    {
                        parameterClone.Value = DBNull.Value;
                    }
                    parameters.Add(parameter.ParameterName, parameterClone);
                }
            }
            return parameters;
        }

        private static OdbcParameter CloneParameter(OdbcParameter input)
        {
            var parameter = new OdbcParameter();
            parameter.ParameterName = input.ParameterName;
            parameter.DbType = input.DbType;
            parameter.Direction = input.Direction;
            parameter.IsNullable = input.IsNullable;
            parameter.OdbcType = input.OdbcType;
            parameter.Precision = input.Precision;
            parameter.Scale = input.Scale;
            parameter.Size = input.Size;
            parameter.SourceColumn = input.SourceColumn;
            parameter.SourceColumnNullMapping = input.SourceColumnNullMapping;
            parameter.SourceVersion = input.SourceVersion;
            parameter.Value = input.Value;
            return parameter;
        }

        public static List<DataTable> GetDataTables(OdbcDataReader reader)
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
