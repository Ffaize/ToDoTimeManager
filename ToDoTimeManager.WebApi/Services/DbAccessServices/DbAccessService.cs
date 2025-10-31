using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace MeetUp.TrustLocker.DataAccess.DbAccessServices
{
    public interface IDbAccessService
    {
        /// <summary>
        /// Gets the connection string from the configuration.
        /// </summary>
        /// <returns>Connection string</returns>
        string? GetConnectionString();

        /// <summary>
        /// Gets all items from the database using a stored procedure.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="procedureName"></param>
        /// <returns>List of items, type of stored procedure table item type</returns>
        Task<List<TResult>> GetRecords<TResult>(string procedureName);

        /// <summary>
        /// Adds one record to the database using a stored procedure.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="entity"></param>
        /// <returns>The number of rows affected</returns>
        Task<int> AddRecord<TEntity>(string procedureName, TEntity entity);

        /// <summary>
        /// Updates one record in the database using a stored procedure.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="entity"></param>
        /// <returns>The number of rows affected</returns>
        Task<int> UpdateRecord<TEntity>(string procedureName, TEntity entity);

        /// <summary>
        /// Deletes one record by ID from the database using a stored procedure.
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="id"></param>
        /// <returns>The number of rows affected</returns>
        Task<int> DeleteRecordById(string procedureName, Guid id);

        /// <summary>
        /// Gets a single item by ID from the database using a stored procedure.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="id"></param>
        /// <returns>Item with specified ID</returns>
        Task<TResult?> GetRecordById<TResult>(string procedureName, Guid id);

        /// <summary>
        /// Gets a single item by a specific parameter from the database using a stored procedure.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns>Item with specified parameter</returns>
        Task<TResult?> GetOneByParameter<TResult>(string procedureName, string parameterName, object value);

        /// <summary>
        /// Gets all items by a specific parameter from the database using a stored procedure.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns>All items with specified parameter</returns>
        Task<List<TResult>> GetAllByParameter<TResult>(string procedureName, string parameterName, object value);

        /// <summary>
        /// Executes a stored procedure with the specified parameters and retrieves a list of records.
        /// </summary>
        /// <remarks>This method is designed to execute a stored procedure and map the results to a list
        /// of objects of the specified type. Ensure that the stored procedure and the provided parameters are correctly
        /// configured to match the expected result type.</remarks>
        /// <typeparam name="TResult">The type of the records to be returned.</typeparam>
        /// <param name="procedureName">The name of the stored procedure to execute. Cannot be null or empty.</param>
        /// <param name="parameters">The dynamic parameters to pass to the stored procedure. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of records of type
        /// <typeparamref name="TResult"/>.</returns>
        Task<List<TResult>> GetRecordsByParameters<TResult>(string procedureName, DynamicParameters parameters);

        //Task<List<TResult>> GetRecordsAsync<T1, T2, T3, T4, TResult>(
        //    string procedureName,
        //    Func<T1, T2, T3, T4, TResult> map,
        //    string splitOn);

        //Task<TResult?> GetRecordByIdAsync<T1, T2, T3, T4, TResult>(
        //    string procedureName,
        //    Func<T1, T2, T3, T4, TResult> map,
        //    Guid id,
        //    string splitOn);

        //Task<List<TResult>> GetRecordsByParametersAsync<T1, T2, T3, T4, TResult>(
        //    string procedureName,
        //    Func<T1, T2, T3, T4, TResult> map,
        //    DynamicParameters parameters,
        //    string splitOn);
    }

    /// <summary>
    /// Service for accessing the database.
    /// </summary>
    public class DbAccessService : IDbAccessService
    {
        /// <summary>
        /// Configuration for accessing the database.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbAccessService"/> class with the specified configuration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DbAccessService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Gets the connection string from the configuration.
        /// </summary>
        /// <returns>Connection string</returns>
        public string? GetConnectionString()
        {
            try
            {
                return _configuration.GetConnectionString("SqlServer");
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error retrieving connection string from configuration : {e.Message}");
            }
        }


        /// <summary>
        /// Gets all items from the database using a stored procedure.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="procedureName"></param>
        /// <returns>List of items, type of stored procedure table item type</returns>
        public async Task<List<TResult>> GetRecords<TResult>(string procedureName)
        {
            try
            {
                await using var connection = new SqlConnection(GetConnectionString());
                var result = await connection.QueryAsync<TResult>(procedureName, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error retrieving records using procedure '{procedureName}': {e.Message}");
            }
        }

       
        //public async Task<List<TResult>> GetRecordsAsync<T1, T2, T3, T4, TResult>(
        //    string procedureName,
        //    Func<T1, T2, T3, T4, TResult> map,
        //    string splitOn)
        //{
        //    await using var connection = new SqlConnection(GetConnectionString());
        //    var result = await connection.QueryAsync<T1, T2, T3, T4, TResult>(
        //        procedureName,
        //        map,
        //        splitOn: splitOn,
        //        commandType: CommandType.StoredProcedure);
        //    return result.ToList();
        //}

        //public async Task<TResult?> GetRecordByIdAsync<T1, T2, T3, T4, TResult>(
        //    string procedureName,
        //    Func<T1, T2, T3, T4, TResult> map,
        //    Guid id,
        //    string splitOn)
        //{
        //    await using var connection = new SqlConnection(GetConnectionString());
        //    var parameters = new DynamicParameters();
        //    parameters.Add("@Id", id);
        //    var result = await connection.QueryAsync<T1, T2, T3, T4, TResult>(
        //        procedureName,
        //        map,
        //        param: parameters,
        //        splitOn: splitOn,
        //        commandType: CommandType.StoredProcedure);
            
        //    return result.ToList().FirstOrDefault();
        //}

        //public async Task<List<TResult>> GetRecordsByParametersAsync<T1, T2, T3, T4, TResult>(
        //    string procedureName,
        //    Func<T1, T2, T3, T4, TResult> map,
        //    DynamicParameters parameters,
        //    string splitOn)
        //{
        //    await using var connection = new SqlConnection(GetConnectionString());
        //    var result = await connection.QueryAsync<T1, T2, T3, T4, TResult>(
        //        procedureName,
        //        map,
        //        param: parameters,
        //        splitOn: splitOn,
        //        commandType: CommandType.StoredProcedure);
            
        //    return result.ToList();
        //}

        /// <summary>
        /// Adds one record to the database using a stored procedure.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="entity"></param>
        /// <returns>The number of rows affected</returns>
        public async Task<int> AddRecord<TEntity>(string procedureName, TEntity entity)
        {
            try
            {
                await using var connection = new SqlConnection(GetConnectionString());
                return await connection.ExecuteAsync(procedureName, entity, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error adding record using procedure '{procedureName}': {e.Message}");
            }
        }

        /// <summary>
        /// Updates one record in the database using a stored procedure.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="entity"></param>
        /// <returns>The number of rows affected</returns>
        public async Task<int> UpdateRecord<TEntity>(string procedureName, TEntity entity)
        {
            try
            {
                await using var connection = new SqlConnection(GetConnectionString());
                return await connection.ExecuteAsync(procedureName, entity, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error updating record using procedure '{procedureName}': {e.Message}");
            }
        }

        /// <summary>
        /// Deletes one record by ID from the database using a stored procedure.
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="id"></param>
        /// <returns>The number of rows affected</returns>
        public async Task<int> DeleteRecordById(string procedureName, Guid id)
        {
            try
            {
                await using var connection = new SqlConnection(GetConnectionString());
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                return await connection.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error deleting record with ID '{id}' using procedure '{procedureName}': {e.Message}");
            }
        }

        /// <summary>
        /// Gets a single item by ID from the database using a stored procedure.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="id"></param>
        /// <returns>Item with specified ID</returns>
        public async Task<TResult?> GetRecordById<TResult>(string procedureName, Guid id)
        {
            try
            {
                await using var connection = new SqlConnection(GetConnectionString());
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                return await connection.QuerySingleOrDefaultAsync<TResult>(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error retrieving record with ID '{id}' using procedure '{procedureName}' : {e.Message}");
            } 
        }

        /// <summary>
        /// Gets a single item by a specific parameter from the database using a stored procedure.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns>Item with specified parameter</returns>
        public async Task<TResult?> GetOneByParameter<TResult>(string procedureName, string parameterName, object value)
        {
            try
            {
                await using var connection = new SqlConnection(GetConnectionString());
                var parameters = new DynamicParameters();
                parameters.Add(parameterName, value);
                return await connection.QuerySingleOrDefaultAsync<TResult>(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error retrieving record with parameter '{parameterName}' and value '{value}' using procedure '{procedureName}': {e.Message}");
            }
        }

        /// <summary>
        /// Gets all items by a specific parameter from the database using a stored procedure.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns>All items with specified parameter</returns>
        public async Task<List<TResult>> GetAllByParameter<TResult>(string procedureName, string parameterName, object value)
        {
            try
            {
                await using var connection = new SqlConnection(GetConnectionString());
                var parameters = new DynamicParameters();
                parameters.Add(parameterName, value);

                var result = await connection.QueryAsync<TResult>(procedureName, parameters, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error retrieving records with parameter '{parameterName}' and value '{value}' using procedure '{procedureName}': {e.Message}");
            }
        }

       
        public async Task<List<TResult>> GetRecordsByParameters<TResult>(string procedureName, DynamicParameters parameters)
        {
            try
            {
                await using var connection = new SqlConnection(GetConnectionString());
                var result = await connection.QueryAsync<TResult>(procedureName, parameters, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error retrieving records using procedure '{procedureName}' with parameters: {e.Message}");
            }
        }

    }
}
