using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTopWar.Entity
{
    public class EmulatorEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public bool Checked { get; set; }
        public Status Status { get; set; }
        public string Job { get; set; }
        public string Doing { get; set; }

        public EmulatorEntity()
        {
            Status = Status.Disconnected;
        }
    }



    public enum Status
    {
        Connected = 1,
        Disconnected = 0
    }
}
