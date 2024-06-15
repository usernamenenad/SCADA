using DataConcentrator.src;
using PLCSimulator;
using System.Collections.Generic;

namespace DataConcentrator
{
    public class PLCManager
    {
        public PLCSimulatorManager SimulatorManager { get; set; }
        public Dictionary<string, double> AvailibleAnalogInputs { get; set; }
        public Dictionary<string, double> AvailibleAnalogOutputs { get; set; }
        public Dictionary<string, double> AvailibleDigitalInputs { get; set; }
        public Dictionary<string, double> AvailibleDigitalOutputs { get; set; }

        private static PLCSimulatorManager instance;
        public static PLCSimulatorManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PLCSimulatorManager();
                }
                return instance;
            }
        }

        public PLCManager() 
        { 
            SimulatorManager = new PLCSimulatorManager();
            AvailibleAnalogInputs = SimulatorManager.AnalogInputAddress;
            AvailibleAnalogOutputs = SimulatorManager.AnalogOutputAddress;
            AvailibleDigitalInputs = SimulatorManager.DigitalInputAddress;
            AvailibleDigitalOutputs = SimulatorManager.DigitalOutputAddress;
        }

        public void TakeAnalogInput(string address)
        {
            AvailibleAnalogInputs.Remove(address);
        }
        public void TakeAnalogOutput(string address)
        {
            AvailibleAnalogOutputs.Remove(address);
        }
        public void TakeDigitalInput(string address)
        {
            AvailibleDigitalInputs.Remove(address);
        }
        public void TakeDigitalOutput(string address)
        {
            AvailibleDigitalOutputs.Remove(address);
        }

    }
}
