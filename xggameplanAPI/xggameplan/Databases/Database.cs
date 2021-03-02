namespace xggameplan.common.Databases
{
    public class Database
    {
        protected string _connectionString;
        protected int _commandTimeout;

        public string ConnectionString => _connectionString;

        public int CommandTimeout
        {
            get { return _commandTimeout; }
            set { _commandTimeout = value; }
        }

        public virtual void Open() { }

        public virtual void Close() { }

        public virtual void Dispose() { }

        public virtual bool IsOpen => false;
    }
}
