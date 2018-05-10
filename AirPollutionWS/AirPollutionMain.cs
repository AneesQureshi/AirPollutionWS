﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLayer1;
using BusinessLayer1;




namespace AirPollutionWS
{
    public class AirPollutionMain
    {
        
        public static void MainActivity()
        {

            try
            {
                Place objPlace = new Place();
                List<RecordsModel> objRecordsModel = new List<RecordsModel>();
                List<RecordsModel> objCityList = new List<RecordsModel>();

                //fetching country, state, city name from govt api
                objRecordsModel =objPlace.FetchPlace();

               

                //adding country, state, city name from govt api to database 
                 objPlace.AddPlace(objRecordsModel);

                //fetching city details from database(above list may keep changing so we are taking city details from database)
                objCityList = objPlace.FetchCity();

                //fetching station records from pvt api based on the city 
                foreach (var cityRecord in objCityList)
                {
                    Place objPlace1 = new Place();
                    List<StationModel> objStationList = new List<StationModel>();
                    string cityId = cityRecord.id;
                    string city = cityRecord.city;
                    
                        //on the basis of city we call below function and get the station list with aqi,name & lat long from pvt api
                        objStationList = objPlace1.FetchStation(city);

                    //add station name,aqi, lat long into the database
                    objPlace1.addStation(objStationList, cityId);

                    if (objStationList != null)
                    {
                        //fetch and add pollutants of stations of every city.
                        Pollutant objPollutant = new Pollutant();
                        objPollutant.FetchAddPollutants(objStationList);
                    }
                   
                }
                



            }

            catch (Exception ex)
            {
                string message = ex.ToString();
            }


        }
    }
}
