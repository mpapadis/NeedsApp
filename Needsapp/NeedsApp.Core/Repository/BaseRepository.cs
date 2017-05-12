//using MvvmCross.Platform;
//using MvvmCross.Plugins.Sqlite;
//using System.Collections.Generic;
//using NeedsApp.Core.Model;

//namespace NeedsApp.Core.Repository
//{
//    public abstract class BaseRepository<T> : IRepository<T> where T : BaseModel
//    {
//        private static string DbName = "Database.sqlite";

//        private IMvxSqliteConnectionFactory _connectionFactory;

//        public BaseRepository()
//        {
//            _connectionFactory = Mvx.Resolve<IMvxSqliteConnectionFactory>();

//            using (var conn = _connectionFactory.GetConnection(DbName))
//            {
//                conn.CreateTable<T>();
//            }
//        }

//        public  T GetById(string id)
//        {
//            using (var conn = _connectionFactory.GetConnection(DbName))
//            {
//                return conn.Get<T>(id);
//            }
//        }

//        public IEnumerable<T> GetAll()
//        {
//            using (var conn = _connectionFactory.GetConnection(DbName))
//            {
//                return conn.Table<T>();
//            }
//        }

//        public int Insert(T item)
//        {
//            using (var conn = _connectionFactory.GetConnection(DbName))
//            {
//                return conn.Insert(item, typeof(T));
//            }
//        }

//        public int Update(T item)
//        {

//            using (var conn = _connectionFactory.GetConnection(DbName))
//            {
//                return conn.Update(item, typeof(T));
//            }
//        }

//        public int Delete(T item)
//        {
//            using (var conn = _connectionFactory.GetConnection(DbName))
//            {
//                return conn.Delete<T>(item.ID);
//            }
//        }
//    }
//}
