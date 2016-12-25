using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BossBot
{
    public class AlarmClock
    {

        private EventHandler alarmEvent;
        private Timer timer;
        private DateTime alarmTime;
        private bool enabled;
        public AlarmClock(DateTime alarmTime)
        {
            this.alarmTime = alarmTime;

            timer = new Timer();
            timer.Elapsed += timer_Elapsed;
            timer.Interval = 1000;
            timer.Start();

            enabled = true;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (enabled && DateTime.Now > alarmTime)
            {
                enabled = false;
                OnAlarm();
                timer.Stop();
            }
        }

        protected virtual void OnAlarm()
        {
            alarmEvent?.Invoke(this, EventArgs.Empty);
        }
        
        public event EventHandler Alarm
        {
            add { alarmEvent += value; }
            remove { alarmEvent -= value; }
        }
    }

}