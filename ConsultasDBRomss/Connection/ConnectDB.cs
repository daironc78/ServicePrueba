using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ConsultasDBRomss.Models;
using ConsultasDBRomss.Utils;

namespace ConsultasDBRomss.Connection
{
    public class ConnectDB
    {
        private SqlConnection db = null;
        private String getStrConnection()
        {
            XMLHelper util = new XMLHelper();
            String DS = ConfigurationManager.AppSettings["DATASOURCE"].ToString();
            String DB = ConfigurationManager.AppSettings["CATALOG_DB"].ToString();
            String US = ConfigurationManager.AppSettings["USERKEY"].ToString();
            String PW = util.Desencriptarmd5(ConfigurationManager.AppSettings["PASSKEY"].ToString());

            return "Data Source=" + DS + "; Initial Catalog=" + DB + "; User ID=" + US + "; Password = " + PW + "; Connect Timeout=2000;" + " pooling='true'; Max Pool Size=2000";
        }

        public SqlConnection CreateConnect()
        {
            return new SqlConnection(this.getStrConnection());
        }

        private bool VerifyConnection()
        {
            try
            {
                SqlConnection con = this.CreateConnect();
                con.Open();
                con.Close();
                con.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                Log.save(this, ex, null);
                return false;
            }
        }

        public void openConnection()
        {
            this.db = this.CreateConnect();
            if (this.db.State == ConnectionState.Closed)
            {
                this.db.Open();
            }
        }

        public void closeConnect()
        {
            this.db.Close();
            this.db.Dispose();
        }

        public bool IsConnect()
        {
            return this.db != null;
        }

        public DataTable processQuery(String Query, XMLRomssQuery xMLRomssQuery)
        {
            try
            {
                if (xMLRomssQuery.Params.Where(param => param.Type == "INLINE").Count() > 0)
                {
                    foreach (var item in xMLRomssQuery.Params.Where(param => param.Type == "INLINE"))
                    {
                        if (item.Type.ToUpper() == "INLINE")
                        {
                            if (!item.Value.ToUpper().Contains("UPDATE") && !item.Value.ToUpper().Contains("DROP") && !item.Value.ToUpper().Contains("DELETE") &&
                                !item.Value.ToUpper().Contains("INSERT") && !item.Value.ToUpper().Contains("TRUNCATE") && !item.Value.ToUpper().Contains("ALTER"))
                            {
                                Query = Query.Replace("[" + item.Name + "]", Convert.ToString(item.Value));
                            }
                        }
                    }
                }
                DataTable dataTable = new DataTable();
                this.openConnection();
                using (this.db)
                {
                    SqlCommand dCmd = new SqlCommand(Query, this.db);
                    if (xMLRomssQuery.Params.Where(param => param.Type != "INLINE").Count() > 0)
                    {
                        foreach (var item in xMLRomssQuery.Params.Where(param => param.Type != "INLINE"))
                        {
                            if (item.Type.ToUpper() == "DATETIME")
                                dCmd.Parameters.Add("@" + item.Name, SqlDbType.DateTime).Value = Convert.ToDateTime(item.Value);
                            else if (item.Type.ToUpper() == "VARCHAR")
                                dCmd.Parameters.Add("@" + item.Name, SqlDbType.VarChar).Value = Convert.ToString(item.Value);
                            else if (item.Type.ToUpper() == "NUMBER")
                                dCmd.Parameters.Add("@" + item.Name, SqlDbType.Int).Value = Convert.ToInt32(item.Value);
                            else if (item.Type.ToUpper() == "BOOL")
                                dCmd.Parameters.Add("@" + item.Name, SqlDbType.Bit).Value = Convert.ToBoolean(item.Value);
                            else if (item.Type.ToUpper() == "DECIMAL")
                                dCmd.Parameters.Add("@" + item.Name, SqlDbType.Decimal).Value = Convert.ToDecimal(item.Value);
                        }
                    }
                    dCmd.CommandType = CommandType.Text;
                    SqlDataReader responseSQL = dCmd.ExecuteReader();
                    dataTable.Load(responseSQL);
                    dataTable.TableName = "QueryRomss";
                    if (dataTable.Rows.Count > 0)
                    {
                        this.db.Close();
                        return dataTable;
                    }
                    else
                    {
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.save(this, ex, null);
                return null;
            }
        }
    }
}