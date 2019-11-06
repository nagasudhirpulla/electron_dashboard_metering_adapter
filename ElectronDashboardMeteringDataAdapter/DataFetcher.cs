using System;
using System.Collections.Generic;
using AdapterUtils;
using Npgsql;

namespace ElectronDashboardMeteringDataAdapter
{
    public class DataFetcher
    {
        public static void FetchAndFlushData(AdapterParams prms)
        {
            // get measurement id from command line arguments
            string measId = prms.MeasId;
            // get the start and end times
            DateTime startTime = prms.FromTime;
            DateTime endTime = prms.ToTime;

            // This Configuration data varaible Config_ can be handy here while dealing application secrets or configurations...
            ConfigurationManager Config_ = new ConfigurationManager();
            Config_.Initialize();
            string connString = $"server={Config_.DataHost};user id={Config_.DataUserName};password={Config_.DataPassword};database={Config_.DataDbName};";
            // Connect to a PostgreSQL database
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            conn.Open();

            // Define a query
            NpgsqlCommand pgCmd = new NpgsqlCommand("select data_time, mwh from public.fict_location_energy_data where location_id=@location_id and data_time between @start_time and @end_time order by data_time", conn);
            pgCmd.Parameters.AddWithValue("@location_id", measId);
            pgCmd.Parameters.AddWithValue("@start_time", startTime);
            pgCmd.Parameters.AddWithValue("@end_time", endTime);

            // Execute a query
            NpgsqlDataReader dr = pgCmd.ExecuteReader();
            List<object> fetchResult = new List<object>();
            while (dr.HasRows)
            {
                while (dr.Read())
                {
                    DateTime dt = dr.GetDateTime(0);
                    double ts = TimeUtils.ToMillisSinceUnixEpoch(dt);
                    fetchResult.Add(ts);
                    double val = dr.GetDouble(1);
                    fetchResult.Add(val);
                    if (prms.IncludeQuality)
                    {
                        DataQuality qual = DataQuality.GOOD;
                        fetchResult.Add((int)qual);
                    }
                }
                dr.NextResult();
            }
            // Create output data string
            string outStr = string.Join(",", fetchResult);
            // send the output data to console
            ConsoleUtils.FlushChunks(outStr);
        }
    }
}