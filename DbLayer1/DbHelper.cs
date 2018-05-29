using EntityLayer1;

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DbLayer1
{
    public class DbHelper
    {

        MySqlConnection conn;
        MySqlCommand cmd;
        DbConnect dbc = new DbConnect();

        //adding country, state, city name from govt api to database
        //and add station and pollutants data also 
        public bool addPlace(List<RecordsModel> objRecordsModel)
        {
            bool allRecordsInserted = false;

            string previousStation = "empty";

            try
            {

                foreach (var records in objRecordsModel)
                {
                    
                    conn = dbc.openConnection();
                    cmd = new MySqlCommand("sp_addPlace", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    
                    cmd.Parameters.AddWithValue("val_country", records.country);
                    cmd.Parameters.AddWithValue("val_state", records.state);
                    cmd.Parameters.AddWithValue("val_city", records.city);



                    cmd.Parameters.AddWithValue("val_previousStation", previousStation);
                    //adding govt station and pollutant data
                    cmd.Parameters.AddWithValue("val_station", records.station);
                    

                    //cmd.Parameters.AddWithValue("val_lat", latitude);
                    //cmd.Parameters.AddWithValue("val_long", longitude);
                    cmd.Parameters.AddWithValue("val_pollutantName", records.pollutant_id);
                    cmd.Parameters.AddWithValue("val_pollutantValue", records.pollutant_avg);


                    string replace = records.last_update.Replace("-", "/");
                    DateTime dateValue;
                    DateTime.TryParseExact(replace, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture,DateTimeStyles.None, out dateValue);
                    dateValue = dateValue.AddHours(-5.5);
                    cmd.Parameters.AddWithValue("val_lastUpdated", dateValue);
                    

                    int recordInserted = cmd.ExecuteNonQuery();

                    previousStation = records.station;

                    }



                allRecordsInserted = true;

                return allRecordsInserted;

            }

            catch (Exception ex)
            {
                string message = ex.Message;
            }
            finally
            {
                dbc.closeConnection();
            }
            return allRecordsInserted;
        }

        //fetching city details from database

        public List<RecordsModel> fetchCity()
        {

            List<RecordsModel> objCityList = new List<RecordsModel>();

            try
            {



                conn = dbc.openConnection();


                //bifurcating the service here by calling different procs and getting different city names
                cmd = new MySqlCommand("sp_fetchCityService2", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataReader sdr = cmd.ExecuteReader();




                while (sdr.Read())
                {
                    RecordsModel objCity = new RecordsModel();
                    objCity.id = sdr["id"].ToString();
                    objCity.city = sdr["city"].ToString();
                    objCity.state = sdr["state"].ToString();
                    objCity.country = sdr["country"].ToString();

                    objCityList.Add(objCity);


                }




                return objCityList;

            }

            catch (Exception ex)
            {
                string message = ex.Message;
            }
            finally
            {
                dbc.closeConnection();
            }
            return objCityList;
        }


        //add station name,aqi, lat long 
        public void addStation(List<StationModel> objStationList, string cityId)
        {




            try
            {

                foreach (var records in objStationList)
                {
                    double latitude = records.station.geo[0];
                    double longitude = records.station.geo[1];

                    conn = dbc.openConnection();
                    cmd = new MySqlCommand("sp_addStation", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("val_stationName", records.station.name);
                    //  cmd.Parameters.AddWithValue("val_aqi", records.aqi);
                    cmd.Parameters.AddWithValue("val_lat", latitude);
                    cmd.Parameters.AddWithValue("val_long", longitude);
                    cmd.Parameters.AddWithValue("val_cityid", cityId);


                    int recordInserted = cmd.ExecuteNonQuery();

                }


            }

            catch (Exception ex)
            {
                string message = ex.Message;
            }
            finally
            {
                dbc.closeConnection();
            }

        }

        //add pollutant records from pvt api on the basis of lat long 
        public void AddPollutants(PollutantListModel oneStation)
        {

            try
            {
                string time = oneStation.timeZone;

                string s1 = time;
                string sign = s1.Substring(0, 1);

                time = time.Trim(new Char[] { '+', '-', });

                
                   
                TimeSpan timeZone = TimeSpan.Parse(time);
                
              
                string replace = oneStation.last_update.Replace("-", "/");
                DateTime AqiUpdateDate;
                DateTime.TryParseExact(replace, "yyyy/M/d HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out AqiUpdateDate);


                if (sign == "+")
                {
                    AqiUpdateDate = AqiUpdateDate.Add(-timeZone);
                }
                else
                {
                    AqiUpdateDate = AqiUpdateDate.Add(timeZone);

                }
                double latitude = oneStation.StationLatitude;
                double longitude = oneStation.StationLongitude;

                foreach (var pollutant in oneStation.pollutantModelList)
                {
                    conn = dbc.openConnection();
                    cmd = new MySqlCommand("sp_addPollutants", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("val_PollutantName", pollutant.PollutantName);
                    cmd.Parameters.AddWithValue("val_PollutantValue", pollutant.PollutantValue);
                    cmd.Parameters.AddWithValue("val_lat", latitude);
                    cmd.Parameters.AddWithValue("val_long", longitude);
                    cmd.Parameters.AddWithValue("val_AqiUpdateDate", AqiUpdateDate);
                    int recordInserted = cmd.ExecuteNonQuery();

                }


            }

            catch (Exception ex)
            {
                string message = ex.Message;
            }
            finally
            {
                dbc.closeConnection();
            }

        }

    }
}
