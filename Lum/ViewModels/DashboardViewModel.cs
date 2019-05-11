using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Lum.Annotations;

namespace Lum.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private double _cpu;
        private double _gpu;
        private double _memory;

        public DashboardViewModel()
        {
            Start();
        }

        public double CPU
        {
            get => _cpu;
            set
            {
                if (value == _cpu)
                {
                    return;
                }

                _cpu = value;
                OnPropertyChanged();
            }
        }

        public double GPU
        {
            get => _gpu;
            set
            {
                if (value == _gpu)
                {
                    return;
                }

                _gpu = value;
                OnPropertyChanged();
            }
        }

        public double Memory
        {
            get => _memory;
            set
            {
                if (value == _memory)
                {
                    return;
                }

                _memory = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task Start()
        {
            var rand = new Random();
            CPU = rand.NextDouble();
            GPU = rand.NextDouble();
            Memory = rand.NextDouble();
            while (true)
            {
                CPU += (rand.NextDouble() + -0.5) * 0.1;
                GPU += (rand.NextDouble() + -0.5) * 0.2;
                Memory += (rand.NextDouble() + -0.5) * 0.05;
                await Task.Delay(400);
            }
        }
    }
}