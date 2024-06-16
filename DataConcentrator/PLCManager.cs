using DataConcentrator.src;
using PLCSimulator;
using System.Collections.Generic;
using System.Linq;

namespace DataConcentrator
{
    public class PLCManager
    {
        public PLCSimulatorManager SimulatorManager { get; set; }
        public List<string> AvailibleAnalogInputs { get; set; }
        public List<string> AvailibleAnalogOutputs { get; set; }
        public List<string> AvailibleDigitalInputs { get; set; }
        public List<string> AvailibleDigitalOutputs { get; set; }

        public PLCManager() 
        { 
            var context = DataConcentratorContext.Instance;
            SimulatorManager = new PLCSimulatorManager();

            AvailibleAnalogInputs = GetAvailibleAddresses(SimulatorManager.AnalogInputAddress.Keys.ToList(), context.AnalogInputs.Select(analogInput => analogInput.Address).ToList());
            AvailibleAnalogOutputs = GetAvailibleAddresses(SimulatorManager.AnalogOutputAddress.Keys.ToList(), context.AnalogOutputs.Select(analogOutput => analogOutput.Address).ToList());
            AvailibleDigitalInputs = GetAvailibleAddresses(SimulatorManager.DigitalInputAddress.Keys.ToList(), context.DigitalInputs.Select(digitalInput => digitalInput.Address).ToList());
            AvailibleDigitalOutputs = GetAvailibleAddresses(SimulatorManager.DigitalOutputAddress.Keys.ToList(), context.DigitalOutputs.Select(digitalOutput => digitalOutput.Address).ToList());
        }
        private List<string> GetAvailibleAddresses(List<string> list1, List<string> list2)
        {
            return list1.Except(list2).ToList();    
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

        public void FreeAnalogInput(string address)
        {
            AvailibleAnalogInputs.Add(address);
        }
        public void FreeAnalogOutput(string address)
        {
            AvailibleAnalogOutputs.Add(address);
        }
        public void FreDigitalInput(string address)
        {
            AvailibleDigitalInputs.Add(address);
        }
        public void FreeDigitalOutput(string address)
        {
            AvailibleDigitalOutputs.Add(address);
        }
    }
}
