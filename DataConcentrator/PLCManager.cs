using DataConcentrator.src;
using PLCSimulator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace DataConcentrator
{
    public class PLCManager
    {
        public DataConcentratorContext Context = DataConcentratorContext.Instance;
        public PLCSimulatorManager SimulatorManager { get; set; }
        public List<string> AvailibleAnalogInputs { get; set; }
        public List<string> AvailibleAnalogOutputs { get; set; }
        public List<string> AvailibleDigitalInputs { get; set; }
        public List<string> AvailibleDigitalOutputs { get; set; }
        public PLCManager() 
        { 
            SimulatorManager = new PLCSimulatorManager();

            AvailibleAnalogInputs = GetAvailibleAddresses(SimulatorManager.AnalogInputAddress.Keys.ToList(), Context.AnalogInputs.Select(analogInput => analogInput.Address).ToList());
            AvailibleAnalogOutputs = GetAvailibleAddresses(SimulatorManager.AnalogOutputAddress.Keys.ToList(), Context.AnalogOutputs.Select(analogOutput => analogOutput.Address).ToList());
            AvailibleDigitalInputs = GetAvailibleAddresses(SimulatorManager.DigitalInputAddress.Keys.ToList(), Context.DigitalInputs.Select(digitalInput => digitalInput.Address).ToList());
            AvailibleDigitalOutputs = GetAvailibleAddresses(SimulatorManager.DigitalOutputAddress.Keys.ToList(), Context.DigitalOutputs.Select(digitalOutput => digitalOutput.Address).ToList());
        }
        private List<string> GetAvailibleAddresses(List<string> list1, List<string> list2)
        {
            return list1.Except(list2).ToList();    
        }
        
        // Set outputs
        public void SetOutputs()
        {
            foreach (var analogOutput in Context.AnalogOutputs)
            {
                analogOutput.Value = analogOutput.InitialValue;
            }
        }
        
        // Take addresses
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

        // Free addresses
        public void FreeAnalogInput(string address)
        {
            AvailibleAnalogInputs.Add(address);
            AvailibleAnalogInputs.Sort();
        }
        public void FreeAnalogOutput(string address)
        {
            AvailibleAnalogOutputs.Add(address);
            AvailibleAnalogOutputs.Sort();
        }
        public void FreDigitalInput(string address)
        {
            AvailibleDigitalInputs.Add(address);
            AvailibleDigitalInputs.Sort();
        }
        public void FreeDigitalOutput(string address)
        {
            AvailibleDigitalOutputs.Add(address);
            AvailibleDigitalOutputs.Sort();
        }
    }
}
