using Paises.Modelos;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paises.Services
{
    public class DataService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService;

        private string path = @"Data\Countries.sqlite";

        public void CreateDatabase()
        {
            dialogService = new DialogService();

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }
            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists Countries " +
                    "(Alpha2Code varchar(2), Name varchar(50), Capital varchar(50), " +
                    "Region varchar(50), " +
                    "Subregion varchar(50), Population int," +
                    "Gini real, Flag varchar(200))";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public void CreateTranslations()
        {
            dialogService = new DialogService();

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "Create table if not exists translations " +
                    "(Alpha2Code varchar(2), De varchar(50), Es varchar(50)," +
                    "Fr varchar(50), Ja varchar(50), It varchar(50)," +
                    "Br varchar(50), Pt varchar(50), Nl varchar(50)," +
                    "Hr varchar(50), Fa varchar(50))";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public void CreateLanguages()
        {
            dialogService = new DialogService();

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists Languages " +
                    "(Alpha2Code varchar(2), LanguageName varchar(50)," +
                    "NativeName varchar(50))";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage("Error", ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public void SaveData(List<Country> Countries)
        {
            connection = new SQLiteConnection("Data Source=" + path);

            try
            {
                connection.Open();

                foreach (var country in Countries)
                {
                    string sqlCountry = string.Format("insert into Countries (Alpha2Code, Name, Capital, Region, Subregion, Population, Gini, Flag)" +
                   " values (@Alpha2Code, @Name, @Capital, @Region, @Subregion, @Population, @Gini, @Flag)");

                    command = new SQLiteCommand(sqlCountry, connection);

                    command.Parameters.Add(new SQLiteParameter("@Alpha2Code", country.Alpha2Code));
                    command.Parameters.Add(new SQLiteParameter("@Name", country.Name));
                    command.Parameters.Add(new SQLiteParameter("@Capital", country.Capital));
                    command.Parameters.Add(new SQLiteParameter("@Region", country.Region));
                    command.Parameters.Add(new SQLiteParameter("@Subregion", country.Subregion));
                    command.Parameters.Add(new SQLiteParameter("@Population", country.Population));
                    command.Parameters.Add(new SQLiteParameter("@Gini", country.Gini));
                    command.Parameters.Add(new SQLiteParameter("@Flag", country.Flag));

                    command.ExecuteNonQuery(); // nao passa aqui


                    foreach (var language in country.Languages)
                    {
                        string sqlLanguages = string.Format("insert into languages (Alpha2Code, LanguageName, NativeName)" +
                            " values (@Alpha2Code, @LanguageName, @NativeName)");

                        command = new SQLiteCommand(sqlLanguages, connection);

                        command.Parameters.Add(new SQLiteParameter("@Alpha2Code", country.Alpha2Code));
                        command.Parameters.Add(new SQLiteParameter("@LanguageName", language.Name));
                        command.Parameters.Add(new SQLiteParameter("@NativeName", language.NativeName));

                        command.ExecuteNonQuery();
                    }

                    string sqlTranslation = string.Format("insert into translations (Alpha2Code, De, Es, Fr, Ja, It, Br, Pt, Nl, Hr, Fa)" +
                        " values (@Alpha2Code, @De, @Es, @Fr, @Ja, @It, @Br, @Pt, @Nl, @Hr, @Fa)");

                    command = new SQLiteCommand(sqlTranslation, connection);

                    command.Parameters.Add(new SQLiteParameter("@Alpha2Code", country.Alpha2Code));
                    command.Parameters.Add(new SQLiteParameter("@De", country.Translations.De));
                    command.Parameters.Add(new SQLiteParameter("@Es", country.Translations.Es));
                    command.Parameters.Add(new SQLiteParameter("@Fr", country.Translations.Fr));
                    command.Parameters.Add(new SQLiteParameter("@Ja", country.Translations.Ja));
                    command.Parameters.Add(new SQLiteParameter("@It", country.Translations.It));
                    command.Parameters.Add(new SQLiteParameter("@Br", country.Translations.Br));
                    command.Parameters.Add(new SQLiteParameter("@Pt", country.Translations.Pt));
                    command.Parameters.Add(new SQLiteParameter("@Nl", country.Translations.Nl));
                    command.Parameters.Add(new SQLiteParameter("@Hr", country.Translations.Hr));
                    command.Parameters.Add(new SQLiteParameter("@Fa", country.Translations.Fa));

                    command.ExecuteNonQuery();
                }
            }

            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public List<Country> GetData()
        {
            List<Country> countries = new List<Country>();


            SQLiteConnection connection = new SQLiteConnection("Data Source=" + path);

            try
            {
                connection.Open();

                string countriesSql = "select Alpha2Code, Name, Capital, Region, Subregion, Population, Gini, Flag from Countries";

                SQLiteDataReader reader = new SQLiteCommand(countriesSql, connection).ExecuteReader();

                while (reader.Read())
                {
                    Country newCountry = new Country
                    {
                        Alpha2Code = GetDBStringValue(reader, 0),
                        Name = GetDBStringValue(reader, 1),
                        Capital = GetDBStringValue(reader, 2),
                        Region = GetDBStringValue(reader, 3),
                        Subregion = GetDBStringValue(reader, 4),
                        Population = reader.GetInt32(5),
                        Gini = reader.GetDouble(6),
                        Flag = reader.GetString(7),
                    };

                    string sqlLanguages = "select LanguageName, NativeName from languages where alpha2code = @country";

                    command = new SQLiteCommand(sqlLanguages, connection);
                    command.Parameters.AddWithValue("@country", reader.GetString(0));

                    try
                    {
                        SQLiteDataReader readerLang = command.ExecuteReader();

                        List<Language> languages = new List<Language>();

                        while (readerLang.Read())
                        {
                            languages.Add(new Language
                            {
                                Name = GetDBStringValue(readerLang, 0),
                                NativeName = GetDBStringValue(readerLang, 1),
                            });
                        }
                        newCountry.Languages = languages;
                    }
                    catch (Exception e)
                    {
                        dialogService.ShowMessage("Error", e.Message);
                        return null;
                    }

                    string sqlTranslations = "select De, Es, Fr, Ja, It, Br, Pt, Nl, Hr, Fa from translations where alpha2code = @country";

                    command = new SQLiteCommand(sqlTranslations, connection);
                    command.Parameters.AddWithValue("@country", reader.GetString(0));

                    try
                    {
                        SQLiteDataReader readerTrans = command.ExecuteReader();

                        while (readerTrans.Read())
                        {
                            Translations translations = new Translations
                            {
                                De = GetDBStringValue(readerTrans, 0),
                                Es = GetDBStringValue(readerTrans, 1),
                                Fr = GetDBStringValue(readerTrans, 2),
                                Ja = GetDBStringValue(readerTrans, 3),
                                It = GetDBStringValue(readerTrans, 4),
                                Br = GetDBStringValue(readerTrans, 5),
                                Pt = GetDBStringValue(readerTrans, 6),
                                Nl = GetDBStringValue(readerTrans, 7),
                                Hr = GetDBStringValue(readerTrans, 8),
                                Fa = GetDBStringValue(readerTrans, 9),
                            };
                            newCountry.Translations = translations;
                        }
                    }
                    catch (Exception e)
                    {
                        dialogService.ShowMessage("Error", e.Message);
                        return null;
                    }
                    countries.Add(newCountry);
                }

                return countries;
            }

            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
                return null;
            }
        }

        private string GetDBStringValue(SQLiteDataReader reader, int id)
        {
            return reader.IsDBNull(id) ? null : reader.GetString(id);
        }


        public void DeleteData()
        {
            connection = new SQLiteConnection("Data Source=" + path);

            try
            {
                connection.Open();

                string sql = "delete from Countries";

                command = new SQLiteCommand(sql, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
            finally
            {
                connection.Close();
            }
        }
    }

}
