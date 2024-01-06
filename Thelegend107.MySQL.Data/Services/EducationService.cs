﻿using MapDataReader;
using MySql.Data.MySqlClient;
using System.Data;
using Thelegend107.MySQL.Data.Lib.Entities;
using Thelegend107.MySQL.Data.Lib.Helpers;

namespace Thelegend107.MySQL.Data.Lib.Services
{
    public class EducationService
    {
        private readonly MySqlConnection _sqlConnection;
        private readonly AddressService _addressService;

        public EducationService(MySqlConnection MySqlConnection, AddressService addressService)
        {
            _sqlConnection = MySqlConnection;
            _addressService = addressService;
        }

        public async Task<IEnumerable<Education>> RetrieveEducations(int userId)
        {
            List<Education> educations = new List<Education>();

            string sql = ObjectToSQLHelper<Education>.GenerateSelectQuery().ToString();

            using (MySqlConnection MySqlConnection = new MySqlConnection(_sqlConnection.ConnectionString))
            {
                MySqlConnection.Open();
                IDataReader dataReader = await new MySqlCommand(sql, MySqlConnection).ExecuteReaderAsync();
                educations = dataReader.ToEducation();
            }

            Parallel.ForEach(educations, education =>
            {
                education.Address = _addressService.RetrieveAddressById(education.AddressId).Result;
                education.EducationItems = RetrieveEducationItems(education.Id).Result;
            });

            return educations;
        }

        private async Task<IEnumerable<EducationItem>> RetrieveEducationItems(int educationId)
        {
            List<EducationItem> educationItems = new List<EducationItem>();

            string sql = ObjectToSQLHelper<EducationItem>.GenerateSelectQuery()
                .AppendLine($"WHERE EducationId = {educationId}")
                .ToString();

            using (MySqlConnection MySqlConnection = new MySqlConnection(_sqlConnection.ConnectionString))
            {
                MySqlConnection.Open();
                IDataReader dataReader = await new MySqlCommand(sql, MySqlConnection).ExecuteReaderAsync();
                educationItems = dataReader.ToEducationItem();
            }

            return educationItems;
        }
    }
}
