using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using ConsultasDBRomss.Utils;

namespace ConsultasDBRomss.Connection
{public class ConnectDB
    {
        private SqlConnection db = null;
        private String getStrConnection()
        {
            XMLHelper util = new XMLHelper();
            String DS = ConfigurationManager.AppSettings["DATASOURCE"].ToString();
            String DB = ConfigurationManager.AppSettings["CATALOG_DB"].ToString();
            String US = ConfigurationManager.AppSettings["USERKEY"].ToString();
            String PW = ConfigurationManager.AppSettings["PASSKEY"].ToString();
            //String PW = util.decodeDataByBase64(ConfigurationManager.AppSettings["PASSKEY"].ToString());

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

        public DataTable processProcedure(String NameProcedure, params IDataParameter[] sqlParams)
        {                        
            try
            {                
                this.openConnection();
                using (this.db)
                {
                    SqlCommand dCmd = new SqlCommand(NameProcedure, this.db);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    if (sqlParams != null)
                    {
                        foreach (IDataParameter param in sqlParams)
                        {
                            dCmd.Parameters.Add(param);
                        }
                    }
                    SqlDataReader responseSQL = dCmd.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(responseSQL);
                    if (dataTable.Rows.Count > 0) 
                    {
                        this.db.Close();
                        return dataTable; 
                    } 
                    else { 
                        return null; 
                    }                    
                }                                                           
            }
            catch (Exception ex)
            {
                return null;
                Log.save(this, ex, null);
                throw new Exception(ex.Message);
            }
        }
    }
}