using System.Data.SqlClient;

namespace ChatBotCoachWebsite.Helpers
{
    public class SqlAccessDb
    {
        SqlConnection con = new();
        SqlCommand com = new();

        void ConnectionString()
        {
            con.ConnectionString = "data source=chatbotcoachserver.database.windows.net; database=ChatBotCoachDb; integrated security=";
        }
    }
}
