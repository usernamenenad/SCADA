using System;
using System.Collections.Generic;
using System.Threading;
namespace PLCSimulator
{
    /// <summary>
    /// PLC Simulator
    /// 
    /// 16 x ANALOG INPUT   : ADDR001 - ADDR004
    /// 16 x ANALOG OUTPUT  : ADDR005 - ADDR008
    /// 16 x DIGITAL INPUT  : ADDR009 - ADDR012
    /// 16 x DIGITAL OUTPUT : ADDR013 - ADDR016
    /// </summary>
    public class PLCSimulatorManager
    {
        public Dictionary<string, double> AnalogInputAddress = new Dictionary<string, double>();
        public Dictionary<string, double> AnalogOutputAddress = new Dictionary<string, double>();
        public Dictionary<string, double> DigitalInputAddress = new Dictionary<string, double>();
        public Dictionary<string, double> DigitalOutputAddress = new Dictionary<string, double>();

        private readonly object locker = new object();
        private Thread t1;
        private Thread t2;

        public PLCSimulatorManager()
        {
            // AI
            AnalogInputAddress.Add("ADDR000", 0);
            AnalogInputAddress.Add("ADDR001", 0);
            AnalogInputAddress.Add("ADDR002", 0);
            AnalogInputAddress.Add("ADDR003", 0);
            AnalogInputAddress.Add("ADDR004", 0);
            AnalogInputAddress.Add("ADDR005", 0);
            AnalogInputAddress.Add("ADDR006", 0);
            AnalogInputAddress.Add("ADDR007", 0);
            AnalogInputAddress.Add("ADDR008", 0);
            AnalogInputAddress.Add("ADDR009", 0);
            AnalogInputAddress.Add("ADDR010", 0);
            AnalogInputAddress.Add("ADDR011", 0);
            AnalogInputAddress.Add("ADDR012", 0);
            AnalogInputAddress.Add("ADDR013", 0);
            AnalogInputAddress.Add("ADDR014", 0);
            AnalogInputAddress.Add("ADDR015", 0);

            // AO
            AnalogOutputAddress.Add("ADDR016", 0);
            AnalogOutputAddress.Add("ADDR017", 0);
            AnalogOutputAddress.Add("ADDR018", 0);
            AnalogOutputAddress.Add("ADDR019", 0);
            AnalogOutputAddress.Add("ADDR020", 0);
            AnalogOutputAddress.Add("ADDR021", 0);
            AnalogOutputAddress.Add("ADDR022", 0);
            AnalogOutputAddress.Add("ADDR023", 0);
            AnalogOutputAddress.Add("ADDR024", 0);
            AnalogOutputAddress.Add("ADDR025", 0);
            AnalogOutputAddress.Add("ADDR026", 0);
            AnalogOutputAddress.Add("ADDR027", 0);
            AnalogOutputAddress.Add("ADDR028", 0);
            AnalogOutputAddress.Add("ADDR029", 0);
            AnalogOutputAddress.Add("ADDR030", 0);
            AnalogOutputAddress.Add("ADDR031", 0);

            // DI
            DigitalInputAddress.Add("ADDR032", 0);
            DigitalInputAddress.Add("ADDR033", 0);
            DigitalInputAddress.Add("ADDR034", 0);
            DigitalInputAddress.Add("ADDR035", 0);
            DigitalInputAddress.Add("ADDR036", 0);
            DigitalInputAddress.Add("ADDR037", 0);
            DigitalInputAddress.Add("ADDR038", 0);
            DigitalInputAddress.Add("ADDR039", 0);
            DigitalInputAddress.Add("ADDR040", 0);
            DigitalInputAddress.Add("ADDR041", 0);
            DigitalInputAddress.Add("ADDR042", 0);
            DigitalInputAddress.Add("ADDR043", 0);
            DigitalInputAddress.Add("ADDR044", 0);
            DigitalInputAddress.Add("ADDR045", 0);
            DigitalInputAddress.Add("ADDR046", 0);
            DigitalInputAddress.Add("ADDR047", 0);

            // DO
            DigitalOutputAddress.Add("ADDR048", 0);
            DigitalOutputAddress.Add("ADDR049", 0);
            DigitalOutputAddress.Add("ADDR050", 0);
            DigitalOutputAddress.Add("ADDR051", 0);
            DigitalOutputAddress.Add("ADDR052", 0);
            DigitalOutputAddress.Add("ADDR053", 0);
            DigitalOutputAddress.Add("ADDR054", 0);
            DigitalOutputAddress.Add("ADDR055", 0);
            DigitalOutputAddress.Add("ADDR056", 0);
            DigitalOutputAddress.Add("ADDR057", 0);
            DigitalOutputAddress.Add("ADDR058", 0);
            DigitalOutputAddress.Add("ADDR059", 0);
            DigitalOutputAddress.Add("ADDR060", 0);
            DigitalOutputAddress.Add("ADDR061", 0);
            DigitalOutputAddress.Add("ADDR062", 0);
            DigitalOutputAddress.Add("ADDR063", 0);
        }

        public void StartPLCSimulator()
        {
            t1 = new Thread(GeneratingAnalogInputs);
            t1.Start();

            t2 = new Thread(GeneratingDigitalInputs);
            t2.Start();
        }

        private void GeneratingAnalogInputs()
        {
            while (true)
            {
                Thread.Sleep(100);

                lock (locker)
                {
                    AnalogInputAddress["ADDR001"] = 100 * Math.Sin((double)DateTime.Now.Second / 60 * Math.PI); //SINE
                    AnalogInputAddress["ADDR002"] = 100 * DateTime.Now.Second / 60; //RAMP
                    AnalogInputAddress["ADDR003"] = 50 * Math.Cos((double)DateTime.Now.Second / 60 * Math.PI); //COS
                    AnalogInputAddress["ADDR004"] = RandomNumberBetween(0, 50);  //rand
                }
            }
        }

        private void GeneratingDigitalInputs()
        {
            while (true)
            {
                Thread.Sleep(1000);

                lock (locker)
                {
                    DigitalInputAddress["ADDR009"] = DigitalInputAddress["ADDR009"] == 0 ? 1 : 0;
                    DigitalInputAddress["ADDR010"] = DigitalInputAddress["ADDR010"] == 0 ? 1 : 0;
                    DigitalInputAddress["ADDR011"] = DigitalInputAddress["ADDR011"] == 0 ? 1 : 0;
                    DigitalInputAddress["ADDR012"] = DigitalInputAddress["ADDR012"] == 0 ? 1 : 0;
                }
            }
        }

        public double GetAnalogValue(string address)
        {

            if (AnalogInputAddress.ContainsKey(address))
            {
                return AnalogInputAddress[address];
            }
            else
            {
                return -1;
            }
        }

        public void SetAnalogValue(string address, double value)
        {
            if (AnalogOutputAddress.ContainsKey(address))
            {
                AnalogOutputAddress[address] = value;
            }
        }

        public void SetDigitalValue(string address, double value)
        {
            if (DigitalInputAddress.ContainsKey(address))
            {
                DigitalInputAddress[address] = value;
            }
        }

        private static double RandomNumberBetween(double minValue, double maxValue)
        {
            Random random = new Random();
            var next = random.NextDouble();

            return minValue + (next * (maxValue - minValue));
        }

        public void Abort()
        {
            t1.Abort();
            t2.Abort();
        }
    }
}
