﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Web.Needsa.Data;
using Web.Needsa.Models.Db;
using Web.Needsa.Models.Dto;

namespace Web.Needsa.Services
{
    public static class StupidShedulerTimerService
    {
        private static Thread thread;
        private static int TimeSpan = 1000 * 30; //every 10 seconds
        private static Timer timer;
        private static IServiceProvider _serviceProvider;

        public static void StupidShedulerTimerStart(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Thread thread = new Thread(new ThreadStart(ThreadFunc));
            thread.IsBackground = true;
            thread.Name = "ThreadFunc";
            thread.Start();
        }

        private static void ThreadFunc()
        {
            timer = new Timer(_ => OnCallBack(), null, 0, TimeSpan);
        }

        private static int connectTimeOut =(int)Math.Round( (double)TimeSpan * 0.666, 0);
        private static CancellationTokenSource CancellationTokenSource = null;

        private static void OnCallBack()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite); //stops the timer
            //Thread.Sleep(3000); //doing some long operation
            ApplicationDbContext db = (ApplicationDbContext)_serviceProvider.GetService(typeof(ApplicationDbContext));
            var lstArduinoStations = db.ArduinoStations.ToList();
            foreach (var arduinoStation in lstArduinoStations)
            {
                var uri = new Uri(arduinoStation.Uri + "getsensorsdata");
                try
                {
                    CancellationTokenSource = new CancellationTokenSource(connectTimeOut);
                    var t = CancellationTokenSource.Token;
                    using (HttpClient client = new HttpClient())
                    
                    using (HttpResponseMessage response = client.GetAsync(uri, t).Result)
                    using (HttpContent content = response.Content)
                    {
                        // ... Read the string.
                        string result = content.ReadAsStringAsync().Result;

                        // ... Display the result.
                        if (result != null && result.Length >= 50)
                        {
                            var Variables = result.Substring(result.IndexOf("<body>") + 6, result.IndexOf("</body>") - result.IndexOf("<body>") - 7);
                            var VariablesArray = Variables.Split(',');
                            foreach (var variable in VariablesArray)
                            {
                                var variableVal = variable.Substring(1, variable.Length - 2);
                                var keyVallue = variableVal.Split(':');
                                var vv = new ArduinoStationVariable();
                                vv.ArduinoStationId = arduinoStation.Id;
                                vv.DateCaptured = DateTimeOffset.Now;
                                vv.VariableId = int.Parse(keyVallue[0]);
                                vv.ValueCaptured = decimal.Parse(keyVallue[1], CultureInfo.InvariantCulture);
                                db.ArduinoStationVariables.Add(vv);
                                db.SaveChanges();

                                //edw mporoume na kanoume entoles me tin igrasia
                                if (vv.VariableId == 5)
                                {
                                    //var avg = db.ArduinoStationVariables.Where(x=>x.VariableId == 5).Take(2).Select(x=>x.ValueCaptured).Average(x=>x);
                                    //teleutees kapies
                                    //if (avg < 10)
                                    //{
                                    //    //xamila ara anoikse
                                    //}
                                    //else 
                                    if (vv.ValueCaptured > 100)
                                    {
                                        //ipsila  ara kleise
                                        var arduinoService = new ArduinoService(db);
                                        var openCloseCommandDto = new OpenCloseCommandDto() { StationId = arduinoStation.Id, StationStatus = false };
                                        var aa = arduinoService.SendOpenCLoseCommand(openCloseCommandDto).Result;
                                    }
                                }

                            }
                            Console.WriteLine(result.Substring(0, 50) + "...");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    //throw;
                }
            }
            

            timer.Change(0, TimeSpan);  //restarts the timer
        }
    }
}
