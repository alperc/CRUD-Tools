using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using CRUD;

namespace CRUD.Repostory
{
    public class MainRepos
    {
        public static string _table = "BLOG";                   // Dinamik tablo adı.
        public static string _orderColon = "OLUSTURMA_TARIHI";  // Varsayılan sıralama kolonu
        public static string _orderBy = "DESC";                 // Varsayılan sıralama şartı.

        public static Models.Blog _model = new Models.Blog();   // Modelimiz.
        public static string _primary = "KOD";                  // Birincil anahtarımız.
        public static string[] _colons = _model.COLONS;         // Model'den gelen table kolonları.
        public static string _select, _update, _insert;         // Dinamik query'ler

        protected SqlConnection GetOpenConnection()
        {
            SqlConnection cnstr = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString);

            if (cnstr.State != ConnectionState.Open)
            {
                cnstr.Open();
            }
            return cnstr;
        }

        public IEnumerable<Models.Blog> getAll()
        {
            using (SqlConnection conn = GetOpenConnection())
            {
                foreach (var item in _colons)
                {
                    _select += item + ",";
                }
                return conn.Query<Models.Blog>("SELECT " + _select +" FROM " + _table + " ORDER BY " + _orderColon + " " + _orderBy).ToList();
            }
        }
        
        public Models.Blog Get(string KOD)
        {
            using (SqlConnection conn = GetOpenConnection())
            {
                for (int key = 0; key < _colons.Length; ++key)
                {
                    _select += (_colons.Length > key + 1) ? _colons[key] + "," : _colons[key];
                }
                return conn.Query<Models.Blog>("SELECT " + _select + " FROM " + _table + " WHERE " + _primary + " = @KOD", new { KOD }).SingleOrDefault();
            }
        }
        
        public int Insert(Models.Blog Blog)
        {
            using (SqlConnection conn = GetOpenConnection())
            {
                string query1 = "(";
                string query2 = "Values(";
                for (int key = 0; key < _colons.Length; ++key)
                {
                    query1 += (_colons.Length > key + 1) ? _colons[key] + "," : _colons[key] + ")";
                    query2 += (_colons.Length > key + 1) ? "@" + _colons[key] + "," : "@" + _colons[key] + ")";
                } 
                _insert = query1 + query2;
                return conn.Execute("INSERT " + _table + _insert, Blog);
            }
        }
        
        public int Update(Models.Blog Blog)
        {
            using (SqlConnection conn = GetOpenConnection())
            {
                string query = "(";
                for (int key = 0; key < _colons.Length; ++key)
                {
                    query += (_colons.Length > key + 1) ? _colons[key] + " = @" + _colons[key] + ", " : _colons[key] + " = @" + _colons[key];
                }

                _update = query;

                return conn.Execute("UPDATE " + _table + " SET " + _update + " WHERE " + _primary + " = @KOD", Blog);
            }
        }

        public int Control(string KOD)
        {
            using (SqlConnection conn = GetOpenConnection())
            {
                return conn.Query<Models.Blog>("SELECT * FROM " + _table + " WHERE " + _primary + " = @KOD", new { KOD }).Count();
            }
        }

        public int Delete(string KOD)
        {
            using (var conn = GetOpenConnection())
            {
                return conn.Execute("DELETE FROM " + _table + " WHERE " + _primary + " = @KOD", new { KOD });
            }
        }
    }
}
