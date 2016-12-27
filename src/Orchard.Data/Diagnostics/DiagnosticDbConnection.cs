﻿using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Orchard.Data.Diagnostics
{
    public class DiagnosticDbConnection : DbConnection
    {
        private readonly DbConnection _connection;

        private static readonly DiagnosticSource _source = new DiagnosticListener("Orchard.Data");

        public DiagnosticDbConnection(DbConnection connection)
        {
            _connection = connection;
        }

        public override string ConnectionString
        {
            get { return _connection.ConnectionString; }
            set { _connection.ConnectionString = value; }
        }

        public override string Database => _connection.Database;
        public override string DataSource => _connection.DataSource;
        public override string ServerVersion => _connection.ServerVersion;
        public override ConnectionState State => _connection.State;

        public override void ChangeDatabase(string databaseName)
        {
            _connection.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            _connection.Close();
        }

        public override void Open()
        {
            _connection.Open();
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new DiagnosticDbTransaction(_connection.BeginTransaction(isolationLevel));
        }

        protected override DbCommand CreateDbCommand()
        {
            return new DiagnosticDbCommand(_source, _connection.CreateCommand());
        }
    }
}
