using ApiMyGame.Database;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Globalization;

namespace ApiMyGame.Classes
{
    public class NossaBase
    {
        private DBConnection dbConnection;

        public NossaBase()
        {
            string username = "RM93161";
            string password = "220802";
            string tns = "oracle.fiap.com.br";
            string sid = "ORCL";
            string porta = "1521";

            dbConnection = new DBConnection(username, password, tns, sid, porta);
        }
        public void ProcessGameDetails(string jsonData, string gameId)
        {
            dynamic gameData = JsonConvert.DeserializeObject(jsonData);
            dynamic gameDetails = gameData[gameId]["data"];
            try
            {
                using (OracleConnection connection = dbConnection.GetOracleConnection())
                {
                    connection.Open();
                    string insertSql = "INSERT INTO DETALHES_JOGO (" +
                                       "TYPE, NAME, STEAM_APPID, REQUIRED_AGE, IS_FREE, " +
                                       "DLCS, DETAILED_DESCRIPTION, ABOUT_THE_GAME, " +
                                       "SHORT_DESCRIPTION, SUPPORTED_LANGUAGES, HEADER_IMAGE, " +
                                       "CAPSULE_IMAGE, CAPSULE_IMAGEV5, WEBSITE, " +
                                       "RELEASE_DATE, ID_JOGO" +
                                       ") " +
                                       "VALUES (" +
                                       ":TYPE, :NAME, :STEAM_APPID, :REQUIRED_AGE, :IS_FREE, " +
                                       ":DLCS, :DETAILED_DESCRIPTION, :ABOUT_THE_GAME, " +
                                       ":SHORT_DESCRIPTION, :SUPPORTED_LANGUAGES, :HEADER_IMAGE, " +
                                       ":CAPSULE_IMAGE, :CAPSULE_IMAGEV5, :WEBSITE, " +
                                       "TO_DATE('1990-01-01 00:00:00', 'yyyy-mm-dd hh24:mi:ss'), :ID_JOGO)";

                    using (OracleCommand cmd = new OracleCommand(insertSql, connection))
                    {
                        cmd.Parameters.Add("TYPE", OracleDbType.Varchar2).Value = gameDetails.type.ToString().ToUpper();
                        cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = gameDetails.name.ToString().ToUpper();
                        cmd.Parameters.Add("STEAM_APPID", OracleDbType.Varchar2).Value = gameDetails.steam_appid.ToString().ToUpper();
                        cmd.Parameters.Add("REQUIRED_AGE", OracleDbType.Int32).Value = Convert.ToInt32(gameDetails.required_age);
                        cmd.Parameters.Add("IS_FREE", OracleDbType.Varchar2).Value = gameDetails.is_free.ToString().ToUpper();
                        cmd.Parameters.Add("DLCS", OracleDbType.Clob).Value = JsonConvert.SerializeObject(gameDetails.dlc);
                        cmd.Parameters.Add("DETAILED_DESCRIPTION", OracleDbType.Clob).Value = gameDetails.detailed_description.ToString().ToUpper();
                        cmd.Parameters.Add("ABOUT_THE_GAME", OracleDbType.Clob).Value = gameDetails.about_the_game.ToString().ToUpper();
                        cmd.Parameters.Add("SHORT_DESCRIPTION", OracleDbType.Clob).Value = gameDetails.short_description.ToString().ToUpper();
                        cmd.Parameters.Add("SUPPORTED_LANGUAGES", OracleDbType.Clob).Value = gameDetails.supported_languages.ToString().ToUpper();
                        cmd.Parameters.Add("HEADER_IMAGE", OracleDbType.Varchar2).Value = gameDetails.header_image.ToString().ToUpper();
                        cmd.Parameters.Add("CAPSULE_IMAGE", OracleDbType.Varchar2).Value = gameDetails.capsule_image.ToString().ToUpper();
                        cmd.Parameters.Add("CAPSULE_IMAGEV5", OracleDbType.Varchar2).Value = gameDetails.capsule_imagev5.ToString().ToUpper();
                        cmd.Parameters.Add("WEBSITE", OracleDbType.Varchar2).Value = gameDetails.website?.ToString().ToUpper();
                        cmd.Parameters.Add("ID_JOGO", OracleDbType.Varchar2).Value = gameId;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {

            }
            
        }

        public void ProcessAppList(string jsonData)
        {
            dynamic appListData = JsonConvert.DeserializeObject(jsonData);

            using (OracleConnection connection = dbConnection.GetOracleConnection())
            {
                connection.Open();

                foreach (var app in appListData.applist.apps)
                {
                    string insertSql = "INSERT INTO LISTA_APPS (APP_ID, NAME) VALUES (:APP_ID, :NAME)";

                    using (OracleCommand cmd = new OracleCommand(insertSql, connection))
                    {
                        cmd.Parameters.Add("APP_ID", app.appid.ToString().ToUpper());
                        cmd.Parameters.Add("NAME", app.name.ToString().ToUpper());

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void ProcessImage(string imageUrl)
        {
            using (OracleConnection connection = dbConnection.GetOracleConnection())
            {
                connection.Open();

                string insertSql = "INSERT INTO IMAGENS (IMAGE_URL) VALUES (:IMAGE_URL)";

                using (OracleCommand cmd = new OracleCommand(insertSql, connection))
                {
                    cmd.Parameters.Add("IMAGE_URL", imageUrl);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
